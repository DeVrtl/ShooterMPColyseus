using UnityEngine;

namespace ShooterMP.Multiplayer
{
    public class SpawnPoints : MonoBehaviour
    {
        [SerializeField] private Transform[] _points;
        
        public int Length => _points.Length;

        public void GetPoint(int index, out Vector3 position, out Vector3 rotation)
        {
            if (index >= _points.Length || index < 0)
            {
                position = Vector3.zero;
                rotation = Vector3.zero;
                return;
            }
            
            position = _points[index].position;
            rotation = _points[index].eulerAngles;
        }
    }
}
