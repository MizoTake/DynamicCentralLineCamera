using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace DynamicCentralLineCamera
{
    class DynamicCentralLineCamera : MonoBehaviour
    {

        [SerializeField] private Material material;
        [SerializeField] private bool dummy = false;

        private GameObject dummyCube;
        private Vector3 CachePosition = Vector3.zero;
        private static readonly int CenterX = Shader.PropertyToID("_CenterX");
        private static readonly int CenterY = Shader.PropertyToID("_CenterY");
        private static readonly int CentralEdge = Shader.PropertyToID("_CentralEdge");

        // Start is called before the first frame update
        void Start()
        {
            if (!dummy) return;
            dummyCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            dummyCube.transform.position = transform.position + Vector3.forward * 10f;
        }

        // Update is called once per frame
        void Update()
        {
            if (!dummy) return;
            var position = dummyCube.transform.position;
            var screenPos = Camera.main.WorldToScreenPoint(position);
            material.SetFloat(CenterX, screenPos.x / Screen.width);
            material.SetFloat(CenterY, screenPos.y / Screen.height);
            var camPos = Camera.main.transform.position;
            var distance = Vector3.Distance(position, camPos) / 10f;
            material.SetFloat(CentralEdge, Mathf.Clamp(distance, 0f, 0.5f));
            Debug.Log(Vector3.Distance(position, camPos));
            
            if (CachePosition == transform.position) return;
            var direction = transform.TransformDirection(CachePosition).normalized;
            dummyCube.transform.position = direction;
            Debug.Log(transform.TransformDirection(CachePosition).normalized);

            CachePosition = transform.position;
        }
        
        void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
//            Graphics.Blit(src, dest, material);
        }
    }
}