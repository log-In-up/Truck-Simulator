using Input;
using TMPro;
using UnityEngine;
using InputAction = UnityEngine.InputSystem.InputAction;

namespace UserInterface
{
    public sealed class Game : ScreenObserver
    {
        #region Editor Fields
        [SerializeField] private TextMeshProUGUI _speedometer = null;
        [SerializeField] private TextMeshProUGUI _transmissionRatio = null;
        #endregion

        #region Fields
        private PlayerInput _input = null;
        #endregion

        #region Properties
        public TextMeshProUGUI Speedometer => _speedometer;
        public TextMeshProUGUI TransmissionRatio => _transmissionRatio;
        public override UIScreen Screen => UIScreen.GameScreen;
        #endregion

        #region Overridden methods
        public override void Activate()
        {
            _input.Enable();
            _input.Player.PlayerInfo.performed += OnPlayerInfo;

            base.Activate();
        }

        public override void Deactivate()
        {
            _input.Player.PlayerInfo.performed -= OnPlayerInfo;
            _input.Disable();

            base.Deactivate();
        }

        public override void Setup()
        {
            _input = new PlayerInput();

            base.Setup();
        }
        #endregion

        #region Event handlers
        private void OnPlayerInfo(InputAction.CallbackContext context)
        {
            UICore.OpenScreen(UIScreen.PlayerInfo);
        }
        #endregion
    }
}