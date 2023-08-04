using UnityEngine;

namespace JazzApps.Utils
{
    public class NodeUtils : MonoBehaviour
    {
        // TODO: have a node visual instead of a debug visual
        private void OnDrawGizmos()
        {
            Transform t = this.transform;
            for (int i = 0; i < t.childCount; i++)
            {
            
                Gizmos.DrawWireCube(t.GetChild(i).position + .5f*Vector3.down, new Vector3(3,1,3));;
            }
        }
    }

}