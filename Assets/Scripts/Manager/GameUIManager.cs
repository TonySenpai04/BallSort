using TMPro;
using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    [SerializeField] private GameObject winPanel;
    [SerializeField] private TextMeshProUGUI levelTxt;
    [SerializeField] private LevelLoaderFromJSON levelLoader;

    private void Update()
    {
        if (levelLoader != null && levelTxt != null)
        {
            int level = levelLoader.currentLevelIndex + 1;
            levelTxt.text = $"Level {level}";
        }
    }

    public void ShowWinPanel(bool active) => winPanel?.SetActive(active);

    public void Replay() => levelLoader?.LoadCurentLevel();

    public void LoadNextLevel()
    {
        winPanel?.SetActive(false);
        levelLoader?.LoadNextLevel();
    }
}
