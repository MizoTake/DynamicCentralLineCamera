using UnityEngine;
using UnityEngine.Assertions;

namespace DynamicCentralLineCamera
{
    class DynamicCentralLineCamera : MonoBehaviour
    {
        
        [SerializeField] private Material material;
        [SerializeField] private Camera camera = null;
        [SerializeField] private GameObject target;
        
        private Vector3 cachePosition = Vector3.zero;
        private Renderer targetRenderer;
        private float delta;

        private static readonly float EDGE_MAX = 0.35f;
        
        private static readonly int CenterX = Shader.PropertyToID("_CenterX");
        private static readonly int CenterY = Shader.PropertyToID("_CenterY");
        private static readonly int CentralEdge = Shader.PropertyToID("_CentralEdge");
        private static readonly int CentralLength = Shader.PropertyToID("_CentralLength");
        private static readonly int Central = Shader.PropertyToID("_Central");

        // Start is called before the first frame update
        void Start()
        {
            targetRenderer = target.GetComponent<Renderer>();
            if (!targetRenderer)
            {
                targetRenderer = target.GetComponentInChildren<Renderer>();
            }
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
            var central = noise + distance / 100f;
            
            if (!targetRenderer.isVisible)
            {
                if(delta < 1f) delta += Time.deltaTime;
                material.SetFloat(Central, Mathf.Lerp(central, 0, delta * 2f));
            }
            else
            {
                if (delta < 0)
                {
                    material.SetFloat(Central, central);
                    delta = 0;
                }
                else
                {
                    delta -= Time.deltaTime;
                    material.SetFloat(Central, Mathf.Lerp(central, 0, delta * 2f));
                }
            }
            
            if (cachePosition == transform.position) return;
            var direction = transform.TransformDirection(cachePosition).normalized;
            target.transform.position = direction;
            cachePosition = transform.position;
        }
        
        void OnRenderImage(RenderTexture src, RenderTexture dest) 
        {
            Graphics.Blit(src, dest, material);
        }
    }
}