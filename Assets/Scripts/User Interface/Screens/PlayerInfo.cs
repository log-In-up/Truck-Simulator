using Bootstraps;
using SkillSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using InputAction = UnityEngine.InputSystem.InputAction;

namespace UserInterface
{
    public sealed class PlayerInfo : ScreenObserver
    {
        #region Editor Fields
        [SerializeField] private SkillUpgradeHandler _drivingSkillHandler = null;
        [SerializeField] private SkillUpgradeHandler _gunsmithSkillHandler = null;
        [SerializeField] private SkillUpgradeHandler _mechanicSkillHandler = null;
        [SerializeField] private SkillUpgradeHandler _tradeSkillHandler = null;
        #endregion

        #region Fields
        private Input.PlayerInput _input = null;

        private InputAction _lookInputAction = null;
        #endregion

        #region Properties
        public override UIScreen Screen => UIScreen.PlayerInfo;
        #endregion

        #region Overridden methods
        public override void Activate()
        {
            _drivingSkillHandler.Activate();
            _gunsmithSkillHandler.Activate();
            _mechanicSkillHandler.Activate();
            _tradeSkillHandler.Activate();

            _input.Enable();
            _input.Player.PlayerInfo.performed += OnPlayerInfo;

            _lookInputAction.Disable();

            base.Activate();
        }

        public override void Deactivate()
        {
            _drivingSkillHandler.Deactivate();
            _gunsmithSkillHandler.Deactivate();
            _mechanicSkillHandler.Deactivate();
            _tradeSkillHandler.Deactivate();

            _input.Player.PlayerInfo.performed -= OnPlayerInfo;
            _input.Disable();

            _lookInputAction.Enable();

            base.Deactivate();
        }

        public override void Setup()
        {
            PlayerBootstrapper playerBootstrapper = FindObjectOfType<PlayerBootstrapper>();

            _drivingSkillHandler.Init(Skill.Driving, playerBootstrapper.Characteristics);
            _gunsmithSkillHandler.Init(Skill.Gunsmith, playerBootstrapper.Characteristics);
            _mechanicSkillHandler.Init(Skill.Mechanic, playerBootstrapper.Characteristics);
            _tradeSkillHandler.Init(Skill.Trade, playerBootstrapper.Characteristics);

            _input = new Input.PlayerInput();

            InputActionMap playerActionMap = playerBootstrapper.InputActionAsset.FindActionMap("Player");
            _lookInputAction = playerActionMap.FindAction("Look");

            base.Setup();
        }
        #endregion

        #region Event handlers
        private void OnPlayerInfo(InputAction.CallbackContext context)
        {
            UICore.OpenScreen(UIScreen.GameScreen);
        }
        #endregion
    }
}