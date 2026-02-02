using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuizManager : MonoBehaviour
{
    [Header("Questions (set Size = 10)")]
    public Question[] questions;

    [Header("UI References")]
    public GameObject quizPanel;
    public TMP_Text questionText;
    public TMP_Text feedbackText;
    public Button answerButtonA;
    public Button answerButtonB;
    public Button answerButtonC;

    [Header("Timing")]
    public float minSecondsBetweenQuizzes = 0f;
    public float maxSecondsBetweenQuizzes = 5f;
    public float cooldownAfterPipe = 0.5f; // Don't show quiz for 0.5 seconds after passing a pipe

    [Header("Player Controller Script (drag Player component here)")]
    public MonoBehaviour playerController;
    
    [Header("Spawner (drag Spawner component here)")]
    public Spawner spawner;

    private Question current;
    private bool quizActive = false;
    private Coroutine quizLoopCoroutine = null;
    private float lastPipePassTime = 0f;

    void Start()
    {
        if (quizPanel != null) quizPanel.SetActive(false);
        if (feedbackText != null) feedbackText.text = "";
        
        // Auto-find Spawner if not assigned
        if (spawner == null)
        {
            spawner = FindObjectOfType<Spawner>();
        }
    }

    public void StartQuizLoop()
    {
        // Stop any existing coroutine
        if (quizLoopCoroutine != null)
        {
            StopCoroutine(quizLoopCoroutine);
        }
        
        lastPipePassTime = 0f;
        
        // Start the quiz loop coroutine - first quiz will appear after random delay (0-5 seconds)
        quizLoopCoroutine = StartCoroutine(QuizLoop());
    }

    public void StopQuizLoop()
    {
        // Stop the coroutine
        if (quizLoopCoroutine != null)
        {
            StopCoroutine(quizLoopCoroutine);
            quizLoopCoroutine = null;
        }
        
        // Hide quiz panel if it's showing
        if (quizPanel != null && quizPanel.activeSelf)
        {
            quizPanel.SetActive(false);
            quizActive = false;
        }
    }

    public void OnPipePassed()
    {
        // Record the time when a pipe was passed (for cooldown)
        lastPipePassTime = Time.realtimeSinceStartup;
    }

    private IEnumerator QuizLoop()
    {
        // Main loop for all quizzes
        while (true)
        {
            // Wait for random time between quizzes (0-5 seconds)
            float wait = Random.Range(minSecondsBetweenQuizzes, maxSecondsBetweenQuizzes);
            yield return new WaitForSecondsRealtime(wait);

            // Check if enough time has passed since last pipe (cooldown)
            float timeSinceLastPipe = Time.realtimeSinceStartup - lastPipePassTime;
            if (timeSinceLastPipe < cooldownAfterPipe)
            {
                // Wait a bit more if we just passed a pipe
                yield return new WaitForSecondsRealtime(cooldownAfterPipe - timeSinceLastPipe);
            }

            // Only show quiz if game is playing and no quiz is currently active
            if (GameManager.Instance != null && 
                GameManager.Instance.IsGamePlaying && 
                !quizActive)
            {
                ShowRandomQuiz();
                
                // Wait for this quiz to be answered before scheduling the next one
                while (quizActive)
                {
                    if (GameManager.Instance == null || !GameManager.Instance.IsGamePlaying)
                    {
                        break;
                    }
                    yield return new WaitForSecondsRealtime(0.05f);
                }
            }
        }
    }

    private void ShowRandomQuiz()
    {
        if (questions == null || questions.Length == 0) return;

        quizActive = true;
        current = questions[Random.Range(0, questions.Length)];

        // Pause game + disable player input and spawner
        Time.timeScale = 0f;
        
        if (playerController != null) 
        {
            playerController.enabled = false;
        }
        
        if (spawner != null)
        {
            spawner.enabled = false;
        }

        // Show UI
        quizPanel.SetActive(true);
        feedbackText.text = "";
        questionText.text = current.prompt;

        // Set button labels
        answerButtonA.GetComponentInChildren<TMP_Text>().text = current.optionA;
        answerButtonB.GetComponentInChildren<TMP_Text>().text = current.optionB;
        answerButtonC.GetComponentInChildren<TMP_Text>().text = current.optionC;

        // Hook up button clicks
        answerButtonA.onClick.RemoveAllListeners();
        answerButtonB.onClick.RemoveAllListeners();
        answerButtonC.onClick.RemoveAllListeners();

        answerButtonA.onClick.AddListener(() => OnAnswer(0));
        answerButtonB.onClick.AddListener(() => OnAnswer(1));
        answerButtonC.onClick.AddListener(() => OnAnswer(2));
    }

    private void OnAnswer(int selectedIndex)
    {
        if (selectedIndex == current.correctIndex)
        {
            feedbackText.text = "Correct";
            ResumeGame();
        }
        else
        {
            feedbackText.text = "Wrong - Try again.";
        }
    }

    private void ResumeGame()
    {
        quizPanel.SetActive(false);
        quizActive = false; // Set this first so the coroutine can continue

        // Only resume game if it's still playing
        if (GameManager.Instance != null && GameManager.Instance.IsGamePlaying)
        {
            Time.timeScale = 1f;
            
            if (playerController != null) 
            {
                playerController.enabled = true;
            }
            
            if (spawner != null)
            {
                spawner.enabled = true;
            }
        }
    }
}
