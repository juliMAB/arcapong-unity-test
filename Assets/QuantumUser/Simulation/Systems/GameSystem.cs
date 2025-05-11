using UnityEngine.Scripting;
using Quantum;

namespace Tomorrow.Quantum
{
    [Preserve]
    public unsafe class GameSystem : SystemMainThread
    {
        public override void Update(Frame f)
        {
            var game = f.Unsafe.GetPointerSingleton<Game>();
            game->Update(f);
        }
    }
}

