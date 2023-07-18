using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UserInterface
{
    public sealed class PlayerInfo : ScreenObserver
    {
        #region Editor Fields

        #endregion

        #region Properties
        public override UIScreen Screen => UIScreen.PlayerInfo;
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