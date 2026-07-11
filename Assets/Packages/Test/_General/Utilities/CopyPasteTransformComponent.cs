using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

#if UNITY_EDITOR

public static class CopyPasteTransformComponent
{
    struct TransformData
    {
        public Vector3 localPosition;
        public Quaternion localRotation;
        public Vector3 localScale;

        public TransformData(Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
        {
            this.localPosition = localPosition;
            this.localRotation = localRotation;
            this.localScale = localScale;
        }
    }

    private static TransformData _data;
    private static Vector3? _dataCenter;

    private static List<TransformData> transformDatas = new List<TransformData>();

    [MenuItem("Auto/Transform/Copy Transform Values &c", false, -101)]
    public static void CopyTransformValues()
    {
        if (Selection.gameObjects.Length == 0) return;
        //var selectionTr = Selection.gameObjects[0].transform;
        //_data = new TransformData(selectionTr.localPosition, selectionTr.localRotation, selectionTr.localScale);

        transformDatas.Clear();

        GameObject[] gameObjects = Selection.gameObjects;

        //sap xem lai cho dung thu tu ten
        //selection se k biet thu tu thang nao

        //sort lai thu tu
        //so sanh tung ky tu nen k check dc nhung thang co so > 10
        for (int i = 0; i < gameObjects.Length; i++)
        {
            for (int j = i + 1; j < gameObjects.Length; j++)
            {
                string x = gameObjects[i].name;
                string y = gameObjects[j].name;

                for (int t = 0; t < x.Length; t++)
                {
                    if (t >= y.Length)
                    {
                        //swap
                        GameObject go = gameObjects[j];
                        gameObjects[j] = gameObjects[i];
                        gameObjects[i] = go;
                        break;
                    }

                    if (x[t].CompareTo(y[t]) > 0)
                    {
                        //swap
                        GameObject go = gameObjects[j];
                        gameObjects[j] = gameObjects[i];
                        gameObjects[i] = go;

                        break;
                    }
                    else
                    if (x[t].CompareTo(y[t]) < 0)
                    {
                        break;
                    }
                }
            }
        }

        //save data transform
        for (int i = 0; i < gameObjects.Length; i++)
        {
            Transform tf = gameObjects[i].transform;
            TransformData data = new TransformData(tf.localPosition, tf.localRotation, tf.localScale);
            transformDatas.Add(data);
        }
    }

    [MenuItem("Auto/Transform/Paste Transform Values &v", false, -101)]
    public static void PasteTransformValues()
    {
        //foreach (var selection in Selection.gameObjects)
        //{
        //    Transform selectionTr = selection.transform;
        //    Undo.RecordObject(selectionTr, "Paste Transform Values");
        //    selectionTr.localPosition = _data.localPosition;
        //    selectionTr.localRotation = _data.localRotation;
        //    selectionTr.localScale = _data.localScale;
        //}

        if (Selection.gameObjects.Length == 0) return;

        GameObject[] gameObjects = Selection.gameObjects;

        //sap xem lai cho dung thu tu ten
        //selection se k biet thu tu thang nao

        //sort lai thu tu
        //so sanh tung ky tu nen k check dc nhung thang co so > 10
        for (int i = 0; i < gameObjects.Length; i++)
        {
            for (int j = i + 1; j < gameObjects.Length; j++)
            {
                string x = gameObjects[i].name;
                string y = gameObjects[j].name;

                for (int t = 0; t < x.Length; t++)
                {
                    if (t >= y.Length)
                    {
                        //swap
                        GameObject go = gameObjects[j];
                        gameObjects[j] = gameObjects[i];
                        gameObjects[i] = go;
                        break;
                    }

                    if (x[t].CompareTo(y[t]) > 0)
                    {
                        //swap
                        GameObject go = gameObjects[j];
                        gameObjects[j] = gameObjects[i];
                        gameObjects[i] = go;

                        break;
                    }
                    else
                    if (x[t].CompareTo(y[t]) < 0)
                    {
                        break;
                    }
                }
            }
        }


        int amount = Mathf.Min(transformDatas.Count, gameObjects.Length);

        for (int i = 0; i < amount; i++)
        {
            Transform selectionTr = gameObjects[i].transform;
            selectionTr.localPosition = transformDatas[i].localPosition;
            selectionTr.localRotation = transformDatas[i].localRotation;
            selectionTr.localScale = transformDatas[i].localScale;
            Undo.RecordObject(selectionTr.gameObject, "Paste Transform Values");
        }

        //neu la prefab thi save prefab lai
        {
            //tim prefab thang root tren cung
            GameObject go = PrefabUtility.GetOutermostPrefabInstanceRoot(Selection.gameObjects[0]);
            if (go != null)
            {
                Undo.RegisterCompleteObjectUndo(go, "Save prefab");
            }
        }
    }

