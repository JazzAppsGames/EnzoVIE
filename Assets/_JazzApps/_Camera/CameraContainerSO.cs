using System.Collections;
using System.Collections.Generic;
using Fusion;
using JazzApps.Application;
using UnityEngine;


namespace JazzApps.Camera
{
    [CreateAssetMenu(fileName = "CameraContainer", menuName = "ContainerSO/CameraContainer", order = 0)]
    public class CameraContainerSO : Container
    {
        public CameraManager Manager => (CameraManager)BaseManager;
    }
}