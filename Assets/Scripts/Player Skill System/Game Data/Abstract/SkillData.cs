using UnityEngine;

namespace SkillSystem
{
    public abstract class SkillData : ScriptableObject
    {
        #region Basic data
        [field: Header("Basic data")]
        [field: SerializeField] public Skill SkillType { get; private set; }
        [field: SerializeField, Min(0)] public int SkillMaxLevel { get; private set; }
        #endregion
    }
}