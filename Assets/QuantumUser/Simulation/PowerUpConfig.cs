namespace Quantum
{
    using Photon.Deterministic;

    public abstract class PowerUpConfig : AssetObject
    {
        public FP duration;
    }
    public enum PowerUpMode
    {
        None,
        Speed,
        Big,
        Small,
    }
    public class PowerUpSpeed : PowerUpConfig
    {
        public FP speedMultiplier;
    }
    public class PowerUpBig : PowerUpConfig
    {
        public FP sizeMultiplier;
    }
}
