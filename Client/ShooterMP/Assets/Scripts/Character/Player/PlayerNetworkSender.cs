using System.Collections.Generic;
using UnityEngine;

namespace ShooterMP.Character.Player
{
    using ShooterMP.Multiplayer;
    using ShooterMP.Gun;
    
    public class PlayerNetworkSender : MonoBehaviour
    {
        private Multiplayer _multiplayer;

        private void Start()
        {
            _multiplayer = Multiplayer.Instance;
        }

        public void SendShoot(ref ShootInfo shootInfo)
        {
            shootInfo.key = _multiplayer.GetSessionID();
            
            string json = JsonUtility.ToJson(shootInfo);
            
            _multiplayer.SendMessage("shoot", json);
        }
        
        public void SendMove(Vector3 position, Vector3 velocity, float rotateX, float rotateY)
        {
            Dictionary<string, object> data = new()
            {
                {"pX", position.x},
                {"pY", position.y},
                {"pZ", position.z},
                {"vX", velocity.x},
                {"vY", velocity.y},
                {"vZ", velocity.z},
                {"rX", rotateX},
                {"rY", rotateY}
            };
            _multiplayer.SendMessage("move", data);
        }
    }
}

