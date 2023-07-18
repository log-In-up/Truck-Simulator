using UnityEngine;

namespace SkillSystem
{
    [CreateAssetMenu(fileName = "Driving Skill Data", menuName = "Game Data/Player/Characteristics/Driving", order = 1)]
    public sealed class Driving : SkillData
    {
        #region Driving skill data
        [field: Header("Driving skill data")]
        [field: SerializeField, Min(0)] public float[] EarlyGearShiftMultiplier { get; private set; }
        #endregion

        #region Editor
        private void OnValidate()
        {
            if (EarlyGearShiftMultiplier.Length != SkillMaxLevel)
            {
                EarlyGearShiftMultiplier = new float[SkillMaxLevel];
            }
        }
        #endregion
    }
}