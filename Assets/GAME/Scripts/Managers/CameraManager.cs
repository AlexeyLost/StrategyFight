using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace StrategyFight
{
    public class CameraManager : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera cam;

        private CinemachineTransposer camTrans;

        private void Start()
        {
            camTrans = cam.GetCinemachineComponent<CinemachineTransposer>();
        }

        private void OnEnable()
        {
            EventBus.Instance.CameraEvents.SetTarget += SetTarget;
            EventBus.Instance.CameraEvents.SetRandomTarget += SetRandomTarget;
            EventBus.Instance.CameraEvents.CheckIfNeedChangeTarget += CheckIfNeedChangeTarget;
            EventBus.Instance.CameraEvents.ZoomIn += ZoomIn;
        }

        private void OnDisable()
        {
            EventBus.Instance.CameraEvents.SetTarget -= SetTarget;
            EventBus.Instance.CameraEvents.SetRandomTarget -= SetRandomTarget;
            EventBus.Instance.CameraEvents.CheckIfNeedChangeTarget -= CheckIfNeedChangeTarget;
            EventBus.Instance.CameraEvents.ZoomIn -= ZoomIn;
        }

        private void SetTarget(Transform targetTrans)
        {
            cam.Follow = targetTrans;
            cam.LookAt = targetTrans;
        }

        private void SetRandomTarget()
        {
            if (GameData.Instance.AllCharacters.Count == 1) 
                SetTarget(GameData.Instance.AllCharacters[0].View.transform);
            else
            {
                Transform targetTrans = GameData.Instance
                    .AllCharacters[Random.Range(0, GameData.Instance.AllCharacters.Count)].View.transform;
                SetTarget(targetTrans);
            }
        }

        private void CheckIfNeedChangeTarget(Transform curTrans)
        {
            if (cam.Follow == curTrans) SetRandomTarget();
        }

        private void ZoomIn()
        {
            DOTween.To(() => camTrans.m_FollowOffset, x => 
                camTrans.m_FollowOffset = x, new Vector3(0, 2, -6), 1f);
        }
    }
}
