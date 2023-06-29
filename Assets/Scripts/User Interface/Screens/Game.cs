using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UserInterface
{
    public sealed class Game : ScreenObserver
    {
        #region Fields

        #endregion

        #region Properties
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