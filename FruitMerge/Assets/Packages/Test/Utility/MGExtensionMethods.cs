using System;
//using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
//using System.Collections.Specialized;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using DG.Tweening;
using System.Globalization;
using System.Runtime.Serialization.Formatters.Binary;
//using CodeStage.AntiCheat.ObscuredTypes;

public static class MGExtensionMethods
{
	/// <summary>
	/// Returns editor-like rotation Z (can be negative)
	/// </summary>
	/// <param name="t"></param>
	/// <returns></returns>
	public static float GetRotationZ(this Transform t)
	{
        float z = t.eulerAngles.z;
        if (z > 180) z -= 360;
        return z;
	}

    public static T[] DeepClone<T>(this T[] originalArray) where T : class
    {
        if (originalArray == null)
        {
            return null;
        }

        T[] clonedArray = new T[originalArray.Length];
        for (int i = 0; i < originalArray.Length; i++)
        {
            clonedArray[i] = originalArray[i] != null ? CloneObject(originalArray[i]) : null;
        }

        return clonedArray;
    }

    private static T CloneObject<T>(T original) where T : class
    {
        if (original == null)
        {
            return null;
        }

        Type type = original.GetType();
        T clone = (T)Activator.CreateInstance(type);
        PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (var property in properties)
        {
            if (property.CanWrite && property.GetIndexParameters().Length == 0)
            {
                try
                {
                    property.SetValue(clone, property.GetValue(original));
                }
                catch
                {
                    // Handle any exceptions that might occur during property copy
                }
            }
        }

        return clone;
    }

    public static void AddAnchoredPosition(this RectTransform @this, float x, float y)
    {
        @this.anchoredPosition += new Vector2(x, y);
    }

    public static void AddAnchoredPositionY(this RectTransform @this, float value)
	{
		@this.anchoredPosition += new Vector2(0, value);
	}

    public static void AddAnchoredPositionX(this RectTransform @this, float value)
    {
        @this.anchoredPosition += new Vector2(value, 0);
    }

    /// <summary>
    /// Converts number of seconds to MM:SS
    /// </summary>
    public static string ToStringMMSS(this int @this)
	{
		int minutes = @this / 60;
		int seconds = @this - minutes * 60;
		return minutes.ToString("00") + ":" + seconds.ToString("00");
	}

    //public static bool HasState(this Animator animator, string stateName)
    //{
    //    if (animator.runtimeAnimatorController is AnimatorController controller)
    //    {
    //        foreach (var layer in controller.layers)
    //        {
    //            foreach (var state in layer.stateMachine.states)
    //            {
    //                if (state.state.name == stateName)
    //                {
    //                    return true;
    //                }
    //            }
    //        }
    //    }

    //    return false;
    //}

    public static bool HasState(this Animator animator, string stateName)
    {
		int stateId = Animator.StringToHash(stateName);
		for (int i = 0; i < animator.layerCount; i++)
		{
			if (animator.HasState(i, stateId))
			{
				return true;
			}
		}
        return false;
    }


    public static Vector2 GetWorldScale2D(this Transform transform)
    {
        Vector2 worldScale = transform.localScale;

        Transform parent = transform.parent;
        while (parent != null)
        {
            worldScale.x *= parent.localScale.x;
            worldScale.y *= parent.localScale.y;
            //worldScale.z *= parent.localScale.z;
            parent = parent.parent;
        }

        return worldScale;
    }

    public static float GetWorldScaleRatio2D(this Transform transform)
    {
		Vector2 wScale = transform.GetWorldScale2D();
        return (wScale.x + wScale.y) * 0.5f;
    }

    public static List<string> Split(this string input, int maxLength)
    {
        List<string> result = new List<string>();

        int startIndex = 0;
        while (startIndex < input.Length)
        {
            int length = Mathf.Min(maxLength, input.Length - startIndex);
            string substring = input.Substring(startIndex, length);
            result.Add(substring);

            startIndex += length;
        }

        return result;
    }

    public static string RemoveColorTags(this string text)
    {
		// SOURCE: Google Bard
        // Create a regular expression to match color tags.
        Regex regex = new Regex(@"<color=.*?>|<\/color>");

        // Replace all color tags with an empty string.
        text = regex.Replace(text, "");

        // Return the text without color tags.
        return text;
    }

    public static IEnumerable<string> SplitInParts(this string s, int partLength)
	{
		// SOURCE: https://stackoverflow.com/questions/4133377/splitting-a-string-number-every-nth-character-number
		if (s == null)
			throw new ArgumentNullException(nameof(s));
		if (partLength <= 0)
			throw new ArgumentException("Part length has to be positive.", nameof(partLength));

		for (var i = 0; i < s.Length; i += partLength)
			yield return s.Substring(i, Math.Min(partLength, s.Length - i));
	}

