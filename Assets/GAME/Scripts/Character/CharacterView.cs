using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace StrategyFight
{
    public class CharacterView : MonoBehaviour
    {
        public MeshRenderer rend;
        public Transform modelTransform;
        public Image healthBarImage;
        public NavMeshAgent agent;
        public Weapon weapon;
        public CanvasGroup canvasGroup;
    }
}
