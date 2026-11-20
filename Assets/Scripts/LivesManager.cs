using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LivesManager : MonoBehaviour
{
    public static LivesManager Instance { get; private set; }
    public static event Action<int> OnLivesChanged;

    [Header("Pengaturan")]
    public int maxLives = 3;
    public float invincibleDuration = 1.5f;

    [Header("UI")]
    public GameObject gameOverPanel;

    public int CurrentLives { get; private set; }

    float invincibleTimer;
    bool isGameOver;

    void Awake()
    {
        Instance = this;
        Time.timeScale = 1f;
        CurrentLives = maxLives;

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    void Start()
    {
        OnLivesChanged?.Invoke(CurrentLives);
    }

    void Update()
    {
        if (invincibleTimer > 0f) invincibleTimer -= Time.deltaTime;
    }

    public void LoseLife()
    {
        if (isGameOver || invincibleTimer > 0f) return;

        CurrentLives--;
        invincibleTimer = invincibleDuration;
        OnLivesChanged?.Invoke(CurrentLives);
        Debug.Log("Nyawa tersisa: " + CurrentLives);

        if (CurrentLives <= 0) GameOver();
    }

    void GameOver()
    {
        isGameOver = true;
        AudioManager.PlayGameOver();
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}