using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Link.Cooking
{
    public class TestUI : MonoBehaviour
    {
        [Header("Hint Text")]
        [SerializeField] List<HintTextData> hintTextDatas = new List<HintTextData>();
        [SerializeField] GGSheetReader sheetReader;
        [SerializeField] Transform hintTextContainer;
        private bool IsNotHintText => hintTextDatas == null || hintTextDatas.Count <= 0;

        [SerializeField] HintText_Title hintTextTitlePrefab, hintTextSubTitlePrefab;
        [SerializeField] HintText_ItemLine hintTextItemLinePrefab;
        [SerializeField] GameObject dividerPrefab;

        [SerializeField] Animation tickAnim;

        [Header("Hint Image")]
        private Sprite[] hints;
        [SerializeField] private Image hintImg;
        [SerializeField] private Image[] heartImgs;
        [SerializeField] GameObject hintPanel, lockPanel, adsPanel, previousButton, nextButton;
        [SerializeField] TestUI_Lock lockPrefab;
        TestUI_Lock lockSelecting;
        List<TestUI_Lock> locks = new List<TestUI_Lock>();

        private void Start()
        {
            ReadHintText();
        }

        public void OnInit(Sprite[] hints)
        {
            this.hints = hints;
            locks.Clear();
            for (int i = 0; i < hints.Length; i++)
            {
                var lockInstance = Instantiate(lockPrefab, lockPanel.transform);
                lockInstance.gameObject.SetActive(true);
                lockInstance.OnLock();
                locks.Add(lockInstance);
            }
            previousButton.transform.SetSiblingIndex(0);
            nextButton.transform.SetSiblingIndex(1000);
            SelectLock(0);
        }

        public void SetHint(Sprite hint)
        {
            for (int i = 0; i < hints.Length; i++)
            {
                if (hints[i] == hint)
                {
                    // Assuming you have a method to set the hint in your UI
                    // SetHintInUI(hints[i]);
                    // Debug.Log($"Hint set: {hints[i].name}");
                    SelectLock(i);
                    return;
                }
            }
            Debug.LogError("Hint not found in the provided array.");
        }

        public void AdsHintButton()
        {
            lockSelecting.OnUnlock();
            CheckHint(lockSelecting);
        }

        private void CheckHint(TestUI_Lock select)
        {
            if (select == null)
            {
                return;
            }

            if (select.IsLock)
            {
                adsPanel.SetActive(true);
                hintImg.gameObject.SetActive(false);
                return;
            }

            int i = locks.IndexOf(select);
            adsPanel.SetActive(false);
            hintImg.gameObject.SetActive(true);
            hintImg.sprite = hints[i];
        }

        public void OpenHintButton()
        {
            hintPanel.SetActive(true);
            Time.timeScale = 0;
        }

        public void CloseHintButton()
        {
            hintPanel.SetActive(false);
            Time.timeScale = 1;
        }

        public void NextHintButton()
        {
            // Assuming you have a method to get the next hint in your UI
            // GetNextHintInUI();
            // Debug.Log("Next hint requested.");
            int i = locks.IndexOf(lockSelecting) + 1;
            SelectLock(Mathf.Clamp(i, 0, locks.Count - 1));

        }

        public void PreviousHintButton()
        {
            // Debug.Log("Previous hint requested.");
            int i = locks.IndexOf(lockSelecting) - 1;
            SelectLock(Mathf.Clamp(i, 0, locks.Count - 1));
        }

        internal void SetHeart(int heartLevel)
        {
            if (heartLevel <= 0)
            {
                Debug.LogError("Lose all hearts!!!");
            }
            for (int i = 0; i < heartImgs.Length; i++)
            {
                heartImgs[i].fillAmount = heartLevel >= 2 ? 1 : heartLevel >= 1 ? 0.5f : 0;
                heartLevel -= 2;
            }
        }

        private void SelectLock(int index)
        {
            if (locks.Count <= 0) return;
            if (lockSelecting != null)
            {
                lockSelecting.Unselect();
            }
            lockSelecting = locks[index];
            lockSelecting.Select();
            CheckHint(lockSelecting);
        }

        #region Hint Text

        int maxPhase = 0;
        private void ReadHintText()
        {
            sheetReader.Reader((data) =>
            {
                if (data == null || data.Count <= 0) return;

                for (int i = 0; i < data.Count; i++)
                {
                    HintTextData hintTextData = new HintTextData();
                    hintTextData.phaseID = int.Parse(data[i][0]);
                    hintTextData.indexID = data[i][1] == "" ? 0 : int.Parse(data[i][1]);
                    hintTextData.hintText = data[i][2];
                    hintTextDatas.Add(hintTextData);

                    maxPhase = Mathf.Max(maxPhase, hintTextData.phaseID);
                }

                //TODO: test
                OnInitHintText();
            });
        }
        Dictionary<int, HintText_ItemLine> hintTextLines = new Dictionary<int, HintText_ItemLine>();
        private void OnInitHintText()
        {
            if (IsNotHintText) return;

            hintTextLines.Clear();

            for (int i = 0; i < hintTextDatas.Count; i++)
            {
                if (hintTextDatas[i].indexID == 0)
                {
                    Instantiate(dividerPrefab, hintTextContainer);

                    HintText_Title subTitle = Instantiate(hintTextSubTitlePrefab, hintTextContainer);
                    subTitle.OnInit($"Phase {hintTextDatas[i].phaseID}/{maxPhase}");

                    HintText_Title title = Instantiate(hintTextTitlePrefab, hintTextContainer);
                    title.OnInit($"{hintTextDatas[i].hintText}");
                    continue;
                }
                else
                {
                    int id = hintTextDatas[i].phaseID * 1000 + hintTextDatas[i].indexID;

                    HintText_ItemLine itemLine = Instantiate(hintTextItemLinePrefab, hintTextContainer);
                    itemLine.OnInit(hintTextDatas[i].hintText);

                    hintTextLines.Add(id, itemLine);
                }
            }
        }

        [Button]
        public void OnDoneHintText(int phaseID, int indexID)
        {
            if (IsNotHintText) return;
            int id = phaseID * 1000 + indexID;
            if (hintTextLines.ContainsKey(id))
            {
                hintTextLines[id].OnDone();

                //tick done
                tickAnim.gameObject.SetActive(true);
                tickAnim.Play();
            }
            else
            {
                Debug.LogError($"Not found hint text line: {phaseID}, {indexID}");
            }
        }


        [System.Serializable]
        public class HintTextData
        {
            public int phaseID, indexID;
            public string hintText;
        }
        #endregion


    }
}
