using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using JazzApps.Game;
using UnityEngine;
using static JazzApps.Game.PlayerGSO;
using L = JazzApps.Utils.Logger;

namespace JazzApps.Visual
{
    [System.Serializable]
    public class PlayerVB : VisualBehavior
    {
        [SerializeField] private GameObject localVisualGO;
        [SerializeField] private Transform localViewTransform;
        [SerializeField] private GameObject networkedVisualGO;
        [SerializeField] private Transform networkedViewTransform;

        private PlayerGB playerGB => (PlayerGB)GB;
        private PlayerGameData playerData => playerGB.PlayerData; 
        
        // GB's Locals
        Vector3 GBLocalCCPosition => playerGB.localCC.transform.position;
        Quaternion GBLocalCCRotation => playerGB.localCC.transform.rotation;
        Quaternion GBLocalViewRotation => playerGB.localViewTransform.rotation;
        // GB's Networked
        Vector3 GBNetworkedCCPosition => playerData.networkedCCPosition;
        Quaternion GBNetworkedCCRotation => playerData.networkedCCRotation;
        Quaternion GBNetworkedViewRotation => playerData.networkedViewPoint;

        public override void Init(GameContainerSO gameContainer, GameBehavior GB)
        {
            base.Init(gameContainer, GB);
            
            if (GB.HasInputAuthority)
            {
                localVisualGO.SetActive(true);
                networkedVisualGO.SetActive(false);
            }
            else
            {                
                localVisualGO.SetActive(false);
                networkedVisualGO.SetActive(true);
            }
        }

        private void Update()
        {
            if (GB.HasInputAuthority)
                ProcessLocalVisuals();
            else
                ProcessNetworkVisuals();
        }

        private void ProcessLocalVisuals()
        {
            localVisualGO.transform.position = GBLocalCCPosition;
            localVisualGO.transform.rotation = GBLocalCCRotation;
            localViewTransform.rotation = GBLocalViewRotation;
        }

        private void ProcessNetworkVisuals()
        {
            networkedVisualGO.transform.position = GBNetworkedCCPosition;
            networkedVisualGO.transform.rotation = GBNetworkedCCRotation;
            networkedViewTransform.rotation = GBNetworkedViewRotation;
        }
    }
}
