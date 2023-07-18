using GameData.Vehicle;
using Input;
using SkillSystem;
using UnityEngine;
using InputAction = UnityEngine.InputSystem.InputAction;

namespace Player
{
#if UNITY_EDITOR
    [DisallowMultipleComponent, RequireComponent(typeof(Rigidbody))]
#endif
    public sealed class Transmission : MonoBehaviour
    {
        #region Fields
        private int _currentTransmissionLevel;

        private CarData _carData = null;
        private Driving _drivingSkill = null;
        private Characteristics _characteristics;
        private Rigidbody _rigidbody = null;
        private PlayerInput _input = null;

        private const float Multiplier_From_MPS_to_KPH = 3.6f, STATIONARY_TOLERANCE = 0.005f;
        private const int NEUTRAL_TRANSMISSION_lEVEL = -1, REVERSE_TRANSMISSION_LEVEL = -2;
        #endregion

        #region Properties
        public float CurrentGearRatio => _carData.GearRatios[_currentTransmissionLevel];
        public int TransmissionLevel => _currentTransmissionLevel;
        private bool IsStationary => _rigidbody.velocity.sqrMagnitude < (STATIONARY_TOLERANCE * STATIONARY_TOLERANCE);
        #endregion

        #region Events
        public delegate void GearRationAction(int ratio);
        public event GearRationAction OnChangeGearRatio;
        #endregion

        #region MonoBehaviour API
        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _input = new PlayerInput();
        }

        private void OnEnable()
        {
            _input.Enable();

            _input.Player.IncreaseGearRatio.performed += OnIncreaseGearRatio;
            _input.Player.ReduceGearRatio.performed += OnReduceGearRatio;
        }

        private void Start()
        {
            _currentTransmissionLevel = NEUTRAL_TRANSMISSION_lEVEL;
        }

        private void OnDisable()
        {
            _input.Player.IncreaseGearRatio.performed -= OnIncreaseGearRatio;
            _input.Player.ReduceGearRatio.performed -= OnReduceGearRatio;

            _input.Disable();
        }
        #endregion

        #region Event Handlers
        private void OnIncreaseGearRatio(InputAction.CallbackContext callbackContext)
        {
            if (_currentTransmissionLevel + 1 >= _carData.GearRatios.Count) return;

            if (_currentTransmissionLevel >= 0)
            {
                int level = _characteristics.GetSkillLevel(Skill.Driving);
                float multiplier = _drivingSkill.EarlyGearShiftMultiplier[level];
                float speedLimit = (1.0f - multiplier) * _carData.SpeedLimitGearRatios[_currentTransmissionLevel];

                if ((_rigidbody.velocity.magnitude * Multiplier_From_MPS_to_KPH) >= speedLimit)
                {
                    _currentTransmissionLevel++;
                }
            }
            else
            {
                if (_currentTransmissionLevel == REVERSE_TRANSMISSION_LEVEL)
                {
                    if (IsStationary)
                    {
                        _currentTransmissionLevel++;
                    }
                }
                else
                {
                    _currentTransmissionLevel++;
                }
            }
            OnChangeGearRatio?.Invoke(_currentTransmissionLevel);
        }

        private void OnReduceGearRatio(InputAction.CallbackContext callbackContext)
        {
            if (_currentTransmissionLevel <= REVERSE_TRANSMISSION_LEVEL) return;

            if (_currentTransmissionLevel - 1 == REVERSE_TRANSMISSION_LEVEL && IsStationary)
            {
                _currentTransmissionLevel--;
            }
            else if (_currentTransmissionLevel - 1 >= NEUTRAL_TRANSMISSION_lEVEL)
            {
                _currentTransmissionLevel--;
            }
            OnChangeGearRatio?.Invoke(_currentTransmissionLevel);
        }
        #endregion

        #region Public API
        public void Init(CarData carData, Driving driving, Characteristics characteristics)
        {
            _carData = carData;
            _drivingSkill = driving;
            _characteristics = characteristics;
        }
        #endregion
    }
}