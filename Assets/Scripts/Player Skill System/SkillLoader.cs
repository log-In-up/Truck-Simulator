using System.Collections.Generic;
using UnityEngine;

namespace SkillSystem
{
    [CreateAssetMenu(fileName = "Skill Loader", menuName = "Game Data/Player/Skill Loader", order = 1)]
    public sealed class SkillLoader : ScriptableObject
    {
        [field: SerializeField] public List<SkillData> Skills { get; private set; }

        public T GetSkill<T>(Skill skill) where T : SkillData
        {
            return Skills.Find(x => x.SkillType.Equals(skill)) as T;
        }
    }
}