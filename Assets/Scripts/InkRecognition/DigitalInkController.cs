using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DigitalInkController : MonoBehaviour
{
    [SerializeField] public GameObject textObject;
    private static bool busy = false;
    private string correctArea = "";
    private TextMeshProUGUI document;


    void Start()
    {
        document = textObject.GetComponent<TextMeshProUGUI>();
        document.text = "";
    }
    void Update()
    {
        bool pressedA = OVRInput.Get(OVRInput.Button.One);
        bool releasedB = OVRInput.GetUp(OVRInput.Button.Two);
        bool releasedGrip = OVRInput.GetUp(OVRInput.Button.SecondaryHandTrigger);

        if (pressedA && !busy)
        {
            busy = true;

            StartCoroutine(HandleRecognition());
        }

        if(releasedB)
        {
            document.text = document.text.Remove(document.text.Length -1, 1);
        }

        if (releasedGrip)
        {
            document.text += " ";
        }

    }

    private IEnumerator HandleRecognition()
    {
        Debug.Log("[InkTest] Left trigger detected. Running handwriting recognition ...");
        List<List<Vector2>> strokes = new List<List<Vector2>>();
        foreach (Transform child in transform)
        {
            LineRenderer line = child.gameObject.GetComponent<LineRenderer>();
            Vector3[] positions = new Vector3[line.positionCount];
            _ = line.GetPositions(positions);
            var (stroke, plane) = ProjectToLargestPlane(positions, correctArea);
            correctArea = plane;
            strokes.Add(stroke);
            Destroy(child.gameObject);
        }

        strokes = NormalizeStrokes(strokes);
        string recognizedText = RecognizeInkInUnity(strokes);
        document.text += recognizedText;
        Debug.Log("[InkTest] Recognized Text: " + recognizedText);

        correctArea = "";

        yield return new WaitForSeconds(1f);
        busy = false;
    }

    public static string RecognizeInkInUnity(List<List<Vector2>> strokes)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.AppendFormat("[InkTestPoints]\n");
        foreach (var stroke in strokes)
        {
            foreach (var point in stroke)
            {
                sb.AppendFormat("({0}, {1}),\n", point.x, point.y);
            }
        }
        Debug.Log(sb);
#if UNITY_ANDROID && !UNITY_EDITOR
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
        using (AndroidJavaClass helperClass = new AndroidJavaClass("com.example.digitalinkwrapper.DigitalInkHelper"))
        {
            helperClass.CallStatic("initialize", activity);

            AndroidJavaObject javaStrokeList = new AndroidJavaObject("java.util.ArrayList");

            foreach (var stroke in strokes)
            {
                AndroidJavaObject javaPoints = new AndroidJavaObject("java.util.ArrayList");
                foreach (var point in stroke)
                {
                    AndroidJavaObject pointF = new AndroidJavaObject("android.graphics.PointF", point.x, point.y);
                    javaPoints.Call<bool>("add", pointF);
                }
                javaStrokeList.Call<bool>("add", javaPoints);
            }

            string result = helperClass.CallStatic<string>("recognizeInkSync", javaStrokeList);
            return result;
        }
#else
        return "Editor mode - not running on Android.";
#endif
    }

    public static (List<Vector2>, string) ProjectToLargestPlane(Vector3[] points, string correctArea)
    {
        if (points == null || points.Length == 0) return (new List<Vector2>(), "");
        List<Vector2> projected = new List<Vector2>();

        if (correctArea != "")
        {
            switch (correctArea)
            {
                case "xy":
                    foreach (var p in points)
                        projected.Add(new Vector2(p.x * 100, p.y * 100));
                    return (projected, correctArea);
                case "xz":
                    foreach (var p in points)
                        projected.Add(new Vector2(p.x * 100, p.z * 100));
                    return (projected, correctArea);
                case "yz":
                    foreach (var p in points)
                        projected.Add(new Vector2(p.y * 100, p.z * 100));
                    return (projected, correctArea);
            }
        }

        float minX = points.Min(p => p.x);
        float maxX = points.Max(p => p.x);
        float minY = points.Min(p => p.y);
        float maxY = points.Max(p => p.y);
        float minZ = points.Min(p => p.z);
        float maxZ = points.Max(p => p.z);

        float areaXY = (maxX - minX) * (maxY - minY);
        float areaXZ = (maxX - minX) * (maxZ - minZ);
        float areaYZ = (maxY - minY) * (maxZ - minZ);

        if (areaXY >= areaXZ && areaXY >= areaYZ)
        {
            correctArea = "xy";
            foreach (var p in points)
                projected.Add(new Vector2(p.x * 100, p.y * 100));
        }
        else if (areaXZ >= areaYZ)
        {
            correctArea = "xz";
            foreach (var p in points)
                projected.Add(new Vector2(p.x * 100, p.z * 100));
        }
        else
        {
            correctArea = "yz";
            foreach (var p in points)
                projected.Add(new Vector2(p.y * 100, p.z * 100));
        }

        return (projected, correctArea);
    }

    public static List<List<Vector2>> NormalizeStrokes(List<List<Vector2>> strokes, float targetSize = 300f)
    {
        if (strokes == null || strokes.Count == 0) return new List<List<Vector2>>();

        var allPoints = strokes.SelectMany(stroke => stroke).ToList();

        float minX = allPoints.Min(p => p.x);
        float maxX = allPoints.Max(p => p.x);
        float minY = allPoints.Min(p => p.y);
        float maxY = allPoints.Max(p => p.y);

        float rangeX = maxX - minX;
        float rangeY = maxY - minY;

        if (rangeX == 0) rangeX = 1f;
        if (rangeY == 0) rangeY = 1f;

        float scale = targetSize / Mathf.Max(rangeX, rangeY);

        List<List<Vector2>> normalized = new List<List<Vector2>>();
        foreach (var stroke in strokes)
        {
            List<Vector2> normStroke = new List<Vector2>();
            foreach (var p in stroke)
            {
                float normX = (p.x - minX) * scale;
                float normY = ((minY - p.y) * scale) + 300;
                normStroke.Add(new Vector2(normX, normY));
            }
            normalized.Add(normStroke);
        }

        return normalized;
    }

}