using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JazzApps.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using L = JazzApps.Utils.Logger;

namespace JazzApps.Application
{
    /// <summary>
    /// The "mediator" class for the entire application.
    /// </summary>
    public class ApplicationManager : Singleton<ApplicationManager>
    {
        
        [field: SerializeField] public ApplicationContainerSO ApplicationContainer { get; private set; }
        public HashSet<Container> Containers { get; private set; } = new();
        public Container GetContainer(Type T)
        {
            var found = Containers.FirstOrDefault(c => c.GetType() == T);
            if (found != null) return found;
            {
                Log($"GetContainer() found null for {T.Name}!!! Will be trying to find via a new Init() call, hang tight!");
                this.Init();
                found = Containers.FirstOrDefault(c => c.GetType() == T);
            }
            return found;
        }
        public void AddContainer(Container container)
        {
            Containers.Remove(container);
            Containers.Add(container);
            container.Inject(ApplicationContainer);
        }
        protected override void Init()
        {
            if (Initialized) return;
            foreach (var manager in FindObjectsOfType<Manager>())
                manager.Awake();
        }

        private void Log(string message) => L.Log(this, message);
    }
}