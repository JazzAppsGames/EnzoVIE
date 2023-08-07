using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using static JazzApps.Game.PlayerGSO;
using L = JazzApps.Utils.Logger;

namespace JazzApps.Visual
{
    [CreateAssetMenu(fileName = "PlayerVisual", menuName = "SO/Visual/Player")]
    public class PlayerVSO : VisualScriptableObject
    {
        [SerializeField] private new string name;
        public override string Name => this.name;
        [SerializeField] private Sprite sprite;
        public override Sprite Sprite => this.sprite;
    }
}