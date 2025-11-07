using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Tube : MonoBehaviour, IPointerClickHandler
{
    public RectTransform parentPanel; // optional: assign a Panel (RectTransform) in the Canvas to host this tube
    public Stack<Ball> balls = new Stack<Ball>();
    public RectTransform[] ballPositions;
    public int maxCapacity => ballPositions != null ? ballPositions.Length : 0;
    public Image tubeImage;

    private void Awake()
    {
        tubeImage = GetComponent<Image>();
        // If a parentPanel is provided, ensure this tube is a child of that panel (useful for Canvas UI layout)
        if (parentPanel != null && transform.parent != parentPanel)
        {
            transform.SetParent(parentPanel, false);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (balls.Count < maxCapacity)
        {
            GameManager.instance.TryMoveSelectedBallTo(this);
        }
    }

    public void AddBall(Ball ball)
    {
        if (ball == null) return;

        balls.Push(ball);
        // set parent reference on the ball
        ball.parentTube = this;

        // Work with UI RectTransform for balls and positions
        RectTransform ballRect = ball.GetComponent<RectTransform>();
        if (ballRect == null)
        {
            ballRect = ball.gameObject.AddComponent<RectTransform>();
        }

        // Parent the ball under this tube's RectTransform (false keeps local UI layout)
        RectTransform myRect = transform as RectTransform;
        if (myRect != null)
        {
            ballRect.SetParent(myRect, false);
        }
        else
        {
            ballRect.SetParent(transform, false);
        }

        int index = balls.Count - 1;

        if (ballPositions == null || index >= ballPositions.Length)
        {
            Debug.LogError($"❌ Tube '{name}' invalid ballPositions! index = {index}, length = {ballPositions?.Length ?? 0}");
            return;
        }

        // Use anchoredPosition so UI aligns with configured slots
        ballRect.anchoredPosition = ballPositions[index].anchoredPosition;
    }
    // Remove OnDrawGizmosSelected as it's not needed for UI elements


    public Ball RemoveTopBall()
    {
        if (balls.Count == 0) return null;
        Ball b = balls.Pop();
        if (b != null)
        {
            // clear parentTube while the ball is in transit
            b.parentTube = null;
        }
        return b;
    }

    public Ball PeekTopBall()
    {
        return balls.Count > 0 ? balls.Peek() : null;
    }

    public bool CanAddBall(Ball newBall)
    {
        if (balls.Count >= ballPositions.Length) return false;
        if (balls.Count == 0) return true;
        return balls.Peek().ballColor == newBall.ballColor;
    }

    public bool IsComplete()
    {
        if (balls.Count != maxCapacity) return false;
        Color color = balls.Peek().ballColor;
        foreach (var b in balls)
        {
            if (b.ballColor != color) return false;
        }
        return true;
    }
}