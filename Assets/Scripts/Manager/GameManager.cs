using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("References")]
    public List<Tube> tubes = new List<Tube>();
    [SerializeField] private GameUIManager uiManager;
    [SerializeField] private LevelLoaderFromJSON levelLoader;

    [Header("Animation")]
    public float moveDuration = 0.18f;

    private BallMoveController moveController;
    private GameWinChecker winChecker;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        winChecker = new GameWinChecker(tubes);
        moveController = new BallMoveController(this, moveDuration, OnWinCheck);
    }

    public void SelectBall(Ball ball, Tube tube)
    {
        moveController.SelectBall(ball, tube);
    }

    public void TryMoveSelectedBallTo(Tube targetTube)
    {
        moveController.TryMoveTo(targetTube);
    }

    public void UnselectBall()
    {
        moveController.UnselectBall();
    }

    private void OnWinCheck()
    {
        if (winChecker.CheckWin())
        {
            uiManager.ShowWinPanel(true);

            if (LevelButtonManager.instance != null)
            {
                int completedIndex = levelLoader?.currentLevelIndex ?? 0;
                LevelButtonManager.instance.OnLevelCompleted(completedIndex);
                LevelButtonManager.instance.PersistProgress();
            }
            else
            {
                Debug.LogWarning("⚠ LevelButtonManager not found — progress not saved.");
            }
        }
    }
    // ✅ Kiểm tra có bóng nào đang được chọn không
    public bool HasSelectedBall()
    {
        return moveController != null && moveController.HasSelectedBall();
    }

    // ✅ Kiểm tra xem bóng này có phải bóng đang được chọn không
    public bool IsSelectedBall(Ball ball)
    {
        return moveController != null && moveController.IsSelectedBall(ball);
    }

}
