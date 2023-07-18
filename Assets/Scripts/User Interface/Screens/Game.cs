using TMPro;
using UnityEngine;

namespace UserInterface
{
    public sealed class Game : ScreenObserver
    {
        #region Editor Fields
        [SerializeField] private TextMeshProUGUI _speedometer = null;
        [SerializeField] private TextMeshProUGUI _transmissionRatio = null;
        #endregion

        #region Properties
        public TextMeshProUGUI Speedometer => _speedometer;
        public TextMeshProUGUI TransmissionRatio => _transmissionRatio;
        public override UIScreen Screen => UIScreen.GameScreen;
        #endregion

        #region Overridden methods
        public override void Activate()
        {
            base.Activate();
        }

        public override void Deactivate()
        {
            base.Deactivate();
        }

        public override void Setup()
        {
            base.Setup();
        }
        #endregion
    }
}