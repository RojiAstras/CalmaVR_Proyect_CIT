[System.Serializable]
public class Question
{
    public string questionText;
    public string questionType; // "yesno" o "scale"
    public bool useEmojis; // 👈 nuevo campo
    public bool yesNoAnswer;
    public int scaleAnswer;
}

