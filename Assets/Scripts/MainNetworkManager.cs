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
        private NameInput nameInput;

        [Space]
        [SerializeField] private int preSpawnedBullets = 200;
        //[SerializeField] private Transform playerOneStartTransform = null, playerTwoStartTransform = null;

        public override void Start()
        {
            nameInput = FindObjectOfType<NameInput>();
            base.Start();
            RegisterWeaponPrefabs();

            InvokeRepeating(nameof(Beat), 0f,5f);

        }

        [ContextMenu(nameof(RegisterWeaponPrefabs))]
        private void RegisterWeaponPrefabs()
        {
            foreach (Item w in GameSystem.Weapons)
            {
                ClientScene.RegisterPrefab(w.gameObject);
            }
        }

        public override void OnStartServer()
        {
            BulletHole holePrefab = GameSystem.EventSingleton.bulletData.bulletHole;
            Vector3 pos = new Vector3(0f, -10000f, 0f);
            
            for (int i = 0; i < preSpawnedBullets; i++)
            {
                BulletHole bulletHole = Instantiate(holePrefab, pos, Quaternion.identity);
                NetworkServer.Spawn(bulletHole.gameObject);
                bulletHole.gameObject.SetActive(false);
                GameSystem.PreSpawnedBulletHoles.Enqueue(bulletHole);
            }

            Debug.Log(GameSystem.PreSpawnedBulletHoles.Count + " Bullet Holes!");
        }

        public override void OnServerConnect(NetworkConnection conn)
        {
            base.OnServerConnect(conn);
            ServerInfo.PlayerData.Add(conn.connectionId, new ServerPlayer{HueShift = Random.Range(0f, 1f)});
        }

        private void Beat() => Debug.Log("Server status: " + (isNetworkActive ? "active" : "inactive") + $" with {FindObjectOfType<FPSDisplay>().FPS} fps!");

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            base.OnServerDisconnect(conn);
            ServerInfo.PlayerData.Remove(conn.connectionId);
        }

        public override void OnStopClient()
        {
            base.OnStopClient();
            nameInput.gameObject.SetActive(true);
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            RegisterWeaponPrefabs();
            nameInput.gameObject.SetActive(false);
        }
    }
}
