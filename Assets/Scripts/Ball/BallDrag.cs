using UnityEngine;

public class BallClick : MonoBehaviour
{
    public Tube currentTube;

    private void Start()
    {
        currentTube = transform.parent.GetComponent<Tube>();
    }

    public void OnClick()
    {
        Ball topBall = currentTube.PeekTopBall();
        if (topBall != GetComponent<Ball>()) return;

        GameManager.instance.SelectBall(GetComponent<Ball>(), currentTube);
    }
}
