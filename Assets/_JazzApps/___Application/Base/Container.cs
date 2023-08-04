using UnityEngine;

namespace JazzApps.Application
{
    public abstract class Container : ScriptableObject
    {
        public ApplicationContainerSO ApplicationContainer { get; private set; }
        internal void Inject(ApplicationContainerSO applicationContainer) => this.ApplicationContainer = applicationContainer;
        protected Manager BaseManager { get; private set; }
        public void Inject(Manager manager) => this.BaseManager = manager;
    }
}