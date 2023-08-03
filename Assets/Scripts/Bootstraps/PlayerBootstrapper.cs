using SkillSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Bootstraps
{
#if UNITY_EDITOR
    [DisallowMultipleComponent]
#endif
    public sealed class PlayerBootstrapper : MonoBehaviour
    {
        #region Editor Fields
        [SerializeField] private SkillLoader _skillLoader = null;
        [SerializeField] private InputActionAsset _actionAsset = null;
        #endregion

        #region Properties
        public Characteristics Characteristics { get; private set; }
        public InputActionAsset InputActionAsset => _actionAsset;
        public SkillLoader SkillLoader => _skillLoader;
        #endregion

        #region MonoBehaviour API
        private void Awake()
        {
            Characteristics = new Characteristics(_skillLoader);
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }
        #endregion
    }
}