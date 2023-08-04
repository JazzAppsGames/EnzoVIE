using System;
using UnityEngine;

namespace JazzApps.Application
{
    public class Manager : MonoBehaviour
    {
        protected virtual Container BaseContainer => null;

        public void Awake()
        {
            ApplicationManager.Instance.AddContainer(BaseContainer);
            BaseContainer.Inject(this);
        }
    }
}