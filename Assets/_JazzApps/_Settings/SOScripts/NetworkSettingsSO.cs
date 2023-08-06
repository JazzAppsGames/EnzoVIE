using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using JazzApps.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using GUIL = UnityEngine.GUILayout;

namespace JazzApps.Settings
{
    [CreateAssetMenu(fileName = "NetworkSettings", menuName = "SO/Settings/Network")]
    public class NetworkSettingsSO : SettingsScriptableObject
    {
        [field: SerializeField] public SceneReference MultiplayerScene { get; private set; }
        [field: SerializeField] public GameMode GameMode { get; set; } // TODO: modifying this with UI?
        [field: SerializeField] public string SessionName { get; set; } // TODO: modifying this with UI
        public StartGameArgs GetStartArgs(byte[] connectionToken) => new StartGameArgs()
        {
            GameMode = GameMode,
            SessionName = SessionName,
            Scene = SceneManager.GetSceneByPath(MultiplayerScene.ScenePath).buildIndex,
            ConnectionToken = connectionToken,
        };
    }
}