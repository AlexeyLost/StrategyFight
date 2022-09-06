using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

namespace StrategyFight
{
    public class Character
    {
        public CharacterView View { get; }

        private int health;
        private int damage;
        private bool started;
        private CharacterState curState;
        private Character targetChar;
        private List<Character> allChars = GameData.Instance.AllCharacters;
        private Coroutine attackCoroutine;
        private Coroutine findTargetCoroutine;
        private int startHealth;

        public Character(Transform charactersParentTrans)
        {
            View = GameObject.Instantiate(GameData.Instance.Prefabs.characterPrefab, GetRandomPosition(), Quaternion.identity);
            View.transform.SetParent(charactersParentTrans);

            Color rndColor = Random.ColorHSV(0.1f, 1f, 0.7f, 1f, 0.7f, 1f);
            View.rend.material.color = rndColor;
            
            health = Random.Range(50, 200);
            startHealth = health;
            damage = Random.Range(5, 20);
            View.healthBarImage.fillAmount = 1f;
            ChangeState(CharacterState.idle);

            EventBus.Instance.MainEvents.Unsubscribe += Unsubscribe;
            EventBus.Instance.MainEvents.GameStarted += StartBehaviour;
            EventBus.Instance.MainEvents.OnUpdate += OnUpdate;
        }

        private void Unsubscribe()
        {
            View.weapon.transform.DOKill();
            View.healthBarImage.DOKill();
            View.StopAllCoroutines();
            EventBus.Instance.MainEvents.Unsubscribe -= Unsubscribe;
            EventBus.Instance.MainEvents.GameStarted -= StartBehaviour;
            EventBus.Instance.MainEvents.OnUpdate -= OnUpdate;
        }

        private Vector3 GetRandomPosition()
        {
            Vector2 rndDirVector = Random.insideUnitCircle.normalized * 
                                   Random.Range(0f, GameData.Instance.CurLevel.GetSpawnRadius());
            Vector3 spawnPoint = new Vector3(rndDirVector.x, 
                GameData.Instance.Prefabs.characterPrefab.transform.position.y, rndDirVector.y);
            
            LayerMask charactersMask = LayerMask.GetMask("Characters");
            while (Physics.CheckSphere(spawnPoint, 2f, charactersMask))
            {
                Vector2 rndVector = Random.insideUnitCircle.normalized * 
                                    Random.Range(0f, GameData.Instance.CurLevel.GetSpawnRadius());
                spawnPoint = new Vector3(rndVector.x, 
                    GameData.Instance.Prefabs.characterPrefab.transform.position.y, rndVector.y);
            }
            
            return spawnPoint;
        }

        private void StartBehaviour()
        {
            started = true;
        }

        private void OnUpdate()
        {
            if (started)
            {
                if (curState == CharacterState.goingToTarget)
                {
                    SetNextDestination(targetChar);
                    if (View.agent.remainingDistance < 1.6f) ChangeState(CharacterState.attacking);
                }
            }
        }

        private void ChangeState(CharacterState _state)
        {
            switch (_state)
            {
                case CharacterState.idle:
                    View.agent.isStopped = true;
                    break;
                case CharacterState.findingTarget:
                    findTargetCoroutine = View.StartCoroutine(FindingTarget());
                    break;
                case CharacterState.goingToTarget:
                    SetNextDestination(targetChar);
                    break;
                case CharacterState.attacking:
                    View.agent.isStopped = true;
                    View.transform.DOLookAt(targetChar.View.transform.position, 0.3f);
                    attackCoroutine = View.StartCoroutine(Attack());
                    break;
                case CharacterState.dead:
                    DeathActions();
                    break;
                case CharacterState.win:
                    View.StartCoroutine(WinActions());
                    break;
            }
            curState = _state;
        }

