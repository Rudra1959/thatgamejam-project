using UnityEngine;
using UnityEditor;
using Random = UnityEngine.Random;

public class GrassBrush : EditorWindow
{
    GameObject prefab;
    Transform parentTransform;
    float radius = 2f;
    int density = 10;
    float avoidDistance = 0.5f;
    Vector2 scaleRange = new Vector2(1f, 1f);
    bool randomRotation = true;
    bool painting = false;

    const string groundTag = "Ground";

    [MenuItem("Art Tools/Grass Brush")]
    static void Open()
    {
        GetWindow<GrassBrush>("Grass Brush");
    }

    void OnEnable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
        if (painting) SceneView.duringSceneGui += OnSceneGUI;
    }

    void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
        painting = false;
    }

    void OnGUI()
    {
        prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false);
        parentTransform = (Transform)EditorGUILayout.ObjectField("Parent", parentTransform, typeof(Transform), true);

        radius = EditorGUILayout.Slider("Brush Radius", radius, 0.1f, 20f);
        density = EditorGUILayout.IntSlider("Density", density, 1, 300);
        avoidDistance = EditorGUILayout.Slider("Avoid Distance", avoidDistance, 0.05f, 3f);
        scaleRange = EditorGUILayout.Vector2Field("Scale Range", scaleRange);
        randomRotation = EditorGUILayout.Toggle("Random Rotation", randomRotation);

        GUILayout.Space(10);

        if (!painting)
        {
            if (GUILayout.Button("Start Painting"))
            {
                if (prefab != null)
                {
                    painting = true;
                    SceneView.duringSceneGui += OnSceneGUI;
                }
                else
                {
                    EditorUtility.DisplayDialog("Grass Brush", "Assign a Prefab first.", "OK");
                }
            }
        }
        else
        {
            if (GUILayout.Button("Stop Painting"))
            {
                painting = false;
                SceneView.duringSceneGui -= OnSceneGUI;
            }
        }
    }

    void OnSceneGUI(SceneView sv)
    {
        if (!painting || prefab == null) return;

        Event e = Event.current;
        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 500f))
        {
            Handles.color = Color.green;
            Handles.DrawWireDisc(hit.point, hit.normal, radius);

            if (e.type == EventType.MouseDown && e.button == 0 && !e.alt)
            {
                Paint(hit.point, hit.collider);
                e.Use();
            }
        }

        sv.Repaint();
    }

    bool OverlappingExisting(Vector3 pos, float dist)
    {
        if (parentTransform == null) return false;
        foreach (Transform t in parentTransform)
        {
            if (t == null) continue;
            if (Vector3.Distance(t.position, pos) < dist)
                return true;
        }
        return false;
    }

    void Paint(Vector3 center, Collider hitCol)
    {
        if (hitCol == null) return;

        if (!hitCol.CompareTag(groundTag))
            return;

        for (int i = 0; i < density; i++)
        {
            Vector2 r = Random.insideUnitCircle * radius;
            Vector3 start = center + new Vector3(r.x, 10f, r.y);

            if (!Physics.Raycast(start, Vector3.down, out RaycastHit hit, 50f))
                continue;

            if (!hit.collider.CompareTag(groundTag))
                continue;

            if (OverlappingExisting(hit.point, avoidDistance))
                continue;

            GameObject inst;
            try { inst = (GameObject)PrefabUtility.InstantiatePrefab(prefab); }
            catch { inst = Instantiate(prefab); }

            if (inst == null) continue;

            Undo.RegisterCreatedObjectUndo(inst, "Grass Paint");
            inst.transform.position = hit.point;

            if (randomRotation)
                inst.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

            float s = Random.Range(scaleRange.x, scaleRange.y);
            inst.transform.localScale = new Vector3(s, s, s);

            if (parentTransform != null)
                Undo.SetTransformParent(inst.transform, parentTransform, "Set Parent");
        }
    }
}