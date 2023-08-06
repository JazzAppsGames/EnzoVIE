using System;
using System.Collections.Generic;
using Fusion;
using JazzApps.Application;
using JazzApps.Settings;
using UnityEngine;
using L = JazzApps.Utils.Logger;

namespace JazzApps.Game
{
    public class LiveGame : NetworkBehaviour
    {
        private ApplicationManager applicationManager => ApplicationManager.Instance;
        private SettingsContainerSO settingsContainer => applicationManager.GetContainer(typeof(SettingsContainerSO)) as SettingsContainerSO;
        private GameContainerSO gameContainer => applicationManager.GetContainer(typeof(GameContainerSO)) as GameContainerSO;

        [Networked, Capacity(32), UnitySerializeField]
        public NetworkDictionary<int, PlayerGB> GamePlayers => default;

        public override void Spawned()
        {
            
        }

        #region TESTING
        private void OnGUI()
        {
            if (Runner != null)
                if (GUI.Button(new Rect(0, 40, 200, 40), "GAME Spawn Map"))
                    SpawnMap();
        }
        #endregion
        
        #region STATE-MACHINE
        
        // TODO: game states
        // Maybe an ChangeGameState()?
        // Wait, Start, Load, Finish, End
        
        #endregion

        #region Spawn GBs
        public void SpawnMap()
        {
            if (!HasStateAuthority)
                return;

            if (FindObjectOfType<MapGB>())
                return;
            
            var prefab = gameContainer.mapGBPrefab;
            var spawnPos = Vector3.zero;
            var spawnRot = Quaternion.identity;
            Runner.Spawn(prefab, spawnPos, spawnRot, onBeforeSpawned: (runner, no) =>
            {
                var GB = no.GetBehaviour<MapGB>();
                GB.Init(gameContainer.mapGSO.Config.Index, default, true); // TODO: other maps? change index?
            });
        }
        public PlayerGB SpawnPlayer(int ownerID, PlayerRef inputAuth)
        {
            if (GamePlayers.ContainsKey(ownerID))
            {
                L.Log($"LiveGame SpawnPlayer() owner: {ownerID} but it already existed, returning that existing player.");
                var existingPlayerGB = GamePlayers.Get(ownerID);
                var existingNO = existingPlayerGB.gameObject.GetComponent<NetworkObject>();
                existingNO.AssignInputAuthority(inputAuth);
                Runner.SetPlayerObject(inputAuth, existingPlayerGB.GetBehaviour<NetworkObject>());
                existingPlayerGB.Spawned();
                return existingPlayerGB;
            }
            var prefab = gameContainer.playerGBPrefab;
            var spawnPos = gameContainer.mapGSO.Config.playerspawnpoint.position;
            var spawnRot = gameContainer.mapGSO.Config.playerspawnpoint.rotation;
            var newPlayerGB = Runner.Spawn(prefab, position: spawnPos, rotation: spawnRot, inputAuthority: inputAuth, onBeforeSpawned: (runner, no) =>
            {
                var GB = no.GetBehaviour<PlayerGB>();
                GB.Init(gameContainer.playerGSO.Config.Index, ownerID, true);
                Runner.SetPlayerObject(inputAuth, GB.GetBehaviour<NetworkObject>());
            });
            GamePlayers.Set(ownerID, newPlayerGB);
            return newPlayerGB;
        }
        public void DespawnPlayer(int ownerID)
        {
            if (!HasStateAuthority)
                return;
            // TODO: do something with the PlayerGb?
        }
        #endregion
        
        private void Log(string message) => L.Log(this, message);
    }
}