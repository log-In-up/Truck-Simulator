using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
#if UNITY_EDITOR
    [DisallowMultipleComponent, RequireComponent(typeof(Rigidbody))]
    [AddComponentMenu("Enemy/Movement Agent")]
#endif
    public sealed class MovementAgent : MonoBehaviour
    {
        #region Editor fields
        [Header("Car Wheels")]
        [SerializeField] private List<WheelCollider> _steeringWheels = null;
        [SerializeField] private List<WheelCollider> _movementWheels = null;

        [Header("Car Front (Transform)")]
        [SerializeField] private Transform _carFront = null;
        [SerializeField, Min(0.0f)] private float _aiFov = 60.0f;

        [Header("General Parameters")]
        [SerializeField] private List<string> _navMeshLayers = null;
        [SerializeField] private Transform _centerOfMass = null;
        [SerializeField, Min(0.0f)] private float _maxSteeringAngle = 45.0f;
        [SerializeField, Min(0.0f)] private float _maxRPM = 150.0f;

        [Header("Destination Parameters")]
        [SerializeField] private Transform _customDestination;
        #endregion

        #region Fields
        private bool _allowMovement;
        private int _currentWayPoint;
        private int _navMeshLayerBite;
        private float _localMaxSpeed, _movementTorque;
        private Vector3 _postionToFollow;
        private List<Vector3> _waypoints;

        private const string ALL_AREAS = "AllAreas";
        #endregion

        #region Properties
        public bool Move { get; set; }
        #endregion

        #region MonoBehaviour API        
        private void Awake()
        {
            GetComponent<Rigidbody>().centerOfMass = _centerOfMass.localPosition;

            _waypoints = new List<Vector3>();
        }

        private void Start()
        {
            _currentWayPoint = 0;
            _movementTorque = 1;
            _postionToFollow = Vector3.zero;
            _allowMovement = true;
            Move = true;

            CalculateNavMashLayerBite();
        }

        private void FixedUpdate()
        {
            ApplySteering();
            PathProgress();
        }
        #endregion

        #region Methods
        private void ApplyBrakes()
        {
            foreach (WheelCollider wheel in _movementWheels)
            {
                wheel.brakeTorque = 5000;
            }
        }

        private void ApplySteering()
        {
            Vector3 relativeVector = transform.InverseTransformPoint(_postionToFollow);
            float steeringAngle = (relativeVector.x / relativeVector.magnitude) * _maxSteeringAngle;

            if (steeringAngle > 15.0f)
            {
                _localMaxSpeed = 100.0f;
            }
            else
            {
                _localMaxSpeed = _maxRPM;
            }

            foreach (WheelCollider wheel in _steeringWheels)
            {
                wheel.steerAngle = steeringAngle;
            }
        }

        private void CalculateNavMashLayerBite()
        {
            if (_navMeshLayers == null || _navMeshLayers[0] == ALL_AREAS)
            {
                _navMeshLayerBite = NavMesh.AllAreas;
            }
            else if (_navMeshLayers.Count == 1)
            {
                _navMeshLayerBite += 1 << NavMesh.GetAreaFromName(_navMeshLayers[0]);
            }
            else
            {
                foreach (string layer in _navMeshLayers)
                {
                    _navMeshLayerBite += 1 << NavMesh.GetAreaFromName(layer);
                }
            }
        }

        private bool CheckForAngle(Vector3 position, Vector3 source, Vector3 direction)
        {
            Vector3 distance = (position - source).normalized;
            float cosAngle = Vector3.Dot(distance, direction);
            float angle = Mathf.Acos(cosAngle) * Mathf.Rad2Deg;

            return angle < _aiFov;
        }

        private void Movement()
        {
            _allowMovement = Move && _allowMovement;

            if (_allowMovement)
            {
                foreach (WheelCollider wheel in _movementWheels)
                {
                    wheel.brakeTorque = 0;
                }

                float allRpm = 0.0f;
                foreach (WheelCollider wheel in _movementWheels)
                {
                    allRpm += wheel.rpm;
                }

                float speedOfWheels = allRpm / _movementWheels.Count;

                if (speedOfWheels < _localMaxSpeed)
                {
                    float value = 400 * _movementTorque;

                    foreach (WheelCollider wheel in _movementWheels)
                    {
                        wheel.motorTorque = value;
                    }
                }
                else if (speedOfWheels < _localMaxSpeed + (_localMaxSpeed * 1 / _movementWheels.Count))
                {
                    foreach (WheelCollider wheel in _movementWheels)
                    {
                        wheel.motorTorque = 0;
                    }
                }
                else
                {
                    ApplyBrakes();
                }
            }
            else
            {
                ApplyBrakes();
            }
        }

        private void PathProgress()
        {
            WayPointManager();
            Movement();
            ListOptimizer();

            void CreatePath()
            {
                if (_customDestination == null)
                {
                    _allowMovement = false;
                }
                else
                {
                    CustomPath(_customDestination.position);
                }
            }

            void ListOptimizer()
            {
                if (_currentWayPoint > 1 && _waypoints.Count > 30)
                {
                    _waypoints.RemoveAt(0);
                    _currentWayPoint--;
                }
            }

            void WayPointManager()
            {
                if (_currentWayPoint >= _waypoints.Count)
                {
                    _allowMovement = false;
                }
                else
                {
                    _postionToFollow = _waypoints[_currentWayPoint];
                    _allowMovement = true;
                    if (Vector3.Distance(_carFront.position, _postionToFollow) < 2)
                    {
                        _currentWayPoint++;
                    }
                }

                if (_currentWayPoint >= _waypoints.Count - 3)
                {
                    CreatePath();
                }
            }
        }
        #endregion

        #region Public API        
        public void CustomPath(Vector3 destination)
        {
            NavMeshPath path = new NavMeshPath();
            Vector3 sourcePostion;

            if (_waypoints.Count == 0)
            {
                sourcePostion = _carFront.position;
                Calculate(destination, sourcePostion, _carFront.forward, _navMeshLayerBite);
            }
            else
            {
                sourcePostion = _waypoints[^1];
                Vector3 direction = (_waypoints[^1] - _waypoints[_waypoints.Count - 2]).normalized;
                Calculate(destination, sourcePostion, direction, _navMeshLayerBite);
            }

            void Calculate(Vector3 destination, Vector3 source, Vector3 direction, int NavMeshAreaBite)
            {
                if (NavMesh.SamplePosition(destination, out NavMeshHit hit, 150, NavMeshAreaBite) &&
                    NavMesh.CalculatePath(source, hit.position, NavMeshAreaBite, path))
                {
                    if (path.corners.ToList().Count() > 1 && CheckForAngle(path.corners[1], source, direction))
                    {
                        _waypoints.AddRange(path.corners.ToList());
                    }
                    else
                    {
                        if (path.corners.Length > 2 && CheckForAngle(path.corners[2], source, direction))
                        {
                            _waypoints.AddRange(path.corners.ToList());
                        }
                    }
                }
            }
        }
        #endregion
    }
}