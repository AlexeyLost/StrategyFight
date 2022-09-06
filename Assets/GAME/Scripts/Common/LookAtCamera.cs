using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StrategyFight
{
    public class LookAtCamera : MonoBehaviour
    {
        private Camera cam;

        private void OnEnable()
        {
            cam = Camera.main;
        }

        private void Update()
        {
            Vector3 v = cam.transform.position - transform.position;
            v.x = v.z = 0.0f;
            transform.LookAt( cam.transform.position - v ); 
            transform.Rotate(0,180,0);
        }
    }
}
