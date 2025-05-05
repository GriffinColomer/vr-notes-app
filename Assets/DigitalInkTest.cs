// using UnityEngine;
// using System.Collections.Generic;
// using System;

// public class DigitalInkTest : MonoBehaviour
// {
//     private DigitalInk recognizer;
//     private List<Vector2> testStroke = new List<Vector2>();

//     void Start()
//     {
//         MainThreadDispatcher.Initialize();
//         Debug.Log("1 - Before recognizer creation"); // Add this

//         CheckAARLoaded();

//         recognizer = new DigitalInk("en-US");
//         Debug.Log("2 - After recognizer creation"); // Add this

//         CreateHelloStroke();
//         Debug.Log($"3 - Stroke created with {testStroke.Count} points"); // Add this

//         Invoke("RunTestRecognition", 1f);
//         Debug.Log("4 - Invoke scheduled"); // Add this
//     }

//     void Update()
//     {
//         // Draw debug lines every frame
//         for (int i = 1; i < testStroke.Count; i++)
//         {
//             Debug.DrawLine(
//                 new Vector3(testStroke[i - 1].x, testStroke[i - 1].y, 0),
//                 new Vector3(testStroke[i].x, testStroke[i].y, 0),
//                 Color.green);
//         }
//     }

//     void CheckAARLoaded()
//     {
//         try
//         {
//             using (var pluginClass = new AndroidJavaClass("com.example.digitialinkrecognizerwrapper.DigitalInkRecognizerWrapper"))
//             {
//                 Debug.Log("AAR loaded successfully");
//             }
//         }
//         catch (Exception e)
//         {
//             Debug.LogError($"AAR loading failed: {e.Message}");
//         }
//     }

//     void CreateHelloStroke()
//     {
//         // Clear any existing points
//         testStroke.Clear();

//         // Create points that roughly form the word "hello"
//         // These are relative coordinates that should work in most screen sizes

//         // Letter 'h'
//         AddVerticalLine(0.2f, 0.8f, 0.5f); // Down stroke
//         AddCurve(0.2f, 0.5f, 0.4f, 0.5f);   // Top curve

//         // Letter 'e'
//         AddCurve(0.5f, 0.5f, 0.7f, 0.5f);   // Top curve
//         AddHorizontalLine(0.7f, 0.5f, 0.7f); // Short connecting line

//         // Letter 'l'
//         AddVerticalLine(0.8f, 0.8f, 0.5f);  // Down stroke
//         AddVerticalLine(0.8f, 0.5f, 0.3f);  // Continue down

//         // Letter 'l' (second)
//         AddVerticalLine(1.0f, 0.8f, 0.5f);  // Down stroke
//         AddVerticalLine(1.0f, 0.5f, 0.3f);  // Continue down

//         // Letter 'o'
//         AddCurve(1.2f, 0.5f, 1.4f, 0.5f);  // Oval shape
//     }

//     void AddVerticalLine(float x, float yStart, float yEnd)
//     {
//         float step = (yEnd - yStart) / 10f;
//         for (int i = 0; i <= 10; i++)
//         {
//             testStroke.Add(new Vector2(x, yStart + (step * i)));
//         }
//     }

//     void AddHorizontalLine(float xStart, float y, float xEnd)
//     {
//         float step = (xEnd - xStart) / 5f;
//         for (int i = 0; i <= 5; i++)
//         {
//             testStroke.Add(new Vector2(xStart + (step * i), y));
//         }
//     }

//     void AddCurve(float xStart, float yStart, float xEnd, float yEnd)
//     {
//         // Simple curve approximation
//         int segments = 10;
//         for (int i = 0; i <= segments; i++)
//         {
//             float t = i / (float)segments;
//             float x = Mathf.Lerp(xStart, xEnd, t);
//             float y = yStart + Mathf.Sin(t * Mathf.PI) * 0.2f;
//             testStroke.Add(new Vector2(x, y));
//         }
//     }

//     void RunTestRecognition()
//     {
//         Debug.Log("Starting recognition test with " + testStroke.Count + " points...");

//         recognizer.RecognizeInk(
//             testStroke,
//             result =>
//             {
//                 Debug.Log("Recognition Success! Result: " + result);

//                 // If you want to see the actual points in the console:
//                 //Debug.Log("Points used:\n" + string.Join("\n", testStroke));

//                 // For demo purposes, let's recognize again after a delay
//                 if (!result.ToLower().Contains("hello"))
//                 {
//                     Debug.Log("Didn't recognize 'hello', trying again with adjusted points...");
//                     testStroke.Clear();
//                     CreateHelloStroke(); // Try again with slightly different points
//                     Invoke("RunTestRecognition", 1f);
//                 }
//             },
//             error =>
//             {
//                 Debug.LogError("Recognition Failed: " + error);

//                 // If model isn't downloaded yet, wait and try again
//                 if (error.Contains("not downloaded"))
//                 {
//                     Debug.Log("Model downloading... will try again in 3 seconds");
//                     Invoke("RunTestRecognition", 3f);
//                 }
//             }
//         );
//     }

//     void OnDestroy()
//     {
//         recognizer?.Dispose();
//     }

//     // Optional: Visualize the test stroke in the Scene view
//     void OnDrawGizmos()
//     {
//         if (testStroke == null || testStroke.Count < 2) return;

//         Gizmos.color = Color.green;
//         for (int i = 1; i < testStroke.Count; i++)
//         {
//             Vector3 start = new Vector3(testStroke[i - 1].x, testStroke[i - 1].y, 0);
//             Vector3 end = new Vector3(testStroke[i].x, testStroke[i].y, 0);
//             Gizmos.DrawLine(start, end);
//         }
//     }
// }