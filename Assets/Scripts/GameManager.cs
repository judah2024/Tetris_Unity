using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance => instance;

    int mScore;
    int mLinesCleared;
    GameState mGameState;
    
    float mPlayTimeInSeconds;
    float mTimeStep = 0.1f;
    
    // UI
    public GameObject kGameOverUI;
    public TMP_Text kEndText;
    public TMP_Text kTimeText;
    public TMP_Text kLineText;
    public TMP_Text kScoreText;
    public Image kNextBlockImage;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        ResetGame();
        GameEvents.OnScoreChanged += AddScore;
        GameEvents.OnLinesCleared += AddClearedLines;
        GameEvents.OnGameStateChanged += UpdateGameState;
        GameEvents.OnNextBlockChanged += UpdateNextBlockUI;
    }
    
    private void OnDisable()
    {
        GameEvents.OnScoreChanged -= AddScore;
        GameEvents.OnLinesCleared -= AddClearedLines;
        GameEvents.OnGameStateChanged -= UpdateGameState;
        GameEvents.OnNextBlockChanged -= UpdateNextBlockUI;
    }

    /// <summary>
    /// 게임 경과시간을 증가시킨다.
    /// </summary>
    IEnumerator PlayTimeCoroutine()
    {
        while (mGameState == GameState.Play)
        {
            mPlayTimeInSeconds += mTimeStep;
            TimeSpan timeSpan = TimeSpan.FromSeconds(mPlayTimeInSeconds);
            kTimeText.text = $"{timeSpan.Hours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
            
            yield return new WaitForSeconds(mTimeStep);
        }
    }

    /// <summary>
    /// 게임의 정보를 초기화한다.
    /// </summary>
    public void ResetGame()
    {
        mScore = 0;
        mLinesCleared = 0;
        mGameState = GameState.Play;
        mPlayTimeInSeconds = 0.0f;
        
        // 게임 보드 초기화 로직
        kGameOverUI.SetActive(false);

        StartCoroutine(PlayTimeCoroutine());
    }

    public void UpdateNextBlockUI(Sprite blockSprite)
    {
        kNextBlockImage.sprite = blockSprite;
    }

    public void AddScore(int score)
    {
        mScore += score;
        // UI 업데이트 로직
        kScoreText.text = mScore.ToString();
    }

    public void AddClearedLines(int lines)
    {
        mLinesCleared += lines;
        kLineText.text = mLinesCleared.ToString();
        
        // 게임 클리어 조건에 도달한다면
        if (mLinesCleared >= 40)
        {
            GameEvents.GameStateChanged(GameState.Win);
        }
    }
    
    private void UpdateGameState(GameState newState)
    {
        mGameState = newState;
    }

    public void EndGame()
    {
        kGameOverUI.SetActive(true);
        // 승패에 따라 text 변경
        kEndText.text = mGameState == GameState.Win ? "Game Clear!" : "Game Over...";
        kEndText.color = mGameState == GameState.Win ? Color.blue : Color.red;
    }

    public bool IsWin() => mGameState == GameState.Win;
}
