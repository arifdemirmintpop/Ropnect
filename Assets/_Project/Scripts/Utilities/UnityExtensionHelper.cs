using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;

namespace Extensions
{
    public static class UnityExtensionHelper
    {
        // --- STRING ---
        public static bool ContainsIgnoreCase(this string source, string toCheck)
            => source?.IndexOf(toCheck, StringComparison.OrdinalIgnoreCase) >= 0;

        public static string Capitalize(this string s)
            => string.IsNullOrEmpty(s) ? s : char.ToUpper(s[0]) + s.Substring(1);

        public static string Repeat(this string s, int count)
        {
            if (count <= 0) return "";
            return new System.Text.StringBuilder(s.Length * count).Insert(0, s, count).ToString();
        }

        public static bool IsAlphaNumeric(this string s)
        {
            if (string.IsNullOrEmpty(s)) return false;
            foreach (char c in s)
                if (!char.IsLetterOrDigit(c))
                    return false;
            return true;
        }

        public static string Reverse(this string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            char[] array = s.ToCharArray();
            Array.Reverse(array);
            return new string(array);
        }

        public static string RemoveWhitespace(this string s)
        {
            return string.IsNullOrEmpty(s) ? s : System.Text.RegularExpressions.Regex.Replace(s, @"\s+", "");
        }

        public static bool EndsWithIgnoreCase(this string s, string value)
            => s?.EndsWith(value, StringComparison.OrdinalIgnoreCase) ?? false;

        public static bool StartsWithIgnoreCase(this string s, string value)
            => s?.StartsWith(value, StringComparison.OrdinalIgnoreCase) ?? false;

        public static string RemovePrefix(this string s, string prefix)
        {
            if (string.IsNullOrEmpty(s) || string.IsNullOrEmpty(prefix)) return s;
            return s.StartsWith(prefix) ? s.Substring(prefix.Length) : s;
        }

        public static string RemoveSuffix(this string s, string suffix)
        {
            if (string.IsNullOrEmpty(s) || string.IsNullOrEmpty(suffix)) return s;
            return s.EndsWith(suffix) ? s.Substring(0, s.Length - suffix.Length) : s;
        }

        public static string RepeatWithSeparator(this string s, int count, string separator)
        {
            if (count <= 0) return "";
            return string.Join(separator, Enumerable.Repeat(s, count));
        }

