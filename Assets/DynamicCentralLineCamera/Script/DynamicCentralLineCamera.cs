using UnityEngine;
using UnityEngine.Assertions;

namespace DynamicCentralLineCamera
{
    class DynamicCentralLineCamera : MonoBehaviour
    {
        
        [SerializeField] private Material material;
        [SerializeField] private Camera camera = null;
        [SerializeField] private GameObject target;
        
        private Vector3 CachePosition = Vector3.zero;

        private static readonly float EDGE_MAX = 0.35f;
        
        private static readonly int CenterX = Shader.PropertyToID("_CenterX");
        private static readonly int CenterY = Shader.PropertyToID("_CenterY");
        private static readonly int CentralEdge = Shader.PropertyToID("_CentralEdge");
        private static readonly int CentralLength = Shader.PropertyToID("_CentralLength");
        private static readonly int Central = Shader.PropertyToID("_Central");

        // Start is called before the first frame update
        void Start()
        {
            camera = camera ? camera : Camera.main;
            
            Assert.IsNotNull(target);
        }

        // Update is called once per frame
        void Update()
        {
            var position = target.transform.position;
            var screenPos = camera.WorldToScreenPoint(position);
            material.SetFloat(CenterX, screenPos.x / Screen.width);
            material.SetFloat(CenterY, screenPos.y / Screen.height);
            
            var camPos = camera.transform.position;
            var distance = Vector3.Distance(position, camPos) / 10f;
            var edge = EDGE_MAX * (1f - Mathf.Clamp(distance / 10f, 0f, EDGE_MAX) / EDGE_MAX);
            material.SetFloat(CentralEdge, edge);
            material.SetFloat(CentralLength, edge + distance / 10f);

            var noise = Random.Range(0.195f, 0.2f);
            material.SetFloat(Central, noise + distance / 100f);
            
            if (CachePosition == transform.position) return;
            var direction = transform.TransformDirection(CachePosition).normalized;
            target.transform.position = direction;
            CachePosition = transform.position;
        }
        
        void OnRenderImage(RenderTexture src, RenderTexture dest) 
        {
            Graphics.Blit(src, dest, material);
        }
    }
}