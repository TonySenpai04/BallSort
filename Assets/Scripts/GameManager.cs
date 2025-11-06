using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public List<Tube> tubes = new List<Tube>();

    [SerializeField]  private Ball selectedBall = null;
    [SerializeField] private Tube fromTube = null;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private LevelLoaderFromJSON levelLoader;
    [Header("Animation")]
    public float moveDuration = 0.18f;

    [SerializeField] private TextMeshProUGUI levelTxt;

    void Awake()
    {
        instance = this;
    }

    public void SelectBall(Ball ball, Tube tube)
    {
        if (selectedBall != null) return;

        if (tube.PeekTopBall() == ball)
        {
            selectedBall = ball;
            fromTube = tube;
            ball.RaiseBall();
            SoundManager.instance?.PlayClick();
        }
    }
    private void Update()
    {
        int level = levelLoader.currentLevelIndex + 1;
        levelTxt.text= "Level:"+level;
    }
    public void TryMoveSelectedBallTo(Tube targetTube)
    {
        // start the coroutine that performs animated moves
        StartCoroutine(TryMoveSelectedBallToRoutine(targetTube));
    }

    private IEnumerator TryMoveSelectedBallToRoutine(Tube targetTube)
    {
        if (selectedBall == null || fromTube == null) yield break;

        // ❌ Không cho thả vào chính tube cũ
        if (targetTube == fromTube)
        {
            UnselectBall(); // trả về vị trí cũ
            yield break;
        }

        Ball currentBall = selectedBall;

        if (!targetTube.CanAddBall(currentBall))
        {
            selectedBall.ReturnToOriginalPosition();
            selectedBall = null;
            fromTube = null;
            SoundManager.instance?.PlayFail();
            yield break;
        }

        // ✅ Chuyển các bóng liên tiếp cùng màu (animated)
        while (currentBall != null && targetTube.CanAddBall(currentBall))
        {
            // remove logically from source
            fromTube.RemoveTopBall();

            // determine target slot (before adding) and animate
            int targetIndex = targetTube.balls.Count; // slot index
            RectTransform targetSlot = null;
            if (targetTube.ballPositions != null && targetIndex < targetTube.ballPositions.Length)
            {
                targetSlot = targetTube.ballPositions[targetIndex];
            }

            if (targetSlot != null)
            {
                // animate ball from its current position to the slot on the panel
                yield return StartCoroutine(AnimateBallToSlot(currentBall, targetSlot, targetTube.parentPanel, moveDuration));
            }

            // finalize add (this will parent and set exact slot position)
            targetTube.AddBall(currentBall);
            SoundManager.instance?.PlayDrop();

            // next ball (peek from source)
            Ball nextBall = fromTube.PeekTopBall();

            if (nextBall != null && nextBall.ballColor == currentBall.ballColor)
            {
                currentBall = nextBall;
            }
            else
            {
                currentBall = null;
            }

            // small delay between chained moves so animation is perceptible
            yield return new WaitForSeconds(0.03f);
        }

        selectedBall = null;
        fromTube = null;

        if (CheckWin())
        {
            winPanel.SetActive(true);
            StartCoroutine(NextLevelDelay(1f));
        }
    }


    public bool HasSelectedBall()
    {
        return selectedBall != null;
    }
    public bool IsSelectedBall(Ball ball)
    {
        return selectedBall == ball;
    }

    public void UnselectBall()
    {
        if (selectedBall != null)
        {
            selectedBall.ReturnToOriginalPosition();
            selectedBall = null;
            fromTube = null;
            SoundManager.instance?.PlayFail();
        }
    }

    // Helper to convert a world position to an anchored local point inside a RectTransform (panel)
    private Vector2 WorldToLocalAnchoredPosition(RectTransform parent, Vector3 worldPos)
    {
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(null, worldPos);
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, screenPoint, null, out localPoint);
        return localPoint;
    }

    private IEnumerator AnimateBallToSlot(Ball ball, RectTransform targetSlot, RectTransform panel, float duration)
    {
        if (ball == null || targetSlot == null || panel == null)
            yield break;

        RectTransform ballRT = ball.GetComponent<RectTransform>();
        if (ballRT == null)
            yield break;

        // compute start and target anchored positions relative to the panel
        Vector2 startAnch = WorldToLocalAnchoredPosition(panel, ballRT.position);
        Vector2 targetAnch = WorldToLocalAnchoredPosition(panel, targetSlot.position);

        // parent ball under panel to animate in same coordinate space (preserve world position)
        ballRT.SetParent(panel, true);
        ballRT.anchoredPosition = startAnch;

        float t = 0f;
        while (t < duration)
        {
            ballRT.anchoredPosition = Vector2.Lerp(startAnch, targetAnch, t / duration);
            t += Time.deltaTime;
            yield return null;
        }

        ballRT.anchoredPosition = targetAnch;
        yield return null;
    }

    public bool CheckWin()
        {
        foreach (var tube in tubes)
        {
            if (tube.balls.Count == 0) continue;
            if (!tube.IsComplete()) return false;
        }
        return true;
        }
    private IEnumerator NextLevelDelay(float delay)
    {
        levelLoader.LoadNextLevel();
        yield return new WaitForSeconds(delay);
        winPanel.SetActive(false);

        if (levelLoader != null)
        {
            levelLoader. LoadLevel(levelLoader.currentLevelIndex);
           
        }
        else
        {
            Debug.LogError("❌ levelLoader chưa được gán!");
        }
    }
    public void Replay()
    {
        levelLoader.LoadCurentLevel();
    }

}
