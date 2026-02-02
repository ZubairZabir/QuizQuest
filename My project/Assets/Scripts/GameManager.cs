using UnityEngine;
using TMPro; 

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Player player; 

    public TextMeshProUGUI scoreText;
    
    public GameObject playButton;

    public GameObject gameOver;
    
    private int score;
    private bool isGamePlaying = false;
    private QuizManager quizManager;

    public bool IsGamePlaying => isGamePlaying;

    private void Awake()
    {
        if (Instance != null) {
            DestroyImmediate(gameObject);
        } else {
            Instance = this;
        }
        Pause();
    }

    private void Start()
    {
        // Find QuizManager in the scene
        quizManager = FindObjectOfType<QuizManager>();
    }

    private void OnDestroy()
    {
        if (Instance == this) {
            Instance = null;
        }
    }


    public void Play()
    {
        score = 0;
        scoreText.text = score.ToString();

        playButton.SetActive(false);
        gameOver.SetActive(false);

        Time.timeScale = 1f;
        player.enabled = true;
        isGamePlaying = true;

        Pipes[] pipes = FindObjectsByType<Pipes>(FindObjectsSortMode.None);

        for( int i = 0; i < pipes.Length; i++) {
            Destroy(pipes[i].gameObject);
        }

        // Start quiz loop when game starts
        if (quizManager != null)
        {
            quizManager.StartQuizLoop();
        }
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        player.enabled = false;
        isGamePlaying = false;
    }

    public void GameOver()
    {
        gameOver.SetActive(true);
        playButton.SetActive(true);
        isGamePlaying = false;

        // Stop quiz loop when game is over
        if (quizManager != null)
        {
            quizManager.StopQuizLoop();
        }

        Pause();
    }

    public void IncreaseScore()
    {
        score++;
        if (scoreText != null) {
            scoreText.text = score.ToString();
        }
        
        // Notify QuizManager that a pipe was passed
        if (quizManager != null)
        {
            quizManager.OnPipePassed();
        }
    }

    public int GetScore()
    {
        return score;
    }

}