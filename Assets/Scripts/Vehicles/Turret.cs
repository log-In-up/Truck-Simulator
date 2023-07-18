using UnityEngine;

namespace Vehicle
{
#if UNITY_EDITOR
    [DisallowMultipleComponent]
#endif
    public sealed class Turret : MonoBehaviour
    {
        #region Editor fields
        [Header("Rotations")]
        [SerializeField] private Transform _turretBase = null;
        [SerializeField] private Transform _barrels = null;

        [Header("Elevation")]
        [SerializeField, Min(0.0f)] private float _elevationSpeed = 30f;
        [SerializeField, Min(0.0f)] private float _maxElevation = 60f;
        [SerializeField, Min(0.0f)] private float _maxDepression = 5f;

        [Header("Traverse")]
        [SerializeField] private bool _hasLimitedTraverse = false;
        [SerializeField, Min(0.0f)] private float _traverseSpeed = 60f;
        [SerializeField, Range(0.0f, 179.9f)] private float _leftLimit = 120f;
        [SerializeField, Range(0.0f, 179.9f)] private float _rightLimit = 120f;

        [Header("Behavior")]
        [SerializeField] private bool _isIdle = false;
#if UNITY_EDITOR
        [Header("Debug")]
        [SerializeField] private bool _drawDebugRay = true;
        [SerializeField] private bool _drawDebugArcs = false;
        [SerializeField, Min(0.0f)] private float kArcSize = 10.0f;
#endif
        #endregion

        #region Fields
        private bool _hasBarrels;
        private bool _isBaseAtRest, _isBarrelAtRest;

        private float _elevation;
        private float _limitedTraverseAngle;

        private Vector3 _aimPosition;
        #endregion

        #region Properties
        public bool IsTurretAtRest => _isBarrelAtRest && _isBaseAtRest;

        public Vector3 AimPosition { set => _aimPosition = value; }
        #endregion

        #region MonoBehaviour API
        private void Awake()
        {
            _hasBarrels = _barrels != null;
            if (_turretBase == null)
            {
                Debug.LogError($"{name} TurretAim requires an assigned TurretBase!");
            }
        }

        private void Start()
        {
            _isBaseAtRest = false;
            _isBarrelAtRest = false;

            _elevation = 0.0f;
            _limitedTraverseAngle = 0.0f;

            _aimPosition = Vector3.zero;
        }

