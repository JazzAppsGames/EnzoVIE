using System.Collections;
using System.Collections.Generic;
using Fusion;
using JazzApps.Application;
using UnityEngine;

namespace JazzApps.Display
{
    [CreateAssetMenu(fileName = "DisplayContainer", menuName = "ContainerSO/DisplayContainer")]
    public class DisplayContainerSO : Container
    {
        public DisplayManager Manager => (DisplayManager)BaseManager;
        
    }
}