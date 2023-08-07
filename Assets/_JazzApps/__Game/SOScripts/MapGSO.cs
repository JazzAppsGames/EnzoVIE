using System.Collections.Generic;
using Fusion;
using UnityEngine;
using Random = UnityEngine.Random;

// LATER: a custom map builder lol 

namespace JazzApps.Game
{
    /// <summary>
    /// The overall configuration and pointer for things
    /// </summary>
    [CreateAssetMenu(fileName = "Map", menuName = "SO/Game/Map")]
    public class MapGSO : GameScriptableObject
    {
        public MapConfig Config;
    }



    [System.Serializable]
    public struct MapConfig : IGameConfig
    {
        public Vector2Int Index { get; set; }

        public string name;
        public List<Point> waypoints;
        public List<Point> nodepoints;// LATER: nodeRotations
        public Point playerspawnpoint;
        [Range(0,3)] public float randomSpawnRange;
        public Point GetPlayerSpawn()
        {
            float r = this.randomSpawnRange;
            Vector3 randomSpawn = new Vector3(
                this.playerspawnpoint.position.x + Random.Range(-r, r),
                this.playerspawnpoint.position.y, 
                this.playerspawnpoint.position.z + Random.Range(-r, r));
            return new Point(randomSpawn, this.playerspawnpoint.rotation);
        }
    }
    [System.Serializable]
    public struct Point
    {
        [SerializeField] public Vector3 position;
        [SerializeField] public Quaternion rotation;
        public Point(Vector3 position)
        {
            this.position = position;
            this.rotation = Quaternion.identity;
        }
        public Point(Vector3 position, Quaternion rotation)
        {
            this.position = position;
            this.rotation = rotation;
        }
    }
    public struct PointInputStruct : INetworkInput
    {
        public Vector3 position { get; set; }
        public Quaternion rotation { get; set; }
    }
}