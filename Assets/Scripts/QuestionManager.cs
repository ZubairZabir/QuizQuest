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
    private TextAlignmentOptions originalAlignment;
    private int correctStreak = 0;

    private void Start()
    {
        remainingQuestions = new List<Question>(questions);
        originalAlignment = questionText.alignment;

        Debug.Log("QuestionManager Start. Buttons: " + answerButtons.Length);

        for (int i = 0; i < answerButtons.Length; i++)
        {
            int index = i;
            if (answerButtons[i] != null)
            {
                Debug.Log("Wiring button index: " + index + " -> " + answerButtons[i].name);
                answerButtons[i].onClick.AddListener(() => Answer(index));
            }
            else
            {
                Debug.Log("Button at index " + i + " is NULL");
            }
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

        Debug.Log("Showing question: " + currentQuestion.questionText);
        Debug.Log("Correct index: " + currentQuestion.correctIndex);

        questionText.alignment = originalAlignment;
        questionText.color = Color.white;
        questionText.text = currentQuestion.questionText;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (answerButtons[i] != null)
                answerButtons[i].gameObject.SetActive(true);
        }

        for (int i = 0; i < answerTexts.Length; i++)
        {
            if (i < currentQuestion.answers.Length)
                answerTexts[i].text = currentQuestion.answers[i];
        }

        if (questionPanel != null)
            questionPanel.SetActive(true);

        Time.timeScale = 0f;
        questionActive = true;
        Debug.Log("questionActive = true");
    }

    private void HideQuestion()
    {
        if (questionPanel != null)
            questionPanel.SetActive(false);
    }

    public void Answer(int index)
    {
        Debug.Log("Answer clicked. Index: " + index + " | questionActive: " + questionActive);

        if (!questionActive) return;

        questionActive = false;

        questionText.alignment = TextAlignmentOptions.Center;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (answerButtons[i] != null)
                answerButtons[i].gameObject.SetActive(false);
        }

        if (index == currentQuestion.correctIndex)
        {
            Debug.Log("Correct clicked");
            correctStreak++;

            if (correctStreak >= 3)
            {
                questionText.text = "Correct! +1 Life!";
                questionText.color = Color.green;
                GameManager.Instance.AddLife();
                correctStreak = 0;
            }
            else
            {
                questionText.text = "Correct!";
                questionText.color = Color.green;
            }

            GameManager.Instance.IncreaseScore();
        }
        else
        {
            Debug.Log("Wrong clicked. Correct answer: " + currentQuestion.answers[currentQuestion.correctIndex]);
            correctStreak = 0;
            GameManager.Instance.DeductLife();

            if (GameManager.Instance.lives <= 0)
            {
                questionText.text = "Wrong! Game Over!\nAnswer: " + currentQuestion.answers[currentQuestion.correctIndex];
            }
            else
            {
                questionText.text = "Wrong! -1 Life!\nAnswer: " + currentQuestion.answers[currentQuestion.correctIndex];
            }
            questionText.color = Color.red;
        }

        StartCoroutine(ResumeAfterFeedback());
    }

    private IEnumerator ResumeAfterFeedback()
    {
        yield return new WaitForSecondsRealtime(2f);
        HideQuestion();

        if (GameManager.Instance.lives <= 0)
        {
            GameManager.Instance.GameOver();
        }
        else
        {
            Time.timeScale = 1f;
        }
    }
}
