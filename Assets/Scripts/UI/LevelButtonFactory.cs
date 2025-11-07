using UnityEngine;
using UnityEngine.UI;

public class LevelButtonFactory
{
    private GameObject buttonPrefab;
    private RectTransform container;
    private GameObject playZone;
     private LevelLoaderFromJSON levelLoader;

    public LevelButtonFactory(GameObject buttonPrefab, RectTransform container,
     GameObject playZone, LevelLoaderFromJSON levelLoader)
    {
        this.buttonPrefab = buttonPrefab;
        this.container = container;
        this.playZone = playZone;
        this.levelLoader = levelLoader;
    }

    public LevelButton CreateButton(int levelIndex, bool isUnlocked)
    {
        GameObject buttonObj = GameObject.Instantiate(buttonPrefab, container);
        LevelButton levelButton = buttonObj.GetComponent<LevelButton>();
        
        if (levelButton != null)
        {
            levelButton.Setup(levelIndex, playZone,levelLoader);
            levelButton.UpdateVisuals(isUnlocked);
        }

        return levelButton;
    }
}