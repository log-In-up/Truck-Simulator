using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UserInterface
{
    public sealed class Main : ScreenObserver
    {
        #region Fields
        [SerializeField] private Button _play = null;
        #endregion

        #region Properties
        public override UIScreen Screen => UIScreen.MainMenuScreen;
        #endregion

        #region Overridden methods
        public override void Activate()
        {
            _play.interactable = true;

            _play.onClick.AddListener(OnClickPlay);
            SceneManager.sceneLoaded += SceneLoaded;

            base.Activate();
        }

        public override void Deactivate()
        {
            _play.onClick.RemoveListener(OnClickPlay);
            SceneManager.sceneLoaded -= SceneLoaded;

            base.Deactivate();
        }

        public override void Setup()
        {
            base.Setup();
        }
        #endregion

        #region Event Handlers
        private void OnClickPlay()
        {
            _play.interactable = false;

            SceneManager.LoadScene((int)ScenesInBuild.Game);
        }

        private void SceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            UICore.OpenScreen(UIScreen.GameScreen);
        }
        #endregion
    }
}