using SkillSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
#if UNITY_EDITOR
    [DisallowMultipleComponent]
#endif
    public sealed class SkillUpgradeHandler : MonoBehaviour
    {
        #region Editor fields
        [SerializeField] private Button _decrease = null, _increase = null;
        [SerializeField] private TextMeshProUGUI _level = null;
        #endregion

        #region Fields
        private Characteristics _characteristics = null;
        private Skill _skill;
        #endregion

        #region Properties
        private bool SkillLevelCanBeRaised => _characteristics.SkillLevelCanBeIncreased(_skill);
        private bool SkillLevelCanBeLowered => _characteristics.SkillLevelCanBeLowered(_skill);
        #endregion

        #region Public API
        public void Init(Skill skill, Characteristics characteristics)
        {
            _skill = skill;
            _characteristics = characteristics;

            _level.text = $"{characteristics.GetSkillLevel(_skill) + 1}";

            UpdateButtons();
        }

        public void Activate()
        {
            _decrease.onClick.AddListener(OnClickDecrease);
            _increase.onClick.AddListener(OnClickIncrease);
        }

        public void Deactivate()
        {
            _decrease.onClick.RemoveListener(OnClickDecrease);
            _increase.onClick.RemoveListener(OnClickIncrease);
        }
        #endregion

        #region Methods
        private void UpdateButtons()
        {
            _decrease.interactable = SkillLevelCanBeLowered;
            _increase.interactable = SkillLevelCanBeRaised;
        }
        #endregion

        #region Event Handlers
        private void OnClickDecrease()
        {
            _characteristics.ReduceSkillLevel(_skill);
            _level.text = $"{_characteristics.GetSkillLevel(_skill) + 1}";

            UpdateButtons();
        }

        private void OnClickIncrease()
        {
            _characteristics.IncreaseSkillLevel(_skill);
            _level.text = $"{_characteristics.GetSkillLevel(_skill) + 1}";

            UpdateButtons();
        }
        #endregion
    }
}