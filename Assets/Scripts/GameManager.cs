using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public List<Tube> tubes = new List<Tube>();

    [SerializeField]  private Ball selectedBall = null;
    [SerializeField] private Tube fromTube = null;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private LevelLoaderFromJSON levelLoader;

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
        if (selectedBall == null || fromTube == null) return;

        // ❌ Không cho thả vào chính tube cũ
        if (targetTube == fromTube)
        {
            UnselectBall(); // trả về vị trí cũ
            return;
        }

        Ball currentBall = selectedBall;

        if (!targetTube.CanAddBall(currentBall))
        {
            selectedBall.ReturnToOriginalPosition();
            selectedBall = null;
            fromTube = null;
            SoundManager.instance?.PlayFail();
            return;
        }

        // ✅ Chuyển các bóng liên tiếp cùng màu
        while (currentBall != null && targetTube.CanAddBall(currentBall))
        {
            fromTube.RemoveTopBall();
            targetTube.AddBall(currentBall);
            SoundManager.instance?.PlayDrop();
            Ball nextBall = fromTube.PeekTopBall();

            if (nextBall != null && nextBall.ballColor == currentBall.ballColor)
            {
                currentBall = nextBall;
            }
            else
            {
                currentBall = null;
            }
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
