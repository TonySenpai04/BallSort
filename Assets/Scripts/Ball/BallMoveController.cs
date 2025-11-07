using System.Collections;
using UnityEngine;

public class BallMoveController
{
    private Ball selectedBall;
    private Tube fromTube;

    private readonly MonoBehaviour coroutineHost;
    private readonly float moveDuration;
    private readonly System.Action onWinCheck;

    public BallMoveController(MonoBehaviour host, float moveDuration, System.Action onWinCheck)
    {
        coroutineHost = host;
        this.moveDuration = moveDuration;
        this.onWinCheck = onWinCheck;
    }


    public void SelectBall(Ball ball, Tube tube)
    {
        if (selectedBall != null) return;
        if (tube.PeekTopBall() != ball) return;

        selectedBall = ball;
        fromTube = tube;
        ball.RaiseBall();
        SoundManager.instance?.PlayClick();
    }

    public void TryMoveTo(Tube targetTube)
    {
        if (selectedBall == null || fromTube == null) return;
        coroutineHost.StartCoroutine(TryMoveRoutine(targetTube));
    }

    private IEnumerator TryMoveRoutine(Tube targetTube)
    {
        if (targetTube == fromTube)
        {
            UnselectBall();
            yield break;
        }

        Ball currentBall = selectedBall;

        if (!targetTube.CanAddBall(currentBall))
        {
            UnselectBall();
            SoundManager.instance?.PlayFail();
            yield break;
        }

        // chuyển các bóng liên tiếp cùng màu
        while (currentBall != null && targetTube.CanAddBall(currentBall))
        {
            fromTube.RemoveTopBall();

            int targetIndex = targetTube.balls.Count;
            RectTransform targetSlot = targetTube.ballPositions[targetIndex];
            if (targetSlot != null)
                yield return coroutineHost.StartCoroutine(AnimateBallToSlot(currentBall, targetSlot, targetTube.parentPanel));

            targetTube.AddBall(currentBall);
            SoundManager.instance?.PlayDrop();

            Ball next = fromTube.PeekTopBall();
            currentBall = (next != null && next.ballColor == currentBall.ballColor) ? next : null;
            yield return new WaitForSeconds(0.03f);
        }

        selectedBall = null;
        fromTube = null;
        onWinCheck?.Invoke();
    }

    public void UnselectBall()
    {
        selectedBall?.ReturnToOriginalPosition();
        selectedBall = null;
        fromTube = null;
        SoundManager.instance?.PlayFail();
    }

    private IEnumerator AnimateBallToSlot(Ball ball, RectTransform targetSlot, RectTransform panel)
    {
        if (ball == null || targetSlot == null || panel == null) yield break;
        RectTransform ballRT = ball.GetComponent<RectTransform>();
        if (ballRT == null) yield break;

        Vector2 startAnch = WorldToLocalAnchored(panel, ballRT.position);
        Vector2 targetAnch = WorldToLocalAnchored(panel, targetSlot.position);

        ballRT.SetParent(panel, true);
        ballRT.anchoredPosition = startAnch;

        float t = 0f;
        while (t < moveDuration)
        {
            ballRT.anchoredPosition = Vector2.Lerp(startAnch, targetAnch, t / moveDuration);
            t += Time.deltaTime;
            yield return null;
        }

        ballRT.anchoredPosition = targetAnch;
    }

    private Vector2 WorldToLocalAnchored(RectTransform parent, Vector3 worldPos)
    {
        Vector2 screen = RectTransformUtility.WorldToScreenPoint(null, worldPos);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, screen, null, out var local);
        return local;
    }
    public bool HasSelectedBall()
    {
        return selectedBall != null;
    }

    public bool IsSelectedBall(Ball ball)
    {
        return selectedBall == ball;
    }


}
