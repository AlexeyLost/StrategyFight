using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace StrategyFight
{
    public class Weapon : MonoBehaviour
    {
        private Quaternion startRotation;

        private void Start()
        {
            startRotation = transform.rotation;
        }

        public void Attack(float duration)
        {
            transform.DOLocalRotate(Vector3.zero, duration).SetLoops(2, LoopType.Yoyo);
        }
    }
}
