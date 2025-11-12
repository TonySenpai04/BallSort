using UnityEngine;

public class GoldUI : MonoBehaviour
{
    public TMPro.TextMeshProUGUI goldText;
    private void Start()
    {
        UpdateGoldDisplay();
    }
    public void UpdateGoldDisplay()
    {
        if (goldText != null && GoldManager.instance != null)
        {
            goldText.SetText(GoldManager.instance.GetCurrentGold().ToString());
        }
    }
    void FixedUpdate()
    {
        UpdateGoldDisplay();
    }

}
