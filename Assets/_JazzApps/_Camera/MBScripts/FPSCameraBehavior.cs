using System;
using System.Collections;
using System.Collections.Generic;
using JazzApps.Camera;
using UnityEngine;

public class FPSCameraBehavior : CameraBehavior
{
    public override void Anchor()
    {
        cameraTransform.SetPositionAndRotation(target.position, target.rotation);
    }
}
