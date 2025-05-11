using Photon.Deterministic;
using System.Runtime.InteropServices;

namespace Quantum
{
    public abstract class PowerUpBase : AssetObject
    {
        public FP duration = 10;

        private FP currentTime = 0;

        public unsafe Paddle* Padle = null;

        public unsafe EntityRef PaddleRef;

        public unsafe void InitPowerUp(Frame f,EntityRef entityRef,Paddle* paddleRef)
        {
            currentTime = duration;

            Padle = paddleRef;

            PaddleRef = entityRef;

            ApplyPowerUp(f,entityRef);
        }
        public abstract void ApplyPowerUp(Frame f,EntityRef entity);

        public abstract void ResetPowerUp();

        public unsafe void UpdatePowerUp(FP delta,System.Action<int,int> OnUpdateUI)
        {
            currentTime -= delta;
            OnUpdateUI.Invoke(Padle->Index, (int)currentTime);
            if (currentTime <= 0)
            {
                OnUpdateUI.Invoke(Padle->Index, 0);
                EndPowerUp();
            }
        }
        public void EndPowerUp()
        {
            currentTime = 0;
            ResetPowerUp();
        }
    }
}