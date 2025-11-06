using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Ball : MonoBehaviour, IPointerClickHandler
{
    public Color ballColor;
    public Tube parentTube;
    private Vector2 originalLocalPos;
    private Image ballImage;

    private void Awake()
    {
        ballImage = GetComponent<Image>();
        // cache parent tube if present
        parentTube = GetComponentInParent<Tube>();
    }

    public void SetColor(Color color)
    {
        ballColor = color;
        ballImage.color = color;
    }

    public void RaiseBall()
    {
        originalLocalPos = GetComponent<RectTransform>().anchoredPosition;

        Tube tube = parentTube ?? transform.parent.GetComponent<Tube>();
        if (tube == null)
        {
            Debug.LogError("❌ Ball không có Tube cha.");
            return;
        }

        int topIndex = tube.maxCapacity - 1;
        if (tube.ballPositions != null && topIndex < tube.ballPositions.Length)
        {
            Vector2 raisePos = tube.ballPositions[topIndex].anchoredPosition + new Vector2(0, 100f);
            GetComponent<RectTransform>().anchoredPosition = raisePos;
        }
    }

    public void ReturnToOriginalPosition()
    {
        GetComponent<RectTransform>().anchoredPosition = originalLocalPos;
    }

    public void LowerBall()
    {
        Vector2 currentPos = GetComponent<RectTransform>().anchoredPosition;
        GetComponent<RectTransform>().anchoredPosition = new Vector2(currentPos.x, currentPos.y - 50f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Tube pt = parentTube ?? transform.parent.GetComponent<Tube>();
        if (pt == null) return;

        // if the tube is already complete (all balls same color and full), do not allow selecting
        if (pt.IsComplete()) return;

        GameManager.instance.SelectBall(this, pt);
    }
}
