using System.Collections.Generic;
using UnityEngine;

// Arrange Tube UI elements inside a parent Panel using RectTransform. Provides a ContextMenu to run in the Editor.
[ExecuteAlways]
public class TubeLayoutManager : MonoBehaviour
{
    [Tooltip("Panel (RectTransform) that contains Tube UI elements. If empty, the GameObject this script is on will be used.")]
    public RectTransform parentPanel;

    [Tooltip("Anchored position of the first (0) tube inside the parent.")]
    public Vector2 startPosition = new Vector2(-300f, 100f);

    [Tooltip("Spacing between tubes (x = horizontal spacing, y = vertical spacing)")]
    public Vector2 spacing = new Vector2(160f, 0f);

    [Tooltip("Number of columns. If 0, layout is a single row horizontally.")]
    public int columns = 0;

    private void OnEnable()
    {
        if (parentPanel == null)
            parentPanel = GetComponent<RectTransform>();
    }

    // Run at runtime once on Start to ensure runtime layout is applied
    private void Start()
    {
        if (Application.isPlaying)
        {
            Arrange();
        }
    }

    // Context menu accessible in the Inspector for instant arrangement
    [ContextMenu("Arrange Tubes")]
    public void Arrange()
    {
        if (parentPanel == null)
        {
            Debug.LogWarning("TubeLayoutManager: parentPanel is null. Assign a RectTransform to parentPanel or attach this script to the desired panel.");
            return;
        }

        // Collect tubes that are direct children of parentPanel
        List<Tube> tubes = new List<Tube>();
        for (int i = 0; i < parentPanel.childCount; i++)
        {
            var child = parentPanel.GetChild(i);
            var tube = child.GetComponent<Tube>();
            if (tube != null)
            {
                tubes.Add(tube);
            }
        }

        for (int i = 0; i < tubes.Count; i++)
        {
            int col = (columns > 0) ? (i % columns) : i;
            int row = (columns > 0) ? (i / columns) : 0;

            Vector2 pos = startPosition + new Vector2(col * spacing.x, -row * spacing.y);

            RectTransform rt = tubes[i].GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchoredPosition = pos;
            }
        }

#if UNITY_EDITOR
        // Mark scene dirty in editor so change persists
        if (!Application.isPlaying)
        {
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
        }
#endif
    }
}
