using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using JazzApps.Application;
using JazzApps.Settings;
using JazzApps.Utils;
using UnityEngine;
using L = JazzApps.Utils.Logger;

namespace JazzApps.Game
{
    public class PlayerGB : GameBehavior, INetworkRunnerCallbacks
    {
        private ApplicationManager applicationManager => ApplicationManager.Instance;
        private SettingsContainerSO settingsContainer => applicationManager.GetContainer(typeof(SettingsContainerSO)) as SettingsContainerSO;
        private GameContainerSO gameContainer => applicationManager.GetContainer(typeof(GameContainerSO)) as GameContainerSO;
        
        // Networked
        [Networked] public ref PlayerGameData PlayerData => ref MakeRef<PlayerGameData>();
        protected override IGameData Data => PlayerData;
        public PlayerGSO GSO => container.playerGSO;
        public PlayerConfig Config => GSO.Config;

        [field: SerializeField] public CharacterController localCC { get; private set; }
        [field: SerializeField] public Transform localViewTransform { get; private set; }
        
        private PlayerCharacterController _playerCharacterController;
        private PlayerViewController _playerViewController;
        private PlayerSelectController _playerSelectController;

        public override void Spawned()
        {
            // TODO: Refactor this
            _playerCharacterController ??= new PlayerCharacterController(settingsContainer.PlayerSettings, localCC);
            _playerViewController ??= new PlayerViewController(settingsContainer.PlayerSettings, localViewTransform);
            _playerSelectController ??= new PlayerSelectController(settingsContainer.PlayerSettings);
            Runner.AddCallbacks(this);
            ToggleLocalRepresentation(HasInputAuthority);
            base.Spawned();
        }
        
        /// <summary>
        /// This can cause de-sync if used improperly.
        /// </summary>
        public void ToggleLocalRepresentation(bool on)
        {
            if (on)
            {
                localCC.enabled = true;
                _playerCharacterController.Enable();
                _playerViewController.Enable();
                _playerSelectController.Enable();
            }
            else
            {
                localCC.enabled = false;
                _playerCharacterController.Disable();
                _playerViewController.Disable();
                _playerSelectController.Disable();
            }
        }
        
        public override void Init(Vector2Int index, int ownerID, bool reset)
        {
            PlayerData.ownerID = ownerID;
            PlayerData.index = index;
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            if (!HasInputAuthority)
                return;
            var outgoing = new PlayerSyncStruct{
                ccPosition = localCC.transform.position,
                ccRotation = localCC.transform.rotation,
                viewRotation = localViewTransform.rotation,
            };
            input.Set(outgoing);
        }
        
        public override void FixedUpdateNetwork()
        {
            if (!HasStateAuthority)
                return;
            if (GetInput(out PlayerSyncStruct incoming))
            {
                PlayerData.networkedCCPosition = incoming.ccPosition;
                PlayerData.networkedCCRotation = incoming.ccRotation;
                PlayerData.networkedViewPoint = incoming.viewRotation;
            }
        }

        public void Update()
        {
            if (HasInputAuthority)
            {
                if (Cursor.lockState == CursorLockMode.Locked)
                {
                    _playerViewController.Process();
                    _playerSelectController.Process();
                    _playerCharacterController.ProcessRotate(_playerViewController.GetPitch(), Time.deltaTime);
                }
                _playerCharacterController.ProcessMove(Time.deltaTime);
            }
        }

