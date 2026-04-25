using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Networking;

public class GGSheetReader : MonoBehaviour
{
    [SerializeField] string url;

    [Button]
    public void Reader(Action<List<string[]>> action)
    {
        if (string.IsNullOrEmpty(url))
        {
            Debug.LogError("URL is not set.");
            return;
        }
        StartCoroutine(IEReader(action));
    }

    IEnumerator IEReader(Action<List<string[]>> action)
    {
        string url = ConvertEditToExportLink(this.url);
        //https://docs.google.com/spreadsheets/d/1x_ti--D8d4lh73t4Crgrlwsq1_m6rG-FoSuD0Aj37uU/edit?gid=1155553860#gid=1155553860
        // string url = "https://docs.google.com/spreadsheets/d/1x_ti--D8d4lh73t4Crgrlwsq1_m6rG-FoSuD0Aj37uU/export?format=csv&gid=1155553860";
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(www.error);
        }
        else
        {
            string text = www.downloadHandler.text;
            Debug.Log(text);

            // Parse
            List<string[]> result = new List<string[]>();
            string[] lines = text.Split('\n');
            foreach (string line in lines)
            {
                string[] cells = line.Split(',');
                result.Add(cells);
            }

            // Debug.Log("Parsed CSV: " + result.Count + " rows");
            // foreach (var row in result)
            // {
            //     Debug.Log(row.Length);
            //     // foreach (var cell in row)
            //     // {
            //     //     Debug.Log(cell);
            //     // }
            // }

            action?.Invoke(result);
            Debug.LogError("CSV parsing completed");
        }
    }

    private string ConvertEditToExportLink(string editLink)
    {
        int editIndex = editLink.LastIndexOf("/");
        if (editIndex == -1)
        {
            return editLink;
        }

        string prefix = editLink.Substring(0, editIndex);

        string gid = "0";
        int gidIndex = editLink.IndexOf("gid=");
        if (gidIndex != -1)
        {
            string gidValue = editLink.Substring(gidIndex + 4);
            int sharpIndex = gidValue.IndexOf('#');
            if (sharpIndex != -1)
            {
                gidValue = gidValue.Substring(0, sharpIndex);
            }
            gid = gidValue;
        }

        string exportLink = $"{prefix}/export?format=csv&gid={gid}";
        return exportLink;
    }
}
