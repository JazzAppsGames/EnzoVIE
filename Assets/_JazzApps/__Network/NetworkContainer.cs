using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using JazzApps.Application;
using JazzApps.Game;
using UnityEngine;

namespace JazzApps.Network
{
    [CreateAssetMenu(fileName = "NetworkContainer", menuName = "ContainerSO/NetworkContainer")]
    public class NetworkContainerSO : Container
    {
        public NetworkManager Manager => (NetworkManager)BaseManager;

        [field: SerializeField] public NetworkRunner RunnerPrefab { get; private set; }
        
        #region EVENTS
        
        #region GENERAL EVENTS

        #endregion

        #endregion
    }
}