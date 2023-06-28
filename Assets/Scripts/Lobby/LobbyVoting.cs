using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Lobby;
using Mirror;
using TMPro;
using Unity.Burst.Intrinsics;
using UnityEngine;
using Random = UnityEngine.Random;

public class LobbyVoting : NetworkBehaviour, IPlayerDisconnect
{
    public Transform layoutRoot;
    public LobbyVoteButton gamemodeVotePrefab;
    //Stores gamemode vote of a player
    private readonly Dictionary<NetworkConnection, LobbyVoteButton> playerVotes = new Dictionary<NetworkConnection, LobbyVoteButton>();
    
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private float votingTime = 30f; //Usually 30f ?
    [SyncVar(hook = nameof(OnTimerChanged))] private float timer;
    private Dictionary<LobbyVoteButton, int> votes
    {
        get
        {
            Dictionary<LobbyVoteButton, int> temp = new Dictionary<LobbyVoteButton, int>();

            foreach (var vote in playerVotes)
            {
                if (temp.ContainsKey(vote.Value))
                    temp[vote.Value] += 1;
                else
                    temp.Add(vote.Value, 1);
            }

            return temp;
        }
    }

    private readonly List<LobbyVoteButton> voteButtons = new List<LobbyVoteButton>();
    
    private bool votingOver;
    
    
    /// <summary>
    /// Responsible for vote button initialization
    /// </summary>
    public void SetVotes()
    {
        timer = votingTime;
        
        List<Gamemode> gamemodes = Resources.LoadAll<Gamemode>("Gamemodes").ToList();
        
        //Trims the list of all possible gamemodes to random 5
        for (int i = 0; i < gamemodes.Count - 5; i++)
        {
            int rnd = Random.Range(0, gamemodes.Count);
            gamemodes.RemoveAt(rnd);
        }

        //Spawns voting buttons (stored in a list) and assigns them button ids
        //Button id equals its position in the button list
        for (int i = 0; i < gamemodes.Count; i++)
        {
            Gamemode gamemode = gamemodes[i];
            LobbyVoteButton voteButton = Instantiate(gamemodeVotePrefab, layoutRoot);
            voteButton.Gamemode = gamemode;
            voteButton.buttonId = (byte)i;
            Debug.Log(gamemode.title);

            voteButtons.Add(voteButton);
            
            NetworkServer.Spawn(voteButton.gameObject);
        }
    }
    
    public void Vote(NetworkConnection voterConn, byte buttonId)
    {
        if(votingOver) return;
        
        LobbyVoteButton voteButton = voteButtons[buttonId];

        bool isVoted = true;
        
        if (playerVotes.ContainsKey(voterConn))
        {
            if (playerVotes[voterConn] == voteButton)
            {
                isVoted = false;
                playerVotes.Remove(voterConn);
                voteButton.SelectionChanged(voterConn, false);
            }
            else
            {
                playerVotes[voterConn].SelectionChanged(voterConn, false);
                playerVotes[voterConn] = voteButton;
                voteButton.SelectionChanged(voterConn, true);
            }
        }
        else
        {
            playerVotes.Add(voterConn, voteButton);
            voteButton.SelectionChanged(voterConn, true);
        }

        //Sets IsVote for only the pressing player
        foreach (LobbyPlayer player in _manager.PlayerList.Where(player => player.connectionToClient == voterConn))
            player.LobbyInfo.IsVoted = isVoted;

        //Update button bars
        foreach (var button in voteButtons)
        {
            int totalVotes = votes.Sum(vote => vote.Value);

            Debug.Log("Updating vote buttons");
            
            if(!votes.ContainsKey(button))
                button.Percentage = 0f;
            else
                button.Percentage = (float)votes[button] / totalVotes;
        }
    }

    private LobbyManager _manager;
    
    [ServerCallback]
    public void ManagerUpdate(LobbyManager manager)
    {
        _manager = manager;
        if (timer > 0f)
            timer -= Time.deltaTime;
        else
        {
            if(votingOver)
                return;
            votingOver = true;
            
            
            //Select winner
            List<LobbyVoteButton> winners;
            if (votes.Count != 0)
            {
                int mostVotes = votes.Aggregate(0, (current, pair) => Mathf.Max(current, pair.Value));
                winners = (from pair in votes where pair.Value == mostVotes select pair.Key).ToList();
            }
            else
                winners = voteButtons;

            LobbyVoteButton winner = winners[Random.Range(0, winners.Count)];
            winner.SetWinner();
            ServerInfo.Gamemode = winner.Gamemode;
            manager.gamemode = winner.Gamemode;

            //Start 5s pause
            manager.Invoke(nameof(manager.StartReadyStage), 5f);
        }
    }

    public void Leave() => FindObjectOfType<MainNetworkManager>().StopAny();
    private void OnTimerChanged(float oldTimer, float newTimer)
    {
        bool isSmall = newTimer <= 5f;
        string format = !isSmall ? "0" : "0.0";
        string time = newTimer.ToString(format, CultureInfo.InvariantCulture);
        timerText.text = time;
        if(isSmall)
            timerText.color = Color.red;
    }

    public void PlayerDisconnected(NetworkConnection conn)
    {
        //If player has voted -> simulate unpressing the vote button
        if(playerVotes.ContainsKey(conn))
            Vote(conn, playerVotes[conn].buttonId);
    }
}
