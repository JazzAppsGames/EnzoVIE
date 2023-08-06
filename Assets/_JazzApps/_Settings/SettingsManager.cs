using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using JazzApps.Application;
using JazzApps.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace JazzApps.Settings
{
    public class SettingsManager : Manager
    {
        [field: SerializeField] private SettingsContainerSO container;
        protected override Container BaseContainer => container;
        
    }
}