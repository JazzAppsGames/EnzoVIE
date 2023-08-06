using Fusion;
using JazzApps.Settings;
using UnityEngine;

namespace JazzApps.Game
{
    /// <summary>
    /// Handles polling input.
    /// </summary>
    public abstract class PlayerInputPoller
    {
        private readonly InputActions inputActions;
        protected readonly PlayerSettingsSO settings;

        // Constructor
        protected PlayerInputPoller(PlayerSettingsSO settings)
        {
            this.settings = settings;
            this.inputActions = new InputActions();
            Disable();
        }

        public void Enable() => this.inputActions.Enable();
        public void Disable() => this.inputActions.Disable();

        public PlayerInputStruct PollInput()
        {
            var PIS = new PlayerInputStruct();
            
            var wasdCache = inputActions.Player.WASD.ReadValue<Vector2>();
            PIS.WASD = new Vector2(Mathf.Clamp((float)wasdCache.x, -1, 1), Mathf.Clamp((float)wasdCache.y, -1, 1));
            PIS.MouseDelta = inputActions.Player.MouseDelta.ReadValue<Vector2>();
            PIS.LeftClick = inputActions.Player.LeftClick.triggered;
            PIS.LeftClickHold = inputActions.Player.LeftClick.IsPressed();
            PIS.RightClick = inputActions.Player.RightClick.triggered;
            PIS.RightClickHold = inputActions.Player.RightClick.IsPressed();
            PIS.Shift = inputActions.Player.Shift.IsPressed();
            PIS.Ctrl = inputActions.Player.Ctrl.IsPressed();
            PIS.Spacebar = inputActions.Player.Spacebar.IsPressed();
            PIS.PauseEscape = inputActions.Player.PauseEscape.triggered;
            PIS.Q = inputActions.Player.Q.triggered;
            PIS.E = inputActions.Player.E.triggered;
            PIS.F = inputActions.Player.F.triggered;
            PIS.C = inputActions.Player.C.triggered;
            PIS.R = inputActions.Player.R.triggered;
            if (inputActions.Player.NumKey.triggered)
                PIS.NumKey = (int)inputActions.Player.NumKey.ReadValue<float>();
            PIS.Scroll = Mathf.Clamp((float)inputActions.Player.Scroll.ReadValue<float>(), -1, 1);

            return PIS;
        }
    }
    
    public struct PlayerInputStruct
    {
        public Vector2 WASD;
        public Vector2 MouseDelta;
        public float Scroll;
        public int NumKey;
        public bool LeftClick, LeftClickHold, RightClick, RightClickHold, Shift, Ctrl, Spacebar, PauseEscape, Q, E, F, C, R;
    }
    public struct PlayerMovementInputStruct : INetworkInput
    {
        public static PlayerMovementInputStruct GetFromPlayerInputStruct(PlayerInputStruct playerInputStruct)
        {
            return new PlayerMovementInputStruct
            {
                moveInput = playerInputStruct.WASD,
                isSprinting = playerInputStruct.Shift,
                isJumping = playerInputStruct.Spacebar,
                isCrouching = playerInputStruct.Ctrl,
                toggleFlying = playerInputStruct.F,
            };
        }
        public Vector2 moveInput { get; private set; }
        public bool isSprinting { get; private set; }
        public bool isJumping { get; private set; }
        public bool isCrouching { get; private set; }
        public bool toggleFlying { get; private set; }
    }
    public struct PlayerDisplayInputStruct
    {
        public static PlayerDisplayInputStruct GetFromPlayerInputStruct(PlayerInputStruct playerInputStruct)
        {
            return new PlayerDisplayInputStruct
            {
                TogglePauseOrEscape = playerInputStruct.PauseEscape,
                toggleBuildSection = playerInputStruct.Q,
                toggleSendMenu = playerInputStruct.E,
                toggleShopMenu = playerInputStruct.C,
                towerSelectNum = playerInputStruct.NumKey,
                towerSelectUp = playerInputStruct.Scroll > 0,
                towerSelectDown = playerInputStruct.Scroll < 0,
            };
        }
        public bool TogglePauseOrEscape { get; private set; }
        public bool toggleBuildSection { get; private set; }
        public bool toggleSendMenu { get; private set; }
        public bool toggleShopMenu { get; private set; }
        public int towerSelectNum { get; private set; }
        public bool towerSelectUp { get; private set; }
        public bool towerSelectDown { get; private set; }
    }
}