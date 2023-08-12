using System.Collections.Generic;
using UnityEngine;
using Vehicle;

namespace Player
{
#if UNITY_EDITOR
    [DisallowMultipleComponent]
    [AddComponentMenu("Player/TowerRotator")]
#endif
    public sealed class TowerRotator : MonoBehaviour
    {
        #region Editor Fields
        [SerializeField] private List<Turret> _turrets = null;
        #endregion

        #region Fields
        private Camera _camera = null;

        private Vector2 _middleOfScreen;
        private int _playerLayer;
        private const float MAX_DISTANCE = 250.0f;
        private const string PLAYER_LAYER = "Player";
        #endregion

        #region MonoBehaviour API
        private void Awake()
        {
            _camera = Camera.main;
        }

        private void Start()
        {
            _middleOfScreen = new Vector2(Screen.width / 2, Screen.height / 2);
            _playerLayer = LayerMask.NameToLayer(PLAYER_LAYER);
        }

        private void Update()
        {
            SetAimForTurrets();
        }
        #endregion

        #region Methods
        private void SetAimForTurrets()
        {
            Ray ray = _camera.ScreenPointToRay(_middleOfScreen);
            Vector3 aimPoint = Physics.Raycast(ray, out RaycastHit hitInfo, MAX_DISTANCE, _playerLayer)
                ? hitInfo.point
                : ray.GetPoint(MAX_DISTANCE);

            foreach (Turret turret in _turrets)
            {
                turret.AimPosition = aimPoint;
            }
        }
        #endregion
    }
}