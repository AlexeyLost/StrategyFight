using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StrategyFight
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private Button restartButton;
        [SerializeField] private Button camButton;
        [SerializeField] private LevelCompleteScreen _levelCompleteScreen;

        private void OnEnable()
        {
            restartButton.onClick.AddListener(RestartClicked);
            camButton.onClick.AddListener(CamClicked);
            EventBus.Instance.UIEvents.ShowLevelCompleteScreen += ShowLevelCompleteScreen;
        }

        private void OnDisable()
        {
            restartButton.onClick.RemoveListener(RestartClicked);
            camButton.onClick.RemoveListener(CamClicked);
            EventBus.Instance.UIEvents.ShowLevelCompleteScreen -= ShowLevelCompleteScreen;
        }

        private void RestartClicked()
        {
            EventBus.Instance.MainEvents.RestartGame?.Invoke();
        }

        private void ShowLevelCompleteScreen()
        {
            _levelCompleteScreen.Show();
        }

        private void CamClicked()
        {
            EventBus.Instance.CameraEvents.SetRandomTarget?.Invoke();
        }
    }
}
