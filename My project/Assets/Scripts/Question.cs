using System;
using UnityEngine;

[Serializable]
public class Question
{
    [TextArea(2, 4)]
    public string prompt;

    public string optionA;
    public string optionB;
    public string optionC;

    [Range(0, 2)]
    public int correctIndex; 
}
