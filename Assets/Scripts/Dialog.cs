using UnityEngine;

public enum TextAlignment { Left = 0, Right = 1 }
[System.Serializable]
public class Dialog 
{
    public string name;
    public Color nameColor = Color.white;
    public TextAlignment nameAlignment;

    [TextArea(3, 10)]
    public string[] sentences;
}
