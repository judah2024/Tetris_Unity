using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class GameEvents
{
    public static event UnityAction<int> OnScoreChanged;
    public static event UnityAction<int> OnLinesCleared;
    public static event UnityAction<GameState> OnGameStateChanged;
    public static event UnityAction<Sprite> OnNextBlockChanged;
    
    public static void ScoreChanged(int newScore) => OnScoreChanged?.Invoke(newScore);
    public static void LinesCleared(int lines) => OnLinesCleared?.Invoke(lines);
    public static void GameStateChanged(GameState newState) => OnGameStateChanged?.Invoke(newState);
    public static void NextBlockChanged(Sprite blockSprite) => OnNextBlockChanged?.Invoke(blockSprite);
}

public enum GameState
{
    Play,
    Pause,
    Lose,
    Win
}