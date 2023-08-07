using Fusion;
using JazzApps.Application;
using JazzApps.Settings;
using UnityEngine;

namespace JazzApps.Game
{
    public abstract class GameBehavior : NetworkBehaviour
    {
        protected ApplicationManager applicationManager => ApplicationManager.Instance;
        protected SettingsContainerSO settingsContainer => applicationManager.GetContainer(typeof(SettingsContainerSO)) as SettingsContainerSO;
        protected SettingsManager settingsManager => settingsContainer.Manager;
        protected GameContainerSO container => applicationManager.GetContainer(typeof(GameContainerSO)) as GameContainerSO;
        protected GameManager gameManager => container.Manager;

        public abstract void Init(Vector2Int index, int ownerID, bool reset);
        
        protected abstract IGameData Data { get; }
        protected abstract void ResetND();
        
        public override void Spawned()
        {
            base.Spawned();
            container.TryInvokeGBSpawnedEvent(this);
        }
        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            base.Despawned(runner, hasState);
        }
    }
    
    public struct GameDataStruct : INetworkStruct
    {
        // LATER: this is where we'll cache things for performance >:)
    }
}