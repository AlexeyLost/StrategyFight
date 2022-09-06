using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StrategyFight
{
    public class GameData
    {
        private static GameData instance;
        
        public static GameData Instance
        {
            get
            {
                if (instance == null) instance = new GameData();
                return instance;
            }
        }

        public GameData()
        {
            AllCharacters = new List<Character>();
            
            EventBus.Instance.MainEvents.Unsubscribe += Unsubscribe;
        }

        private void Unsubscribe()
        {
            AllCharacters.Clear();
            CurLevel = null;
            
            EventBus.Instance.MainEvents.Unsubscribe -= Unsubscribe;
        }
        
        public PrefabsSO Prefabs { get; set; }
        public List<Character> AllCharacters { get; }
        public Level CurLevel { get; set; }
    }
}
