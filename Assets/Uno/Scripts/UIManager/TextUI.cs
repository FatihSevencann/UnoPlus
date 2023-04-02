using UnityEngine;
using UnityEngine.UI;

public class TextUI : MonoBehaviour
{
    public int lengthLimit = 15;
    public void UpdateText()
    {
        string text = GetComponent<Text>().text;
        if (text.Length > lengthLimit)
        {
            text = text.Substring(0, lengthLimit);
            GetComponent<Text>().text = text;
        }
    }
    
}
