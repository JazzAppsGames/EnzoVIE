using System;
using System.Collections.Generic;
using JazzApps;
using UnityEngine;
using Random = UnityEngine.Random;

namespace JazzApps.Game
{
    [CreateAssetMenu(fileName = "ElementName", menuName = "JazzApps/GSOs/Element", order = 1)]
    public class ElementGSO : GameScriptableObject
    {
        [SerializeField] private string ID;
        protected override string _guid => ID;
    }
}
