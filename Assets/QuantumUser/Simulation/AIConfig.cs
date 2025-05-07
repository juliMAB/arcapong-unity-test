namespace Quantum
{
    using Photon.Deterministic;
    using UnityEngine;
    [System.Serializable]
    public class AIConfig : AssetObject
    {
        [Header("Movement Settings")]
        [Tooltip("Base movement speed multiplier")]
        public FP BaseSpeedMultiplier = FP._1_01;

        [Tooltip("How quickly the paddle accelerates")]
        public FP AccelerationFactor = FP._0_50;

        [Tooltip("Dead zone where no movement occurs")]
        public FP DeadZone = FP._0_10;

        [Tooltip("Maximum speed multiplier when far from target")]
        public FP MaxSpeedMultiplier = FP._1_50;

        [Header("Advanced Settings")]
        [Tooltip("Time to predict ball position ahead")]
        public FP PredictionTime = FP._0_20;

        [Tooltip("Different behaviors for different distances")]
        public AIDistanceBehavior[] DistanceBehaviors;
    }
    [System.Serializable]
    public struct AIDistanceBehavior
    {
        [Tooltip("Distance threshold for this behavior")]
        public FP Distance;

        [Tooltip("Speed multiplier at this distance")]
        public FP SpeedMultiplier;

        [Tooltip("Acceleration factor at this distance")]
        public FP Acceleration;
    }
}
