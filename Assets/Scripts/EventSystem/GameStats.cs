using EventSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameStats
{
    public static int pedestriansPickpocketed { get; set; }
    public static int pickpocketedValue { get; set; }
    public static float GameDuration { get; set; }
    public static GameOverReason GameOverReason { get; set; }
    private static TickSystem tickSystem;

    public static void SetTickSystem(TickSystem tick)
    {
        tickSystem = tick;
    }

    public static void EndStats()
    {
        if (tickSystem != null)
        {
            GameDuration = (float)tickSystem.GetCurrentTick() / (float)tickSystem.GetTickSpeed();
        }
    }

}