using UnityEngine;

public class Ball : MonoBehaviour
{
    public Color ballColor;
    public Tube parentTube;
    private Vector3 originalLocalPos;

    private void Awake()
    {
        
    }
    public void SetColor(Color color)
    {
        ballColor = color;
        GetComponent<Renderer>().material.color = color;
    }

    public void RaiseBall()
    {
        originalLocalPos = transform.localPosition;

        Tube parentTube = transform.parent.GetComponent<Tube>();
        if (parentTube == null)
        {
            Debug.LogError("❌ Ball không có Tube cha.");
            return;
        }

        int topIndex = parentTube.maxCapacity - 1;
        if (parentTube.ballPositions != null && topIndex < parentTube.ballPositions.Length)
        {
            Vector3 raisePos = parentTube.ballPositions[topIndex].localPosition + new Vector3(0, 1f, 0);
            transform.localPosition = raisePos;

        }
    }

    public void ReturnToOriginalPosition()
    {
        transform.localPosition = originalLocalPos;
    }

    public void LowerBall()
    {
     //   transform.localPosition -= Vector3.up * 0.5f;
    }

    public void OnMouseDown()
    {
        Tube parentTube = transform.parent.GetComponent<Tube>();
        if (parentTube != null)
        {
            GameManager.instance.SelectBall(this, parentTube);
        }
    }
}
