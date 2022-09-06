using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace StrategyFight
{
    public class LevelCompleteScreen : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Button playAgainButton;

        private void OnEnable()
        {
            playAgainButton.onClick.AddListener(PlayAgainClicked);
        }

        private void OnDisable()
        {
            playAgainButton.onClick.RemoveListener(PlayAgainClicked);
        }

        public void Show()
        {
            canvasGroup.alpha = 0;
            gameObject.SetActive(true);
            canvasGroup.DOFade(1, 0.3f);
        }

        private void PlayAgainClicked()
        {
            EventBus.Instance.MainEvents.RestartGame?.Invoke();
        }
    }
}
