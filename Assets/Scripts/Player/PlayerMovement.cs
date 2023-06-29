using GameData.Vehicle;
using Input;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Player
{
#if UNITY_EDITOR
    [DisallowMultipleComponent, RequireComponent(typeof(Rigidbody))]
#endif
    public sealed class PlayerMovement : MonoBehaviour
    {
        #region Editor fields
        [SerializeField] private CarData _carData = null;
        [SerializeField] private Transform _centerOfMass = null;
        [SerializeField] private List<WheelCollider> _steeringWheels = null;
        [SerializeField] private List<WheelCollider> _movementWheels = null;
        #endregion

        #region Fields
        private PlayerInput _input = null;
        private Rigidbody _rigidbody = null;
        private List<WheelCollider> _wheels = null;

        private const float ZERO = 0.0f;
        private const float Multiplier_From_MPS_to_KPH = 3.6f;
        #endregion

        #region MonoBehaviour API
        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.centerOfMass = _centerOfMass.localPosition;
            _input = new PlayerInput();

            _wheels = new List<WheelCollider>();
            _wheels.AddRange(_steeringWheels);
            _wheels.AddRange(_movementWheels);
        }

        private void OnEnable()
        {
            _input.Enable();
        }

        private void Update()
        {
            Vector2 direction = _input.Player.Move.ReadValue<Vector2>();

            Movement(direction);
            Steering(direction);
            Limit(direction);
        }

        private void OnDisable()
        {
            _input.Disable();
        }
        #endregion

        #region Methods
        private void Movement(Vector2 direction)
        {
            bool inMove = direction.y != ZERO;

            foreach (WheelCollider wheel in _movementWheels)
            {
                wheel.motorTorque = inMove ? direction.y * _carData.MotorTorque : ZERO;

                wheel.brakeTorque = inMove ? ZERO : _carData.BrakeTorque;
            }
        }

        private void Steering(Vector2 direction)
        {
            foreach (WheelCollider wheel in _steeringWheels)
            {
                wheel.steerAngle = direction.x * _carData.RotationAngle;
            }
        }

        private void Limit(Vector2 direction)
        {
            foreach (WheelCollider wheel in _wheels)
            {
                if (!wheel.isGrounded) return;
            }

            if (_rigidbody.velocity.magnitude > (_carData.ForwardSpeed / Multiplier_From_MPS_to_KPH) && direction.y > ZERO)
            {
                _rigidbody.velocity = _rigidbody.velocity.normalized * (_carData.ForwardSpeed / Multiplier_From_MPS_to_KPH);
            }
            else if (_rigidbody.velocity.magnitude > (_carData.BackwardSpeed / Multiplier_From_MPS_to_KPH) && direction.y < ZERO)
            {
                _rigidbody.velocity = _rigidbody.velocity.normalized * (_carData.BackwardSpeed / Multiplier_From_MPS_to_KPH);
            }
        }
        #endregion
    }
}
