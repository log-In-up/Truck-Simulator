using UnityEngine.SceneManagement;

namespace UserInterface
{
    public sealed class Splash : ScreenObserver
    {
        #region Fields

        #endregion

        #region Properties
        public override UIScreen Screen => UIScreen.SplashScreen;
        #endregion

        #region Overridden methods
        public override void Activate()
        {
            SceneManager.sceneLoaded += SceneLoaded;

            base.Activate();

            SceneManager.LoadScene((int)ScenesInBuild.Main);
        }

        public override void Deactivate()
        {
            SceneManager.sceneLoaded -= SceneLoaded;

            base.Deactivate();
        }

        public override void Setup()
        {
            base.Setup();
        }
        #endregion

        #region Event Handlers
        private void SceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            UICore.OpenScreen(UIScreen.MainMenuScreen);
        }
        #endregion
    }
}