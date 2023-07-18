using Player;
using UnityEngine;
using UserInterface;

namespace Mediator
{
#if UNITY_EDITOR
    [DisallowMultipleComponent]
#endif
    public sealed class DataSender : MonoBehaviour
    {
        #region Fields
        private Game _gameScreen = null;
        private Movement _movement = null;
        private Transmission _transmission = null;
        #endregion

        #region MonoBehaviour API
        private void Start()
        {
            _gameScreen = FindObjectOfType<Game>();
            _movement = FindObjectOfType<Movement>();
            _transmission = FindObjectOfType<Transmission>();

            if (_transmission != null)
            {
                _transmission.OnChangeGearRatio += OnChangeGearRatio;
                OnChangeGearRatio(_transmission.TransmissionLevel);
            }
        }

        private void FixedUpdate()
        {
            if (_gameScreen == null) return;

            if (_movement != null)
            {
                _gameScreen.Speedometer.text = $"{_movement.CurrentSpeedInKPH:f1} km/h";
            }
        }

        private void OnDisable()
        {
            if (_transmission != null)
            {
                _transmission.OnChangeGearRatio -= OnChangeGearRatio;
            }
        }
        #endregion

        #region Event Handlers
        private void OnChangeGearRatio(int ratio)
        {
            string text;

            if (ratio >= 0)
            {
                text = $"{ratio + 1}";
            }
            else
            {
                text = ratio == -2 ? "R" : "N";
            }

            _gameScreen.TransmissionRatio.text = text;
        }
        #endregion
    }
}