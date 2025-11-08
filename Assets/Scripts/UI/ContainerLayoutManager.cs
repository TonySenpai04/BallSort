using UnityEngine;
using UnityEngine.UI;

public class ContainerLayoutManager
{
    private RectTransform container;
    private GridLayoutGroup gridLayout;

    public ContainerLayoutManager(RectTransform container)
    {
        this.container = container;
        this.gridLayout = container != null ? container.GetComponent<GridLayoutGroup>() : null;

        if (this.container != null)
            this.container.pivot = new Vector2(0.5f, 1f);

        if (this.gridLayout != null)
            this.gridLayout.childAlignment = TextAnchor.UpperLeft;
    }

    public void AdjustSize(int totalButtons)
    {
        if (container == null || gridLayout == null) return;

        float containerWidth = container.rect.width;
        int buttonsPerRow = Mathf.FloorToInt((containerWidth + gridLayout.spacing.x) /
                                             (gridLayout.cellSize.x + gridLayout.spacing.x));
        if (buttonsPerRow <= 0) buttonsPerRow = 1;

        int numberOfRows = Mathf.CeilToInt((float)totalButtons / buttonsPerRow);
        float requiredHeight = (numberOfRows * gridLayout.cellSize.y) +
                               ((numberOfRows - 1) * gridLayout.spacing.y) +
                               (gridLayout.padding.top + gridLayout.padding.bottom);

        Vector2 newSize = container.sizeDelta;
        newSize.y = requiredHeight;
        container.sizeDelta = newSize;

        LayoutRebuilder.ForceRebuildLayoutImmediate(container);
    }

    public void ClearContainer()
    {
        if (container == null) return;
        foreach (Transform child in container)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
}
