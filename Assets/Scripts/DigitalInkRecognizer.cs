using System.Collections.Generic;
using UnityEngine;

public class DigitalInkRecognizer : MonoBehaviour
{
    public static string RecognizeInkInUnity(List<List<Vector2>> strokes)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
    using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
    using (AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
    using (AndroidJavaClass helperClass = new AndroidJavaClass("com.example.digitalinkwrapper.DigitalInkHelper"))
    {
        // Initialize helper once
        helperClass.CallStatic("initialize", activity);

        // Prepare Java strokes: List<List<PointF>>
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

        // Call the synchronous Java function
        string result = helperClass.CallStatic<string>("recognizeInkSync", javaStrokeList);
        return result;
    }
#else
        return "Editor mode dummy";
#endif
    }

}