        private IEnumerator WinActions()
        {
            View.canvasGroup.DOFade(0, 0.2f);
            View.modelTransform.DOLocalRotate(new Vector3(0, 180, 0), 0.3f);
            View.modelTransform.DOLocalJump(Vector3.zero, 1f, 1, 0.5f).
                SetLoops(-1, LoopType.Restart).SetDelay(0.4f);
            EventBus.Instance.CameraEvents.ZoomIn?.Invoke();
            yield return new WaitForSeconds(2f);
            EventBus.Instance.UIEvents.ShowLevelCompleteScreen?.Invoke();
        }

        private void SetNextDestination(Character target)
        {
            View.agent.isStopped = false;
            Vector3 newPoint = new Vector3(target.View.transform.position.x, View.transform.position.y, 
                target.View.transform.position.z);
            NavMeshPath path = new NavMeshPath();
            View.agent.CalculatePath(newPoint, path);
            View.agent.SetPath(path);
        }

        public Character GetNearestFreeChar(List<Character> availableCharacters)
        {
            float minDist = float.PositiveInfinity;
            Character nearestChar = null;
            availableCharacters.Clear();

            foreach (var curChar in allChars)
            {
                if (curChar == this) continue;
                if (curChar.CanBeTarget()) availableCharacters.Add(curChar);
            }

            if (availableCharacters.Count == 0) return nearestChar;
            
            foreach (var curChar in availableCharacters)
            {
                float curDist = Vector3.Distance(View.transform.position, curChar.View.transform.position);
                if (curDist < minDist)
                {
                    minDist = curDist;
                    if (curChar.CanBeTarget()) nearestChar = curChar;
                }
            }

            return nearestChar;
        }
        
        public bool CanBeTarget()
        {
            return curState == CharacterState.idle ||
                   curState == CharacterState.findingTarget;
        }
        
        public void SetTarget(Character _target)
        {
            if (findTargetCoroutine != null) View.StopCoroutine(findTargetCoroutine);
            targetChar = _target;
            ChangeState(CharacterState.goingToTarget);
        }

        private IEnumerator Attack()
        {
            bool targetAlive = true;
            float attackDuration = Random.Range(0.2f, 0.5f);
            
            while (targetAlive)
            {
                View.weapon.Attack(attackDuration);
                targetChar.TakeDamage(damage, out targetAlive, attackDuration);
                yield return new WaitForSeconds(attackDuration * 2f);
                attackDuration = Random.Range(0.2f, 0.5f);
            }

            allChars.Remove(targetChar);
            EventBus.Instance.CameraEvents.CheckIfNeedChangeTarget?.Invoke(targetChar.View.transform);
            targetChar = null;
            if (allChars.Count > 1)
            {
                ChangeState(CharacterState.findingTarget);
            } else ChangeState(CharacterState.win);
        }

        private IEnumerator FindingTarget()
        {
            bool targetFound = false;
            WaitForSeconds delay = new WaitForSeconds(1f);
            List<Character> availableChars = new List<Character>();

            yield return null;
            while (!targetFound)
            {
                Character nextTarget = GetNearestFreeChar(availableChars);
                if (nextTarget != null)
                {
                    targetFound = true;
                    SetTarget(nextTarget);
                    nextTarget.SetTarget(this);
                }
                yield return delay;
            }
        }
        
        public void TakeDamage(int _damage, out bool _alive, float duration)
        {
            health -= _damage;
            View.rend.material.DOKill(true);
            View.rend.material.DOColor(Color.red, duration / 2).SetLoops(2, LoopType.Yoyo);
            if (health <= 0)
            {
                View.StopAllCoroutines();
                ChangeState(CharacterState.dead);
                health = 0;
            }

            float healthNormalizedValue = Mathf.InverseLerp(0, startHealth, health);
            View.healthBarImage.DOFillAmount(healthNormalizedValue, 0.3f);
            
            _alive = health > 0;
        }

        private void DeathActions()
        {
            View.transform.DOScale(0, 0.5f).
                OnComplete(() => View.gameObject.SetActive(false));
        }
        
        private enum CharacterState
        {
            idle = 0,
            findingTarget = 1,
            goingToTarget = 2,
            attacking = 3,
            dead = 4,
            win = 5,
        }
    }
}
