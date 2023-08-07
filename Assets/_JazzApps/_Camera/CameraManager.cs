using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using JazzApps.Application;
using JazzApps.Game;
using JazzApps.Settings;
using JazzApps.Utils;
using UnityEngine;
using Logger = JazzApps.Utils.Logger;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace JazzApps.Camera
{
    public class CameraManager : Manager
    {
        // Injection
        [field: SerializeField] private CameraContainerSO container;
        protected override Container BaseContainer => container;

        private ApplicationManager applicationManager => ApplicationManager.Instance;
        private SettingsContainerSO settingsContainer => applicationManager.GetContainer(typeof(SettingsContainerSO)) as SettingsContainerSO;
        private GameContainerSO gameContainer => applicationManager.GetContainer(typeof(GameContainerSO)) as GameContainerSO;
        
        private CameraBehavior _currentCameraBehavior;

        private FPSCameraBehavior fpsCB;
        
        private void OnEnable()
        {
            gameContainer.GBSpawnedEvent += GBSpawned;
            fpsCB = gameObject.AddComponent<FPSCameraBehavior>(); // LATER: there's gotta be a better way of doing this
        }
        private void OnDisable()
        {
            gameContainer.GBSpawnedEvent -= GBSpawned;
        }

        private void GBSpawned(GameBehavior GB)
        {
            switch (GB)
            {
                case MapGB mapGB:
                    // LATER: map camera stuff?
                    break;
                case PlayerGB playerGB:
                    if (playerGB.HasInputAuthority)
                    {
                        // TODO: more camera behaviors?
                        StartFPSCamera(playerGB.localViewTransform);
                    }
                    break;
            }
        }

        private void StartFPSCamera(Transform target)
        {
            _currentCameraBehavior = fpsCB;
            fpsCB.GiveTarget(target);
            fpsCB.GiveCamera(UnityEngine.Camera.main);
        }
    }
}