        #region Runner Callbacks
        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player){}
        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player){}
        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input){}
        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason){}
        // ReSharper disable once Unity.IncorrectMethodSignature
        public void OnConnectedToServer(NetworkRunner runner){}
        public void OnDisconnectedFromServer(NetworkRunner runner){}
        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token){}
        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason){}
        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message){}
        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList){}
        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data){}
        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken){}
        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data){}
        public void OnSceneLoadDone(NetworkRunner runner){}
        public void OnSceneLoadStart(NetworkRunner runner){}
        #endregion

        #region EVENTS & EVENTS-RPC
        // TODO: Player events
        #endregion
    }
    
    public class PlayerViewController : PlayerInputPoller
    {
        // Externals
        private Angle yaw;
        public Angle GetYaw() => _viewTransform.rotation.eulerAngles.x;
        private Angle pitch;
        public Angle GetPitch() => _viewTransform.rotation.eulerAngles.y;
        // Internals readonly
        private readonly Transform _viewTransform;
        private PlayerViewSettings config => settings.viewSettings;

        public PlayerViewController(PlayerSettingsSO settings, Transform viewTransform) : base(settings)
        {
            _viewTransform = viewTransform;
        }
        
        public void Process()
        {
            var input = PollInput();
            DoRotateCamera(input.MouseDelta);
        }
        
        private void DoRotateCamera(Vector2 delta)
        {
            yaw += delta.x;
            pitch += delta.y;
            
            pitch = AngleUtil.CustomClampAngle(pitch);
            
            var rotation = Quaternion.Euler((float)pitch, (float)yaw, 0);
            _viewTransform.rotation = rotation;
        }
    }
    
    public class PlayerCharacterController : PlayerInputPoller
    {
        // Consts
        private const float gravity = -18f;
        // Internals readonly
        private readonly CharacterController _characterController;
        private Transform _forwardTransform => _characterController.transform;
        private PlayerMovementSettings config => settings.MovementSettings;
        // Internals
        private Vector3 fallingVelocity = Vector3.zero;
        private float lerpValueForWalkingAndFlying = 0f;
        private bool isFlying;

        // Constructor
        public PlayerCharacterController(PlayerSettingsSO settings, CharacterController characterController) : base(settings)
        {
            _characterController = characterController;
        }
        
        public void ProcessMove(float deltaTime)
        {
            var input = PlayerMovementInputStruct.GetFromPlayerInputStruct(PollInput());
            isFlying = input.toggleFlying ? !isFlying : isFlying;
            DoLerp(input, out float forwardInputValue, out float sideInputValue, isFlying, deltaTime);
            DoMove(input, forwardInputValue, sideInputValue, deltaTime);
        }

        public void ProcessRotate(Angle yawTarget, float deltaTime)
        {
            var newRotation = Quaternion.Euler(0, (float)yawTarget, 0);
            _characterController.transform.rotation = newRotation;
        }

        private void DoMove(PlayerMovementInputStruct input, float forwardInputValue, float sideInputValue, float deltaTime)
        {
            if (isFlying)
            {
                var elevationChange = Vector3.zero;
                if (input.isJumping)
                    elevationChange.y++;
                if (input.isCrouching)
                    elevationChange.y--;
                fallingVelocity = elevationChange * config.FlyUpSpeed * 1000 * deltaTime;
                if ((_characterController.collisionFlags & CollisionFlags.Below) != 0)
                    _characterController.Move((_forwardTransform.forward * forwardInputValue + _forwardTransform.right * sideInputValue) * deltaTime);
                else
                    _characterController.Move((_forwardTransform.forward * forwardInputValue + _forwardTransform.right * sideInputValue + fallingVelocity) * deltaTime);
            }
            else
            {
                if ((_characterController.collisionFlags & CollisionFlags.Below) != 0 && input.isJumping)
                    fallingVelocity = Vector3.up * config.JumpSpeed;
                else if ((_characterController.collisionFlags & CollisionFlags.Below) != 0)
                    fallingVelocity = Vector3.zero;
                else
                    fallingVelocity += gravity * Vector3.up * deltaTime;
                _characterController.Move((_forwardTransform.forward * forwardInputValue + _forwardTransform.right * sideInputValue + fallingVelocity) * deltaTime);
            }
        }
        
        #region UTIL
        private void DoLerp(PlayerMovementInputStruct input, out float forwardInputValue, out float sideInputValue, bool isFlying, float deltaTime)
        {
            float moveSpeed = config.GetSpeed(input.isSprinting, isFlying);
            forwardInputValue = input.moveInput.y * MovementLerp(0,  moveSpeed, deltaTime);
            sideInputValue = input.moveInput.x * MovementLerp(0, moveSpeed, deltaTime);
            if (forwardInputValue == 0 && sideInputValue == 0 && (_characterController.collisionFlags & CollisionFlags.Below) != 0)
                lerpValueForWalkingAndFlying = 0f;
        }
        private float MovementLerp(float moveSpeed, float flySpeed, float deltaTime)
        {
            deltaTime *= deltaTime < 0 ? 2f : 1;
            lerpValueForWalkingAndFlying = Mathf.Clamp(0.75f * deltaTime + lerpValueForWalkingAndFlying, 0f, 1f);
            return Mathf.Lerp(moveSpeed, flySpeed, lerpValueForWalkingAndFlying);
        }
        #endregion
    }

    public class PlayerSelectController : PlayerInputPoller
    {
        public PlayerSelectController(PlayerSettingsSO settings) : base(settings) { }

        public void Process()
        {
            // TODO: player selection processing or when a player clicks etc.
            var input = PollInput();
            if (input.LeftClick)
            {
                
            }
            if (input.LeftClickHold)
            {
                
            }
            if (input.RightClick)
            {
                
            }
            if (input.RightClickHold)
            {
                
            }
            if (input.Shift)
            {
                
            }
        }
    }
    
    public struct PlayerGameData : IGameData
    {
        public int ownerID { get; set; }
        public Vector2Int index { get; set; }
        // Visual/Movement
        public Vector3 networkedCCPosition { get; set; }
        public Quaternion networkedCCRotation { get; set; }
        public Quaternion networkedViewPoint { get; set; }
    }

    // TODO: i think this should be changed ? it's weird
    public struct PlayerSyncStruct : INetworkInput
    {
        public Vector3 ccPosition { get; set; }
        public Quaternion ccRotation { get; set; }
        public Quaternion viewRotation { get; set; }
    }
}
