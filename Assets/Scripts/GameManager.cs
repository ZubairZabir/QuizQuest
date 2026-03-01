using UnityEngine;
using UnityEngine.UI;
using TMPro;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Player player;
    [SerializeField] private Spawner spawner;
    [SerializeField] private Text scoreText;
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject gameOver;
    [SerializeField] private TextMeshProUGUI livesText;

    public int score { get; private set; } = 0;
    public int lives { get; private set; } = 3;

    private void Awake()
    {
        if (Instance != null) {
            DestroyImmediate(gameObject);
        } else {
            Instance = this;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this) {
            Instance = null;
        }
    }

    private void Start()
    {
        Pause();
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        player.enabled = false;
    }

    public void Play()
    {
        playButton.SetActive(false);

        if (lives <= 0)
        {
            lives = 3;
            livesText.text = "Lives: " + lives;

            score = 0;
            scoreText.text = score.ToString();

            gameOver.SetActive(false);

            player.ResetPosition();

            Pipes[] pipes = FindObjectsByType<Pipes>(FindObjectsSortMode.None);
            for (int i = 0; i < pipes.Length; i++) {
                Destroy(pipes[i].gameObject);
            }
        }

        Time.timeScale = 1f;
        player.enabled = true;
    }

    public void GameOver()
    {
        playButton.SetActive(true);
        gameOver.SetActive(true);

        Pause();
    }

    public void LoseLife()
    {
        lives--;
        livesText.text = "Lives: " + lives;

        if (lives <= 0)
        {
            GameOver();
        }
        else
        {
            MovePlayerToSafeSpot();
            PauseAfterHit();
        }
    }

    private void MovePlayerToSafeSpot()
    {
        Pipes closestPipe = FindClosestPipe();

        if (closestPipe != null)
        {
            float middleY = closestPipe.GetMiddleY();
            player.transform.position = new Vector3(
                player.transform.position.x,
                middleY,
                0f
            );
        }

        player.ResetVelocity();
    }

    private Pipes FindClosestPipe()
    {
        Pipes[] pipes = FindObjectsByType<Pipes>(FindObjectsSortMode.None);

        Pipes closest = null;
        float minDistance = Mathf.Infinity;

        foreach (Pipes pipe in pipes)
        {
            float distance = pipe.transform.position.x - player.transform.position.x;

            if (distance > 0 && distance < minDistance)
            {
                minDistance = distance;
                closest = pipe;
            }
        }

        return closest;
    }

    private void PauseAfterHit()
    {
        Time.timeScale = 0f;
        playButton.SetActive(true);
    }

    public void IncreaseScore()
    {
        score++;
        scoreText.text = score.ToString();
    }

}
