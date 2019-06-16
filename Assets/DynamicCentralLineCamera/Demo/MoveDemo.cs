using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DynamicCentralLineCamera.Demo
{
    public class MoveDemo : MonoBehaviour
    {
        
        [SerializeField] private float speed = 5f;
        [SerializeField] private float radius = 0.1f;

        private float zDelta;
        private bool isForward;
        private float angle;

        private void Update()
        {
            var delta = Time.deltaTime;
            angle += speed * delta;

            if (zDelta > 1)
            {
                isForward = false;
            }
            else if(zDelta < 0)
            {
                isForward = true;
            }
            
            zDelta += isForward ? delta : delta * -1;

            var rot = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle)) * radius;
            transform.position = new Vector3(rot.x, rot.y, Mathf.Lerp(-5, 50, zDelta));
        }
    }
}