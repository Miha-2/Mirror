using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace Lobby
{
    public class LobbyPlayer : NetworkBehaviour
    {
        private LobbyInfo _lobbyInfo;
        public LobbyInfo LobbyInfo => _lobbyInfo; //mb temp

        private bool _isReady;

        public bool IsReady
        {
            get => _isReady;
            set
            {
                if (_isReady != value && isClient)
                    CmdUpdateStatus(value);
                _isReady = value;
            }
        }

        public override void OnStartAuthority()
        {
            FindObjectOfType<LobbyManager>().lobbyButtons.LobbyPlayer = this;
            
            foreach (LobbyVoteButton button in FindObjectsOfType<LobbyVoteButton>())
                button.VoteEvent.AddListener(Vote);
        }

        private void Vote(byte buttonId) => CmdVote(buttonId);

        [Command]
        private void CmdVote(byte voteId) => FindObjectOfType<LobbyVoting>().Vote(connectionToClient, voteId);


        public override void OnStartServer()
        {
            _lobbyInfo = FindObjectOfType<LobbyList>().AddPlayer(connectionToClient);
            _lobbyInfo.IsVoting = FindObjectOfType<LobbyManager>().VotingStage;
        }

        [Command]
        private void CmdUpdateStatus(bool status)
        {
            _isReady = status;
            _lobbyInfo.IsReady = status;
        }

        [Command]
        public void CmdChangeTeam() => FindObjectOfType<LobbyList>().ChangeTeam(_lobbyInfo);
    }
}
