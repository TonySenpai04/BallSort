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

        if (targetTube.CanAddBall(selectedBall))
        {
            fromTube.RemoveTopBall();
            targetTube.AddBall(selectedBall);

            
            selectedBall = null;
            fromTube = null;

            if (CheckWin())
            {
                winPanel.SetActive(true);
                StartCoroutine(NextLevelDelay(1f)); 
            }
        }
        else
        {
            selectedBall.ReturnToOriginalPosition();
            fromTube = null;
            selectedBall = null;
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
        yield return new WaitForSeconds(delay);
        winPanel.SetActive(false);

        if (levelLoader != null)
        {
            levelLoader.LoadNextLevel();
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
