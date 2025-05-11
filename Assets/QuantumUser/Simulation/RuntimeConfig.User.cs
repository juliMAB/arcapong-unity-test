using System;
using System.Collections.Generic;
using Photon.Deterministic;

namespace Quantum
{
    public partial class RuntimeConfig
    {
        public AssetRef<EntityPrototype> PaddlePrototype;
        public AssetRef<EntityPrototype> BallPrototype;
        public AssetRef<EntityPrototype> BlockPrototype;
        public AssetRef<EntityPrototype> PowerUpPrototype;
        public FP PaddleSpeed;
        public FP AIPaddleSpeed;
        public FP BallSpeed;
        public FPVector2 GameSize;
        public int PlayersCount;
        public int CountdownTime;
        public int GameTime;
        public int FinishedTime;
        public FPVector2 GridSize;
        public FPVector2 GridOffset;
        public AssetRef<AIConfig> aIConfig;
    }
}
