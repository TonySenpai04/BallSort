using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Game.Progress;

public class ProgressService : IProgressService
{   
    private readonly IProgressStorage storage;
    private readonly HashSet<int> completed = new HashSet<int>();
    private int totalLevels = 0;

    public int UnlockedLevels { get; private set; } = 1;

    public ProgressService()
    {
        this.storage = new JsonProgressStorage();
    }

    public ProgressService(IProgressStorage storage)
    {
        this.storage = storage;
    }

    public void Initialize(int totalLevels)
    {
        this.totalLevels = totalLevels;
        var data = storage.Load();
        if (data != null)
        {
            UnlockedLevels = Mathf.Clamp(data.unlockedLevels, 1, totalLevels);
            completed.Clear();
            foreach (var idx in data.completedLevels)
            {
                if (idx >= 0 && idx < totalLevels) completed.Add(idx);
            }
        }
        else
        {
            UnlockedLevels = 1;
            completed.Clear();
        }
    }

    public bool IsCompleted(int levelIndex)
    {
        return completed.Contains(levelIndex);
    }

    public void MarkCompleted(int levelIndex)
    {
        if (!completed.Contains(levelIndex)) completed.Add(levelIndex);
        // levelIndex is 0-based; UnlockedLevels stores count (1-based)
        UnlockedLevels = Mathf.Max(UnlockedLevels, levelIndex + 2);
    }

    public void Reset()
    {
        UnlockedLevels = 1;
        completed.Clear();
        Persist();
    }

    public void Persist()
    {
        var data = new ProgressData { unlockedLevels = this.UnlockedLevels, completedLevels = completed.ToList() };
        storage.Save(data);
    }
}
