using UnityEngine;
using UnityEngine.Scripting;
using Quantum;
using Photon.Deterministic;

namespace Tomorrow.Quantum
{
    [Preserve]
    public unsafe class CollisionSystem : SystemSignalsOnly, ISignalOnCollisionEnter3D, ISignalOnTriggerEnter3D
    {
        public void OnTriggerEnter3D(Frame f, TriggerInfo3D info)
        {
            if (f.Unsafe.TryGetPointer<Ball>(info.Entity, out Ball* ball))
            {
                if (f.Unsafe.TryGetPointer<Transform3D>(info.Entity, out Transform3D* ballTransform))
                {
                    if (f.Unsafe.TryGetPointer<Transform3D>(info.Other, out Transform3D* otherTransform))
                    {
                        if (f.Unsafe.TryGetPointer<Block>(info.Other, out Block* block))
                        {
                            FPVector3 direction = (ballTransform->Position - otherTransform->Position).Normalized;

                            ball->Velocity = FPVector3.Reflect(ball->Velocity, direction);

                            ball->Velocity = FPVector3.Normalize(ball->Velocity) * f.RuntimeConfig.BallSpeed;

                            f.Signals.OnScoreChanged(info.Entity, info.Other);

                            f.Signals.OnBlockBreak(info.Other, info.Entity);
                        }
                    }
                }
            }
        }
    
      
        public void OnCollisionEnter3D(Frame f, CollisionInfo3D info)
        {
            if (f.Unsafe.TryGetPointer<Ball>(info.Entity, out Ball *ball))
            {
                if (f.Unsafe.TryGetPointer<Transform3D>(info.Entity, out Transform3D *ballTransform))
                {
                    if (f.Unsafe.TryGetPointer<Transform3D>(info.Other, out Transform3D *otherTransform))
                    {
                        // collide with paddles
                        if (f.Unsafe.TryGetPointer<Paddle>(info.Other, out Paddle *paddle))
                        {
                            Debug.Log($"Ball collided with paddle {paddle->Index}");
                            var direction = FPVector3.Normalize(
                                info.ContactNormal + 
                                (paddle->Index == 0 ? 1 : -1) * (ballTransform->Position.X - otherTransform->Position.X) * FPVector3.Right
                            );
                            ball->Velocity = f.RuntimeConfig.BallSpeed * direction;
                            ball->Paddle = info.Other;
                        }
                        // collide with walls
                        if (f.Unsafe.TryGetPointer<Wall>(info.Other, out _))
                        {
                            var direction = FPVector3.Normalize(
                                FPVector3.Reflect(ball->Velocity, info.ContactNormal)
                            );
                            // var direction = new FPVector3(info.ContactNormal.X, 0, -info.ContactNormal.Z);
                            ball->Velocity = f.RuntimeConfig.BallSpeed * direction;
                        }

                        // collide with goals
                        if (f.Unsafe.TryGetPointer<Goal>(info.Other, out _))
                        {
                            ball->Velocity = FPVector3.Zero;
                            f.Signals.OnScoreChanged(info.Entity, info.Other);
                        }
                        //if (f.Unsafe.TryGetPointer<Block>(info.Other,out Block* block))
                        //{
                        //    ball->Velocity = FPVector3.Reflect(ball->Velocity, info.ContactNormal);

                        //    ball->Velocity = FPVector3.Normalize(ball->Velocity) * f.RuntimeConfig.BallSpeed;

                        //    f.Signals.OnScoreChanged(info.Entity, info.Other);

                        //    f.Signals.OnBlockBreak(info.Entity, info.Other);
                        //}
                    }
                }
            }

            else if ((f.Unsafe.TryGetPointer<Paddle>(info.Entity, out Paddle* padleA)) && (f.Unsafe.TryGetPointer<PowerUP>(info.Other, out PowerUP* powerUp)))
            {
                if(f.TryFindAsset<PowerUpBase>(powerUp->poweUpType.Id, out var powerUpAsset))
                {
                    Debug.Log("GetPowerUP");
                    f.Destroy(info.Other);
                    padleA->PowerUp = powerUpAsset;
                    powerUpAsset.InitPowerUp(f,info.Entity,padleA);
                }
            }
        }
    }
}
