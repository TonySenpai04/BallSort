using System.IO;
using UnityEngine;
using Game.Progress;

namespace Game.Progress
{
    public class JsonProgressStorage : IProgressStorage
    {
        private readonly string path;

        public JsonProgressStorage(string fileName = "progress.json")
        {
            path = Path.Combine(Application.persistentDataPath, fileName);
        }

        public ProgressData Load()
        {
            if (!File.Exists(path)) return null;
            try
            {
                string json = File.ReadAllText(path);
                return JsonUtility.FromJson<ProgressData>(json);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"JsonProgressStorage.Load failed: {ex}");
                return null;
            }
        }

        public void Save(ProgressData data)
        {
            try
            {
                string json = JsonUtility.ToJson(data, true);
                File.WriteAllText(path, json);
                Debug.Log($"Progress saved to JSON: {path}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"JsonProgressStorage.Save failed: {ex}");
            }
        }
    }
}
