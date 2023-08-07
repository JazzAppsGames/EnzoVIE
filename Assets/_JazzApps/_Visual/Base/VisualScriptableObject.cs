using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.UI;

namespace JazzApps.Visual
{
    [System.Serializable]
    public abstract class VisualScriptableObject : ScriptableObject
    {
        public abstract string Name { get; }
        public abstract Sprite Sprite { get; }
    }
}