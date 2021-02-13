using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Random = UnityEngine.Random;

namespace MirrorProject.TestSceneTwo
{
    public class MainNetworkManager : NetworkManager
    {
        [Header("Custom")]
        [SerializeField] Camera mainCam = null;

        private NameInput nameInput;
        //[SerializeField] private Transform playerOneStartTransform = null, playerTwoStartTransform = null;

        public override void Start()
        {
            nameInput = FindObjectOfType<NameInput>();
            base.Start();
            RegisterWeaponPrefab();
        }

        [ContextMenu(nameof(RegisterWeaponPrefab))]
        private void RegisterWeaponPrefab()
        {
            foreach (Item w in GameSystem.Weapons)
            {
                ClientScene.RegisterPrefab(w.gameObject);
            }
        }

        public override void OnStopServer()
        {
            base.OnStopServer();

            //foreach (GameObject player in players)
            //{
            //    Destroy(player);
            //}
            mainCam.gameObject.SetActive(true);
        }

        public override void OnServerConnect(NetworkConnection conn)
        {
            base.OnServerConnect(conn);
            ServerInfo.PlayerData.Add(conn.connectionId, new ServerPlayer{HueShift = Random.Range(0f, 1f)});
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            base.OnClientDisconnect(conn);
            ServerInfo.PlayerData.Remove(conn.connectionId);
        }

        public override void OnStopClient()
        {
            base.OnStopClient();
            mainCam.gameObject.SetActive(true);
            nameInput.gameObject.SetActive(true);
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            nameInput.gameObject.SetActive(false);
        }
    }
}
