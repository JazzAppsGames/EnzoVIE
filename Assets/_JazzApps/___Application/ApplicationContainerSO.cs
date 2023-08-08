using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JazzApps;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JazzApps.Application
{
    [CreateAssetMenu(fileName = "ApplicationContainer", menuName = "JazzApps/ContainerSO/ApplicationContainer", order = 1)]
    public class ApplicationContainerSO : ScriptableObject
    {
        public ApplicationManager ApplicationManager { get; set; }
    }
}