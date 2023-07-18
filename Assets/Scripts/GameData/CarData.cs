using System.Collections.Generic;
using UnityEngine;

namespace GameData.Vehicle
{
    [CreateAssetMenu(fileName = "Car Data", menuName = "Game Data/Vehicle/Car Data", order = 1)]
    public class CarData : ScriptableObject
    {
        [field: SerializeField, Min(0.0f)] public float ForwardSpeed { get; private set; }
        [field: SerializeField, Min(0.0f)] public float BackwardSpeed { get; private set; }
        [field: SerializeField, Min(0.0f)] public float RotationAngle { get; private set; }

        [field: Header("Motor data")]
        [field: SerializeField, Min(0.0f)] public float MinMotorTorque { get; private set; }
        [field: SerializeField, Min(0.0f)] public float MaxMotorTorque { get; private set; }
        [field: SerializeField, Min(0.0f)] public float MaxMotorTorqueTime { get; private set; }

        [field: Header("Gear data")]
        [field: SerializeField, Min(0.01f)] public float ReverseGearRatio { get; private set; }
        [field: SerializeField, Min(0.01f)] public List<float> GearRatios { get; private set; }
        [field: SerializeField, Min(0.0f)] public List<float> SpeedLimitGearRatios { get; private set; }
    }
}