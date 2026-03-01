using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestionManager : MonoBehaviour
{
    [SerializeField] private GameObject questionPanel;
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private TextMeshProUGUI[] answerTexts;
    [SerializeField] private Button[] answerButtons;
    [SerializeField] private List<Question> questions = new List<Question>();

    private List<Question> remainingQuestions = new List<Question>();
    private Question currentQuestion;
    private bool questionActive = false;

    private void Start()
    {
        remainingQuestions = new List<Question>(questions);

        // Wire button clicks automatically
        for (int i = 0; i < answerButtons.Length; i++)
        {
            int index = i;
            if (answerButtons[i] != null)
                answerButtons[i].onClick.AddListener(() => Answer(index));
        }

        HideQuestion();
        StartCoroutine(QuestionCycle());
    }

    private IEnumerator QuestionCycle()
    {
        while (true)
        {
            float waitTime = Random.Range(5f, 12f);
            yield return new WaitForSecondsRealtime(waitTime);

            // Only show question if game is running
            if (Time.timeScale > 0f)
            {
                ShowNextQuestion();

                while (questionActive)
                {
                    yield return null;
                }
            }
        }
    }

    private void ShowNextQuestion()
    {
        if (remainingQuestions.Count == 0)
        {
            remainingQuestions = new List<Question>(questions);
        }

        int randomIndex = Random.Range(0, remainingQuestions.Count);
        currentQuestion = remainingQuestions[randomIndex];
        remainingQuestions.RemoveAt(randomIndex);

        questionText.color = Color.white;
        questionText.text = currentQuestion.questionText;

        for (int i = 0; i < answerTexts.Length; i++)
        {
            if (i < currentQuestion.answers.Length)
                answerTexts[i].text = currentQuestion.answers[i];
        }

        // Show panel and pause game
        if (questionPanel != null)
            questionPanel.SetActive(true);

        Time.timeScale = 0f;
        questionActive = true;
    }

    private void HideQuestion()
    {
        if (questionPanel != null)
            questionPanel.SetActive(false);
    }

    public void Answer(int index)
    {
        if (!questionActive) return;

        questionActive = false;

        if (index == currentQuestion.correctIndex)
        {
            questionText.text = "Correct!";
            questionText.color = Color.green;
            GameManager.Instance.IncreaseScore();
        }
        else
        {
            questionText.text = "Wrong! Answer: " + currentQuestion.answers[currentQuestion.correctIndex];
            questionText.color = Color.red;
        }

        StartCoroutine(ResumeAfterFeedback());
    }

    private IEnumerator ResumeAfterFeedback()
    {
        yield return new WaitForSecondsRealtime(2f);
        HideQuestion();
        Time.timeScale = 1f;
    }
}
