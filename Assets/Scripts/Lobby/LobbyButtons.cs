using System.Collections;
using System.Collections.Generic;
using MirrorProject.TestSceneTwo;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyButtons : MonoBehaviour
{
    private LobbyPlayer _lobbyPlayer;
    [SerializeField] private TextMeshProUGUI readyText;

    public LobbyPlayer LobbyPlayer
    {
        get => _lobbyPlayer;
        set
        {
            readyText.text = value.IsReady ? "Unready" : "Ready";
            _lobbyPlayer = value;
        }
    }

    public void ToggleReady()
    {
        LobbyPlayer.IsReady = !LobbyPlayer.IsReady;
        readyText.text = LobbyPlayer.IsReady ? "Unready" : "Ready";
    }

    public void Leave()
    {
        FindObjectOfType<MainNetworkManager>().StopAny();
    }
}
