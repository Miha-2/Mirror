using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace MirrorProject.TestSceneTwo
{
    public class PlayerObject : NetworkBehaviour
    {
        [SerializeField] private GameObject playerPrefab = null;
        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();

            //Camera.main.gameObject.SetActive(false);
            CmdSpawnPlayer();
        }

        [Command]
        private void CmdSpawnPlayer()
        {
            GameObject player = Instantiate(playerPrefab, transform.position, transform.rotation);

            NetworkServer.Spawn(player, connectionToClient);
        }
    }
}
