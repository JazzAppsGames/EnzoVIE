using System;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using JazzApps.Application;
using JazzApps.Settings;
using JazzApps.Utils;
using UnityEngine;
using L = JazzApps.Utils.Logger;

namespace JazzApps.Game
{
    /// <summary>
    /// The "mediator" class for games.
    /// </summary>
    public class GameManager : Manager
    {
        [field: SerializeField] private GameContainerSO container;
        protected override Container BaseContainer => container;
        
        private ApplicationManager applicationManager => ApplicationManager.Instance;
        private GameContainerSO gameContainer => applicationManager.GetContainer(typeof(GameContainerSO)) as GameContainerSO;
        private SettingsContainerSO settingsContainer => applicationManager.GetContainer(typeof(SettingsContainerSO)) as SettingsContainerSO;

        private void OnEnable()
        {
            gameContainer.LaunchLiveGameEvent += LaunchLiveGame;
            gameContainer.PlayerJoinLiveGameEvent += PlayerJoinLiveGame;
            gameContainer.PlayerLeaveLiveGameEvent += PlayerLeaveLiveGame;
            gameContainer.PlayerToggleLocalRepresentationEvent += PlayerToggleLocalRepresentation;
            gameContainer.HostMigrationEvent += ProcessMigration;
        }
        private void OnDisable()
        {
            // TODO: make sure the disables are here for all of the events @Lucas
            gameContainer.LaunchLiveGameEvent -= LaunchLiveGame;
            gameContainer.PlayerJoinLiveGameEvent -= PlayerJoinLiveGame;
            gameContainer.PlayerLeaveLiveGameEvent -= PlayerLeaveLiveGame;
            gameContainer.PlayerToggleLocalRepresentationEvent -= PlayerToggleLocalRepresentation;
            gameContainer.HostMigrationEvent -= ProcessMigration;
        }

        private void LaunchLiveGame(NetworkRunner runner)
        {
            // TODO: (I assume it needs more logic maybe constraints idk)
            SpawnLiveGame(runner);
        }

        private void PlayerJoinLiveGame(int id, PlayerRef playerRef)
        {
            // TODO: (I assume it needs more logic maybe constraints idk)
            var newPlayer = gameContainer.LiveGame.SpawnPlayer(id, playerRef);
        }

        private void PlayerToggleLocalRepresentation(int id, bool on)
        {
            if (gameContainer.LiveGame.GamePlayers.TryGet(id, out var player))
                player.ToggleLocalRepresentation(on);
        }

        private void PlayerLeaveLiveGame(int id)
        {
            // LATER: PlayerLeaveLiveGame
            // As it stands, we don't really need to do much about players leaving right now
        }

        private void ProcessMigration(NetworkRunner newRunner)
        {
            // TODO: Use the LiveGame Spawn methods for these things and do the events?
            var snapshotNOs = newRunner.GetResumeSnapshotNetworkObjects();
            LiveGame newGame = null;
            // First pass
            foreach (var oldNO in snapshotNOs)
            {
                if (oldNO.TryGetBehaviour<LiveGame>(out var oldGame))
                {
                    newGame = newRunner.Spawn(oldNO, onBeforeSpawned: (runner, newNO) =>
                    {
                        newNO.CopyStateFrom(oldNO);
                        newGame = newNO.GetComponent<LiveGame>();
                        this.container.LiveGame = newGame;
                        newGame.CopyStateFrom(oldGame);
                        //newGame.Init(this.container); // TODO: init of GameGB in host migration
                    }).GetComponent<LiveGame>();
                }
            }

            if (newGame == null)
            {
                Log("GameManager's ProcessMigration could not find a LiveGame! Aborting!");
                return;
            }
            // Second Pass
            foreach (var oldNO in snapshotNOs)
            {
                if (oldNO.TryGetBehaviour<PlayerGB>(out var oldPlayer)) 
                {
                    newRunner.Spawn(
                        prefab:oldNO, 
                        position:oldPlayer.PlayerData.networkedCCPosition, 
                        rotation:oldPlayer.PlayerData.networkedCCRotation, 
                        onBeforeSpawned:(runner, newNO) =>
                        {
                            newNO.CopyStateFrom(oldNO);
                            var newGB = newNO.GetComponent<PlayerGB>();
                            newGB.CopyStateFrom(oldPlayer);
                            newGB.Init(oldPlayer.PlayerData.index, oldPlayer.PlayerData.ownerID, false);
                            newGB.PlayerData = oldPlayer.PlayerData;
                            newGame.GamePlayers.Set(oldPlayer.PlayerData.ownerID, newGB);
                        });
                }
                if (oldNO.TryGetBehaviour<MapGB>(out var oldMap)) 
                {
                    newRunner.Spawn(oldNO, onBeforeSpawned: (runner, newNO) =>
                    {
                        newNO.CopyStateFrom(oldNO);
                        var newGB = newNO.GetComponent<MapGB>();
                        newGB.CopyStateFrom(oldMap);
                        newGB.Init(oldMap.MapData.index, oldMap.MapData.ownerID, false); // TODO: init of GameGB in host migration
                        newGB.MapData = oldMap.MapData;
                    });
                }
            }
        }

        private void SpawnLiveGame(NetworkRunner runner)
        {
            if (gameContainer.LiveGame != null)
            {
                Log("SpawnLiveGame() called but a LiveGame already exists.");
                return;
            }
            gameContainer.LiveGame = 
                runner.Spawn(
                    prefab: gameContainer.liveGamePrefab, 
                    position: Vector3.zero, 
                    rotation: Quaternion.identity, 
                    inputAuthority: runner.LocalPlayer)
                .GetComponent<LiveGame>();
            Log("LiveGame has been spawned.");
        }
        
        private void Log(string message) => L.Log(this, message);
    }
}