        public static string ToSnakeCase(this string str)
        {
            if (string.IsNullOrEmpty(str)) return str;
            var builder = new System.Text.StringBuilder();
            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];
                if (char.IsUpper(c))
                {
                    if (i > 0) builder.Append('_');
                    builder.Append(char.ToLower(c));
                }
                else
                {
                    builder.Append(c);
                }
            }
            return builder.ToString();
        }


        public static string ToTitleCase(this string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            System.Globalization.CultureInfo cultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;
            System.Globalization.TextInfo textInfo = cultureInfo.TextInfo;
            return textInfo.ToTitleCase(s);
        }

        public static string RemoveSpecialCharacters(this string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            return System.Text.RegularExpressions.Regex.Replace(s, "[^a-zA-Z0-9_.]+", "", System.Text.RegularExpressions.RegexOptions.Compiled);
        }

        // --- LIST ---
        public static void RemoveNulls<T>(this IList<T> list) where T : class
        {
            if (list == null) return;
            for (int i = list.Count - 1; i >= 0; i--)
                if (list[i] == null)
                    list.RemoveAt(i);
        }

        public static void RemoveDuplicates<T>(this IList<T> list)
        {
            if (list == null) return;
            HashSet<T> seen = new HashSet<T>();
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (seen.Contains(list[i]))
                    list.RemoveAt(i);
                else
                    seen.Add(list[i]);
            }
        }

        public static void AddIfNotContains<T>(this IList<T> list, T item)
        {
            if (list == null) return;
            if (!list.Contains(item))
                list.Add(item);
        }

        public static void ShuffleInPlace<T>(this IList<T> list)
        {
            if (list == null) return;
            int n = list.Count;
            for (int i = 0; i < n; i++)
            {
                int r = UnityEngine.Random.Range(i, n);
                (list[i], list[r]) = (list[r], list[i]);
            }
        }

        public static bool TryGetAt<T>(this IList<T> list, int index, out T result)
        {
            if (list != null && index >= 0 && index < list.Count)
            {
                result = list[index];
                return true;
            }
            result = default;
            return false;
        }

        public static void RemoveFirst<T>(this IList<T> list)
        {
            if (list == null || list.Count == 0) return;
            list.RemoveAt(0);
        }

        public static void RemoveLast<T>(this IList<T> list)
        {
            if (list == null || list.Count == 0) return;
            list.RemoveAt(list.Count - 1);
        }

        public static bool ContainsAny<T>(this IList<T> list, IEnumerable<T> values)
        {
            if (list == null || values == null) return false;
            foreach (var v in values)
                if (list.Contains(v))
                    return true;
            return false;
        }

        // --- GAMEOBJECT ---
        public static void SetLayer(this GameObject go, int layer)
        {
            if (go != null)
                go.layer = layer;
        }

        public static List<GameObject> GetDirectChildren(this GameObject go)
        {
        List<GameObject> children = new List<GameObject>();
        for (int i = 0; i < go.transform.childCount; i++)
        {
            children.Add(go.transform.GetChild(i).gameObject);
        }
        return children;
        }
        public static void SetLayerRecursive(this GameObject go, int layer)
        {
            if (go == null) return;
            go.layer = layer;
            foreach (Transform t in go.transform)
                t.gameObject.SetLayerRecursive(layer);
        }

        public static GameObject FindChildByName(this GameObject go, string name)
        {
            if (go == null) return null;
            foreach (Transform child in go.transform)
            {
                if (child.gameObject.name == name)
                    return child.gameObject;
                var found = child.gameObject.FindChildByName(name);
                if (found != null)
                    return found;
            }
            return null;
        }

        public static GameObject InstantiateChild(this GameObject go, GameObject prefab)
        {
            if (go == null || prefab == null) return null;
            GameObject obj = UnityEngine.Object.Instantiate(prefab, go.transform);
            return obj;
        }

        public static void DestroyChildrenImmediate(this GameObject go)
        {
            if (go == null) return;
            int childCount = go.transform.childCount;
            for (int i = childCount - 1; i >= 0; i--)
            {
                UnityEngine.Object.DestroyImmediate(go.transform.GetChild(i).gameObject);
            }
        }

        public static void DestroyChildrenImmediateSafe(this GameObject go)
        {
            if (go == null) return;
            for (int i = go.transform.childCount - 1; i >= 0; i--)
            {
                UnityEngine.Object.DestroyImmediate(go.transform.GetChild(i).gameObject);
            }
        }

        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            var comp = go.GetComponent<T>();
            if (comp == null) comp = go.AddComponent<T>();
            return comp;
        }

        public static void SetActiveIfDifferent(this GameObject go, bool active)
        {
            if (go == null) return;
            if (go.activeSelf != active) go.SetActive(active);
        }

        public static void SetActiveIfNot(this GameObject go, bool active)
        {
            if (go.activeSelf != active)
                go.SetActive(active);
        }

        // --- TRANSFORM ---
        public static void SetLocalX(this Transform t, float x)
        {
            if (t == null) return;
            var pos = t.localPosition;
            pos.x = x;
            t.localPosition = pos;
        }

        public static void SetLocalY(this Transform t, float y)
        {
            if (t == null) return;
            var pos = t.localPosition;
            pos.y = y;
            t.localPosition = pos;
        }

        public static void SetLocalZ(this Transform t, float z)
        {
            if (t == null) return;
            var pos = t.localPosition;
            pos.z = z;
            t.localPosition = pos;
        }

        public static void ResetLocalScale(this Transform t)
        {
            if (t == null) return;
            t.localScale = Vector3.one;
        }

        public static void RotateAroundPoint(this Transform t, Vector3 point, Vector3 axis, float angle)
        {
            if (t == null) return;
            t.RotateAround(point, axis, angle);
        }

        public static Vector3 GetForwardFlat(this Transform t)
        {
            Vector3 forward = t.forward;
            forward.y = 0;
            return forward.normalized;
        }

        public static Vector3 GetRightFlat(this Transform t)
        {
            Vector3 right = t.right;
            right.y = 0;
            return right.normalized;
        }

        public static void SetLocalEulerX(this Transform t, float x)
        {
            var euler = t.localEulerAngles;
            euler.x = x;
            t.localEulerAngles = euler;
        }

        public static void SetLocalEulerY(this Transform t, float y)
        {
            var euler = t.localEulerAngles;
            euler.y = y;
            t.localEulerAngles = euler;
        }

        public static void SetLocalEulerZ(this Transform t, float z)
        {
            var euler = t.localEulerAngles;
            euler.z = z;
            t.localEulerAngles = euler;
        }

        public static void ResetLocalEulerAngles(this Transform t)
        {
            t.localEulerAngles = Vector3.zero;
        }

        // --- VECTOR3 ---
        public static Vector3 SetX(this Vector3 v, float x) => new Vector3(x, v.y, v.z);
        public static Vector3 SetY(this Vector3 v, float y) => new Vector3(v.x, y, v.z);
        public static Vector3 SetZ(this Vector3 v, float z) => new Vector3(v.x, v.y, z);

        public static Vector3 AddX(this Vector3 v, float x) => new Vector3(v.x + x, v.y, v.z);
        public static Vector3 AddY(this Vector3 v, float y) => new Vector3(v.x, v.y + y, v.z);
        public static Vector3 AddZ(this Vector3 v, float z) => new Vector3(v.x, v.y, v.z + z);

        public static Vector3 Multiply(this Vector3 v, float multiplier) => v * multiplier;

        public static Vector3 Clamp(this Vector3 v, Vector3 min, Vector3 max)
        {
            return new Vector3(
                Mathf.Clamp(v.x, min.x, max.x),
                Mathf.Clamp(v.y, min.y, max.y),
                Mathf.Clamp(v.z, min.z, max.z));
        }

        public static Vector3 WithMagnitude(this Vector3 v, float magnitude)
        {
            return v.normalized * magnitude;
        }

        public static Vector3 Round(this Vector3 v, int decimals = 2)
        {
            float multiplier = Mathf.Pow(10, decimals);
            return new Vector3(
                Mathf.Round(v.x * multiplier) / multiplier,
                Mathf.Round(v.y * multiplier) / multiplier,
                Mathf.Round(v.z * multiplier) / multiplier);
        }

        public static Vector3 WithMagnitudeClamp(this Vector3 v, float maxMagnitude)
        {
            if (v.magnitude > maxMagnitude) return v.normalized * maxMagnitude;
            return v;
        }

        // --- VECTOR2 ---
        public static Vector2 WithX(this Vector2 v, float x) => new Vector2(x, v.y);
        public static Vector2 WithY(this Vector2 v, float y) => new Vector2(v.x, y);

        public static Vector2 AddX(this Vector2 v, float x) => new Vector2(v.x + x, v.y);
        public static Vector2 AddY(this Vector2 v, float y) => new Vector2(v.x, v.y + y);

        public static Vector2 Clamp(this Vector2 v, Vector2 min, Vector2 max)
        {
            return new Vector2(
                Mathf.Clamp(v.x, min.x, max.x),
                Mathf.Clamp(v.y, min.y, max.y));
        }

        public static Vector2 WithMagnitudeClamp(this Vector2 v, float maxMagnitude)
        {
            if (v.magnitude > maxMagnitude) return v.normalized * maxMagnitude;
            return v;
        }

        // --- COLOR ---
        public static Color SetAlpha(this Color c, float alpha)
        {
            c.a = alpha;
            return c;
        }

        public static Color MultiplyAlpha(this Color c, float factor)
        {
            c.a *= factor;
            return c;
        }

        public static Color WithRed(this Color c, float red) => new Color(red, c.g, c.b, c.a);
        public static Color WithGreen(this Color c, float green) => new Color(c.r, green, c.b, c.a);
        public static Color WithBlue(this Color c, float blue) => new Color(c.r, c.g, blue, c.a);

        public static Color SetRed(this Color c, float r) => new Color(r, c.g, c.b, c.a);
        public static Color SetGreen(this Color c, float g) => new Color(c.r, g, c.b, c.a);
        public static Color SetBlue(this Color c, float b) => new Color(c.r, c.g, b, c.a);

        public static Color ToColor(this string hex)
        {
            if (ColorUtility.TryParseHtmlString(hex, out var color))
                return color;
            return Color.white;
        }

        public static string ToHex(this Color color)
        {
            Color32 c32 = color;
            return $"#{c32.r:X2}{c32.g:X2}{c32.b:X2}{c32.a:X2}";
        }

        // --- UI ---
        public static void SetInteractable(this Selectable s, bool interactable)
        {
            if (s != null)
                s.interactable = interactable;
        }

        public static void SetTextSafe(this Text t, string text)
        {
            if (t != null)
                t.text = text;
        }

        public static void SetImageColor(this Image img, Color color)
        {
            if (img != null)
                img.color = color;
        }

        public static void ToggleVisibility(this CanvasGroup cg, bool visible)
        {
            if (cg == null) return;
            cg.alpha = visible ? 1f : 0f;
            cg.interactable = visible;
            cg.blocksRaycasts = visible;
        }

        public static void SetAlpha(this CanvasGroup cg, float alpha)
        {
            if (cg == null) return;
            cg.alpha = alpha;
            cg.blocksRaycasts = alpha > 0f;
            cg.interactable = alpha > 0f;
        }

        public static void SetButtonInteractableWithColor(this Button btn, bool interactable, Color enabledColor, Color disabledColor)
        {
            if (btn == null) return;
            btn.interactable = interactable;
            var colors = btn.colors;
            colors.normalColor = interactable ? enabledColor : disabledColor;
            btn.colors = colors;
        }

        public static void SetTextFontSize(this Text t, int size)
        {
            if (t == null) return;
            t.fontSize = size;
        }

        public static void SetButtonText(this Button btn, string text)
        {
            if (btn == null) return;
            var txt = btn.GetComponentInChildren<Text>();
            if (txt != null) txt.text = text;
        }

        public static void SetRichText(this Text text, string content, Color color, bool bold = false, bool italic = false)
        {
            if (text == null) return;
            string richText = $"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>";
            if (bold) richText += "<b>";
            if (italic) richText += "<i>";
            richText += content;
            if (italic) richText += "</i>";
            if (bold) richText += "</b>";
            richText += "</color>";
            text.text = richText;
        }

        // --- AUDIO ---
        public static void Mute(this AudioSource audioSource, bool mute)
        {
            if (audioSource != null)
                audioSource.mute = mute;
        }

        public static void SetVolume(this AudioSource audioSource, float volume)
        {
            if (audioSource != null)
                audioSource.volume = Mathf.Clamp01(volume);
        }

        public static void PlayRandomClip(this AudioSource source, params AudioClip[] clips)
        {
            if (source == null || clips == null || clips.Length == 0) return;
            AudioClip clip = clips[UnityEngine.Random.Range(0, clips.Length)];
            source.PlayOneShot(clip);
        }

        

        private static IEnumerator FadeVolumeCoroutine(AudioSource source, float targetVolume, float duration)
        {
            float startVolume = source.volume;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                source.volume = Mathf.Lerp(startVolume, targetVolume, elapsed / duration);
                yield return null;
            }
            source.volume = targetVolume;
        }

        // --- RIGIDBODY ---
        public static void FreezePosition(this Rigidbody rb, bool freeze)
        {
            if (rb == null) return;
            rb.constraints = freeze
                ? RigidbodyConstraints.FreezePosition
                : RigidbodyConstraints.None;
        }

        public static void FreezeRotation(this Rigidbody rb, bool freeze)
        {
            if (rb == null) return;
            rb.constraints = freeze
                ? RigidbodyConstraints.FreezeRotation
                : RigidbodyConstraints.None;
        }

        public static void FreezeAll(this Rigidbody rb)
        {
            if (rb == null) return;
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }

        public static void UnfreezeAll(this Rigidbody rb)
        {
            if (rb == null) return;
            rb.constraints = RigidbodyConstraints.None;
        }

        public static void AddForceRelative(this Rigidbody rb, Vector3 force)
        {
            if (rb == null) return;
            rb.AddRelativeForce(force);
        }

        // --- ANIMATOR ---
        public static void ResetTriggerSafe(this Animator anim, string triggerName)
        {
            if (anim != null && anim.isInitialized)
                anim.ResetTrigger(triggerName);
        }

        public static bool IsAnimatorPlaying(this Animator anim)
        {
            if (anim == null) return false;
            return anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1;
        }

        // --- MATH ---
        public static int ClampInt(this int value, int min, int max)
        {
            return Mathf.Clamp(value, min, max);
        }

        public static bool IsPowerOfTwo(this int x)
        {
            return (x != 0) && ((x & (x - 1)) == 0);
        }

        public static int NextPowerOfTwo(this int x)
        {
            if (x < 0) return 0;
            x--;
            x |= x >> 1;
            x |= x >> 2;
            x |= x >> 4;
            x |= x >> 8;
            x |= x >> 16;
            x++;
            return x;
        }

        public static float ToSeconds(this TimeSpan ts)
        {
            return (float)ts.TotalSeconds;
        }

        public static string ToShortTimeString(this TimeSpan ts)
        {
            if (ts.Hours > 0)
                return $"{ts.Hours:D2}:{ts.Minutes:D2}:{ts.Seconds:D2}";
            else
                return $"{ts.Minutes:D2}:{ts.Seconds:D2}";
        }

        // --- DEBUG ---
        public static void LogWithTimestamp(this object obj)
        {
            Debug.Log($"[{DateTime.Now:HH:mm:ss}] {obj}");
        }

        public static void LogWarningIfNull(this UnityEngine.Object obj, string message)
        {
            if (obj == null)
                Debug.LogWarning(message);
        }

        public static void LogErrorIfNull(this UnityEngine.Object obj, string message)
        {
            if (obj == null)
                Debug.LogError(message);
        }

        // --- COROUTINE ---
        public static IEnumerator RepeatAction(float interval, int count, Action action)
        {
            if (count <= 0) yield break;
            for (int i = 0; i < count; i++)
            {
                action?.Invoke();
                yield return new WaitForSeconds(interval);
            }
        }

        public static IEnumerator WaitForEndOfFrameAction(Action action)
        {
            yield return new WaitForEndOfFrame();
            action?.Invoke();
        }

        public static IEnumerator WaitUntilCondition(Func<bool> condition)
        {
            yield return new WaitUntil(condition);
        }

        public static void ExecuteNextFrame(this MonoBehaviour mono, Action action)
        {
            mono.StartCoroutine(ExecuteNextFrameCoroutine(action));
        }

        private static IEnumerator ExecuteNextFrameCoroutine(Action action)
        {
            yield return null;
            action?.Invoke();
        }

        // --- MISC ---
        public static bool IsNullOrDestroyed(this UnityEngine.Object obj)
        {
            return obj == null || obj.Equals(null);
        }

        public static void SafeDestroy(this UnityEngine.Object obj)
        {
            if (obj != null)
                UnityEngine.Object.Destroy(obj);
        }

        public static void SwapValues<T>(ref T lhs, ref T rhs)
        {
            T temp = lhs;
            lhs = rhs;
            rhs = temp;
        }

        // --- ENUM ---
        public static IEnumerable<T> GetEnumValues<T>() where T : Enum
        {
            return (T[])Enum.GetValues(typeof(T));
        }

        public static bool TryParseEnum<T>(this string s, out T result) where T : struct, Enum
        {
            return Enum.TryParse(s, true, out result);
        }

        public static string GetDescription<T>(this T enumeration) where T : Enum
        {
            var type = enumeration.GetType();
            var memInfo = type.GetMember(enumeration.ToString());
            if (memInfo.Length > 0)
            {
                var attrs = memInfo[0].GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
                if (attrs.Length > 0)
                    return ((System.ComponentModel.DescriptionAttribute)attrs[0]).Description;
            }
            return enumeration.ToString();
        }

        // --- PLAYER PREFS ---
        public static void SetFloatPref(this string key, float value)
        {
            PlayerPrefs.SetFloat(key, value);
        }

        public static float GetFloatPref(this string key, float defaultValue = 0f)
        {
            return PlayerPrefs.GetFloat(key, defaultValue);
        }

        // --- PHYSICS ---
        public static bool IsCollidingWithLayer(this Collider col, int layerMask)
        {
            if (col == null) return false;
            return (layerMask & (1 << col.gameObject.layer)) != 0;
        }

        // --- CAMERA ---
        public static Vector3 ScreenCenter(this Camera cam)
        {
            return cam.ScreenToWorldPoint(new Vector3(Screen.width / 2f, Screen.height / 2f, cam.nearClipPlane));
        }

        public static Vector3 ViewportToWorld(this Camera cam, Vector3 viewportPos)
        {
            return cam.ViewportToWorldPoint(viewportPos);
        }

        // --- RECT ---
        public static Vector2 GetCenter(this Rect rect)
        {
            return new Vector2(rect.x + rect.width / 2, rect.y + rect.height / 2);
        }

        public static bool ContainsPoint(this Rect rect, Vector2 point)
        {
            return rect.Contains(point);
        }

        // --- INPUT ---
        public static bool IsPointerOverUI(this UnityEngine.EventSystems.EventSystem eventSystem)
        {
            return eventSystem.IsPointerOverGameObject();
        }
    }
}
