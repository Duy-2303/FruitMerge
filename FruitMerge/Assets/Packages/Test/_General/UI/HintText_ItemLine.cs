using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Link.Cooking
{
    public class HintText_ItemLine : MonoBehaviour
    {
        [SerializeField] Text hintTxt;
        string content;

        public void OnInit(string text)
        {
            this.content = text;
            this.hintTxt.text = "<size=40><b>☐</b></size> " + content;
            this.hintTxt.fontStyle = FontStyle.Normal;
            this.hintTxt.color = new Color(0.5f, 0.5f, 0.5f, 1f); // Grey color
        }

        public void OnDone()
        {
            this.hintTxt.text = "<size=40><b>☑</b></size> " + content;
            this.hintTxt.fontStyle = FontStyle.Bold;
            this.hintTxt.color = Color.black;
        }
    }
}