	public static string InsertEveryNthChar(this string s, string separator, int partLength)
    {
		// SOURCE: https://stackoverflow.com/questions/17215045/best-way-to-remove-the-last-character-from-a-string-built-with-stringbuilder
		return string.Join(separator, s.SplitInParts(partLength));
    }

	public static int GetValueAt(this ulong @this, int index)
    {
		// SOURCE: https://stackoverflow.com/questions/49277543/c-sharp-finding-a-specific-digit-in-an-int
		return (int)(@this / (ulong)Math.Pow(10, index)) % 10;
	}

	public static int GetValueAt(this int @this, int index)
	{
		// SOURCE: https://stackoverflow.com/questions/49277543/c-sharp-finding-a-specific-digit-in-an-int
		return (@this / (int)Math.Pow(10, index)) % 10;
	}

	public static string ReplaceAt(this string @this, int index, char value)
    {
		char[] arr = @this.ToCharArray();
		arr[index] = value;
		return new string(arr);
    }

	/// <summary>
	/// Converts string to integer array. 
	/// Example: "1,3,5,7" -> { 1, 3, 5, 7 }
	/// </summary>
	/// <param name="this"></param>
	/// <param name="separator"></param>
	/// <returns></returns>
	public static int[] ToIntArray(this string @this, char separator = ',')
    {
		string[] splits = @this.Split(separator);
		int[] result = new int[splits.Length];
        for (int i = 0; i < result.Length; i++)
        {
			result[i] = int.Parse(splits[i]);
        }
		return result;
    }

	private static Color _color;

    /// <summary>
    /// Randomize color with min and max brightness.
    /// </summary>
    /// <param name="min">Min brightness</param>
    /// <param name="max">Max brightness</param>
    public static Color Random(this Color @this, float min = 0, float max = 1)
    {
        switch (UnityEngine.Random.Range(0, 3))
        {
            case 0:
                @this.r = UnityEngine.Random.Range(min, max);
                @this.g = UnityEngine.Random.Range(0, max);
                @this.b = UnityEngine.Random.Range(0, max);
                break;
            case 1:
                @this.r = UnityEngine.Random.Range(0, max);
                @this.g = UnityEngine.Random.Range(min, max);
                @this.b = UnityEngine.Random.Range(0, max);
                break;
            case 2:
                @this.r = UnityEngine.Random.Range(0, max);
                @this.g = UnityEngine.Random.Range(0, max);
                @this.b = UnityEngine.Random.Range(min, max);
                break;
        }
        return @this;
    }

	public static SpriteRenderer Clone(this SpriteRenderer sr, Transform intoParent = null)
		//, Transform intoParent = null, bool zeroToParent = false)
    {
		SpriteRenderer newSR = new GameObject().AddComponent<SpriteRenderer>();
		newSR.CopyFrom(sr);
		newSR.transform.SetParent(sr.transform);
		newSR.transform.ZeroToParent();
		if (intoParent != null) newSR.transform.SetParent(intoParent);
		//     if (intoParent != null)
		//     {
		//         newSR.transform.SetParent(intoParent);
		//if (zeroToParent) newSR.transform.ZeroToParent();
		//     }
		return newSR;
    }
    public static void CopyFrom(this SpriteRenderer sr, SpriteRenderer from)
    {
        sr.sprite = from.sprite;
        sr.color = from.color;
        sr.flipX = from.flipX;
        sr.flipY = from.flipY;
        sr.sharedMaterial = from.sharedMaterial;
        sr.drawMode = from.drawMode;
        sr.sortingLayerID = from.sortingLayerID;
        sr.sortingOrder = from.sortingOrder;
        sr.maskInteraction = from.maskInteraction;
    }
	public static void CopyFrom(this Image img, Image from)
	{
		img.sprite = from.sprite;
		img.color = from.color;
		img.rectTransform.sizeDelta = from.rectTransform.sizeDelta;
		img.raycastTarget = from.raycastTarget;
		img.type = from.type;
		img.fillMethod = from.fillMethod;
		img.fillOrigin = from.fillOrigin;
		img.fillAmount = from.fillAmount;
		img.preserveAspect = from.preserveAspect;
	}

	public static T GetCopyOf<T>(this Component comp, T other) where T : Component
    {
        // Source: https://answers.unity.com/questions/530178/how-to-get-a-component-from-an-object-and-add-it-t.html
        Type type = comp.GetType();
        if (type != other.GetType()) return null; // type mis-match
        BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
        PropertyInfo[] pinfos = type.GetProperties(flags);
        foreach (var pinfo in pinfos)
        {
            if (pinfo.CanWrite)
            {
                try
                {
                    pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
                }
                catch { } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
            }
        }
        FieldInfo[] finfos = type.GetFields(flags);
        foreach (var finfo in finfos)
        {
            finfo.SetValue(comp, finfo.GetValue(other));
        }
        return comp as T;
    }


