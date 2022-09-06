using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StrategyFight
{
    [CreateAssetMenu(fileName = "PrefabsSO", menuName = "GAME/PrefabsSO")]
    public class PrefabsSO : ScriptableObject
    {
        public CharacterView characterPrefab;
        public LevelView levelPrefab;
    }
}
