using System;
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

        private GlobalEventSingleton _globalEventSingleton;
        // [SerializeField] private float respawnDelay = 3f;

        private void Start()
        {
            _globalEventSingleton = GameSystem.EventSingleton;
        }

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();

            //Camera.main.gameObject.SetActive(false);
            CmdSpawnPlayer(false);
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
                TargetStartSpawnDelay(_globalEventSingleton.serverSettings.respawnTime);
                yield return new WaitForSeconds(_globalEventSingleton.serverSettings.respawnTime);
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
        private void TargetOnPlayerDeath()
        {
            _globalEventSingleton.SetActiveMain();
            CmdSpawnPlayer(true);
        }

        [TargetRpc]
        private void TargetStartSpawnDelay(float delay)
        {
            GameSystem.EventSingleton.Timer.StartTimer(delay);
        }
    }
}
