using System;
using System.Collections.Generic;
using Fusion;
using JazzApps;
using JazzApps.Application;
using JazzApps.Game;
using JazzApps.Settings;
using JazzApps.Utils;
using JazzApps.Visual;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using static JazzApps.Game.PlayerGSO;
using Logger = JazzApps.Utils.Logger;

namespace JazzApps.Display
{
    public class DisplayManager : Manager
    {
        [field: SerializeField] private DisplayContainerSO container;
        protected override Container BaseContainer => container;
        
        private ApplicationManager applicationManager => ApplicationManager.Instance;
        private GameContainerSO gameContainer => applicationManager.GetContainer(typeof(GameContainerSO)) as GameContainerSO;
        private VisualContainerSO visualContainer => applicationManager.GetContainer(typeof(VisualContainerSO)) as VisualContainerSO;
        private SettingsContainerSO settingsContainer => applicationManager.GetContainer(typeof(SettingsContainerSO)) as SettingsContainerSO;

        //[Header("Menus")] 
        //[Header("Sections")]
        
        //private IEnumerable<Display> gameDisplays => new Display[] {  };

        private GenericPlayerInputPoller inputPoller;

        #region MONOBEHAVIOUR CALLBACKS
        private void OnEnable()
        {
            SetVisibilityMenus(false);
            SetVisibilitySections(false);
            inputPoller = new(settingsContainer.PlayerSettings);
            inputPoller?.Enable();
            gameContainer.GBSpawnedEvent += BindGbToGDs;
            gameContainer.GBSelectedEvent += SelectGbToGDs;
        }
        private void OnDisable()
        {
            SetVisibilityMenus(false);
            SetVisibilitySections(false);
            inputPoller?.Disable();
            gameContainer.GBSpawnedEvent -= BindGbToGDs;
            gameContainer.GBSelectedEvent -= SelectGbToGDs;
        }
        private void Start()
        {
            InitGameDisplays(gameContainer, true);
            SetVisibilityMenus(false);
            SetVisibilitySections(false);
        }
        private void Update()
        {
            ProcessInput();
        }
        private void LateUpdate()
        {
            // TODO:
            /*
            foreach (var d in gameDisplays)
                d.LateRefresh();
            */
        }
        #endregion MONOBEHAVIOUR CALLBACKS
        
        private void InitGameDisplays(GameContainerSO gameContainerSo, bool resetAll)
        {
            // TODO: setting up display subbaseclasses
            /*
            foreach (var d in gameDisplays)
            {
                d.Init(gameContainerSo, resetAll);
                d.SetVisibility(false);
            }
            */
            /*
            foreach (var p in gameGSO.Players)
            {
                // TODO: init based on players? idk
            }
            */
        }
        private void ProcessInput()
        {
            var input = PlayerDisplayInputStruct.GetFromPlayerInputStruct(inputPoller.PollInput());
            
            if (input.TogglePauseOrEscape)
            {
                if (input.TogglePauseOrEscape)
                {
                    if (Cursor.lockState == CursorLockMode.Locked)
                    {
                        SetVisibilityMenus(false);
                        // TODO: show pause menu
                        Cursor.lockState = CursorLockMode.None;
                    }
                    else if (Cursor.lockState == CursorLockMode.None) 
                    {
                        SetVisibilityMenus(false);
                        // TODO: hide pause menu
                        Cursor.lockState = CursorLockMode.Locked;
                    }
                }
            }
            if (input.towerSelectNum > 0)
            {
                // TODO: toggle selectNum
                
            }
        }
        
        #region EVENTS RECEIVING
        private void BindGbToGDs(GameBehavior GB)
        {
            /*
            foreach (var d in gameDisplays)
            {
                d.BindSpawn(GB);
                d.Refresh();
                //TODO:
                switch (GB)
                {
                    case PlayerGB playerGB when playerGB.HasInputAuthority:
                        Cursor.lockState = CursorLockMode.Locked;
                        SetVisibilityMenus(false);
                        break;
                }
            }
            */
        }
        private void SelectGbToGDs(GameBehavior GB)
        {
            /*
            foreach (var d in gameDisplays)
            {
                d.BindSelect(GB);
                d.Refresh();
                // TODO:
                if (GB is PlayerGB GB && GB.HasInputAuthority)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    SetVisibilityMenus(false);
                    playerScoreboardSection.SetVisibility(true);
                }
            }
            */
        }
        #endregion EVENTS RECEIVING
        
        private void SetVisibilityMenus(bool visibility)
        {
            // TODO:
            /*
            foreach (var d in gameDisplays)
                if (d is DisplayMenu)
                    d.SetVisibility(visibility);
            if (!visibility)
                menuLabelText.text = "";
            */
        }
        private void SetVisibilitySections(bool visibility)
        {
            // TODO:
            /*
            foreach (var d in gameDisplays)
                if (d is DisplayBar)
                    d.SetVisibility(visibility);
            */
        }
    }
}
