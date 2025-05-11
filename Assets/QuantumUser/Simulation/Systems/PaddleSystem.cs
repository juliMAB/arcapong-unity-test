using UnityEngine;
using UnityEngine.Scripting;
using Photon;
using Photon.Deterministic;
using Quantum;
using Quantum.Collections;

using Input = Quantum.Input;

namespace Tomorrow.Quantum
{
    [Preserve]
    public unsafe class PaddleSystem : SystemMainThreadFilter<PaddleSystem.Filter>
    {
        public struct Filter
        {
            public EntityRef Entity;
            public Transform3D* Transform;
            public PhysicsBody3D* Body;
            public PlayerLink* PlayerLink;
        }

        public override void Update(Frame f, ref Filter filter)
        {
            // TODO: this is executed for remote players too?
            Input* input = f.GetPlayerInput(filter.PlayerLink->Player);
            // move local paddle
            FP speedMultiplier = 1;
            if (f.Unsafe.TryGetPointer<Paddle>(filter.Entity, out Paddle* paddle))
            {
                if (paddle->PowerUp != null)
                {
                    if (f.TryFindAsset<PowerUpBase>(paddle->PowerUp.Id, out var powerUpAsset))
                    {
                        powerUpAsset.UpdatePowerUp(f.DeltaTime,f.Signals.OnUpdateTimeRemain);
                    }
                }
                filter.Body->Velocity = f.RuntimeConfig.PaddleSpeed * input->Direction * FPVector3.Right * paddle->VelocityMultiplier;
            }

            // set player ready
            if(f.GetPlayerInput(filter.PlayerLink->Player)->Ready.WasPressed)
            {
                filter.PlayerLink->Ready = !filter.PlayerLink->Ready;
            }
        }
    }
}
