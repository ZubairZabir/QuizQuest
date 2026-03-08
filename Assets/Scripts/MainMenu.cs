using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void LoadCMS121ArraysSorting()
    {
        SceneManager.LoadScene("Flappy Bird");
    }

    public void LoadCMS250Recurrence()
    {
        SceneManager.LoadScene("Flappy Bird 1");
    }

    public void LoadCMS250Recursion()
    {
        SceneManager.LoadScene("Flappy Bird 2");
    }
}