    public static T AddComponent<T>(this GameObject go, T toAdd) where T : Component
    {
        return go.AddComponent<T>().GetCopyOf(toAdd) as T;
    }

    private static Color tmpColor;

    public static Tweener DOWidthMultiplier(this LineRenderer target, float endValue, float duration)
    {
        return DOTween.To(() => target.widthMultiplier, x => target.widthMultiplier = x, endValue, duration);
    }

    public static void SetAlpha(this SpriteRenderer sr, float alpha)
    {
        tmpColor = sr.color;
        tmpColor.a = alpha;
        sr.color = tmpColor;
    }

    public static int IndexOf<T>(this T[] array, T element)
    {
        int result = -1;
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i].Equals(element))
            {
                result = i;
                break;
            }
        }
        return result;
    }

    public static bool RemoveSafely<T>(this List<T> list, T element)
    {
        if (list.Contains(element))
        {
            list.Remove(element);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Get Layer sockAvailble from LayerMask
    /// </summary>
    /// <param name="mask"></param>
    /// <returns></returns>
    public static int ToLayer(this LayerMask mask)
    {
        // Source: https://sites.google.com/site/simonhildebrandt/home/unitytipconvertingalayermasktoalayerindex
        return Mathf.RoundToInt(Mathf.Log(mask, 2));
    }

    /// <summary>
    /// Adds an item to the list if the list does not contain it.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="item"></param>
    /// <returns>True if added.</returns>
    public static bool AddDistinct<T>(this List<T> list, T item)
    {
        if (!list.Contains(item))
        {
            list.Add(item);
            return true;
        }
        return false;
    }

	/// <summary>
	/// Adds an integer sequence (inclusive)
	/// </summary>
	/// <param name="list"></param>
	/// <param name="from"></param>
	/// <param name="to"></param>
	public static void AddSequenceIncl(this List<int> list, int to, int from = 0)
	{
		for (int i = from; i <= to; i++)
		{
			list.Add(i);
		}
	}

	public static int Sign(this float f)
    {
        if (f < 0) return -1;
        if (f == 0) return 0;
        else return 1;
    }

    public static Transform[] GetChildren(this Transform t)
    {
        Transform[] result = null;
        if (t.childCount > 0)
        {
            result = new Transform[t.childCount];
            for (int i = 0; i < t.childCount; i++)
            {
                result[i] = t.GetChild(i);
            }
        }
		if (result == null) result = new Transform[0];
        return result;
    }

    public static void SetLossyScale(this Transform t, Vector3 scale, bool unaffectChildren = false)
    {
        Transform parent = t.parent;
        Transform[] children = null;
        if (unaffectChildren)
        {
            children = t.GetChildren();
            if (children != null)
            {
                foreach (var child in children)
                {
                    child.parent = null;
                }
            }
        }
        t.parent = null;
        t.localScale = scale;
        t.SetParent(parent);
        if (unaffectChildren && children != null)
        {
            foreach (var child in children)
            {
                child.SetParent(t);
            }
        }
    }

	public static void CopyLocalTransform(this Transform from, Transform to)
    {
		to.localPosition = from.localPosition;
		to.localRotation = from.localRotation;
		to.localScale = from.localScale;
    }

    public static void CopyTransform(this Transform from, Transform to, bool setSameParent = true, bool setNextSibling = true)
    {
        if (setSameParent) to.SetParent(from.parent);
        if (setNextSibling) to.SetSiblingIndex(from.GetSiblingIndex() + 1);
        to.position = from.position;
        to.rotation = from.rotation;
        to.localScale = from.localScale;
    }

    //public static void CopyComponentValues(this Component component, Component new_component)
    //{
    //    // Source: https://answers.unity.com/questions/12653/editor-wizard-copy-existing-components-to-another.html
    //    foreach (FieldInfo f in component.GetType().GetFields())
    //    {
    //        f.SetValue(new_component, f.GetValue(component));
    //    }
    //}

    //public static void CopyComponent(this Component component, GameObject to)
    //{
    //    // Source: https://answers.unity.com/questions/12653/editor-wizard-copy-existing-components-to-another.html
    //    Component new_component = to.AddComponent(component.GetType());
    //    component.CopyComponentValues(new_component);
    //    //foreach (FieldInfo f in component.GetType().GetFields())
    //    //{
    //    //    f.SetValue(new_component, f.GetValue(component));
    //    //}
    //}

    public static T GetLastElement<T>(this T[] arr)
    {
        return arr[arr.Length - 1];
    }
    public static T GetRandomElement<T>(this List<T> list)
    {
        return list[UnityEngine.Random.Range(0, list.Count)];
    }

	public static string GetPathToParent(this Transform t, Transform targetParent, int loopLimit = 100)
	{
		string path = t.name;
		int i = 0;
		Transform cT = t.parent;
		while (cT != null && cT != targetParent && i < loopLimit)
		{
			path = cT.name + "/" + path;
			cT = cT.parent;
			i++;
		}
		if (i >= loopLimit)
		{
			return null;
		}
		return path;
	}

	public static bool Contains<T>(this T[] array, T obj)
	{
		bool result = false;
		foreach (T item in array)
		{
			if (item.Equals(obj))
			{
				result = true;
				break;
			}
		}
		return result;
	}

	public static bool HasParameter(this Animator animator, string paramName)
	{
		// source: https://answers.unity.com/questions/571414/is-there-a-way-to-check-if-an-animatorcontroller-h.html
		foreach (AnimatorControllerParameter param in animator.parameters)
		{
			if (param.name == paramName)
				return true;
		}
		return false;
	}

	public static string ToString_yyyyMMddHHmmss(this DateTime @this)
	{
		return @this.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);
	}

	public static string ToString_yyyyMMddHHmmssF(this DateTime @this)
	{
		return @this.ToString("yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture);
	}

	public static string ToStringHHMMSS(this TimeSpan ts, bool ignoreHour = false)
	{
		int h = ts.Days * 24 + ts.Hours;
		return (ignoreHour && h <= 0 ? "" : "<size=50><color=#ffd570>" + (h).ToString("D2") + "</color></size>h   ") + "<size=50><color=#ffd570>" + ts.Minutes.ToString("D2") + "</color></size>m   <size=50><color=#ffd570>" + ts.Seconds.ToString("D2") + "</color></size>s";
	}

	public static string ToStringDDHHMMSS(this TimeSpan ts)
	{
		return (ts.Days > 0 ? ts.Days.ToString() + "d " : "") + ts.Hours.ToString("D2") + ":" + ts.Minutes.ToString("D2") + ":" + ts.Seconds.ToString("D2");
	}
	
	/// <summary>
	/// Example: varName = icon; string = "This is a <icon=my/icon/path>."; return = my/icon/path; string = "This is a .";
	/// </summary>
	/// <param name="this"></param>
	/// <param name="varName"></param>
	/// <returns></returns>
	public static string[] ReplaceRichTextVar(this string @this, string varName)
	{
		// TODO: return sockAvailble too so this can be used for replacing matched var with a new value
		GroupCollection c = Regex.Match(@this, @"<" + varName + @"=([^>]*)>").Groups;
		if (c == null || c.Count == 0) return null;
		string ret = c[1].Value;
		//@this = @this.Replace(c[0].Value, "");
		return new string[] { c[0].Value, c[1].Value };
	}

	private static int[] toRoman_nums = { 1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1 };
	private static string[] toRoman_rum = { "M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX", "V", "IV", "I" };
	public static string ToRoman(this int number)
	{
		// source: https://rosettacode.org/wiki/Roman_numerals/Encode
		string value = "";
		for (int i = 0; i < toRoman_nums.Length && number != 0; i++)
		{
			while (number >= toRoman_nums[i])
			{
				number -= toRoman_nums[i];
				value += toRoman_rum[i];
			}
		}
		return value;
	}

	public static string ReplaceSpecialChars(this string @this, string allowedChars = "0-9a-zA-Z_", string replaceTo = "_")
	{
		// source: http://www.c-sharpcorner.com/blogs/replace-special-characters-from-string-using-regex1
		return Regex.Replace(@this, @"[^" + @allowedChars + @"]+", replaceTo);
	}

	public static void AttachEventTrigger(this GameObject target, Action handler, EventTriggerType type = EventTriggerType.PointerDown)
	{
		// source: https://answers.unity.com/questions/854251/how-do-you-add-an-ui-eventtrigger-by-script.html
		EventTrigger trigger = target.GetOrAddComponent<EventTrigger>();
		var entry = new EventTrigger.Entry();
		entry.eventID = type;
		entry.callback.AddListener((e) => { handler(); });
		trigger.triggers.Add(entry);
	}

	public static void AttachEventTrigger(this GameObject target, Action<string> handler, string param, EventTriggerType type = EventTriggerType.PointerDown)
	{
		// source: https://answers.unity.com/questions/854251/how-do-you-add-an-ui-eventtrigger-by-script.html
		EventTrigger trigger = target.GetOrAddComponent<EventTrigger>();
		var entry = new EventTrigger.Entry();
		entry.eventID = type;
		entry.callback.AddListener((e) => { handler(param); });
		trigger.triggers.Add(entry);
	}

	public static void AddOrSet<TKey, TValue>(this Dictionary<TKey, TValue> @this, TKey key, TValue value)
	{
		if (@this.ContainsKey(key))
		{
			@this[key] = value;
		}
		else
		{
			@this.Add(key, value);
		}
	}

	public static bool HasComponent<T>(this GameObject flag) where T : Component
    {
        return flag.GetComponent<T>() != null;
    }

	public static T GetRandomElement<T>(this T[] @this)
	{
		return @this[UnityEngine.Random.Range(0, @this.Length)];
	}

	public static void Merge<TKey, TValue>(this Dictionary<TKey, TValue> @this, Dictionary<TKey, TValue> dic)
	{
		foreach (var item in dic)
		{
			@this[item.Key] = item.Value;
		}
	}

	/// <summary>
	/// NOTE: Max value is already +1, no need to add +1 to array.
	/// </summary>
	/// <param name="this"></param>
	/// <returns></returns>
	public static int GetRandomBetween(this int[] @this)
	{
		return UnityEngine.Random.Range(@this[0], @this[1] + 1);
	}

	/// <summary>
	/// Get a random value within offset range.
	/// Exampple: offset = 0.1 means +-10%, if this = 100, offset = 0.1, return value will be within 90 to 110, inclusively.
	/// </summary>
	/// <param name="this"></param>
	/// <param name="offset"></param>
	/// <returns></returns>
	public static int ApplyOffset(this int @this, float offset)
	{
		return Mathf.RoundToInt(@this * (UnityEngine.Random.Range(1 - offset, 1 + offset)));
	}

	public static void SetLeftRightTopBottom(this RectTransform @this, float left, float right, float top, float bottom)
	{
		@this.offsetMin = new Vector2(left, bottom);
		@this.offsetMax = new Vector2(-right, -top);
	}

	public static void SetLeft(this RectTransform @this, float val)
	{
		@this.offsetMin = new Vector2(val, @this.offsetMin.y);
	}
	public static void SetRight(this RectTransform @this, float val)
	{
		@this.offsetMax = new Vector2(-val, @this.offsetMin.y);
	}
	public static void SetTop(this RectTransform @this, float val)
	{
		@this.offsetMax = new Vector2(@this.offsetMin.x, -val);
	}
	public static void SetBottom(this RectTransform @this, float val)
	{
		@this.offsetMin = new Vector2(@this.offsetMin.x, val);
	}

	public static float GetLeft(this RectTransform @this)
	{
		return @this.offsetMin.x;
	}
	public static float GetRight(this RectTransform @this)
	{
		return -@this.offsetMax.x;
	}
	public static float GetTop(this RectTransform @this)
	{
		return -@this.offsetMax.y;
	}
	public static float GetBottom(this RectTransform @this)
	{
		return @this.offsetMin.y;
	}

	//	public static T GetVarByName<T> (this object obj, string varName) where T : class {
	//		T ret = obj.GetType().GetProperty(varName).GetValue(obj, new object[1]) as T;
	//		return ret;
	//	}

	public static Dictionary<T1, List<T2>> Clone<T1, T2>(this Dictionary<T1, List<T2>> @this)
	{
		Dictionary<T1, List<T2>> ret = new Dictionary<T1, List<T2>>();
		foreach (KeyValuePair<T1, List<T2>> item in @this)
		{
			ret.Add(item.Key, new List<T2>(item.Value));
		}
		return ret;
	}

	public static void SetTangents(this AnimationCurve animationCurve, int index, float inTangent, float outTangent)
	{
		Keyframe key = animationCurve.keys[index];
		key.inTangent = inTangent;
		key.outTangent = outTangent;
		animationCurve.MoveKey(index, key);
	}

	public static void EaseIn(this AnimationCurve animationCurve)
	{
		animationCurve.SetTangents(0, 2, 2);
		animationCurve.SetTangents(1, 0, 0);
	}

	public static void EaseOut(this AnimationCurve animationCurve)
	{
		animationCurve.SetTangents(0, 0, 0);
		animationCurve.SetTangents(1, 2, 2);
	}

	//public static void PlayFromStart(this UITweener twn, bool resetAfter = false)
	//{
		
	//	if (!resetAfter)
	//	{
	//		twn.ResetToBeginning();
	//	}
	//	twn.PlayForward();
	//	if (resetAfter)
	//	{
	//		twn.ResetToBeginning();
	//	}
	//}

	//// Source: http://www.pvladov.com/2012/09/make-color-lighter-or-darker.html
	//public static Color SetBrightness(this Color color, float correctionFactor)
	//{
	//	float red = color.r;
	//	float green = color.g;
	//	float blue = color.b;

	//	if (correctionFactor < 0)
	//	{
	//		correctionFactor = 1 + correctionFactor;
	//		red *= correctionFactor;
	//		green *= correctionFactor;
	//		blue *= correctionFactor;
	//	}
	//	else
	//	{
	//		red = (255 - red) * correctionFactor + red;
	//		green = (255 - green) * correctionFactor + green;
	//		blue = (255 - blue) * correctionFactor + blue;
	//	}

	//	return new Color(red, green, blue, color.a);
	//}

	public static void SetStartColor(this ParticleSystem ps, Color c, bool withChildren = false)
	{
        ParticleSystem.MainModule main;
		if (withChildren)
		{
			ParticleSystem[] pss = ps.transform.GetComponentsInChildren<ParticleSystem>();
			for (int i = 0; i < pss.Length; i++)
			{
                //pss[i].startColor = c;
                main = pss[i].main;
                main.startColor = c;
			}
		}
		else
		{
			//ps.startColor = c;
            main = ps.main;
            main.startColor = c;
        }
	}

	
	public static void SetStartColorAlpha(this ParticleSystem ps, float alpha)
	{
		_color = ps.main.startColor.color;
		_color.a = alpha;
		ps.SetStartColor(_color);
	}

	public static void TintColor(this ParticleSystem ps, Color c, bool withChildren = false)
	{
		if (withChildren)
		{
			ParticleSystem[] pss = ps.transform.GetComponentsInChildren<ParticleSystem>();
			for (int i = 0; i < pss.Length; i++)
			{
				pss[i].TintColor(c, false);
			}
		}
		else
		{
			ParticleSystem.MainModule main = ps.main;
			main.startColor = MGUtils.TintColor(main.startColor.color, c);
		}
	}

	public static void ZeroToParent(this Transform trans, bool pos = true, bool rot = true, bool scale = true)
	{
		if (pos) trans.localPosition = Vector3.zero;
		if (rot) trans.localRotation = Quaternion.Euler(Vector3.zero);
		if (scale) trans.localScale = Vector3.one;
	}

	public static string ToColorTagged(this string str, string color)
	{
		return "[" + color + "]" + str + "[-]";
	}

	public static string ToHex(this Color color)
	{
		// Source: http://wiki.unity3d.com/sockAvailble.php?title=HexConverter
		float red = color.r * 254f;
		float green = color.g * 254f;
		float blue = color.b * 254f;
 
		string a = MGUtils.GetHex(Mathf.FloorToInt(red / 16f)).ToString();
		string b = MGUtils.GetHex(Mathf.RoundToInt(red % 16f)).ToString();
		string c = MGUtils.GetHex(Mathf.FloorToInt(green / 16f)).ToString();
		string d = MGUtils.GetHex(Mathf.RoundToInt(green % 16f)).ToString();
		string e = MGUtils.GetHex(Mathf.FloorToInt(blue / 16f)).ToString();
		string f = MGUtils.GetHex(Mathf.RoundToInt(blue % 16f)).ToString();
 
		return a + b + c + d + e + f;
	}

	public static Color ToColor(this string color)
	{
		// Source: http://wiki.unity3d.com/sockAvailble.php?title=HexConverter
		float red = (MGUtils.HexToInt(color[1]) + MGUtils.HexToInt(color[0]) * 16f) / 255f;
		float green = (MGUtils.HexToInt(color[3]) + MGUtils.HexToInt(color[2]) * 16f) / 255f;
		float blue = (MGUtils.HexToInt(color[5]) + MGUtils.HexToInt(color[4]) * 16f) / 255f;
		Color finalColor = new Color();
		finalColor.r = red;
		finalColor.g = green;
		finalColor.b = blue;
		finalColor.a = 1;
		//Debug.Log("TOCOLOR:: " + color + " -> " + finalColor.ToString() + " hex: " + finalColor.ToHex());
		return finalColor;
	}

	public static float PlusAbsolute(this float @this, float increment)
	{
		return (Mathf.Abs(@this) + increment) * (@this > 0 ? 1 : -1);
	}

	public static int PlusAbsolute(this int @this, int increment)
	{
		return (Mathf.Abs(@this) + increment) * (@this > 0 ? 1 : -1);
	}

	//public static ToolEnum[] ToToolEnums (this string[] @this) {
	//	ToolEnum[] eTools = new ToolEnum[@this.Length];
	//	for (int i = 0; i < @this.Length; i++) {
	//		if (!string.IsNullOrEmpty(@this[i])) eTools[i] = (ToolEnum)System.Enum.Parse(typeof(ToolEnum), @this[i]);
	//		else eTools[i] = ToolEnum.None;
	//	}
	//	return eTools;
	//}
	//public static SkillEnum[] ToSkillEnums(this string[] @this)
	//{
	//	SkillEnum[] eTools = new SkillEnum[@this.Length];
	//	for (int i = 0; i < @this.Length; i++)
	//	{
	//		if (!string.IsNullOrEmpty(@this[i])) eTools[i] = (SkillEnum)System.Enum.Parse(typeof(SkillEnum), @this[i]);
	//		else eTools[i] = SkillEnum.None;
	//	}
	//	return eTools;
	//}

	public static float GetRandomWithin(this Vector2 @this)
	{
		return UnityEngine.Random.Range(@this.x, @this.y);
	}

	public static float GetRandomWithin(this float[] @this)
	{
		if (@this.Length == 2)
		{
			return UnityEngine.Random.Range(@this[0], @this[1]);
		}
		else return 0;
	}

	public static bool IsInRange (this float @this, float min, float max) {
		if (@this >= min && @this <= max) return true;
		return false;
	}

	public static int GetIndexInRangeArray (this float @this, params float[] array) {
		for (int i = 0; i < array.Length; i++) {
			float prevValue = 0;
			if (i > 0) prevValue = array[i-1];
			float curValue = array[i];
//			Debug.Log("GetIndexInRangeArray " + @this + " prev " + prevValue + " cur " + curValue);
			if (@this.IsInRange(prevValue, curValue)) {
//				Debug.Log("GetIndexInRangeArray " + @this + " sockAvailble " + i);
				return i;
			}
		}
		
		return -1;
	}


	public static void SetSortingOrder (this GameObject go, int sortingOrder) {
		if (go.GetComponent<Renderer>() != null) go.GetComponent<Renderer>().sortingOrder = sortingOrder;
		for (int i = 0; i < go.transform.childCount; i++) {
			go.transform.GetChild(i).gameObject.SetSortingOrder(sortingOrder);
		}
	}

	public static string ReplaceIn(this string @this, string chars, string newValue = "")
	{
		string copy = @this;
		foreach (char c in chars)
		{
			copy = copy.Replace(c.ToString(), newValue);
		}
		return copy;
	}

	public static string ToUpperFirstLetter(this string @this)
	{
//		@this = @this.First().ToString().ToUpper() + String.Join("", @this.Skip(1));
		@this = @this.Substring(0,1).ToUpper() + @this.Substring(1);
		return @this;
	}

	public static string ToUpperFirstLetterWithSpaces(this string @this)
	{
		@this = @this.Substring(0, 1).ToUpper() + @this.Substring(1);
		int i = 0;
		foreach (char c in @this)
		{
			if (i > 0 && char.IsUpper(@this[i]) && @this[i - 1] != ' ')
			{
				@this = @this.Insert(i, " ");
				i++;
			}
			i++;
		}
		return @this;
	}

	public static bool In(this float @this, params float[] values)
	{
		return new List<float>(values).Contains(@this); 
	}

	public static bool In(this int @this, params int[] values)
	{
		return new List<int>(values).Contains(@this); 
	}

	//public static bool In(this ObscuredInt @this, params ObscuredInt[] values)
	//{
	//	return new List<ObscuredInt>(values).Contains(@this);
	//}

	//public static bool In(this ObscuredInt @this, params int[] values)
	//{
	//	return new List<int>(values).Contains(@this);
	//}

	public static bool In(this string @this, params string[] values)
	{
		//// Source: http://stackoverflow.com/questions/1962791/c-sharp-multiple-string-comparision-with-same-value
		//return new List<string>(values).Contains(@this); 

		for (int i = 0; i < values.Length; i++)
		{
			if (@this == values[i])
			{
				return true;
			}
		}

		return false;
	}

	/// <summary>
	/// Example: { "a", "b", "c" }.ToLongString() = "a,b,c"
	/// </summary>
	/// <param name="this"></param>
	public static string ToLongString(this List<string> @this)
	{
		string result = "";
		for (int i = 0; i < @this.Count; i++)
		{
			result += @this[i];
			if (i < @this.Count - 1) result += ",";
		}
		return result;
	}

	// Shuffle List
	public static void Shuffle<T>(this IList<T> list)  
	{  
		// Source: http://stackoverflow.com/questions/273313/randomize-a-listt-in-c-sharp
		System.Random rng = new System.Random();  
		int n = list.Count;  
		while (n > 1) {  
			n--;  
			int k = rng.Next(n + 1);  
			T value = list[k];  
			list[k] = list[n];  
			list[n] = value;  
		}  
	}

	/// <summary>
	/// Instantiate the prefab, set its parent to this GameObject's Transform, then get the specified component from it, automatically attach a new component if needed.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="go"></param>
	/// <param name="itemPrefab"></param>
	/// <param name="resetScale"></param>
	/// <param name="resetPos"></param>
	/// <returns></returns>
	public static T InstantiatePrefab<T> (this GameObject go, GameObject itemPrefab, bool resetScale = true, bool resetPos = true) where T : class
	{
		T ret = default(T);
		GameObject goRet = go.InstantiatePrefab(itemPrefab, resetScale, resetPos);
		if (typeof(T) != typeof(GameObject) && goRet.GetComponent(typeof(T)) == null) goRet.AddComponent(typeof(T));
		ret = goRet.GetComponent(typeof(T)) as T;
		return ret;
	}

	public static GameObject InstantiatePrefab (this GameObject go, GameObject itemPrefab, bool resetScale = true, bool resetPos = true)
	{
		GameObject goRet = null;
		goRet = (GameObject)GameObject.Instantiate(itemPrefab);
		goRet.transform.parent = go.transform;
		if (resetScale) goRet.transform.localScale = Vector3.one;
		if (resetPos) goRet.transform.localPosition = Vector3.zero;
		return goRet;
	}

	public static T GetOrAddComponent<T> (this GameObject go, bool disableC = false) where T : class {
		if (go.GetComponent(typeof(T)) == null) return go.AddComponent(typeof(T)) as T;
		return go.GetComponent(typeof(T)) as T;
	}


	public static IEnumerable<T> MoveSection<T>(this IEnumerable<T> @this, int insertionPoint, int startIndex, int numElements)
	{
		// Source: http://stackoverflow.com/questions/17158051/c-sharp-moving-a-section-of-items-within-the-list
		var counter = 0;
		var range = Enumerable.Range(startIndex, numElements);
		foreach (var i in @this)
		{
			if (counter == insertionPoint)
			{
				foreach (var j in @this.Skip(startIndex).Take(numElements))
				{
					yield return j;
				}
			}
			if (!range.Contains(counter))
			{
				yield return i;
			}
			counter++;
		}
		//The insertion point might have been after the entire list:
		if (counter++ == insertionPoint)
		{
			foreach (var j in @this.Skip(startIndex).Take(numElements))
			{
				yield return j;
			}
		}
	}

	public static void SetAlpha(this Graphic g, float alpha)
	{
		Color c = g.color;
		c.a = alpha;
		g.color = c;
	}

	public static bool HasComponent(this GameObject obj, Type ClassType)
	{
		// Source: https://forum.unity3d.com/threads/check-if-a-gameobject-has-a-certain-script.88484/
		Component[] cs = (Component[])obj.GetComponents(typeof(Component));
		foreach (Component c in cs)
		{

			if (c.GetType() == ClassType)
			{
				return true;
			}

		}
		return false;
	}


    public static bool Contains(this LayerMask layerMask, int layer)
    {
        return layerMask == (layerMask | (1 << layer));
    }

    public static void RemoveNulls<T>(this List<T> list)
    {
        for (int i = list.Count - 1; i >= 0; i--)
        {
            if (list[i] == null)
            {
                list.RemoveAt(i);
            }
        }
    }

	/// <summary>
	/// Finds all children with given name.
	/// </summary>
	/// <param name="aParent"></param>
	/// <param name="aName">Name to find.</param>
	/// <returns></returns>
	public static List<Transform> FindDeepChildren(this Transform aParent, string aName)
	{
		List<Transform> results = new List<Transform>();
        foreach (var t in aParent.GetComponentsInChildren<Transform>(true))
        {
            if (t.name == aName)
            {
				results.Add(t);
            }
        }
		return results;
	}

    public static void SetListener<T>(this UnityEvent<T> @this, UnityAction<T> call)
    {
        @this.RemoveAllListeners();
        @this.AddListener(call);
    }
    public static void SetListener(this UnityEvent @this, UnityAction call)
    {
        @this.RemoveAllListeners();
        @this.AddListener(call);
    }
}

//Extension class to provide serialize / deserialize methods to object.
//src: http://stackoverflow.com/questions/1446547/how-to-convert-an-object-to-a-byte-array-in-c-sharp
//NOTE: You need add [Serializable] attribute in your class to enable serialization
public static class ObjectSerializationExtension
{

	public static byte[] SerializeToByteArray(this object obj)
	{
		if (obj == null)
		{
			return null;
		}
		var bf = new BinaryFormatter();
		using (var ms = new MemoryStream())
		{
			bf.Serialize(ms, obj);
			return ms.ToArray();
		}
	}

	public static T Deserialize<T>(this byte[] byteArray) where T : class
	{
		if (byteArray == null)
		{
			return null;
		}
		using (var memStream = new MemoryStream())
		{
			var binForm = new BinaryFormatter();
			memStream.Write(byteArray, 0, byteArray.Length);
			memStream.Seek(0, SeekOrigin.Begin);
			var obj = (T)binForm.Deserialize(memStream);
			return obj;
		}
	}
}