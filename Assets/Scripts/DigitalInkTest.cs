using System.Collections.Generic;
using UnityEngine;

public class DigitalInkTest : MonoBehaviour
{
    [SerializeField]public GameObject lineController;

    void Update()
    {
        float triggerLeft = OVRInput.Get(OVRInput.RawAxis1D.LIndexTrigger);

        if (triggerLeft > 0.9f)
        {
            Debug.Log("[InkTest] Left click detected. Running handwriting recognition test...");

            // Create a dummy stroke: a simple diagonal line
            List<List<Vector2>> strokes = new List<List<Vector2>>();
            List<Vector2> stroke1 = new List<Vector2>
            {
                new Vector2(100, 100),
                new Vector2(110, 110),
                new Vector2(120, 120),
                new Vector2(130, 130),
                new Vector2(140, 140)
            };
            strokes.Add(stroke1);

            string recognizedText = RecognizeInkInUnity(strokes);
            Debug.Log("[InkTest] Recognized Text: " + recognizedText);
        }
    }

    public static string RecognizeInkInUnity(List<List<Vector2>> strokes)
    {
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
}
