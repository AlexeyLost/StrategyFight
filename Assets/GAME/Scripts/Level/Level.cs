using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StrategyFight
{
    public class Level
    {
        public LevelView View { get; }

        public Level()
        {
            View = GameObject.Instantiate(GameData.Instance.Prefabs.levelPrefab);
        }

        public float GetSpawnRadius()
        {
            return View.spawnRadius;
        }
    }
}
