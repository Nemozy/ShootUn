using System;
using System.Threading;
using System.Threading.Tasks;
using Configs;

namespace Modules
{
    public class ModuleManager
    {
        public ModuleBase CurrentModule { get; private set; }

        public async Task LoadBattleModule(Action restart, Action quitGame, BattleStageData battleStageData)
        {
            CurrentModule?.Stop();
            var battleModule = new BattleModule();
            CurrentModule = battleModule;
            await battleModule.PreloadAssets("Assets/Media/Prefabs/BattleView.prefab");
            await battleModule.Connect(restart, quitGame, battleStageData);
            await Task.Delay(200);
            await battleModule.Start();
        }
        
        public async Task StopCurrentModule(CancellationToken ct)
        {
            await CurrentModule.Stop();
        }
        
        public async Task MainTick(float deltaTime)
        {
            if (CurrentModule != null)
            {
                await CurrentModule.MainTick(deltaTime);
            }
        }
    }
}