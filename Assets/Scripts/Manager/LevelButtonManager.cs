using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Game.Progress;

public class LevelButtonManager : MonoBehaviour, ILevelButtonManager
{
    [Header("Button Settings")]
    public GameObject levelButtonPrefab;
    public RectTransform buttonContainer;
    public GameObject playZone;

    [Header("Level Progress")]
    [SerializeField] private int totalLevels;
    [SerializeField] private int unlockedLevels = 1;

    private List<LevelButton> levelButtons = new List<LevelButton>();
   [SerializeField] private LevelLoaderFromJSON levelLoader;
    private IProgressService progressService;
    private LevelButtonFactory buttonFactory;
    private ContainerLayoutManager layoutManager;
    public static LevelButtonManager instance;

    void Awake()
    {
        instance = this;
        progressService = new ProgressService();
        buttonFactory = new LevelButtonFactory(levelButtonPrefab, buttonContainer,
        playZone,levelLoader);
        layoutManager = new ContainerLayoutManager(buttonContainer);
    }

    public void InitializeLevelButtons()
    {
        InitializeLevelCount();
        InitializeProgress();
        CreateButtons();
        UpdateAllButtonStates();
    }

    private void InitializeLevelCount()
    {
        if (levelLoader?.LevelWrapper?.levels != null)
        {
            totalLevels = levelLoader.LevelWrapper.levels.Count;
        }
    }

    private void InitializeProgress()
    {
        progressService.Initialize(totalLevels);
        unlockedLevels = progressService.UnlockedLevels;
    }

    private void CreateButtons()
    {
        if (buttonContainer == null) return;

        layoutManager.ClearContainer();
        levelButtons.Clear();

        for (int i = 0; i < totalLevels; i++)
        {
            var button = buttonFactory.CreateButton(i, i < unlockedLevels);
            if (button != null)
            {
                levelButtons.Add(button);
            }
        }

        layoutManager.AdjustSize(levelButtons.Count);
    }

    public void UpdateAllButtonStates()
    {
        for (int i = 0; i < levelButtons.Count; i++)
        {
            if (levelButtons[i] != null)
            {
                bool isUnlocked = i < unlockedLevels;
                levelButtons[i].UpdateVisuals(isUnlocked);
            }
        }
    }

    public void OnLevelCompleted(int levelIndex)
    {
        SetLevelCompleted(levelIndex);

        if (levelIndex + 1 >= unlockedLevels)
        {
            unlockedLevels = Mathf.Max(unlockedLevels, levelIndex + 2);
        }

        UpdateAllButtonStates();
    }

    public void PersistProgress()
    {
        progressService.Persist();
    }
        
    private bool IsLevelCompleted(int levelIndex)
    {
        return progressService.IsCompleted(levelIndex);
    }

    private void SetLevelCompleted(int levelIndex)
    {
        progressService.MarkCompleted(levelIndex);
        unlockedLevels = progressService.UnlockedLevels;
    }

    public void ResetProgress()
    {
        progressService.Reset();
        unlockedLevels = progressService.UnlockedLevels;
        UpdateAllButtonStates();
    }
}