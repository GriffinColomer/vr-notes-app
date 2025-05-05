using UnityEngine;
using UnityEditor;
using System.Linq;

public class LineRenderHelper
{
   [MenuItem("Tools/copy LineRenderer Positions")]
   public static void CopyLineRendererPositions()
   {
        if (Selection.activeGameObject == null)
        {
            Debug.LogWarning("No object selected");
        }
        LineRenderer lineRenderer = Selection.activeGameObject.GetComponent<LineRenderer>();

        Vector3[] originalPositions = new Vector3[lineRenderer.positionCount];
        lineRenderer.GetPositions(originalPositions);

        Vector3 offset = originalPositions[0];
        Vector3[] normalizedPositions = originalPositions.Select(p => p - offset).ToArray();

        string output = "x,y,z\n" + string.Join("\n", normalizedPositions.Select(p => $"{p.x},{p.y},{p.z}"));
        EditorGUIUtility.systemCopyBuffer = output;
        Debug.Log(output);
   }
}
