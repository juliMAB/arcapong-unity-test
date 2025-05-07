using UnityEngine;
using UnityEngine.Scripting;
using Quantum;
using Photon.Deterministic;
using Photon.Deterministic.Protocol;

namespace Tomorrow.Quantum
{
    [Preserve]
    public unsafe class AISystem : SystemMainThreadFilter<AISystem.Filter>
    {
        public struct Filter
        {
            public EntityRef Entity;
            public Transform3D* Paddle;
            public PhysicsBody3D* Body;
            public PlayerAI* ai;
        }

        // Referencia a la configuración

        public override void Update(Frame f, ref Filter filter)
        {
            AIConfig config = f.FindAsset<AIConfig>(f.RuntimeConfig.aIConfig.Id);
            if (config == null)
            {
                Debug.Log("ERROR!"); return;
            }
            foreach (var pair in f.GetComponentIterator<Ball>())
            {
                if (f.Unsafe.TryGetPointer<Transform3D>(pair.Entity, out Transform3D* ball))
                {
                    // Predicción de posición
                    FP targetPositionX = ball->Position.X;
                    if (f.Unsafe.TryGetPointer<PhysicsBody3D>(pair.Entity, out var ballBody))
                    {
                        targetPositionX += ballBody->Velocity.X * config.PredictionTime;
                    }

                    FP currentPositionX = filter.Paddle->Position.X;
                    FP positionDifference = targetPositionX - currentPositionX;
                    FP absoluteDifference = FPMath.Abs(positionDifference);

                    // Comportamiento basado en distancia
                    FP effectiveAcceleration = config.AccelerationFactor;
                    FP effectiveSpeedMultiplier = config.BaseSpeedMultiplier;

                    foreach (var behavior in config.DistanceBehaviors)
                    {
                        if (absoluteDifference > behavior.Distance)
                        {
                            effectiveAcceleration = behavior.Acceleration;
                            effectiveSpeedMultiplier = behavior.SpeedMultiplier;
                            break;
                        }
                    }

                    // Movimiento
                    if (absoluteDifference > config.DeadZone)
                    {
                        FP direction = positionDifference > FP._0 ? FP._1 : -FP._1;
                        FP speedMultiplier = FPMath.Clamp(
                            absoluteDifference * FP._2,
                            effectiveSpeedMultiplier,
                            config.MaxSpeedMultiplier * effectiveSpeedMultiplier
                        );

                        FP targetVelocity = f.RuntimeConfig.AIPaddleSpeed * speedMultiplier * direction;

                        filter.Body->Velocity = FPVector3.MoveTowards(
                            filter.Body->Velocity,
                            new FPVector3(targetVelocity, FP._0, FP._0),
                            effectiveAcceleration * f.DeltaTime * f.RuntimeConfig.AIPaddleSpeed
                        );
                    }
                    else
                    {
                        filter.Body->Velocity = FPVector3.MoveTowards(
                            filter.Body->Velocity,
                            FPVector3.Zero,
                            effectiveAcceleration * 2 * f.DeltaTime * f.RuntimeConfig.AIPaddleSpeed
                        );
                    }
                }
            }
        }
    }
}