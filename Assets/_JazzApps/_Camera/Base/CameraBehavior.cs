using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using Random = UnityEngine.Random;

namespace JazzApps.Camera
{
    public abstract class CameraBehavior : MonoBehaviour
    {
        protected Transform cameraTransform;
        internal void GiveCamera(UnityEngine.Camera camera) => cameraTransform = camera.transform;
        
        protected Transform target;
        internal void GiveTarget(Transform target) => this.target = target;
        //LATER: make a TPSCameraBehavior (that derives from this) with a spring arm? maybe?

        private void Update()
        {
            if (cameraTransform != null && target != null)
                Anchor();
        }

        public abstract void Anchor();
    }
}