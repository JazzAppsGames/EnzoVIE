using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using JazzApps.Application;
using JazzApps.Camera;
using JazzApps.Game;
using JazzApps.Settings;
using JazzApps.Utils;
using UnityEngine;
using static JazzApps.Game.PlayerGSO;
using L = JazzApps.Utils.Logger;
using Object = UnityEngine.Object;

namespace JazzApps.Visual
{
    public class VisualManager : Manager
    {
        [field: SerializeField] private VisualContainerSO container;
        protected override Container BaseContainer => container;
        
        private ApplicationManager applicationManager => ApplicationManager.Instance;
        private GameContainerSO gameContainer => applicationManager.GetContainer(typeof(GameContainerSO)) as GameContainerSO;
        private CameraContainerSO cameraContainer => applicationManager.GetContainer(typeof(CameraContainerSO)) as CameraContainerSO;
        private SettingsContainerSO settingsContainer => applicationManager.GetContainer(typeof(SettingsContainerSO)) as SettingsContainerSO;

        private readonly Dictionary<GameBehavior, VisualBehavior> VBs = new Dictionary<GameBehavior, VisualBehavior>() ?? new Dictionary<GameBehavior, VisualBehavior>(); // TODO: does this belong on the container?

        #region MONOBEHAVIOUR CALLBACKS
        private void OnEnable()
        {
            gameContainer.GBSpawnedEvent += GBSpawned;
        }
        private void OnDisable()
        {
            this.gameContainer.GBSpawnedEvent -= GBSpawned;
        }
        #endregion MONOBEHAVIOUR CALLBACKS
        
        private void GBSpawned(GameBehavior GB)
        {
            switch (GB)
            {
                case MapGB mapGB:
                    // TODO: mapVB spawning
                    break;
                case PlayerGB playerGB:
                    SpawnVBPrefab(playerGB, container.PlayerVBPrefab.gameObject);
                    break;
            }
        }

        private void SpawnVBPrefab(GameBehavior GB, GameObject prefab)
        {
            if (!VBs.TryGetValue(GB, out var VB))
            {
                var anchor = GB.transform;
                var newVBObject = Instantiate(prefab, anchor);
                VB = newVBObject.GetComponent<VisualBehavior>();
                VBs.Add(GB, VB);
            }

            VB.Init(this.gameContainer, GB);
        }
    }
}