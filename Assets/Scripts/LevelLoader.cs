using System.Collections.Generic;
using UnityEngine;

public class LevelLoaderFromJSON : MonoBehaviour
{
    public GameObject tubePrefab;
    public GameObject ballPrefab;
    public Transform tubeParent;

    private LevelJsonWrapper levelWrapper;
    private Dictionary<string, Color> colorMap = new Dictionary<string, Color>();

    public int currentLevelIndex = 0;
    public TextAsset jsonText;

    void Awake()
    {
        // Map tên màu sang Color
        colorMap["red"] = Color.red;
        colorMap["blue"] = Color.blue;
        colorMap["green"] = Color.green;
        colorMap["yellow"] = Color.yellow;
        colorMap["purple"] = new Color(0.6f, 0f, 0.8f);
        colorMap["orange"] = new Color(1f, 0.5f, 0f);
        currentLevelIndex = PlayerPrefs.GetInt("current_level", 0);

        LoadJSON();
    }

    private void Start()
    {
        LoadLevel(currentLevelIndex);
    }

    void LoadJSON()
    {
        if (jsonText == null)
        {
            Debug.LogError("❌ jsonText chưa được gán!");
            return;
        }

        levelWrapper = JsonUtility.FromJson<LevelJsonWrapper>(jsonText.text);

        if (levelWrapper == null || levelWrapper.levels == null)
        {
            Debug.LogError("❌ Không parse được JSON hoặc levels null");
        }
        else
        {
            Debug.Log("✅ Đã load JSON: " + levelWrapper.levels.Count + " levels");
        }
    }

    public void LoadLevel(int index)
    {
        if (levelWrapper == null || levelWrapper.levels == null)
        {
            Debug.LogError("❌ levelWrapper hoặc levels null.");
            return;
        }

        if (index < 0 || index >= levelWrapper.levels.Count)
        {
            Debug.LogError($"❌ Level index {index} vượt giới hạn. Tổng level: {levelWrapper.levels.Count}");
            return;
        }

        var data = levelWrapper.levels[index];
        if (data == null)
        {
            Debug.LogError($"❌ Level {index} = null");
            return;
        }

        if (data.tubes == null)
        {
            Debug.LogError($"❌ Level {index} tubes = null");
            return;
        }

        Debug.Log($"✅ Load level {index}, số tube: {data.tubes.Count}");

        ClearLevel();

        float spacing = 3f;
        int columns = 3;
        int totalTubes = data.tubes.Count;
        int rows = Mathf.CeilToInt((float)totalTubes / columns);
        float yOffset = -(rows - 1) * 1f;
  
        for (int i = 0; i < data.tubes.Count; i++)
        {
            int col = i % 3;
            int row = i / 3;
            Vector3 pos = new Vector3(col * spacing, row *5f+ yOffset, 0);

            GameObject tubeObj = Instantiate(tubePrefab, pos, Quaternion.identity, tubeParent);
            Tube tube = tubeObj.GetComponent<Tube>();
            if (tube == null)
            {
                Debug.LogError("❌ Prefab tube thiếu script Tube.cs");
                continue;
            }

            GameManager.instance.tubes.Add(tube);

            foreach (string colorName in data.tubes[i].colors)
            {
                if (string.IsNullOrEmpty(colorName))
                {
                    Debug.LogWarning($"⚠️ tube {i}: màu null hoặc rỗng, bỏ qua");
                    continue;
                }

                if (!colorMap.ContainsKey(colorName.ToLower()))
                {
                    Debug.LogWarning($"⚠️ tube {i}: màu '{colorName}' không tồn tại trong colorMap");
                    continue;
                }

                GameObject ballObj = Instantiate(ballPrefab);
                Ball ball = ballObj.GetComponent<Ball>();
                if (ball == null)
                {
                    Debug.LogError("❌ Prefab ball thiếu script Ball.cs");
                    continue;
                }

                ball.SetColor(colorMap[colorName.ToLower()]);
                tube.AddBall(ball);
            }

        }

    }

    public void ClearLevel()
    {
        foreach (Transform child in tubeParent)
        {
            Destroy(child.gameObject);
        }
        GameManager.instance.tubes.Clear();
    }

    public void LoadNextLevel()
    {
        currentLevelIndex = (currentLevelIndex + 1) % levelWrapper.levels.Count;
        PlayerPrefs.SetInt("current_level", currentLevelIndex);
        PlayerPrefs.Save();
      
    }
    public void LoadCurentLevel()
    {
        LoadLevel(currentLevelIndex);
    }
}
[System.Serializable]
public class LevelJsonWrapper
{
    public List<LevelJsonData> levels;
}

[System.Serializable]
public class LevelJsonData
{
    public List<TubeData> tubes;
}

[System.Serializable]
public class TubeData
{
    public List<string> colors;
}
