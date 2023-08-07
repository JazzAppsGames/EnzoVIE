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
    public abstract class VisualBehavior : MonoBehaviour
    {
        protected GameContainerSO gameContainer { get; private set; }
        protected GameBehavior GB { get; private set; }

        public virtual void Init(GameContainerSO gameContainer, GameBehavior GB)
        {
            this.gameContainer = gameContainer;
            this.GB = GB;
        }

        public void Awake()
        {
        }
        private void OnEnable()
        {
        }
        private void OnDisable()
        {
        }
    }
}