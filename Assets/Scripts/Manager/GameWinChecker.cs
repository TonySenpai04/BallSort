using System.Collections.Generic;
using UnityEngine;

public class GameWinChecker
{
    private readonly List<Tube> tubes;

    public GameWinChecker(List<Tube> tubes)
    {
        this.tubes = tubes;
    }

    public bool CheckWin()
    {
        foreach (var tube in tubes)
        {
            if (tube.balls.Count == 0) continue;
            if (!tube.IsComplete()) return false;
        }
        return true;
    }
}
