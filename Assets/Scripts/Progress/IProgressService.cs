using UnityEngine;

namespace Game.Progress
{
    public interface IProgressService
    {
        int UnlockedLevels { get; }
        void Initialize(int totalLevels);
        bool IsCompleted(int levelIndex);
        void MarkCompleted(int levelIndex);
        void Persist();
        void Reset();
    }
}