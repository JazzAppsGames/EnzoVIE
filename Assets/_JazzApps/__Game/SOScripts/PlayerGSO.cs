using Fusion;
using UnityEngine;
using Random = UnityEngine.Random;

namespace JazzApps.Game
{
    [CreateAssetMenu(fileName = "Player", menuName = "SO/Game/Player")]
    public class PlayerGSO : GameScriptableObject
    {
        public PlayerConfig Config;
    }

    [System.Serializable]
    public struct PlayerConfig : IGameConfig
    {
        public Vector2Int Index { get; set; }
    }
}