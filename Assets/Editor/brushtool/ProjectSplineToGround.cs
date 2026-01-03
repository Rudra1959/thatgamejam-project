using UnityEngine;
using UnityEditor;
using UnityEngine.Splines;

public class ProjectSplineToGround : EditorWindow
{
    [MenuItem("Tools/Project Spline To Ground")]
    static void Project()
    {
        var splineContainer = Selection.activeGameObject?.GetComponent<SplineContainer>();

        if (!splineContainer)
        {
            Debug.LogError("❌ Select a GameObject with SplineContainer");
            return;
        }

        Undo.RecordObject(splineContainer, "Project Spline");

        foreach (var spline in splineContainer.Splines)
        {
            for (int i = 0; i < spline.Count; i++)
            {
                var knot = spline[i];
                Vector3 worldPos = splineContainer.transform.TransformPoint(knot.Position);

                Ray ray = new Ray(worldPos + Vector3.up * 50f, Vector3.down);

                if (Physics.Raycast(ray, out RaycastHit hit, 200f))
                {
                    Vector3 localPos =
                        splineContainer.transform.InverseTransformPoint(
                            hit.point + hit.normal * 0.05f // slight lift to avoid clipping
                        );

                    knot.Position = localPos;
                    spline[i] = knot;
                }
            }
        }

        Debug.Log("✅ Spline projected onto mountain");
    }
}