    [MenuItem("Auto/Transform/Invert Position X &t", false, -101)]
    public static void CopyCenterPosition()
    {
        if (Selection.gameObjects.Length == 0) return;
        foreach (var item in Selection.gameObjects)
        {
            Undo.RecordObject(item, "Invert Position X");
        }
        int invert = Selection.gameObjects[0].transform.position.x > 0 ? -1 : 1;
        foreach (var item in Selection.gameObjects)
        {
            Vector3 position = item.transform.position;
            position.x = invert * Mathf.Abs(position.x);
            item.transform.position = position;
        }
    }

    [MenuItem("Auto/Transform/Invert Position Y &y", false, -101)]
    public static void InvertPositionY()
    {
        if (Selection.gameObjects.Length == 0) return;
        foreach (var item in Selection.gameObjects)
        {
            Undo.RecordObject(item, "Invert Position X");
        }
        int invert = Selection.gameObjects[0].transform.position.y > 0 ? -1 : 1;
        foreach (var item in Selection.gameObjects)
        {
            Vector3 position = item.transform.position;
            position.y = invert * Mathf.Abs(position.y);
            item.transform.position = position;
        }
    }

    [MenuItem("Auto/Transform/Rotate Y &r", false, -101)]
    public static void RotateY()
    {
        if (Selection.gameObjects.Length == 0) return;
        foreach (var item in Selection.gameObjects)
        {
            Undo.RecordObject(item, "Rotate Y");
        }
        foreach (var item in Selection.gameObjects)
        {
            Vector3 angle = item.transform.eulerAngles;
            angle.y += 180;
            item.transform.eulerAngles = angle;
        }
    }

    //&%# alt ctrl shift
    [MenuItem("Auto/Transform/Rotate Random &%r", false, -101)]
    public static void RotateRandomY()
    {
        if (Selection.gameObjects.Length == 0) return;
        foreach (var item in Selection.gameObjects)
        {
            Undo.RecordObject(item, "Rotate Random");
        }
        foreach (var item in Selection.gameObjects)
        {
            Vector3 angle = item.transform.eulerAngles;
            angle.z += Random.Range(0, 360f);
            item.transform.eulerAngles = angle;
        }
    }

    //&%# alt ctrl shift
    [MenuItem("Auto/Transform/Change Parent &e", false, -101)]
    public static void ResetParentToOne()
    {
        Dictionary<Transform, Transform> parentMap = new Dictionary<Transform, Transform>();

        if (Selection.gameObjects.Length == 0) return;
        foreach (var item in Selection.gameObjects)
        {
            Undo.RecordObject(item, "Change Parent");
        }
        foreach (var item in Selection.gameObjects)
        {
            Transform[] children = item.GetComponentsInChildren<Transform>(true);
            foreach (var child in children)
            {
                parentMap.Add(child, child.parent);
                if (child == item.transform) continue;
                child.SetParent(null);
            }
            item.transform.localScale = Vector3.one;
            foreach(var child in parentMap.Keys)
            {
                if (child == item.transform) continue;
                child.SetParent(parentMap[child]);
            }
        }
    } 
    
       //&%# alt ctrl shift
    [MenuItem("Auto/Transform/Add Square &s", false, -101)]
    public static void AddSquare()
    {
        if (Selection.gameObjects.Length == 0) return;
        foreach (var item in Selection.gameObjects)
        {
            Undo.RecordObject(item, "AddSquare");
        }
        foreach (var item in Selection.gameObjects)
        {
            if(item.transform.childCount == 0 || item.transform.GetChild(0).name != "Square")
            {
                GameObject square = new GameObject("Square");

                GameObject go = new GameObject(item.name);
                go.transform.localRotation = item.transform.localRotation;
                go.transform.localScale = item.transform.localScale;
                go.AddComponent<SpriteRenderer>().sprite = item.GetComponent<SpriteRenderer>().sprite;

                GameObject.DestroyImmediate(item.GetComponent<SpriteRenderer>());

                item.transform.localRotation = Quaternion.identity;
                item.transform.localScale = Vector3.one;

                square.transform.SetParent(item.transform);
                square.transform.localScale = Vector3.one;
                go.transform.SetParent(square.transform);

                square.transform.localPosition = Vector3.zero;
                go.transform.localPosition = Vector3.zero;
            }
        }
    }
}

#endif
