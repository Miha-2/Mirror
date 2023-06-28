using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class LobbyVoteButton : NetworkBehaviour
{
    [HideInInspector] [SyncVar] public byte buttonId;
    [SerializeField] private TextMeshProUGUI modeText;
    [SerializeField] private RectTransform barTransform;
    private Gamemode gamemode;

    public Gamemode Gamemode
    {
        get => gamemode;
        set
        {
            gamemodeName = value.title;
            gamemode = value;
        }
    }

    [HideInInspector] public UnityEvent<byte> VoteEvent = new UnityEvent<byte>();
    public void Vote() => VoteEvent.Invoke(buttonId);

    [SyncVar(hook = nameof(OnNameChanged))] private string gamemodeName;

    private void OnNameChanged(string oldName, string newName) => modeText.text = newName;

    [SyncVar(hook = nameof(OnPercentageChanged))] private float _percentage;

    public override void OnStartClient() => transform.SetParent(FindObjectOfType<LobbyVoting>().layoutRoot, false);

    public float Percentage
    {
        get => _percentage;
        set
        {
            if(value > 1f || value < 0f)
                Debug.LogError("Button bar percentage error: " + value * 100 + "%");

            //SyncVar
            _percentage = value;
        }
    }
    private void OnPercentageChanged(float oldPercentage, float newPercentage)
    {
        barTransform.gameObject.SetActive(newPercentage != 0f);

        float totalWidth = ((RectTransform)transform).rect.width;

        barTransform.sizeDelta = new Vector2(totalWidth * newPercentage, barTransform.sizeDelta.y);
    }

    [TargetRpc]
    public void SelectionChanged(NetworkConnection conn, bool selected) =>
        modeText.fontStyle = selected ? FontStyles.Underline : FontStyles.Normal;

    [ClientRpc]
    public void SetWinner() => modeText.color = new Color(1f, 0.76f, 0f);
}
