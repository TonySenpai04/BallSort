using UnityEngine;
using System.IO;
using System.Collections.Generic;

[System.Serializable]
public class GoldData
{
    public int gold;
    public List<int> rewardedLevels = new List<int>();
}

public class GoldManager : MonoBehaviour
{
    public static GoldManager instance;
    public int currentGold = 0;
    private string saveGoldPath;

    private GoldData goldData = new GoldData();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        saveGoldPath = Path.Combine(Application.persistentDataPath, "player_gold.json");
        LoadGold();
    }

    public void AddGold(int amount)
    {
        currentGold += amount;
        goldData.gold = currentGold;
        SaveGold();
    }

    public bool SpendGold(int amount)
    {
        if (currentGold >= amount)
        {
            currentGold -= amount;
            goldData.gold = currentGold;
            SaveGold();
            return true;
        }
        return false;
    }

    public int GetCurrentGold()
    {
        return currentGold;
    }

    public void SaveGold()
    {
        goldData.gold = currentGold;
        string json = JsonUtility.ToJson(goldData, true);
        File.WriteAllText(saveGoldPath, json);
    }

    public void LoadGold()
    {
        if (File.Exists(saveGoldPath))
        {
            string json = File.ReadAllText(saveGoldPath);
            goldData = JsonUtility.FromJson<GoldData>(json);
            currentGold = goldData.gold;
        }
        else
        {
            goldData = new GoldData();
            currentGold = 0;
            SaveGold();
        }
    }

    // ✅ Kiểm tra level đã được thưởng chưa
    public bool HasReceivedReward(int levelIndex)
    {
        return goldData.rewardedLevels.Contains(levelIndex);
    }

    // ✅ Đánh dấu level đã nhận thưởng
    public void MarkLevelRewarded(int levelIndex)
    {
        if (!goldData.rewardedLevels.Contains(levelIndex))
        {
            goldData.rewardedLevels.Add(levelIndex);
            SaveGold();
        }
    }
}
