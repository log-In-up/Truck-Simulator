using GameData.Vehicle;
using Input;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SkillSystem;

namespace Player
{
#if UNITY_EDITOR
    [DisallowMultipleComponent, RequireComponent(typeof(Rigidbody))]
    [AddComponentMenu("Player/Movement")]
#endif
    public sealed class Movement : MonoBehaviour
    {
        #region Editor fields
        [SerializeField] private Transform _centerOfMass = null;
        [SerializeField] private List<WheelCollider> _steeringWheels = null;
        [SerializeField] private List<WheelCollider> _movementWheels = null;
        #endregion

        #region Fields
        private float _currentMotorTorque, _deltaMotorTorque, _currentSteerAngle;

        private CarData _carData = null;
        private Driving _drivingSkill = null;
        private Characteristics _characteristics = null;
        private Transmission _transmission = null;
        private PlayerInput _input = null;
        private Rigidbody _rigidbody = null;
        private List<WheelCollider> _wheels = null;

        private const float ZERO = 0.0f, NEUTRAL_MULTIPLIER = 0.01f, STEER_ANGLE_DELTA = 0.1f;
        private const float Multiplier_From_MPS_to_KPH = 3.6f;
        #endregion

        #region Properties
        public float CurrentSpeedInKPH => _rigidbody.velocity.magnitude * Multiplier_From_MPS_to_KPH;
        #endregion

        #region MonoBehaviour API
        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.centerOfMass = _centerOfMass.localPosition;
            _input = new PlayerInput();
        }

        private void OnEnable()
        {
            _input.Enable();
        }

        private void Start()
        {
            List<WheelCollider> tempWheels = new List<WheelCollider>();
            tempWheels.AddRange(_steeringWheels);
            tempWheels.AddRange(_movementWheels);

            _wheels = new List<WheelCollider>();
            _wheels = tempWheels.Distinct().ToList();

            _deltaMotorTorque = (_carData.MaxMotorTorque - _carData.MinMotorTorque) / _carData.MaxMotorTorqueTime;
            _currentSteerAngle = 0.0f;
        }

        private void Update()
        {
            Vector2 direction = _input.Player.Move.ReadValue<Vector2>();

            MotorTorque(direction);
            Move(direction);
            Steering(direction);
            Limit(direction);
        }

        private void OnDisable()
        {
            _input.Disable();
        }
        #endregion

        #region Methods
        private void MotorTorque(Vector2 direction)
        {
            float value = direction != Vector2.zero ?
                _currentMotorTorque + (_deltaMotorTorque * Time.deltaTime) :
                _currentMotorTorque - (_deltaMotorTorque * Time.deltaTime);

            _currentMotorTorque = Mathf.Clamp(value, _carData.MinMotorTorque, _carData.MaxMotorTorque);
        }

        private void Move(Vector2 direction)
        {
            float wheelTorque = ZERO;

            if (_transmission.TransmissionLevel >= ZERO)
            {
                if (direction.y > 0)
                {
                    wheelTorque = Mathf.Abs(direction.y * (_currentMotorTorque / _transmission.CurrentGearRatio));
                }
            }
            else
            {
                if (direction.y < 0 && _transmission.TransmissionLevel == -2)
                {
                    wheelTorque = -Mathf.Abs(direction.y * (_currentMotorTorque / _carData.ReverseGearRatio));
                }
            }

            bool inMove = direction.y != ZERO;
            bool transmissionIsOnNeutral = _transmission.TransmissionLevel != -1;

            foreach (WheelCollider wheel in _movementWheels)
            {
                wheel.motorTorque = inMove ? wheelTorque : ZERO;

                if (inMove)
                {
                    wheel.brakeTorque = transmissionIsOnNeutral ? ZERO : _currentMotorTorque * NEUTRAL_MULTIPLIER;
                }
                else
                {
                    wheel.brakeTorque = _currentMotorTorque;
                }
            }
        }

        private void Steering(Vector2 direction)
        {
            float steerAngle;
            int level = _characteristics.GetSkillLevel(Skill.Driving);

            if (direction.x != ZERO)
            {
                steerAngle = _currentSteerAngle + (direction.x * _carData.RotationAngle * _drivingSkill.RotationSpeedMultiplier[level] * Time.deltaTime);
            }
            else
            {
                if (Mathf.Abs(_currentSteerAngle) > STEER_ANGLE_DELTA)
                {
                    if (_currentSteerAngle > ZERO)
                    {
                        steerAngle = _currentSteerAngle - (_carData.RotationAngle * _drivingSkill.RotationSpeedMultiplier[level] * Time.deltaTime);

                    }
                    else
                    {
                        steerAngle = _currentSteerAngle + (_carData.RotationAngle * _drivingSkill.RotationSpeedMultiplier[level] * Time.deltaTime);
                    }
                }
                else
                {
                    steerAngle = ZERO;
                }
            }

            _currentSteerAngle = Mathf.Clamp(steerAngle, -_carData.RotationAngle, _carData.RotationAngle);

            foreach (WheelCollider wheel in _steeringWheels)
            {
                wheel.steerAngle = _currentSteerAngle;
            }
        }

        private void Limit(Vector2 direction)
        {
            foreach (WheelCollider wheel in _wheels)
            {
                if (!wheel.isGrounded) return;
            }

            if (direction.y > ZERO)
            {
                if (_rigidbody.velocity.magnitude > (_carData.ForwardSpeed / Multiplier_From_MPS_to_KPH))
                {
                    _rigidbody.velocity = _rigidbody.velocity.normalized * (_carData.ForwardSpeed / Multiplier_From_MPS_to_KPH);
                }
            }
            else if (direction.y < ZERO)
            {
                bool movementAligned = Vector2.Dot(transform.forward, _rigidbody.velocity.normalized) < 0.0f;

                if (movementAligned && _rigidbody.velocity.magnitude > (_carData.BackwardSpeed / Multiplier_From_MPS_to_KPH))
                {
                    _rigidbody.velocity = _rigidbody.velocity.normalized * (_carData.BackwardSpeed / Multiplier_From_MPS_to_KPH);
                }
            }
        }
        #endregion

        #region Public API
        public void Init(CarData carData, Transmission transmission, Driving driving, Characteristics characteristics)
        {
            _carData = carData;
            _transmission = transmission;
            _drivingSkill = driving;
            _characteristics = characteristics;
        }
        #endregion
    }
}
