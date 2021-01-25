using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

namespace MirrorProject.TestSceneTwo
{
    public class MainNetworkManager : NetworkManager
    {
        [Header("Custom")]
        [SerializeField] Camera mainCam = null;
        //[SerializeField] private Transform playerOneStartTransform = null, playerTwoStartTransform = null;

        public override void Start()
        {
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

        public override void OnStopClient()
        {
            base.OnStopClient();
            mainCam.gameObject.SetActive(true);
        }
    }
}
