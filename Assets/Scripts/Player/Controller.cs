using GameData.Vehicle;
using SkillSystem;
using UnityEngine;
using Bootstraps;

namespace Player
{
#if UNITY_EDITOR
    [DisallowMultipleComponent]
#endif
    public sealed class Controller : MonoBehaviour
    {
        #region Editor Fields
        [SerializeField] private CarData _carData = null;

        [SerializeField] private Transmission _transmission = null;
        [SerializeField] private Movement _movement = null;
        #endregion

        #region MonoBehaviour API
        private void Awake()
        {
            PlayerBootstrapper playerBootstrapper = FindObjectOfType<PlayerBootstrapper>();

            Driving driving = playerBootstrapper.SkillLoader.GetSkill<Driving>(Skill.Driving);

            _transmission.Init(_carData, driving, playerBootstrapper.Characteristics);
            _movement.Init(_carData, _transmission, driving, playerBootstrapper.Characteristics);
        }
        #endregion
    }
}