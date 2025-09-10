using System.Threading.Tasks;

namespace Modules
{
    public abstract class ModuleBase
    {
        public abstract Task Connect();
        public abstract Task Start();
        public abstract Task Stop();
        
        public virtual Task PreloadAssets(string mainView)
        {
            return Task.CompletedTask;
        }
        
        public virtual async Task MainTick(float deltaTime)
        {
            await Task.CompletedTask;
        }
    }
}