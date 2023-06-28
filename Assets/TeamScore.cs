using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TeamScore : NetworkBehaviour
{
    [SerializeField] private bool team1;
    [SerializeField] private TextMeshProUGUI scoreText;

    [SyncVar] private float _hue = 0f;
    [SyncVar] private bool _isActive;

    [SyncVar(hook = nameof(OnScoreChanged))]
    private int _score;

    public override void OnStartServer()
    {
        if(!ServerInfo.Gamemode.isTeam)
           return;
        _hue = ServerInfo.TeamHue[team1];
        Score = 0;
        
        Color hsv = Color.HSVToRGB(_hue, 1f, 1f);
        GetComponent<Image>().color = new Color(hsv.r, hsv.g, hsv.b, .5f);
        
        _isActive = ServerInfo.Gamemode.isTeam;
        gameObject.SetActive(_isActive);
    }
    public override void OnStartClient()
    {
        Color hsv = Color.HSVToRGB(_hue, 1f, 1f);
        GetComponent<Image>().color = new Color(hsv.r, hsv.g, hsv.b, .5f);

        scoreText.text = _score.ToString();
        
        gameObject.SetActive(_isActive);
    }

    public int Score
    {
        set
        {
            _score = value;
            scoreText.text = value.ToString();
        }
    }

    private void OnScoreChanged(int oldScore, int newScore) => scoreText.text = newScore.ToString();
}
