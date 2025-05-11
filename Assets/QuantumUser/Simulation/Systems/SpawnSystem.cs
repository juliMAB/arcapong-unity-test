using UnityEngine;
using UnityEngine.Scripting;
using Quantum;
using Photon.Deterministic;

namespace Tomorrow.Quantum
{
    [Preserve]
    public unsafe class SpawnSystem : SystemSignalsOnly, 
        ISignalOnPlayerAdded, ISignalOnGameStateChanged, ISignalOnScoreChanged, ISignalOnGameStarted, ISignalOnGameOver , ISignalOnBlockBreak
    {
        public void OnPlayerAdded(Frame f, PlayerRef player, bool firstTime)
        {
            int playerCount = f.ComponentCount<PlayerLink>();
            SpawnPlayer(f, playerCount, player);
        }

        public void OnGameStarted(Frame f)
        {
            if(!f.IsVerified) return;
            // comlete missing players with AI
            int playerCount = f.ComponentCount<PlayerLink>();
            int missingPlayers = f.RuntimeConfig.PlayersCount - playerCount;
            if (missingPlayers <= 0) return;
            for (int i = 0; i < missingPlayers; i++)
            {
                SpawnAI(f, playerCount+i);
            }
            #region ADDED_BY_JULIAN
            SpawnGrid(f);
            #endregion
        }
        public void OnGameStateChanged(Frame f, GameState state)
        {
            if(!f.IsVerified) return;
            if (state == GameState.Countdown)
            {
                // spawn the ball
                EntityRef ballEntity = f.Create(f.RuntimeConfig.BallPrototype);
                f.Add(ballEntity, new Ball());
                if (f.Unsafe.TryGetPointer<Transform3D>(ballEntity, out var ballTransform))
                {
                    RespawnBall(f, ballEntity);
                }
            }
        }

        public void OnScoreChanged(Frame f, EntityRef ballEntity, EntityRef goalEntity)
        {
            if(!f.IsVerified) return;
            if (f.Unsafe.TryGetPointer<Ball>(ballEntity, out Ball *ball))
            {
                if (f.Unsafe.TryGetPointer<Paddle>(ball->Paddle, out Paddle *paddle))
                {
                    if (f.Unsafe.TryGetPointer<Goal>(goalEntity, out Goal *goal))
                    {
                        // respawn game
                        var game = f.Unsafe.GetPointerSingleton<Game>();
                        game->Respawn(f);
                        // delete the ball
                        f.Destroy(ballEntity);
                        // notify ui
                        #region FIXED_BY_JULIAN
                        //for some reason the goals are inverted.
                        bool goalHimself = paddle->Index != goal->Index;
                        //here i dont have the other real paddle, so i preffere less 1 score.
                        if (goalHimself)
                        {
                            paddle->Score -= 1;
                        }
                        else
                        {
                            paddle->Score += 1;
                        }

                        f.Events.OnScoreChanged(paddle->Index, paddle->Score);
                        return;
                        #endregion
                    }
                    #region ADDED_BY_JULIAN
                    if (f.Unsafe.TryGetPointer<Block>(goalEntity,out Block* block))
                    {
                        paddle->Score += 1;

                        f.Events.OnScoreChanged(paddle->Index, paddle->Score);
                    }
                    #endregion
                }
            }
        }

        public void OnGameOver(Frame f)
        {
            if(!f.IsVerified) return;
            foreach (var pair in f.GetComponentIterator<Ball>())
            {
                f.Destroy(pair.Entity);
            }
            foreach (var pair in f.GetComponentIterator<Paddle>())
            {
                f.Destroy(pair.Entity);
            }
        }

        void RespawnBall(Frame f, EntityRef ballEntity)
        {
            // reset position
            if (f.Unsafe.TryGetPointer<Transform3D>(ballEntity, out var ballTransform))
            {
                ballTransform->Position = new FPVector3(
                    f.RuntimeConfig.GameSize.X/2, 
                    0,
                    f.RuntimeConfig.GameSize.Y/2
                );
            }
            // reset physics
            if (f.Unsafe.TryGetPointer<Ball>(ballEntity, out var ball))
            {
                ball->Velocity = f.RuntimeConfig.BallSpeed * FPVector3.Forward;
            }
        }

        void SpawnPlayer(Frame f, int index, PlayerRef player)
        {
            var paddleEntity = Spawn(f, index);
            var playerLink = new PlayerLink()
            {
                Player = player,
            };
            f.Add(paddleEntity, playerLink);
        }

        void SpawnAI(Frame f, int index)
        {
            var paddleEntity = Spawn(f, index);
            var playerAI = new PlayerAI();
            f.Add(paddleEntity, playerAI);
        }

        EntityRef Spawn(Frame f, int index)
        {
            EntityRef paddleEntity = f.Create(f.RuntimeConfig.PaddlePrototype);

            if (f.Unsafe.TryGetPointer<Transform3D>(paddleEntity, out var transform))
            {
                transform->Position = new FPVector3(
                    f.RuntimeConfig.GameSize.X/2, 
                    0,
                    index * f.RuntimeConfig.GameSize.Y
                );
            }
            #region FIXED_BY_JULIAN
            if (f.Unsafe.TryGetPointer<Paddle>(paddleEntity, out var paddle))
            {
                paddle->Index = index;
            }
            //this didn't work, not override an existent paddle component
            //    f.Add(paddleEntity, new Paddle(){
            //    Index = index,
            //    Score = 0
            //});
            #endregion
            return paddleEntity;
        }
        #region ADDED_BY_JULIAN
        void SpawnGrid(Frame f)
        {
            for (int i = 0; i < f.RuntimeConfig.GridSize.X; i++)
            {
                for (int j = 0; j < f.RuntimeConfig.GridSize.Y; j++)
                {
                    var BlockEntity = SpawnBlock(f, i* f.RuntimeConfig.GridSize.Y.AsInt + j,new FPVector2(i + f.RuntimeConfig.GridOffset.X, j + f.RuntimeConfig.GridOffset.Y));
                    var BlockGenerate = new Block();
                    f.Add(BlockEntity, BlockGenerate);
                }
            }
        }
        EntityRef SpawnBlock(Frame f, int index,FPVector2 pos)
        {
            EntityRef BlockEntity = f.Create(f.RuntimeConfig.BlockPrototype);

            if (f.Unsafe.TryGetPointer<Transform3D>(BlockEntity, out var transform))
            {
                transform->Position = new FPVector3(
                    pos.X,
                    0,
                    pos.Y
                );
            }
            f.Add(BlockEntity, new Block()
            {
                Index = index,
            });
            return BlockEntity;
        }

        public void OnBlockBreak(Frame f, EntityRef block, EntityRef ball)
        {
            Debug.Log("TryBlockBreack");
            bool SpawnPowrUP = (f.RNG->Next(FP._0, FP._1) < FP._0_25);

            bool isBall0 = (f.Unsafe.TryGetPointer<Ball>(ball, out Ball* ballComp));

            bool isBlock0 = (f.Unsafe.TryGetPointer<Block>(block, out Block* blockComp));

            if ((isBall0) && (isBlock0))
            {
                Debug.Log("BlockBreack");

                bool isPaddle0 = (f.Unsafe.TryGetPointer<Paddle>(ballComp->Paddle, out Paddle* paddleLastHit));

                if(SpawnPowrUP)
                {
                    EntityRef PowerUpEntity = f.Create(f.RuntimeConfig.PowerUpPrototype);

                    PowerUP* powerUp = f.Unsafe.GetPointer<PowerUP>(PowerUpEntity);
                    Transform3D* transform = f.Unsafe.GetPointer<Transform3D>(PowerUpEntity);
                    Transform3D* transformBlock = f.Unsafe.GetPointer<Transform3D>(block);
                    PhysicsBody3D* body = f.Unsafe.GetPointer<PhysicsBody3D>(PowerUpEntity);

                    transform->Position = new FPVector3(
                        transformBlock->Position.X,
                        0,
                        transformBlock->Position.Z
                    );
                    body->AddForce(new FPVector3(0,0,paddleLastHit->Index == 0 ? -1 : 1 ) * 1000);
                }

                f.Destroy(block);
            }
        }
        #endregion
    }
}
