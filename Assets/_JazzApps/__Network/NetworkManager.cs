using System;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using JazzApps.Application;
using JazzApps.Game;
using JazzApps.Settings;
using JazzApps.Utils;
using UnityEngine.SceneManagement;
using L = JazzApps.Utils.Logger;
using Random = UnityEngine.Random;

namespace JazzApps.Network
{
    /// <summary>
    /// The "guy" for networking.
    /// </summary>
    public class NetworkManager : Manager, INetworkRunnerCallbacks
    {
        [field: SerializeField] private NetworkContainerSO container;
        protected override Container BaseContainer => container;
        
        private ApplicationManager applicationManager => ApplicationManager.Instance;
        private GameContainerSO gameContainer => applicationManager.GetContainer(typeof(GameContainerSO)) as GameContainerSO;
        private SettingsContainerSO settingsContainer => applicationManager.GetContainer(typeof(SettingsContainerSO)) as SettingsContainerSO;
        
        private NetworkRunner runnerPrefab => container.RunnerPrefab;
        
        public static NetworkRunner Runner { get; private set; }
        
        private byte[] localConnectionToken;
        public int localConnectionTokenHashed => ConnectionTokenUtils.HashToken(localConnectionToken);
        private HashSet<int> playerTokens;

        private bool isHost(NetworkRunner runner) => runner.IsSinglePlayer || runner.IsServer;
        
        #region TESTING
        private void OnGUI()
        {
            if (Runner == null)
            {
                if (GUI.Button(new Rect(0,0,200,40), "Singleplayer"))
                {
                    StartOrJoinRoom(GameMode.Single);
                }
                if (GUI.Button(new Rect(0,40,200,40), "AutoHostJoin"))
                {
                    StartOrJoinRoom(GameMode.AutoHostOrClient);
                }
                // Party
                if (GUI.Button(new Rect(420,0,200,40), "JoinSpecificRoom"))
                {
                    Log("JoinSpecificRoom");
                }
                GUI.Label(new Rect(630,0,200,20), "Text text text");
            }
            else
            {
                if (GUI.Button(new Rect(0,0,200,40), "Disconnect"))
                {
                    LeaveRoom();
                }
            }
        }
        #endregion
        
        protected void Awake()
        {
            // TODO: get connection token elsewhere?
            localConnectionToken = ConnectionTokenUtils.NewToken(true);
            Log("Our connection token: " + localConnectionTokenHashed);
        }

        async void StartOrJoinRoom(GameMode mode)
        {
            playerTokens = new();

            InstantiateRunner();
            
            await Runner.StartGame(new StartGameArgs()
                {
                    GameMode = mode,
                    SessionName = "TestRoom",
                    Scene = SceneManager.GetActiveScene().buildIndex,
                    ConnectionToken = localConnectionToken
                }
            );
        }
        async void LeaveRoom()
        {
            await Runner.Shutdown(true,  ShutdownReason.Ok);
            Runner = null;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        #region HOST MIGRATION
        public async void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
            Log("Host migration in progress!");
            await runner.Shutdown(shutdownReason: ShutdownReason.HostMigration, destroyGameObject:true);
            
            InstantiateRunner();
            
            StartGameResult result = await Runner.StartGame(new StartGameArgs() {
                Scene = SceneManager.GetActiveScene().buildIndex,
                ConnectionToken = localConnectionToken,
                HostMigrationToken = hostMigrationToken,   // contains all necessary info to restart the Runner
                HostMigrationResume = HostMigrationResume, // this will be invoked to resume the simulation
                GameMode = hostMigrationToken.GameMode,
                // LATER: other args?
            });
            
            if (result.Ok == false)
                Debug.LogWarning(result.ShutdownReason);
            else
                Log("Host migration done.");
        }
        void HostMigrationResume(NetworkRunner runner) 
        {
            Log("Resuming game from host migration!");

            gameContainer.InvokeHostMigrationEvent(runner);
            
            // TODO: spawn behaviors and all that stuff on the LiveGame I think I guess idk
            foreach (var entry in FindObjectOfType<LiveGame>().GamePlayers)
            {
                var playerToken = entry.Key;
                var playerGB = entry.Value;
                playerTokens.Add(playerToken);
                if (playerToken == localConnectionTokenHashed)
                    playerGB.GetComponent<NetworkObject>().AssignInputAuthority(Runner.LocalPlayer);
                Log($"Migrated {playerToken}");
            }
            Log($"Migrated players from LiveGame.");
        }
        #endregion
        #region Runner Callbacks
        public void OnPlayerJoined(NetworkRunner runner, PlayerRef playerRef)
        {
            if (!isHost(runner))
                return;
            
            if (gameContainer.LiveGame == null)
                gameContainer.InvokeLaunchLiveGameEvent(runner);
            
            byte[] token = runner.GetPlayerConnectionToken(playerRef);
            int playerToken = token == null ? 0 : ConnectionTokenUtils.HashToken(token);

            if (playerTokens.Contains(playerToken))
            {
                Log($"{playerToken} was found in the HashSet. Attempting PlayerGB assignment.");
                // Process it as a reconnect
                if (runner.LocalPlayer == playerRef)
                    gameContainer.InvokePlayerToggleLocalRepresentationEvent(playerToken, true);
            }
            else
            {
                Log($"No found value for token {playerToken}.");
                // If the token wasn't found then just proceed as usual Squidward
                playerTokens.Add(playerToken);
            }
            
            gameContainer.InvokePlayerJoinLiveGameEvent(playerToken, playerRef);
        }
        public void OnPlayerLeft(NetworkRunner runner, PlayerRef playerRef) {}
        public void OnInput(NetworkRunner runner, NetworkInput input) {}
        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) {}
        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { /* TODO: perhaps reload the scene idk */ }
        public void OnConnectedToServer(NetworkRunner runner) {}
        public void OnDisconnectedFromServer(NetworkRunner runner) {}
        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) {}
        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) {}
        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) {}
        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) {}
        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) {}
        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) {}
        public void OnSceneLoadDone(NetworkRunner runner) {}
        public void OnSceneLoadStart(NetworkRunner runner) {}
        #endregion
        
        private void InstantiateRunner()
        {
            Runner = Instantiate(runnerPrefab);
            Runner.ProvideInput = true;
            Runner.AddCallbacks(this);
        }
        
        private void Log(string message) => L.Log(this, message);
    }
}
