using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Text;
using System.IO;
using System.IO.Compression;
using UnityEngine.Networking;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;

#endif

public class MGUtils : MonoBehaviour
{
    private static MGUtils _instance;
    /// <summary>
    /// Supposed to be destroyed on load, to host coroutine separately each scene.
    /// </summary>
    public static MGUtils instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject().AddComponent<MGUtils>();
                instance.name = "[MGUtils]";
            }
            return _instance;
        }
    }

#if UNITY_EDITOR
    /// <summary>
    /// Retrieves selected folder on Project view.
    /// </summary>
    /// <returns></returns>
    public static string GetSelectedPathOrFallback()
    {
        // SOURCE: https://answers.unity.com/questions/472808/how-to-get-the-current-selected-folder-of-project.html
        string path = "Assets";

        foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
        {
            path = AssetDatabase.GetAssetPath(obj);
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                path = Path.GetDirectoryName(path);
                break;
            }
        }
        return path;
    }
#endif

    public static Quaternion GetRotationFromDirection2D(Vector2 targetDirection)
    {
        // Convert the Vector2 to a Vector3, with Y component as 0
        Vector3 targetDirection3D = new Vector3(targetDirection.x, 0, targetDirection.y);

        // Calculate the angle between the forward vector and the target direction
        float angle = Mathf.Atan2(targetDirection3D.x, targetDirection3D.z) * Mathf.Rad2Deg;

        // Apply the rotation around the Y axis
        return Quaternion.Euler(0, angle, 0);
    }

    public static Vector2[] GetRandomPointsInGrid(int numPoints, int numCols, int numRows, float gridWidth = 7.2f, float gridHeight = 12.8f)
    {
        int[] indexes = GetRandomIndexes(numCols * numRows, numPoints);
        Vector2[] points = new Vector2[numPoints];
        int index, row, col;
        float gridHalfWidth = gridWidth / 2;
        float gridHalfHeight = gridHeight / 2;
        float cellWidth = gridWidth / numCols;
        float cellHalfWidth = cellWidth / 2;
        float cellHeight = gridHeight / numRows;
        float cellHalfHeight = cellHeight / 2;
        for (int i = 0; i < indexes.Length; i++)
        {
            index = indexes[i];
            row = index / numCols;
            col = index - (row * numCols);
            points[i] = new Vector2(col * cellWidth + cellHalfWidth - gridHalfWidth, row * cellHeight + cellHalfHeight - gridHalfHeight);
        }
        return points;
    }


    public static void RandomInCircleMinDistance(Vector2 circleCenter, float radius, Vector2 distanceCenter, float minDistance, System.Action<bool, Vector2> callback, float timeout = 1, int maxLoopsPerFrame = 5)
    {
        MGCoroutineHost.StartCor(CorRandomInCircleMinDistance(circleCenter, radius, distanceCenter, minDistance, callback, timeout, maxLoopsPerFrame));
    }

    static IEnumerator CorRandomInCircleMinDistance(Vector2 circleCenter, float radius, Vector2 distanceCenter, float minDistance, System.Action<bool, Vector2> callback, float timeout = 1, int maxLoopsPerFrame = 5)
    {
        Vector2 point;
        float minDistanceSqr = minDistance * minDistance;
        float startTime = Time.realtimeSinceStartup; 
        int tries = 0;
        timeout = Time.realtimeSinceStartup + timeout;
        int i = 0;
        while (Time.realtimeSinceStartup <= timeout)
        {
            for (i = 0; i < maxLoopsPerFrame; i++)
            {
                point = Random.insideUnitCircle * radius + circleCenter;
                tries++;
                if ((point - distanceCenter).sqrMagnitude >= minDistanceSqr)
                {
                    //Debug.Log("CorRandomInCircleMinDistance got result after " + (Time.realtimeSinceStartup - startTime).ToString("F2") + "s, " + tries + " tries.");
                    callback.Invoke(true, point);
                    yield break;
                }
            }
            yield return null;
        }
        callback.Invoke(false, Vector2.zero);
    }

    public static int[] GetArrayFromRange(int from, int to)
    {
        return Enumerable.Range(from, to).ToArray();
    }

    /// <summary>
    /// Example:<br/>
    /// int[] stageRanges = { 2, 3, 4 }; // can be converted to: { 0, 1 }, { 2, 3, 4 }, { 5, 6, 7, 8 }<br/>
    /// waveIndex = 4;<br/>
    /// -> stageIndex = 1; // stageIndex in "stageRanges", because waveIndex would belong to the range from 2 to 4
    /// </summary>
    public static int GetIndexInLengths(int[] lengths, int target)
    {
        int total = 0;
        for (int i = 0; i < lengths.Length; i++)
        {
            total += lengths[i];
            if (target < total)
            {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// Example:<br/>
    /// int[] pointsToNextLevel = { 1, 2, 6 }; // -> total amount of exp for each level: 1, 3, 9<br/>
    /// int currentPoints = 7; // -> level = 2, extra = 7 - 3 = 4, extra progress = 4 / 6 = 0.66<br/>
    /// -> returns progress = 2.66;
    /// </summary>
    public static float GetProgressInRanges(int[] ranges, int target)
    {
        int total = 0;
        for (int i = 0; i < ranges.Length; i++)
        {
            total += ranges[i];
            if (target <= total)
            {
                float extra = (float)(target - (total - ranges[i])) / ranges[i];
                if (extra == 1) extra = 0;
                return i + extra;
            }
        }
        return -1;
    }

    /// <summary>
    /// Example:<br/>
    /// int[] totalPoints = { 0, 1, 1, 3, 9 };<br/>
    /// int[] totalPoints = { 1, 3, 9 };<br/>
    /// int currentPoints = 7; // -> level = 2, extra = 7 - 3 = 4, extra progress = 4 / 6 = 0.66<br/>
    /// -> returns progress = 2.66;
    /// </summary>
    public static float GetProgressInTotals(int[] totals, int target)
    {
        int lastTotal = 0;
        for (int i = 0; i < totals.Length; i++)
        {
            if (target <= totals[i])
            {
                float extra = totals[i] - lastTotal != 0 ? (float)(target - lastTotal) / (totals[i] - lastTotal) : 0;
                if (extra == 1) extra = 0;
                return i + extra;
            }
            lastTotal = totals[i];
        }
        return -1;
    }

    public static Vector2 GetRandomPointOnCircle(Vector2 center, float radius)
    {
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad; // Convert to radians
        float x = center.x + radius * Mathf.Cos(angle);
        float y = center.y + radius * Mathf.Sin(angle);
        return new Vector2(x, y);
    }

    public static void SetInstanceVarByName<T>(T instance, string varName, object varValue)
    {
        FieldInfo field = typeof(T).GetField(varName, BindingFlags.Public | BindingFlags.Instance);
        if (field != null) field.SetValue(instance, System.Convert.ChangeType(varValue, field.FieldType));
    }

    public static IEnumerator DownloadText(string url, System.Action<string> onFinish)
    {
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            if (www.error.Contains("404"))
            {
                Debug.LogWarning("DownloadText 404: " + url);
            }
            else
            {
                Debug.LogError("DownloadText Error: " + www.error);
            }
            onFinish?.Invoke(null);
        }
        else
        {
            // Display the downloaded text
            Debug.Log("DownloadText success: " + www.downloadHandler.text);
            onFinish?.Invoke(www.downloadHandler.text);
        }
    }

    //public static List<boxCollider2D> GetCollidersWithinCircle(Transform transform, float radius, int layerMask, bool prioritizeFront = true, bool inScreen = true, List<boxCollider2D> ignoredTargets = null)
    //{
    //    // Get all colliders within the circle
    //    boxCollider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius, layerMask);

    //    // Convert array to list for easier manipulation
    //    List<boxCollider2D> colliderList = new List<boxCollider2D>(colliders);

    //    if (inScreen || ignoredTargets != null)
    //    {
    //        for (int i = colliderList.Count - 1; i >= 0; i--)
    //        {
    //            if ((inScreen && !GameManager.IsInsideScreen(colliderList[i].transform.position)) || (ignoredTargets != null && ignoredTargets.Contains(colliderList[i])))
    //            {
    //                colliderList.RemoveAt(i);
    //            }
    //        }
    //    }

    //    // Sort the list
    //    if (prioritizeFront) colliderList.Sort((a, b) => CompareByAngle(a, b, transform));

    //    return colliderList;
    //}

    //public static boxCollider2D GetClosestCollider(Vector2 center, float radius, int layerMask, bool inScreen = true, List<boxCollider2D> ignoredTargets = null)
    //{
    //    boxCollider2D[] colliders = Physics2D.OverlapCircleAll(center, radius, layerMask);

    //    boxCollider2D closest = null;
    //    float closestDistanceSqr = radius * radius;  // Initialize with the maximum squared distance

    //    foreach (boxCollider2D collider in colliders)
    //    {
    //        if ((inScreen && !GameManager.IsInsideScreen(collider.transform.position)) || (ignoredTargets != null && ignoredTargets.Contains(collider))) continue;

    //        float distanceSqr = (collider.transform.position - (Vector3)center).sqrMagnitude;

    //        if (distanceSqr < closestDistanceSqr)
    //        {
    //            closest = collider;
    //            closestDistanceSqr = distanceSqr;
    //        }
    //    }

    //    return closest;
    //}

    public static int CompareByAngle(Collider2D a, Collider2D b, Transform reference)
    {
        Vector2 dirToA = a.transform.position - reference.position;
        Vector2 dirToB = b.transform.position - reference.position;

        float angleA = Vector2.SignedAngle(reference.up, dirToA);
        float angleB = Vector2.SignedAngle(reference.up, dirToB);

        // Compare angles
        return angleA.CompareTo(angleB);
    }

    public static int CompareByDistance(MonoBehaviour a, MonoBehaviour b, Transform reference)
    {
        float disToA = (a.transform.position - reference.position).sqrMagnitude;
        float disToB = (b.transform.position - reference.position).sqrMagnitude;

        // Compare distances
        return disToA.CompareTo(disToB);
    }

    public static List<Collider2D> GetCollidersInAngle(Transform reference, float radius, float angle, LayerMask layerMask, List<Collider2D> ignoredTargets = null)
    {
        List<Collider2D> colliders = Physics2D.OverlapCircleAll(reference.position, radius, layerMask).ToList();
        
        if (ignoredTargets != null) colliders.RemoveAll(item => ignoredTargets.Contains(item));

        List<Collider2D> collidersInAngle = new List<Collider2D>();

        foreach (Collider2D collider in colliders)
        {
            Vector2 directionToCollider = collider.transform.position - reference.position;
            float angleToCollider = Vector2.Angle(reference.up, directionToCollider);

            if (angleToCollider <= angle)
            {
                collidersInAngle.Add(collider);
            }
        }

        return collidersInAngle;
    }

    public static void SetWindowTitle(string title)
    {
        // Check if the application is running in a headless build
        if (Application.isBatchMode)
        {
            // Set the console window title
            System.Console.Title = title;
        }
    }

    public static string VarsToString(object script, bool valuesOnly = false)
    {
        System.Type type = script.GetType();

        var fields = type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

        string result = "";
        foreach (var field in fields)
        {
            string fieldName = field.Name;
            object fieldValue = field.GetValue(script);

            if (valuesOnly)
            {
                if (fieldValue is float floatValue)
                {
                    // Limit float values to 2 decimal places
                    result += $"{floatValue.ToString("F2")}, ";
                }
                else if (fieldValue is double doubleValue)
                {
                    // Limit double values to 2 decimal places
                    result += $"{doubleValue.ToString("F2")}, ";
                }
                else
                {
                    result += $"{fieldValue}, ";
                }
            }
            else
            {
                if (fieldValue is float floatValue)
                {
                    // Limit float values to 2 decimal places
                    result += $"{fieldName}: {floatValue.ToString("F2")}, ";
                }
                else if (fieldValue is double doubleValue)
                {
                    // Limit double values to 2 decimal places
                    result += $"{fieldName}: {doubleValue.ToString("F2")}, ";
                }
                else
                {
                    result += $"{fieldName}: {fieldValue}, ";
                }
            }
        }

        // Remove the trailing comma and space
        result = result.TrimEnd(',', ' ');

        return result;
    }


    public static string CompressString(string originalString)
    {
        Debug.Log("Compressing string with length: " + originalString.Length);
        string compressedString = "";
        // Convert the string to a byte array
        byte[] originalBytes = Encoding.UTF8.GetBytes(originalString);

        // Create a memory stream to hold the compressed dataBone
        using (MemoryStream compressedStream = new MemoryStream())
        {
            // Create a GZipStream to compress the dataBone
            using (GZipStream compressionStream = new GZipStream(compressedStream, CompressionMode.Compress))
            {
                // Write the original byte array to the compression stream
                compressionStream.Write(originalBytes, 0, originalBytes.Length);
            }

            // Get the compressed dataBone as a byte array
            byte[] compressedBytes = compressedStream.ToArray();

            // Convert the compressed byte array to a string
            compressedString = System.Convert.ToBase64String(compressedBytes);

            // Now you can send the compressedString over the internet
            Debug.Log("Compressed string length: " + compressedString.Length);
        }

        if (compressedString.Length > originalString.Length)
        {
            Debug.Log("Couldn't compress, returning original string");
            return originalString;
        }
        else return compressedString;
    }

    private static System.Random _rnd = new System.Random();

    public static string NewId_FromRandomLong() => _rnd.Next().ToString("x"); // Source: https://dvoituron.com/2022/04/07/generate-small-unique-identifier/

  
    public static string Base64Encode(string plainText)
    {
        // SOURCE: https://stackoverflow.com/questions/11743160/how-do-i-encode-and-decode-a-base64-string
        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
        return System.Convert.ToBase64String(plainTextBytes);
    }

    public static string Base64Decode(string base64EncodedData)
    {
        // SOURCE: https://stackoverflow.com/questions/11743160/how-do-i-encode-and-decode-a-base64-string
        var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
        return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
    }

    /// <summary>
    /// For general caching (not MGUtils').
    /// </summary>
    private static Transform mTransform;
    private static bool mBool;

    public static bool IsNullOrEmpty<T>(T[] array)
    {
        return array == null || array.Length == 0;
    }

    //private void Awake()
    //{
    //    DontDestroyOnLoad(gameObject);
    //}

    /// <summary>
    /// Plays one particle system multiple times at multiple positions, queued continuously on the next frames.
    /// </summary>
    /// <param name="ps"></param>
    /// <param name="positions"></param>
    public static void PlayMultiFx(ParticleSystem ps, params MGPair_Vector3_AudioClip[] positions)
    //public static void PlayMultiFx(ParticleSystem ps, Vector3 position, AudioClip ac = null)
    {
        instance._PlayMultiFx(ps, positions);
        //instance._PlayMultiFx(ps, position, ac);
    }
    private void _PlayMultiFx(ParticleSystem ps, params MGPair_Vector3_AudioClip[] positions)
    //private void _PlayMultiFx(ParticleSystem ps, Vector3 position, AudioClip ac = null)
    {
        if (ps == null) return;
        mBool = false; // to start a new coroutine or not
        if (!_multiFxDic.ContainsKey(ps))
        {
            _multiFxDic[ps] = new List<MGPair_Vector3_AudioClip>();
            mBool = true;
        }
        _multiFxDic[ps].AddRange(positions);
        //_multiFxDic[ps].Add(new MGPair_Vector3_AudioClip(position, ac));
        if (mBool) StartCoroutine(_PlayMultiFxCor(ps));
    }
    //private Dictionary<ParticleSystem, List<ParticleSystem>> _multiFxCache = new Dictionary<ParticleSystem, List<ParticleSystem>>();
    //private ParticleSystem _GetMultiFxFromCache(ParticleSystem psKey)
    //{
    //    if (!_multiFxCache.ContainsKey(psKey)) _multiFxCache[psKey] = new List<ParticleSystem>();
    //    return _multiFxCache[psKey][0];aa
    //}
    private IEnumerator _PlayMultiFxCor(ParticleSystem ps)
    {
        mTransform = ps.transform;
        while (_multiFxDic[ps].Count > 0)
        {
            //if (ps == null) yield break;
            mTransform.position = _multiFxDic[ps][0].position;
            ps.Play(true);
            if (_multiFxDic[ps][0].audio != null) AudioManager.PlaySFX(_multiFxDic[ps][0].audio);
            _multiFxDic[ps].RemoveAt(0);
            if (_multiFxDic[ps].Count == 0)
            {
                _multiFxDic.Remove(ps);
                yield break;
            }
            yield return 0;
        }


        // TODO: purge nulls once in a while
    }
    private Dictionary<ParticleSystem, List<MGPair_Vector3_AudioClip>> _multiFxDic = new Dictionary<ParticleSystem, List<MGPair_Vector3_AudioClip>>();


    public static float ClampAngle(float angle, float min, float max)
    {
        // SOURCE: https://forum.unity.com/threads/limiting-rotation-with-mathf-clamp.171294/
        if (min < 0 && max > 0 && (angle > max || angle < min))
        {
            angle -= 360;
            if (angle > max || angle < min)
            {
                if (Mathf.Abs(Mathf.DeltaAngle(angle, min)) < Mathf.Abs(Mathf.DeltaAngle(angle, max))) return min;
                else return max;
            }
        }
        else if (min > 0 && (angle > max || angle < min))
        {
            angle += 360;
            if (angle > max || angle < min)
            {
                if (Mathf.Abs(Mathf.DeltaAngle(angle, min)) < Mathf.Abs(Mathf.DeltaAngle(angle, max))) return min;
                else return max;
            }
        }

        if (angle < min) return min;
        else if (angle > max) return max;
        else return angle;
    }

    public static Vector3 GetCenterOfObject(GameObject go)
    {
        Vector3 result = default(Vector3);
        if (go != null)
        {
            // check if there is a SR in root object
            SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                result = sr.bounds.center;
            }
            else
            {
                // check if object "Sprite" exists
                Transform t = go.transform.Find("Sprite");
                if (t != null) sr = t.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    result = sr.bounds.center;
                }
                else
                {
                    // calculate center based on all child objects
                    foreach (var s in go.GetComponentsInChildren<SpriteRenderer>())
                    {
                        if (result == default(Vector3)) result = s.bounds.center;
                        else result = Vector3.Lerp(result, s.bounds.center, 0.5f);
                    }
                }
            }
        }
        if (result == default(Vector3)) result = go.transform.position;
        return result;
    }

    /// <summary>
    /// Calculates absolute signed angle (only positive) between the front vector of an object and the vector from that object to the second object.
    /// </summary>
    /// <param name="point1">Start transform</param>
    /// <param name="point2">End transform</param>
    /// <param name="front">Defaults to -point1.up</param>
    /// <returns></returns>
    public static float GetFrontAngle2D(Transform point1, Transform point2, Vector2 front = default(Vector2))
    {
        return Mathf.Abs(Vector2.SignedAngle((front == default(Vector2) ? (Vector2)(-point1.up) : front), (point2.position - point1.position).normalized));
    }

    /// <summary>
    /// Set a variable (field) of an object by name.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="varName"></param>
    /// <param name="value"></param>
    public static void SetVar(object obj, string varName, object value)
    {
        var fieldInfo = obj.GetType().GetField(varName);
        if (fieldInfo != null) fieldInfo.SetValue(obj, value);
    }

    public static void ActivateOnlyOne(GameObject activeObj, params GameObject[] allObjs)
    {
        foreach (var item in allObjs)
        {
            item.SetActive(activeObj == item);
        }
    }

    public static void KillTween(Tween twn)
    {
        if (twn != null && twn.IsActive()) twn.Kill();
    }
    /// <summary>
    /// Converts a full angle (less than 0 or greater than 360) into a within-360-only-positive angle
    /// </summary>
    /// <param name="fullAngle"></param>
    /// <returns></returns>
    public static float FullAngleTo360(float fullAngle)
    {
        float result = fullAngle;
        if (fullAngle < 0) result = 360 + fullAngle % 360;
        else if (fullAngle >= 360) result = fullAngle % 360;
        return result;
    }

    public static void InvokeOnAchievedFrameRate(MonoBehaviour coroutineHost, int desiredFrameRate, System.Action onAchieved, int continuousFrames, int maxCheckFrames = 200, bool skipFirstFrame = true)
    {
        float desiredDeltaTime = 1f / desiredFrameRate;
        coroutineHost.StartCoroutine(_InvokeAchievedDeltaTime(desiredDeltaTime, onAchieved, continuousFrames, maxCheckFrames));
    }

    public static void InvokeOnAchievedDeltaTime(MonoBehaviour coroutineHost, float desiredDeltaTime, System.Action onAchieved, int continuousFrames, int maxCheckFrames = 200, bool skipFirstFrame = true)
    {
        coroutineHost.StartCoroutine(_InvokeAchievedDeltaTime(desiredDeltaTime, onAchieved, continuousFrames, maxCheckFrames));
    }

    private static IEnumerator _InvokeAchievedDeltaTime(float desiredDeltaTime, System.Action onAchieved, int continuousFames, int maxCheckFrames = 200, bool skipFirstFrame = true)
    {
        // skip first frame
        if (skipFirstFrame)
        {
            yield return new WaitForSeconds(Time.deltaTime / 2f);
        }

        Debug.Log("Start testing for achieving deltaTime of " + desiredDeltaTime.ToString("F5") + " in continuously " + continuousFames + " frames.");
        int achievedFrames = 0;// continuously, resets on failing to achieve
        for (int i = 0; i < maxCheckFrames; i++)
        {
            if (Time.deltaTime <= desiredDeltaTime)
            {
                achievedFrames++;
            }
            else
            {
                achievedFrames = 0;
            }
            if (achievedFrames >= continuousFames)
            {
                // achieved!
                Debug.Log("Achieved deltaTime of " + desiredDeltaTime.ToString("F5") + " in continuously " + continuousFames + " frames, tested in totally " + (i + 1) + " frames.");
                if (onAchieved != null) onAchieved();
                yield break;
            }
            else
            {
                yield return new WaitForEndOfFrame();
            }
        }
        Debug.Log("Failed to achieve deltaTime of " + desiredDeltaTime.ToString("F5") + " in continuously " + continuousFames + " frames, tested in totally " + maxCheckFrames + " frames.");
    }

    /// <summary>
    /// Normalizes any float value to the range of 0 to 1.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static float NormalizeFloat(float value, float min, float max)
    {
        return (value - min) / (max - min);
    }

    public static Transform FindChildEndsWith(Transform t, string str)
    {
        int count = t.childCount;
        for (int i = 0; i < count; i++)
        {
            Transform child = t.GetChild(i);
            if (child.name.EndsWith(str)) return child;
            Transform subChild = FindChildEndsWith(child, str);
            if (subChild != null) return subChild;
        }
        return null;
    }

    public static Transform[] FindChildrenEndWith(Transform t, string str)
    {
        Transform[] allChildren = t.GetComponentsInChildren<Transform>();
        List<Transform> result = new List<Transform>();
        foreach (var item in allChildren)
        {
            if (item.name.EndsWith(str)) result.Add(item);
        }
        return result.ToArray();
    }

    public static Transform[] FindChildrenContain(Transform t, string str)
    {
        Transform[] allChildren = t.GetComponentsInChildren<Transform>();
        List<Transform> result = new List<Transform>();
        foreach (var item in allChildren)
        {
            if (item.name.Contains(str)) result.Add(item);
        }
        return result.ToArray();
    }

    public static Transform[] FindChildrenContainSpriteName(Transform t, string str)
    {
        SpriteRenderer[] allChildren = t.GetComponentsInChildren<SpriteRenderer>(true);
        List<Transform> result = new List<Transform>();
        foreach (var item in allChildren)
        {
            if (item.sprite != null && item.sprite.name.Contains(str)) result.Add(item.transform);
        }
        return result.ToArray();
    }

    public static bool isOnEditor
    {
        get
        {
            return Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor;
        }
    }

    public static float GetRandomPosOrNeg(float absMin, float absMax)
    {
        absMin = Mathf.Abs(absMin);
        absMax = Mathf.Abs(absMax);
        return Random.value > 0.5f ? Random.Range(absMin, absMax) : Random.Range(-absMax, -absMin);
    }

    public static void ReplaceTransformRef(ref Transform currentT, Transform newT)
    {
        newT.SetParent(currentT.parent);
        newT.localScale = currentT.localScale;
        newT.localPosition = currentT.localPosition;
        currentT.gameObject.SetActive(false);
        newT.gameObject.SetActive(true);
        currentT = newT;
    }

    public static Quaternion GetGyro()
    {
        Quaternion q = Input.gyro.attitude;
        return new Quaternion(q.x, q.y, -q.z, -q.w);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="totalLength">Length of the original array/collection</param>
    /// <param name="randomLength">Length of the returning array</param>
    /// <param name="isDistinct"></param>
    /// <returns></returns>
    public static int[] GetRandomIndexes(int totalLength, int randomLength)
    {
        if (randomLength > totalLength)
        {
            randomLength = totalLength;
        }

        int[] result = new int[randomLength];

        List<int> indexes = new List<int>();
        for (int i = 0; i < totalLength; i++)
        {
            indexes.Add(i);
        }

        for (int i = 0; i < result.Length; i++)
        {
            result[i] = indexes[UnityEngine.Random.Range(0, indexes.Count)];
            indexes.Remove(result[i]);
        }

        return result;
    }


    /// Use this method to get all loaded objects of some type, including inactive objects. 
    /// This is an alternative to Resources.FindObjectsOfTypeAll (returns project assets, including prefabs), and GameObject.FindObjectsOfTypeAll (deprecated).
    public static List<T> FindSceneObjectsOfTypeAll<T>()
    {
        // source: http://answers.unity3d.com/questions/158172/findsceneobjectsoftype-ignoring-the-inactive-gameo.html
        List<T> results = new List<T>();
        //for (int i = 0; i < SceneManager.sceneCount; i++)
        //{
        //	var s = SceneManager.GetSceneAt(i);
        Scene s = SceneManager.GetActiveScene();
        //Debug.Log("FIND SCENE OBJECTS: " + typeof(T).Name + " scene:" + s.name + " loaded:" + s.isLoaded);
        if (s.isLoaded)
        {
            var allGameObjects = s.GetRootGameObjects();
            for (int j = 0; j < allGameObjects.Length; j++)
            {
                var go = allGameObjects[j];
                results.AddRange(go.GetComponentsInChildren<T>(true));
            }
        }
        //}
        return results;
    }

    /// <summary>
    /// Get total resist after adding extra resist (extra resist reduces after first base reduction)
    /// </summary>
    /// <param name="baseResist"></param>
    /// <param name="addedResist"></param>
    /// <returns>Total resist</returns>
    public static float AddResist(float baseResist, float addedResist)
    {
        //return 1 - ((1 - baseResist) * (1 - addedResist));
        return baseResist + addedResist; // TODO 20171121
    }

    /// <summary>
    /// Melee, Spell, Armor, Resist: 0 means no change
    /// </summary>
    /// <param name="statId"></param>
    /// <param name="current"></param>
    /// <param name="add"></param>
    /// <returns></returns>
    //public static float AddStat(HeroStatId statId, float current, float add)
    //{
    //	//float result = current;
    //	//if (statId == HeroStatId.Armor || statId == HeroStatId.Resist)
    //	//{
    //	//	result = AddResist(current, add);
    //	//}
    //	//else if (statId == HeroStatId.Melee || statId == HeroStatId.Spell)
    //	//{
    //	//	//result = current == 0 ? result + add : (1 + current) * (1 + add) - 1;
    //	//	result = current + add;
    //	//}
    //	//else
    //	//{
    //	//	result = current + add;
    //	//}
    //	return current + add;
    //}

    // 0 is up, 90 is right, 180 is down, 270 is left
    public static Vector3 PointOnArc(float radius, float angle)
    {
        // Source: http://answers.unity3d.com/questions/838900/programmatically-specifying-a-random-point-in-an-a.html  
        float rad = angle * Mathf.Deg2Rad;
        Vector3 position = new Vector3(Mathf.Sin(rad), Mathf.Cos(rad), 0);
        return position * radius;
    }

    //public static Vector2 GetUnitOnCircle(float angleDegrees, float radius)
    //{
    //	// Source: http://answers.unity3d.com/questions/33193/randomonunitcircle-.html

    //	// initialize calculation variables
    //	float _x = 0;
    //	float _y = 0;
    //	float angleRadians = 0;
    //	Vector2 _returnVector;
    //	// convert degrees to radians
    //	angleRadians = angleDegrees * Mathf.PI / 180.0f;
    //	// get the 2D dimensional coordinates
    //	_x = radius * Mathf.Cos(angleRadians);
    //	_y = radius * Mathf.Sin(angleRadians);
    //	// derive the 2D vector
    //	_returnVector = new Vector2(_x, _y);
    //	// return the vector info
    //	return _returnVector;
    //}

    public static Color HexToRGB(string hexColor)
    {
        // Source: http://wiki.unity3d.com/sockAvailble.php?title=HexConverter

        float red = (HexToInt(hexColor[1]) + HexToInt(hexColor[0]) * 16.000f) / 255;
        float green = (HexToInt(hexColor[3]) + HexToInt(hexColor[2]) * 16.000f) / 255;
        float blue = (HexToInt(hexColor[5]) + HexToInt(hexColor[4]) * 16.000f) / 255;
        Color finalColor = new Color();
        finalColor.r = red;
        finalColor.g = green;
        finalColor.b = blue;
        finalColor.a = 1;
        return finalColor;
    }

    public static T GetEnumValueByIndex<T>(int index)
    {
        return (T)System.Enum.GetValues(typeof(T)).GetValue(index);
    }

    public static int GetEnumIndexByValue(object val)
    {
        return System.Array.IndexOf(System.Enum.GetValues(val.GetType()), val);
    }

    public static void SetBoolByInt(ref bool setBool, int byInt)
    {
        if (byInt >= 0)
        {
            setBool = byInt > 0 ? true : false;
        }
    }

    public static string GetCompactNumber(float rawNumber)
    {
        float num = 0;
        string postFix = "";
        if (rawNumber < 1000)
        {
            num = rawNumber;
        }
        else if (rawNumber < 1000000)
        {
            num = rawNumber / 1000;
            postFix = "K";
        }
        else if (rawNumber < 1000000000)
        {
            num = rawNumber / 1000000;
            postFix = "M";
        }
        else
        {
            num = rawNumber / 1000000000;
            postFix = "B";
        }
        if (postFix.Length > 0)
        {
            //return num.ToString("F" + (4 - Mathf.FloorToInt(num).ToString().Length)) + postFix;
            return num.ToString("F1") + postFix;
        }
        else
        {
            return Mathf.FloorToInt(num).ToString();
        }
    }

    public static EventTrigger AddTriggerListener(GameObject target, EventTriggerType type, UnityEngine.Events.UnityAction callback)
    {
        // Source: http://answers.unity3d.com/questions/854251/how-do-you-add-an-ui-eventtrigger-by-script.html
        EventTrigger et = target.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = type;
        entry.callback.AddListener((eventData) => { callback(); });
        et.triggers.Add(entry);
        return et;
    }

    public static EventTrigger AddTriggerListener<T>(GameObject target, EventTriggerType type, UnityEngine.Events.UnityAction<T> callback, T param)
    {
        // Source: http://answers.unity3d.com/questions/854251/how-do-you-add-an-ui-eventtrigger-by-script.html
        EventTrigger et = target.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = type;
        entry.callback.AddListener((eventData) => { callback(param); });
        et.triggers.Add(entry);
        return et;
    }

    public static float Get2DRotationToward(Vector3 targetPos, Vector3 fromPos)
    {
        float angleRad = Mathf.Atan2(targetPos.y - fromPos.y, targetPos.x - fromPos.x);
        float angle = (180 / Mathf.PI) * angleRad;
        return angle - 90;
    }

    public static int GetUpgradeGold(int level, float basePrice, float rate)
    {
        return Mathf.RoundToInt((basePrice * Mathf.Pow(rate, level - 1)) / 10) * 10;
    }

    public static IEnumerator WaitForRealSeconds(float time)
    {
        // Source: http://answers.unity3d.com/questions/301868/yield-waitforseconds-outside-of-timescale.html
        float start = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup < start + time)
        {
            yield return null;
        }
    }

    public static void UIDrawLine(Vector3 pointA, Vector3 pointB, RectTransform imageRectTransform, float lineWidth, bool repos = true)
    {
        Vector3 differenceVector = pointB - pointA;
        imageRectTransform.sizeDelta = new Vector2(differenceVector.magnitude / imageRectTransform.lossyScale.y, lineWidth);
        imageRectTransform.pivot = new Vector2(0, 0.5f);
        if (repos) imageRectTransform.position = pointA;
        float angle = Mathf.Atan2(differenceVector.y, differenceVector.x) * Mathf.Rad2Deg;
        imageRectTransform.rotation = Quaternion.Euler(0, 0, angle);
    }

    //public static void CenterizeToParent(Transform target)
    //{
    //	Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(target);
    //	//Debug.Log("CenterizeToParent " + target.parent.name + " bounds.center.x " + bounds.center.x) ;
    //	target.localPosition = new Vector3(-bounds.center.x, -bounds.center.ySpeed, target.localPosition.z);
    //}

    //public static void StopTweens(UITweener[] twns, bool disableGO = false)
    //{
    //	for (int i = 0; i < twns.Length; i++)
    //	{
    //		twns[i].enabled = false;
    //		if (disableGO && twns[i].gameObject.activeSelf) twns[i].gameObject.SetActive(false);
    //	}
    //}
    //public static void PlayTweens(UITweener[] twns, bool reset = true, bool forward = true, bool enableGO = true)
    //{
    //	for (int i = 0; i < twns.Length; i++)
    //	{
    //		if (enableGO && !twns[i].gameObject.activeSelf) twns[i].gameObject.SetActive(true);
    //		if (forward)
    //		{
    //			//twns[i].delay = twns[i].delay;
    //			if (reset) twns[i].ResetToBeginning();

    //			twns[i].PlayForward();
    //		}
    //		else twns[i].PlayReverse();
    //	}
    //}

    /// <summary>
    /// Tints a color to another without changing saturation nor brightness.
    /// </summary>
    /// <param name="fromColor"></param>
    /// <param name="toBaseColor"></param>
    /// <returns></returns>
    public static Color TintColor(Color fromColor, Color toBaseColor)
    {
        //HSBColor hsbFrom = HSBColor.FromColor(fromColor);
        //HSBColor hsbTo = HSBColor.FromColor(toBaseColor);
        //hsbFrom.h = hsbTo.h;

        float a = fromColor.a;

        float h, s, v;
        Color.RGBToHSV(toBaseColor, out h, out s, out v);

        float fh, fs, fv;
        Color.RGBToHSV(toBaseColor, out fh, out fs, out fv);

        Color retC = Color.HSVToRGB(h, fs, fv);
        retC.a = a;

        return retC;
        //return hsbFrom.ToColor();
    }

    /// <summary>
    /// Tints a color to another with given percentage.
    /// </summary>
    /// <param name="fromColor"></param>
    /// <param name="toColor"></param>
    /// <param name="percentage"></param>
    /// <returns></returns>
    public static Color TintColorPerc(Color fromColor, Color toColor, float percentage = 1)
    {
        float a = fromColor.a;

        Vector3 from;
        Color.RGBToHSV(fromColor, out from.x, out from.y, out from.z);

        Vector3 to;
        Color.RGBToHSV(toColor, out to.x, out to.y, out to.z);

        Vector3 ret = Vector3.Lerp(from, to, percentage);

        Color retC = Color.HSVToRGB(ret.x, ret.y, ret.z);
        retC.a = a;

        return retC;
    }

    public static float RoundToTwoDecimalPlaces(float val)
    {
        return Mathf.Round(val * 100) / 100;
    }

    public static float RoundToOneDecimalPlaces(float val)
    {
        return Mathf.Round(val * 10) / 10;
    }

    public static char GetHex(int integer)
    {
        // Source: http://wiki.unity3d.com/sockAvailble.php?title=HexConverter
        string alpha = "0123456789ABCDEF";
        try
        {
            return alpha[integer];
        }
        catch (System.Exception)
        {
            return 'F';
        }
    }

    public static int HexToInt(char hexChar)
    {
        // Source: http://wiki.unity3d.com/sockAvailble.php?title=HexConverter
        string hex = hexChar.ToString().ToUpper();
        switch (hex)
        {
            case "0": return 0;
            case "1": return 1;
            case "2": return 2;
            case "3": return 3;
            case "4": return 4;
            case "5": return 5;
            case "6": return 6;
            case "7": return 7;
            case "8": return 8;
            case "9": return 9;
            case "A": return 10;
            case "B": return 11;
            case "C": return 12;
            case "D": return 13;
            case "E": return 14;
            case "F": return 15;
        }
        return -1;
    }

    public static void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = (currentSceneIndex + 1) % SceneManager.sceneCountInBuildSettings;
        SceneManager.LoadScene(nextSceneIndex);
    }

    #region Mathematics

    /// <summary>
    /// Calculate two points that form a perpendicular line to the original vector with double the specified width
    /// </summary>
    /// <param name="startPoint">Original position</param>
    /// <param name="endPoint">Original target</param>
    /// <param name="width">Distance from a perpendicular point to the start point</param>
    /// <returns></returns>
    public static Vector2[] CalcPerpendicularPoints2D(Vector2 startPoint, Vector2 endPoint, float width = 1)
    {
        Vector2 oriVector = endPoint - startPoint;
        Vector2 permVector = Vector3.Cross(oriVector, Vector3.forward);
        permVector.Normalize();
        Vector2 permPoint0 = width * permVector + startPoint;
        Vector2 permPoint1 = -width * permVector + startPoint;
        return new Vector2[] { permPoint0, permPoint1 };
    }


    public static int RollEqualChance(int totalChances)
    {
        //float eachChance = 1f / totalChances;
        //float[] chances = new float[totalChances];
        //for (int i = 0; i < chances.Length; i++)
        //{
        //	chances[i] = eachChance;
        //}
        //return RollChance(chances);
        return UnityEngine.Random.Range(0, totalChances);
    }

    public static int RollChance(params float[] chances)
    {
        //float currentChance = 0;
        float randomVal = Random.Range(0.001f, chances.Sum());

        // Debug.Log("randomVal:" + randomVal + " chances.Sum(): " + chances.Sum());
        //for (int i = 0; i < chances.Length; i++)
        //{
        //    currentChance += chances[i];
        //    if (randomVal < currentChance)
        //    {
        //        return i;
        //    }
        //}
        return FindChance(randomVal, chances);
    }

    public static int FindChance(float val, params float[] chances)
    {
        float currentChance = 0;
        //float randomVal = Random.Range(0.001f, chances.Sum());

        //Debug.Log("randomVal:" + randomVal + " chances.Sum(): " + chances.Sum());
        for (int i = 0; i < chances.Length; i++)
        {
            currentChance += chances[i];
            if (val <= currentChance)
            {
                return i;
            }
        }
        return -1;
    }

    #endregion

}
