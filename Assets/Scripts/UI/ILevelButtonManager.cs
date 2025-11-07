using UnityEngine;

public interface ILevelButtonManager
{
    void InitializeLevelButtons();
    void UpdateAllButtonStates();
    void OnLevelCompleted(int levelIndex);
    void PersistProgress();
    void ResetProgress();
}