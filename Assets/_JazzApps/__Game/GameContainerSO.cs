using System;
using System.Collections.Generic;
using Fusion;
using JazzApps.Game;
using JazzApps.Application;
using JazzApps.Settings;
using JazzApps.Utils;
using UnityEngine;

namespace JazzApps.Game
{
    [CreateAssetMenu(fileName = "GameContainer", menuName = "JazzApps/ContainerSO/GameContainer")]
    public class GameContainerSO : Container
    {
        public GameManager Manager => (GameManager)BaseManager;
        
        [HideInInspector] public LiveGame LiveGame;
        [HideInInspector] public int localPlayerIdentity;

        [field: SerializeField] public LiveGame liveGamePrefab { get; private set; }
        
        [field: Header("Prefabs")]
        [field: SerializeField] public MapGB mapGBPrefab { get; private set; }
        [field: SerializeField] public PlayerGB playerGBPrefab { get; private set; }
        [Header("GameSOs")]
        public MapGSO mapGSO;
        public PlayerGSO playerGSO;

        #region EVENTS
        #region EXTERNALLY FIRED
        public event Action<NetworkRunner> LaunchLiveGameEvent;
        public void InvokeLaunchLiveGameEvent(NetworkRunner runner) => LaunchLiveGameEvent?.Invoke(runner);
        public event Action<NetworkRunner> HostMigrationEvent;
        public void InvokeHostMigrationEvent(NetworkRunner newRunner) => HostMigrationEvent?.Invoke(newRunner);
        public event Action<int, bool> PlayerToggleLocalRepresentationEvent;
        public void InvokePlayerToggleLocalRepresentationEvent(int id, bool on) => PlayerToggleLocalRepresentationEvent?.Invoke(id, on);
        public event Action<int, PlayerRef> PlayerJoinLiveGameEvent;
        public void InvokePlayerJoinLiveGameEvent(int id, PlayerRef playerRef) => PlayerJoinLiveGameEvent?.Invoke(id, playerRef);
        public event Action<int> PlayerLeaveLiveGameEvent;
        public void InvokePlayerLeaveLiveGameEvent(int id) => PlayerLeaveLiveGameEvent?.Invoke(id);
        #endregion
        #region INTERNALLY FIRED
        public event Action<GameBehavior> GBSpawnedEvent;
        public void TryInvokeGBSpawnedEvent(GameBehavior GB) => GBSpawnedEvent?.Invoke(GB);
        public event Action<GameBehavior> GBSelectedEvent;
        public void InvokeGBSelectedEvent(GameBehavior GB) => GBSelectedEvent?.Invoke(GB);
        #endregion
        #endregion
    }
}

