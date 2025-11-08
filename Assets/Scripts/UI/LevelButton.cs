using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(Image))]
public class LevelButton : MonoBehaviour, IPointerClickHandler
{
    public Sprite lockedSprite;    // Sprite khi level chưa mở khóa
    public Sprite unlockedSprite;  // Sprite khi level đã mở khóa
    public TextMeshProUGUI levelText;
    
    private Image buttonImage;
    public int levelIndex;
    private bool isLocked;
    private LevelLoaderFromJSON levelLoader;
    private GameObject playZone; // Sẽ tìm dynamic

    public void Setup(int index, GameObject playZoneRef,LevelLoaderFromJSON levelLoader)
    {
        levelIndex = index;
        if (levelText != null)
        {
            levelText.SetText("Level "+ (index + 1).ToString());
        }
        this.levelLoader = levelLoader;
        playZone = playZoneRef;
    }

    private void Awake()
    {
        buttonImage = GetComponent<Image>();
    }

    public void UpdateVisuals(bool unlocked)
    {
        isLocked = !unlocked;
        
        if (buttonImage != null)
        {
            buttonImage.sprite = unlocked ? unlockedSprite : lockedSprite;
            if (levelText != null)
            {
                levelText.color = unlocked ? Color.white : Color.gray;
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isLocked || levelLoader == null) return;

        // Load the selected level
        playZone.SetActive(true);
        levelLoader.currentLevelIndex = levelIndex;
        levelLoader.LoadLevel(levelIndex);
        
        // Hide the level selection UI (assuming it's in a panel)
        Transform levelSelectPanel = transform.parent.parent; // Adjust based on your hierarchy
        if (levelSelectPanel != null)
        {
            levelSelectPanel.gameObject.SetActive(false);
        }
    }
}
