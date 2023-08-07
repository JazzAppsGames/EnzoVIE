using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using JazzApps.Application;
using JazzApps.Game;
using UnityEngine;
using static JazzApps.Game.PlayerGSO;
using L = JazzApps.Utils.Logger;

namespace JazzApps.Visual
{
    [CreateAssetMenu(fileName = "VisualContainer", menuName = "ContainerSO/VisualContainer")]
    public class VisualContainerSO : Container
    {
        public VisualManager Manager => (VisualManager)BaseManager;
        
        [Header("GameGB")]
        public PlayerVB PlayerVBPrefab;
        [Header("SOs")] 
        public SerializableDictionary<PlayerGSO, PlayerVSO> PlayerVSO;
    }
}