namespace Quantum
{
    using Photon.Deterministic;

    public class PowerUpSpeed : PowerUpBase
    {
        public FP speedMultiplier = 2;


        public unsafe override void ApplyPowerUp(Frame f, EntityRef entity)
        {
            Padle->VelocityMultiplier = speedMultiplier;
        }

        public unsafe override void ResetPowerUp()
        {
            Padle->VelocityMultiplier = FP._1;
        }
    }
}
