using Fusion;
using UnityEngine;

namespace JazzApps.Game
{
    public interface IGameData : INetworkStruct
    {
        public int ownerID { get; set; }
        public Vector2Int index { get; set; }
        public int group => index.x; // TODO: utilize these everywhere?
        public int upgrade => index.y;
    }
}