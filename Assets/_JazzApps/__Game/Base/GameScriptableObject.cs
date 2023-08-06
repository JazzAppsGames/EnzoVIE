using System;
using System.Collections.Generic;
using JazzApps;
using UnityEngine;
using Random = UnityEngine.Random;

namespace JazzApps.Game
{
    public abstract class GameScriptableObject : ScriptableObject
    {
        protected abstract string _guid { get; }
    }
}