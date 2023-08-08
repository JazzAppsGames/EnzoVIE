using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.UI;

namespace JazzApps.Settings
{
    [CreateAssetMenu(fileName = "PlayerSettings", menuName = "JazzApps/SO/Settings/Player")]
    public class PlayerSettingsSO : SettingsScriptableObject
    {
        public PlayerMovementSettings MovementSettings;
        public PlayerViewSettings viewSettings;
    }
    
    [System.Serializable]
    public class PlayerMovementSettings
    {
        [SerializeField] private float walkSpeed;
        [SerializeField] private float runSpeed;
        [SerializeField] private float jumpSpeed;
        [SerializeField] private float flySpeed;
        [SerializeField] private float fastFlySpeed;
        public float FlyUpSpeed => jumpSpeed; // LATER: eeeuuuggghhh idk
        public float JumpSpeed => jumpSpeed;
        public float GetSpeed(bool forSprint, bool forFlying)
        {
            if (forSprint)
                return forFlying ? fastFlySpeed : runSpeed;
            return forFlying ? flySpeed : walkSpeed;
        }
    }
    
    [System.Serializable]
    public class PlayerViewSettings
    {
        [SerializeField] private float sensitivity;
        
        public float Sensitivity => sensitivity;
    }
}