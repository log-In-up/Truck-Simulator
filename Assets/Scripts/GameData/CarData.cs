using UnityEngine;

namespace GameData.Vehicle
{
    [CreateAssetMenu(fileName = "Car Data", menuName = "Game Data/Vehicle/Car Data", order = 1)]
    public sealed class CarData : ScriptableObject
    {
        [field: SerializeField, Min(0.0f)] public float MotorTorque { get; private set; }
        [field: SerializeField, Min(0.0f)] public float ForwardSpeed { get; private set; }
        [field: SerializeField, Min(0.0f)] public float BackwardSpeed { get; private set; }
        [field: SerializeField, Min(0.0f)] public float BrakeTorque { get; private set; }
        [field: SerializeField, Min(0.0f)] public float RotationAngle { get; private set; }
    }
}