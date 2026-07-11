using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Link.Cooking;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Link
{
    public class LevelControl : //LevelBase
    TestBase
    {
        public enum Step { Step_1, Step_2, Step_3, Step_4, }
        [SerializeField] LevelStepBase[] levelSteps;
        [SerializeField] bool isControl = true;
        [SerializeField] SpriteRenderer blackSquare;
        public UnityEvent OnWinEvent, OnLoseEvent;

        [Header("Hint Editor")]
        [SerializeField] private Sprite[] hints;
        Step step;
        public LevelStepBase LevelStep { get; private set; }
        public static LevelControl Ins => instance as LevelControl;
        private IItemMoving itemMoving;
        private IItemMoving itemMovingTake;
        private IItemIdle itemIdle;
        Vector3 offset;
        Vector3 point;
        int levelIndex;

        private TestUI testUI;
        private bool isHaveUI => testUI != null;

        protected override void Awake()
        {
            base.Awake();
            testUI = GameObject.FindObjectOfType<TestUI>();
            if (isHaveUI) testUI.OnInit(hints);
        }

        protected override void Start()
        {
            base.Start();
            levelIndex = 0;
            LevelStep = levelSteps[levelIndex];
            LevelStep.OnStart();
            //_hint = levelStep.Hint;
            SetHint(LevelStep.Hint);
        }

        protected override void Update()
        {
            base.Update();
            if (!IsAllowInteract) return;
            if (isControl)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    itemMoving = GetItemMovingCanMove();
                    if (itemMoving != null)
                    {
                        //neu lay dc object co the di chuyen thi di chuyen no
                        SetItemMoving(itemMoving);
                    }
                    else
                    {
                        //khong thi tinh click object idle
                        GetItemIdle()?.OnClickDown();
                    }
                }

                if (Input.GetMouseButton(0))
                {
                    if (itemMoving == null) return;
                    //di chuyen object
                    itemMoving.TF.position = GetPoint() + offset;
                }

                if (Input.GetMouseButtonUp(0))
                {
                    if (itemMoving == null) return;
                    itemMovingTake = GetItemMoving(itemMoving.TF.position);
                    if (itemMovingTake != null && itemMovingTake.OnTake(itemMoving))
                    {
                        //Debug.Log(1);
                        //tim thang co the moving nhung layer cao nhat
                        itemMoving.OnClickTake();
                    }
                    else
                    {
                        if (GetItemIdle(itemMoving))
                        {
                            //Debug.Log(2);
                            //lay item idle
                            itemMoving?.OnClickTake();
                        }
                        else
                        {
                            //Debug.Log(3);
                            itemMoving?.OnDrop();
                        }
                    }
                    itemMoving = null;
                }
            }
        }

        public IItemIdle GetItemIdle()
        {
            IItemIdle item = null;

            RaycastHit2D[] hits = Physics2D.RaycastAll(GetPoint(), Vector2.zero);

            for (int i = 0; i < hits.Length; i++)
            {
                item = hits[i].collider.GetComponent<IItemIdle>();
                if (item != null)
                {
                    break;
                }
            }
            return item;
        }

        public bool GetItemIdle(IItemMoving itemMoving)
        {
            IItemIdle item = null;

            RaycastHit2D[] hits = Physics2D.RaycastAll(itemMoving.TF.position, Vector2.zero);

            for (int i = 0; i < hits.Length; i++)
            {
                item = hits[i].collider.GetComponent<IItemIdle>();
                if (item != null && item.OnTake(itemMoving))
                {
                    return true;
                }
            }
            return false;
        }

        public IItemMoving GetItemMovingCanMove()
        {
            IItemMoving itemCheck = null;
            IItemMoving item = null;

            RaycastHit2D[] hits = Physics2D.RaycastAll(GetPoint(), Vector2.zero);
            //Collider2D collider = null;

            for (int i = 0; i < hits.Length; i++)
            {
                itemCheck = hits[i].collider.GetComponent<IItemMoving>();
                if (itemCheck != null && itemCheck != this.itemMoving && itemCheck.IsCanMove && itemCheck.IsEnable)
                {
                    if (item == null || item.OrderLayer < itemCheck.OrderLayer)
                    {
                        item = itemCheck;
                        //collider = hits[i].collider;
                    }
                }
            }
            //if (collider != null) Debug.Log(collider.name);
            return item;
        }

        public IItemMoving GetItemMoving(Vector3 point)
        {
            IItemMoving itemCheck = null;
            IItemMoving item = null;

            RaycastHit2D[] hits = Physics2D.RaycastAll(point, Vector2.zero);
            //Collider2D collider = null;

            for (int i = 0; i < hits.Length; i++)
            {
                itemCheck = hits[i].collider.GetComponent<IItemMoving>();
                if (itemCheck != null && itemCheck != this.itemMoving)
                {
                    if (item == null || item.OrderLayer < itemCheck.OrderLayer)
                    {
                        item = itemCheck;
                        //collider = hits[i].collider;
                    }
                }
            }
            //if (collider != null) Debug.Log(collider.name);
            return item;
        }

        public T IsHaveObject<T>(Vector3 point) where T : Component
        {
            T t;
            RaycastHit2D[] hits = Physics2D.RaycastAll(point, Vector2.zero);

            for (int i = 0; i < hits.Length; i++)
            {
                t = hits[i].collider.GetComponent<T>();
                if (t != null)
                {
                    return t;
                }
            }
            return null;
        }

        public bool IsHave<T>(Vector3 point, ref T t)
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(point, Vector2.zero);

            for (int i = 0; i < hits.Length; i++)
            {
                t = hits[i].collider.GetComponent<T>();
                if (t != null)
                {
                    return true;
                }
            }
            return false; // Return default value if no object is found
        }

        public bool IsHaveObject<T>(Vector3 point, out T t) where T : Component
        {
            t = IsHaveObject<T>(point);
            return t != null;
        }

        public Vector3 GetPoint()
        {
            point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            point.z = 0;
            return point;
        }

        public void SetItemMoving(IItemMoving itemMoving)
        {
            this.itemMoving = itemMoving;
            offset = itemMoving.TF.position - GetPoint();
            itemMoving.OnClickDown();
        }

        public void CheckStep()
        {
            if (LevelStep.IsDone())
            {
                LevelStep.OnFinish(NextStep);
            }
        }
        public void CheckStep(float time)
        {
            CancelInvoke(nameof(CheckStep));
            Invoke(nameof(CheckStep), time);
        }

        public void NextStep()
        {
            if (levelIndex >= levelSteps.Length - 1)
            {
                Debug.LogError("DONE");
                WinGame();
            }
            else
            {
                //Debug.LogError("NEXT STEP");
                LevelStepBase level = levelSteps[++levelIndex];
                LevelStep = level.IsHaveRetry ? Instantiate(level) : level;
                LevelStep.OnStart();
                //_hint = levelStep.Hint;
                SetHint(LevelStep.Hint);
            }
            itemMoving = null;
        }

        public void LoseGame(float delay)
        {
            if (!isControl) return;
            SetControl(false);
            Debug.LogError("LOSE GAME");
            LevelStep.OnLose();
            OnRetry(delay);
        }

        public void OnRetry(float delay)
        {
            OnBlackSquare(2f,
            () =>
            {
                Destroy(LevelStep.gameObject);
            },
            () =>
            {
                LevelStep = Instantiate(levelSteps[levelIndex]);
                LevelStep.OnStart();
                SetHint(LevelStep.Hint);
                SetControl(true);
            }, 1.5f, delay);
        }

        public void OnBlackSquare(float time, Action onBlack = null, Action onComplete = null, float timeInBlack = 0.5f, float delay = 0)
        {
            blackSquare.color = Color.clear;
            blackSquare.gameObject.SetActive(true);
            blackSquare.DOFade(1, time * 0.5f).OnComplete(() =>
            {
                onBlack?.Invoke();
            }).SetDelay(delay);

            blackSquare.DOFade(0, time * 0.5f).SetDelay(delay + timeInBlack).OnComplete(() =>
            {
                blackSquare.gameObject.SetActive(false);
                onComplete?.Invoke();
            });
        }

        public void AddTrash(IItemMoving item)
        {
            LevelStep.AddTrash(item);
        }

        public void RemoveTrash(IItemMoving item)
        {
            LevelStep.RemoveTrash(item);
        }

        public bool IsStep(Step step)
        {
            return this.step == step;
        }

        public int GetHighestLayer(IItemMoving order, Vector3 point)
        {
            //order cao nhat
            int layer = 0;
            RaycastHit2D[] hits = Physics2D.RaycastAll(point, Vector2.zero);
            for (int i = 0; i < hits.Length; i++)
            {
                checkItem = hits[i].collider.GetComponent<IItemBase>();
                if (checkItem != null && checkItem != order && checkItem.OrderLayer > layer)
                {
                    layer = checkItem.OrderLayer;
                }
            }
            //Debug.Log(hits.Length +" - "+ layer);
            return layer;
        }

        public int GetHighestNoneContactLayer(IItemMoving order, Vector3 point, int layer = 0)
        {
            //order cao nhat cua nhung thang k the tuong tac
            RaycastHit2D[] hits = Physics2D.RaycastAll(point, Vector2.zero);
            for (int i = 0; i < hits.Length; i++)
            {
                checkItem = hits[i].collider.GetComponent<ItemMovingBase>();
                //if(checkItem != null) Debug.Log($"{hits[i].collider.name} {checkItem != order} && {!(checkItem as ItemMovingBase).IsCanMove} && {checkItem.OrderLayer > layer}");
                if (checkItem != null && checkItem != order && !(checkItem as ItemMovingBase).IsCanMove && checkItem.OrderLayer > layer)
                {
                    //Debug.LogError(hits[i].collider.name);
                    layer = checkItem.OrderLayer;
                }
            }
            //Debug.Log(hits.Length + " - " + layer);
            return layer;
        }

        IItemBase checkItem = null;
        Collider2D[] colliders = new Collider2D[20];
        public bool IsHaveObjectOther(IItemMoving itemMoving, Vector3 point, float radius)
        {
            int amount = Physics2D.OverlapCircleNonAlloc(point, radius, colliders);
            //Debug.Log($"{amount} - {point} - {radius}");
            for (int i = 0; i < amount; i++)
            {
                checkItem = colliders[i].GetComponent<IItemMoving>();
                //Debug.Log($"{colliders[i].name}");
                if (checkItem != null && checkItem != itemMoving)
                {
                    Debug.LogError($"{colliders[i].name} - {point}");
                    return true;
                }
            }
            return false;
        }
        public bool IsHaveObjectOther<T>(IItemMoving itemMoving, Vector3 point, float radius) where T : IItemMoving
        {
            int amount = Physics2D.OverlapCircleNonAlloc(point, radius, colliders);
            //Debug.Log($"{amount} - {point} - {radius}");
            for (int i = 0; i < amount; i++)
            {
                checkItem = colliders[i].GetComponent<T>();
                //Debug.Log($"{colliders[i].name}");
                if (checkItem != null && checkItem != itemMoving)
                {
                    //Debug.LogError($"{colliders[i].name} - {point}");
                    return true;
                }
            }
            return false;
        }

        public bool IsHaveObjectOther(IItemMoving itemMoving, Vector3 point, Vector2 size, float angle)
        {
            int amount = Physics2D.OverlapBoxNonAlloc(point, size, angle, colliders);
            //Debug.Log($"{amount} - {point} - {size}");
            for (int i = 0; i < amount; i++)
            {
                checkItem = colliders[i].GetComponent<IItemMoving>();
                //Debug.Log($"{colliders[i].name}");
                if (checkItem != null && checkItem != itemMoving)
                {
                    //Debug.LogError($"{colliders[i].name}");
                    return true;
                }
            }
            return false;
        }

        public void BlockControl(float time)
        {
            CancelInvoke(nameof(ActiveControl));
            isControl = false;
            Invoke(nameof(ActiveControl), time);
            //Debug.Log("Block Control " + Time.time + " - " + time);
        }

        private void ActiveControl()
        {
            itemMoving = null;
            isControl = true;
            //Debug.Log("ActiveControl " + Time.time);
        }
        public void SetControl(bool isControl)
        {
            itemMoving = null;
            this.isControl = isControl;
        }

        public void SetHint(Sprite sprite)
        {
            _hint = sprite;
            if (isHaveUI) testUI.SetHint(sprite);
        }

        public void NextHint()
        {
            LevelStep.NextHint();
        }

        public void SetStep<T>(LevelName level, T step) where T : Enum
        {
            this.SetStep((int)(object)step);
        }

        public void SetHintTextDone(int phaseID, int stepID)
        {
            Debug.Log($"SetHintTextDone: {phaseID}, {stepID}");
            // SetDonePhaseAndStep(phaseID, stepID);
            if (isHaveUI) testUI.OnDoneHintText(phaseID, stepID);
        }

        [Button]
        private void SearchStep()
        {
            List<LevelStepBase> levelStepsList = FindObjectsOfType<LevelStepBase>(true).ToList();
            levelStepsList.Sort((x, y) => x.name.CompareTo(y.name));
            levelSteps = levelStepsList.ToArray();
            for (int i = 0; i < levelStepsList.Count; i++)
            {
                levelStepsList[i].gameObject.SetActive(false);
            }
            Debug.Log(levelStepsList.Count);
        }

        [Button]
        private void PassStep()
        {
            LevelStep.OnFinish(NextStep);
        }

        public void StopTime()
        {
            Debug.LogError("Stop Time");
            // StopTimeCounterModifiers.AddModifier(this);
        }


    }
    
    [System.Serializable]
    public class HintText
    {
        [SerializeField] Vector2Int hintID = new Vector2Int(-1, -1);

        public void OnActiveHint()
        {
            if (hintID.x > 0) LevelControl.Ins.SetHintTextDone(hintID.x, hintID.y);
        }
    }

}
