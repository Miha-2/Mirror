using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class Chat : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI chatDisplay = null;
    [SerializeField] private int maxLines = 6;
    private Queue<string> chatLines = new Queue<string>();

    private void Start()
    {
        chatDisplay.text = String.Empty;
    }

    public void AddLine(string newLine)
    {
        if (chatLines.Count >= maxLines)
            chatLines.Dequeue();
        chatLines.Enqueue(newLine);

        chatDisplay.text = String.Empty;
        foreach (string line in chatLines)
        {
            chatDisplay.text += line + Environment.NewLine;
        }
    }
}
