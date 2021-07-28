﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace MirrorProject.TestSceneTwo
{
    public class PlayerObject : NetworkBehaviour
    {
        [SerializeField] private PlayerState playerPrefab = null;
        private PlayerState player;
        private GameUI gameUI;

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();

            CmdSendPlayerData(MenuInfo.PlayerName, MenuInfo.Hue);
            CmdSpawnPlayer(false);
        }
        public override void OnStartServer() => ServerInfo.AddChat.AddListener(delegate(string arg0) { TargetRpcUpdateChat(connectionToClient, arg0); });
        public override void OnStartAuthority() => gameUI = FindObjectOfType<GameUI>();

        [Command]
        private void CmdSendPlayerData(string playerName, float hue)
        {
            ServerInfo.PlayerData[connectionToClient.connectionId] = new ServerPlayer {PlayerName = playerName, Hue = hue};
        }

        [TargetRpc]
        private void TargetRpcUpdateChat(NetworkConnection target, string newLine)
        {
            FindObjectOfType<Chat>().AddLine(newLine);
        }

        [Command]
        private void CmdSpawnPlayer(bool delay)
        {
            StartCoroutine(SpawnPlayer(delay));
        }

        private IEnumerator SpawnPlayer(bool delay)
        {
            if (delay)
            {
                float respawnDelay = 3f;
                TargetStartSpawnDelay(respawnDelay);
                yield return new WaitForSeconds(respawnDelay);
            }
            
            player = Instantiate(playerPrefab, transform.position, transform.rotation);
            player.PlayerDeath.AddListener(OnPlayerDeath);

            NetworkServer.Spawn(player.gameObject, connectionToClient);
        }

        private void OnPlayerDeath()
        {
            player.PlayerDeath.RemoveListener(OnPlayerDeath);
            TargetOnPlayerDeath();
        }
        
        [TargetRpc]
        private void TargetOnPlayerDeath() => CmdSpawnPlayer(true);

        [TargetRpc]
        private void TargetStartSpawnDelay(float delay) => gameUI.Timer.StartTimer(delay);
    }
}
