namespace Quantum
{
    using Photon.Deterministic;

    public class PowerUpSize : PowerUpBase
    {
        public FP sizeMultiplier = 2;

        public unsafe override void ApplyPowerUp(Frame f, EntityRef entity)
        {
            //apply size multiplier

        }

        public unsafe override void ResetPowerUp()
        {
            //reset size multiplier
        }
    }
}