        private void Update()
        {
            if (_isIdle)
            {
                if (!IsTurretAtRest)
                {
                    RotateTurretToIdle();
                }
            }
            else
            {
                RotateBaseToFaceTarget(_aimPosition);

                if (_hasBarrels)
                    RotateBarrelsToFaceTarget(_aimPosition);

                _isBarrelAtRest = false;
                _isBaseAtRest = false;
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (!_drawDebugArcs)
                return;

            if (_turretBase != null)
            {
                Color colorTraverse = new Color(1f, .5f, .5f, .1f);

                Transform arcRoot = _barrels != null ? _barrels : _turretBase;

                // Red traverse arc
                UnityEditor.Handles.color = colorTraverse;
                if (_hasLimitedTraverse)
                {
                    UnityEditor.Handles.DrawSolidArc(
                        arcRoot.position, _turretBase.up,
                        transform.forward, _rightLimit,
                        kArcSize);
                    UnityEditor.Handles.DrawSolidArc(
                        arcRoot.position, _turretBase.up,
                        transform.forward, -_leftLimit,
                        kArcSize);
                }
                else
                {
                    UnityEditor.Handles.DrawSolidArc(
                        arcRoot.position, _turretBase.up,
                        transform.forward, 360.0f,
                        kArcSize);
                }
            }

            if (_barrels != null)
            {
                Color colorElevation = new Color(.5f, 1f, .5f, .1f);
                Color colorDepression = new Color(.5f, .5f, 1f, .1f);

                // Green elevation arc
                UnityEditor.Handles.color = colorElevation;
                UnityEditor.Handles.DrawSolidArc(
                    _barrels.position, _barrels.right,
                    _turretBase.forward, -_maxElevation,
                    kArcSize);

                // Blue depression arc
                UnityEditor.Handles.color = colorDepression;
                UnityEditor.Handles.DrawSolidArc(
                    _barrels.position, _barrels.right,
                    _turretBase.forward, _maxDepression,
                    kArcSize);
            }
        }
#endif
        #endregion

        #region Methods
        private void RotateTurretToIdle()
        {
            if (_hasLimitedTraverse)
            {
                _limitedTraverseAngle = Mathf.MoveTowards(_limitedTraverseAngle, 0.0f, _traverseSpeed * Time.deltaTime);

                if (Mathf.Abs(_limitedTraverseAngle) > Mathf.Epsilon)
                {
                    _turretBase.localEulerAngles = Vector3.up * _limitedTraverseAngle;
                }
                else
                {
                    _isBaseAtRest = true;
                }
            }
            else
            {
                _turretBase.rotation = Quaternion.RotateTowards(_turretBase.rotation, transform.rotation,
                    _traverseSpeed * Time.deltaTime);

                _isBaseAtRest = Mathf.Abs(_turretBase.localEulerAngles.y) < Mathf.Epsilon;
            }

            if (_hasBarrels)
            {
                _elevation = Mathf.MoveTowards(_elevation, 0.0f, _elevationSpeed * Time.deltaTime);

                if (Mathf.Abs(_elevation) > Mathf.Epsilon)
                {
                    _barrels.localEulerAngles = Vector3.right * -_elevation;
                }
                else
                {
                    _isBarrelAtRest = true;
                }
            }
            else
            {
                _isBarrelAtRest = true;
            }
        }

        private void RotateBarrelsToFaceTarget(Vector3 targetPosition)
        {
            Vector3 localTargetPos = _turretBase.InverseTransformDirection(targetPosition - _barrels.position);
            Vector3 flattenedVecForBarrels = Vector3.ProjectOnPlane(localTargetPos, Vector3.up);

            float targetElevation = Vector3.Angle(flattenedVecForBarrels, localTargetPos);
            targetElevation *= Mathf.Sign(localTargetPos.y);

            targetElevation = Mathf.Clamp(targetElevation, -_maxDepression, _maxElevation);
            _elevation = Mathf.MoveTowards(_elevation, targetElevation, _elevationSpeed * Time.deltaTime);

            if (Mathf.Abs(_elevation) > Mathf.Epsilon)
                _barrels.localEulerAngles = Vector3.right * -_elevation;

#if UNITY_EDITOR
            if (_drawDebugRay)
                Debug.DrawRay(_barrels.position, _barrels.forward * localTargetPos.magnitude, Color.red);
#endif
        }

        private void RotateBaseToFaceTarget(Vector3 targetPosition)
        {
            Vector3 turretUp = transform.up;

            Vector3 directionToTarget = targetPosition - _turretBase.position;
            Vector3 flattenedVecForBase = Vector3.ProjectOnPlane(directionToTarget, turretUp);

            if (_hasLimitedTraverse)
            {
                Vector3 turretForward = transform.forward;
                float targetTraverse = Vector3.SignedAngle(turretForward, flattenedVecForBase, turretUp);

                targetTraverse = Mathf.Clamp(targetTraverse, -_leftLimit, _rightLimit);
                _limitedTraverseAngle = Mathf.MoveTowards(
                    _limitedTraverseAngle,
                    targetTraverse,
                    _traverseSpeed * Time.deltaTime);

                if (Mathf.Abs(_limitedTraverseAngle) > Mathf.Epsilon)
                    _turretBase.localEulerAngles = Vector3.up * _limitedTraverseAngle;
            }
            else
            {
                _turretBase.rotation = Quaternion.RotateTowards(
                    Quaternion.LookRotation(_turretBase.forward, turretUp),
                    Quaternion.LookRotation(flattenedVecForBase, turretUp),
                    _traverseSpeed * Time.deltaTime);
            }

#if UNITY_EDITOR
            if (_drawDebugRay && !_hasBarrels)
                Debug.DrawRay(_turretBase.position,
                    _turretBase.forward * flattenedVecForBase.magnitude,
                    Color.red);
#endif
        }
        #endregion
    }
}
