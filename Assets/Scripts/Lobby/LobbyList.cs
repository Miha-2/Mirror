using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using Telepathy;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Lobby
{
    public class LobbyList : NetworkBehaviour
    {
        [SerializeField] private LobbyManager lobbyManager;
        public LobbyTeam team1;
        public LobbyTeam team2;
        [SerializeField] private LobbyInfo infoPrefab;
        public List<LobbyInfo> lobbyInfos = new List<LobbyInfo>();
        public Transform layoutRoot;

        [SyncVar]
        private bool isVotingStage = true;
        
        [SyncVar] private float hue1 = -1f;
        [SyncVar] private float hue2 = -1f;

        public override void OnStartClient()
        {
            base.OnStartClient();
            if(!isVotingStage)
                ClientReadyStage(-1f, -1f);
        }

        [ClientRpc]
        private void RpcReadyStage(float h1, float h2) => ClientReadyStage(h1, h2);
        
        private void ClientReadyStage(float h1, float h2)
        {
            team1.Hue = h1 < 0f ? hue1 : h1;
            team2.Hue = h2 < 0f ? hue2 : h2;

            team1.gameObject.SetActive(lobbyManager.gamemode.isTeam);
            team2.gameObject.SetActive(lobbyManager.gamemode.isTeam);
        }

        [ServerCallback]
        public void ServerReadyStage()
        {
            isVotingStage = false;
            if (lobbyManager.gamemode.isTeam)
            {
                ServerInfo.TeamHue = new Dictionary<bool, float>();
                hue1 = Random.Range(0f, 1f);
                team1.Hue = hue1;
                ServerInfo.TeamHue.Add(true, hue1);
                hue2 = (hue1 + .5f) % 1f;
                team2.Hue = hue2;
                ServerInfo.TeamHue.Add(false, hue2);

                team1.gameObject.SetActive(lobbyManager.gamemode.isTeam);
                team2.gameObject.SetActive(lobbyManager.gamemode.isTeam);

                
                for (int i = 0; i < lobbyInfos.Count; i++)
                {
                    LobbyInfo info = lobbyInfos[i];
                    bool _team1 = i % 2 == 0;
                    ServerInfo.PlayerData[info.connectionToClient].team = _team1;
                    info.Team = _team1 ? (byte)1 : (byte)2;
                }
            }
            RpcReadyStage(hue1, hue2);
        }

        [ServerCallback]
        public LobbyInfo AddPlayer(NetworkConnection conn)
        {
            Transform root;
            bool isTeam1 = true;
            // bool isTeam1 = team1.Layout.childCount <= team2.Layout.childCount;

            if (isVotingStage || !lobbyManager.gamemode.isTeam)
                root = layoutRoot;
            else
                root = isTeam1 ? team1.Layout : team2.Layout;

            Debug.Log("Adding player to: " + root);
            
            
            LobbyInfo lobbyInfo = Instantiate(infoPrefab, root);

            lobbyInfo.NameInfo = ServerInfo.PlayerData[conn].pName;
            lobbyInfo.HueInfo = ServerInfo.PlayerData[conn].pHue;
            lobbyInfo.IsReady = false;
            
            lobbyInfos.Add(lobbyInfo);

            if (!isVotingStage && lobbyManager.gamemode.isTeam/* && false*/)
            {
                lobbyInfo.Team = isTeam1 ? (byte)1 : (byte)2;
                ServerInfo.PlayerData[conn].team = isTeam1;
            }

            NetworkServer.Spawn(lobbyInfo.gameObject, conn);

            CanPlayersSwitch();
            return lobbyInfo;
        }

        [ServerCallback]
        public void ChangeTeam(LobbyInfo lobbyInfo)
        {
            if (!lobbyManager.gamemode.isTeam) return;

            if (lobbyInfo.IsReady) return;
            NetworkConnection conn = lobbyInfo.connectionToClient;

            bool isTeam1 = ServerInfo.TeamLayout[conn];

            if (isTeam1)
            {
                if (ServerInfo.Team1.Length == 4)
                    return;
            }
            else
            {
                if (ServerInfo.Team2.Length == 4)
                    return;
            }

            ServerInfo.PlayerData[conn].team = !isTeam1;

            lobbyInfo.Team = ServerInfo.TeamLayout[conn] ? (byte)1 : (byte)2;

            //Updates switch button availability
            CanPlayersSwitch();
        }

        private const int maxTeamSize = 4;

        [ServerCallback]
        private void CanPlayersSwitch()
        {
            //Can switch if team is not full
            Dictionary<bool, bool> teamCanSwitch = new Dictionary<bool, bool>
            {
                {true, ServerInfo.Team2.Length < maxTeamSize}, {false, ServerInfo.Team1.Length < maxTeamSize}
            };

            return;
            foreach (KeyValuePair<NetworkConnection, bool> pair in ServerInfo.TeamLayout)
                TargetCanSwitch(pair.Key, teamCanSwitch[pair.Value]);
        }

        [TargetRpc]
        private void TargetCanSwitch(NetworkConnection target, bool canSwitch) =>
            lobbyManager.lobbyButtons.CanSwitch = canSwitch;
    }
}