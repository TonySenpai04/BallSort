using System.Collections.Generic;
using UnityEngine;

public class Tube : MonoBehaviour
{
    public Stack<Ball> balls = new Stack<Ball>();
    public Transform[] ballPositions;
    public int maxCapacity => ballPositions.Length;

    public void OnMouseDown()
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

        ball.transform.parent = transform;

        int index = balls.Count - 1;

        if (ballPositions == null || index >= ballPositions.Length)
        {
            Debug.LogError($"❌ Tube '{name}' invalid ballPositions! index = {index}, length = {ballPositions?.Length ?? 0}");
            return;
        }

        ball.transform.localPosition = ballPositions[index].localPosition;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        if (ballPositions != null)
        {
            for (int i = 0; i < ballPositions.Length; i++)
            {
                Gizmos.DrawSphere(ballPositions[i].position, 0.1f);
            }
        }
    }


    public Ball RemoveTopBall()
    {
        if (balls.Count == 0) return null;
        return balls.Pop();
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