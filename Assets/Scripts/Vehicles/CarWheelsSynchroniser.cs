using System.Collections.Generic;
using UnityEngine;

namespace Vehicle
{
#if UNITY_EDITOR
    [DisallowMultipleComponent]
#endif
    public sealed class CarWheelsSynchroniser : MonoBehaviour
    {
        #region Editor fields
        [SerializeField] private List<WheelCollider> _wheels = null;
        [SerializeField] private List<Transform> _wheelsView = null;
        #endregion

        #region Fields
        private List<Quaternion> _wheelsOffset = null;
        #endregion

        #region MonoBehaviour API
        private void Start()
        {
            _wheelsOffset = new List<Quaternion>();

            foreach (Transform wheelView in _wheelsView)
            {
                _wheelsOffset.Add(wheelView.localRotation);
            }
        }

        private void Update() => SynchroniseWheels();
        #endregion

        #region Methods
        private void SynchroniseWheels()
        {
            for (int index = 0; index < _wheels.Count; index++)
            {
                _wheels[index].GetWorldPose(out Vector3 position, out Quaternion rotation);

                _wheelsView[index].transform.SetPositionAndRotation(position, rotation * _wheelsOffset[index]);
            }
        }
        #endregion
    }
}