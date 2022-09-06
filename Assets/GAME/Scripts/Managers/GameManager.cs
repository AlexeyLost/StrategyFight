using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace StrategyFight
{
    public class GameManager : MonoBehaviour
    {
        [Header("Spawn settings")] 
        [SerializeField] private int charactersCount;
        
        [Header("References")]
        [SerializeField] private PrefabsSO prefabs;
        [SerializeField] private Transform charactersParentTrans;


        private void Start()
        {
            InitGame();
            
            EventBus.Instance.MainEvents.GameInitialized?.Invoke();
            EventBus.Instance.MainEvents.GameStarted?.Invoke();
        }

        private void InitGame()
        {
            Application.targetFrameRate = 60;
            DOTween.SetTweensCapacity(2000, 200);
            
            GameData.Instance.Prefabs = prefabs;

            SpawnLevel();
            SpawnCharacters();

            EventBus.Instance.MainEvents.Unsubscribe += Unsubscribe;
            EventBus.Instance.MainEvents.RestartGame += RestartGame;
        }

        private void Unsubscribe()
        {
            EventBus.Instance.MainEvents.Unsubscribe -= Unsubscribe;
            EventBus.Instance.MainEvents.RestartGame -= RestartGame;
        }
        
        private void RestartGame()
        {
            EventBus.Instance.MainEvents.Unsubscribe?.Invoke();
            DOTween.KillAll();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void SpawnLevel()
        {
            GameData.Instance.CurLevel = new Level();
        }
        
        private void SpawnCharacters()
        {
            GameData.Instance.AllCharacters.Clear();
            for (int i = 0; i < charactersCount; i++)
            {
                Character curChar = new Character(charactersParentTrans);
                GameData.Instance.AllCharacters.Add(curChar);
            }

            //start first attack
            foreach (var curChar in GameData.Instance.AllCharacters)
            {
                if (!curChar.CanBeTarget()) continue;
                List<Character> availableChars = new List<Character>();
                Character nearestChar = curChar.GetNearestFreeChar(availableChars);
                if (nearestChar == null) continue;
                curChar.SetTarget(nearestChar);
                nearestChar.SetTarget(curChar);
            }

            
            EventBus.Instance.CameraEvents.SetTarget?.
                Invoke(GameData.Instance.AllCharacters[Random.Range(0, GameData.Instance.AllCharacters.Count)].View.transform);
        }

        private void Update()
        {
            EventBus.Instance.MainEvents.OnUpdate?.Invoke();
        }
    }
}
