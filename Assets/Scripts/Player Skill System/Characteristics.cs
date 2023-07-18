using System.Collections.Generic;
using UnityEngine;

namespace SkillSystem
{
#if UNITY_EDITOR
    [DisallowMultipleComponent]
#endif
    public sealed class Characteristics
    {
        #region Fields
        private readonly Dictionary<Skill, int> _maxSkillLevel;
        private Dictionary<Skill, int> _currentSkillLevel;
        #endregion

        public Characteristics(SkillLoader skillLoader)
        {
            _maxSkillLevel = new Dictionary<Skill, int>();

            foreach (SkillData skill in skillLoader.Skills)
            {
                _maxSkillLevel.Add(skill.SkillType, skill.SkillMaxLevel);
            }

            _currentSkillLevel = new Dictionary<Skill, int>()
            {
                [Skill.Driving] = 5,
                [Skill.Gunsmith] = 0,
                [Skill.Mechanic] = 0,
                [Skill.Trade] = 0
            };
        }

        #region Public API
        public void ReduceSkillLevel(Skill skill) => _currentSkillLevel[skill]--;

        public void IncreaseSkillLevel(Skill skill) => _currentSkillLevel[skill]++;

        public bool SkillLevelCanBeLowered(Skill skill) => _currentSkillLevel[skill] > 0;

        public bool SkillLevelCanBeIncreased(Skill skill) => _currentSkillLevel[skill] < _maxSkillLevel[skill];

        public int GetSkillLevel(Skill skill) => _currentSkillLevel[skill];
        #endregion
    }
}