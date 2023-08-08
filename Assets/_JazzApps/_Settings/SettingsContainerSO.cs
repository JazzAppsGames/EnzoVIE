using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using JazzApps.Application;
using UnityEngine;
using UnityEngine.UI;

namespace JazzApps.Settings
{
    [CreateAssetMenu(fileName = "SettingsContainer", menuName = "JazzApps/ContainerSO/SettingsContainer", order = 0)]
    public class SettingsContainerSO : Container
    {
        public SettingsManager Manager => (SettingsManager)BaseManager;
        
        [field: SerializeField] public PlayerSettingsSO PlayerSettings { get; private set; }
        [field: SerializeField] public NetworkSettingsSO NetworkSettings { get; private set; }
    }
}