using Fusion;
using UnityEngine;

namespace JazzApps.Game
{
    public class MapGB : GameBehavior
    {
        [Networked] public ref MapGameData MapData  => ref MakeRef<MapGameData>();
        protected override IGameData Data => MapData;
        public MapGSO GSO => container.mapGSO;
        public MapConfig Config => GSO.Config;
        
        public override void Spawned()
        {
            base.Spawned();
        }

        public override void Init(Vector2Int index, int ownerID, bool reset)
        {
            if (reset)
                ResetND();
        }

        protected override void ResetND()
        {
            
        }

        // TODO: MapGB is necessary for "proper" lightning armageddon
    }

    public struct MapGameData : IGameData
    {
        public int ownerID { get; set; }
        public Vector2Int index { get; set; }
    }
}