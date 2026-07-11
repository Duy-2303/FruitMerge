using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Link.Cooking
{
    public class HintText_Title : MonoBehaviour
    {
        [SerializeField] Text hintTxt;

        public void OnInit(string text)
        {
            this.hintTxt.text = text;
        }
    }
}