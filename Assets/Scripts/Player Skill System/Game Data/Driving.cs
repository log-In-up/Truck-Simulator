using System;
using UnityEngine;

namespace SkillSystem
{
    [CreateAssetMenu(fileName = "Driving Skill Data", menuName = "Game Data/Player/Characteristics/Driving", order = 1)]
    public sealed class Driving : SkillData
    {
        #region Driving skill data
        [field: Header("Driving skill data")]
        [field: SerializeField, Min(0)] public float[] EarlyGearShiftMultiplier { get; private set; }
        [field: SerializeField, Min(0)] public float[] RotationSpeedMultiplier { get; private set; }
        #endregion

        #region Editor
        private void OnValidate()
        {
            if (EarlyGearShiftMultiplier.Length != SkillMaxLevel)
            {
                EarlyGearShiftMultiplier = (float[])ResizeArray(EarlyGearShiftMultiplier, SkillMaxLevel);
            }

            if (RotationSpeedMultiplier.Length != SkillMaxLevel)
            {
                RotationSpeedMultiplier = (float[])ResizeArray(RotationSpeedMultiplier, SkillMaxLevel);
            }
        }

        public static Array ResizeArray(Array oldArray, int newSize)
        {
            int oldSize = oldArray.Length;
            Type elementType = oldArray.GetType().GetElementType();
            Array newArray = Array.CreateInstance(elementType, newSize);

            int preserveLength = Math.Min(oldSize, newSize);

            if (preserveLength > 0)
            {
                Array.Copy(oldArray, newArray, preserveLength);
            }

            return newArray;
        }
        #endregion
    }
}