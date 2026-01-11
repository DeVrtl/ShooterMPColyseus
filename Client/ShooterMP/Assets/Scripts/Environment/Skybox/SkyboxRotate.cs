using UnityEngine;
using UnityEngine.Rendering;

namespace Environment.Skybox
{
    public class SkyboxRotate : MonoBehaviour
    {
        [SerializeField] private float _rotationSpeed = 1.0f;
        
        private Material _skyboxMaterial;

        private void Start()
        {
            _skyboxMaterial = RenderSettings.skybox;
        }

        private void Update()
        {
            float rotation = _skyboxMaterial.GetFloat("_Rotation");
        
            rotation += _rotationSpeed * Time.deltaTime;
            
            _skyboxMaterial.SetFloat("_Rotation", rotation);
        }
    }
}
