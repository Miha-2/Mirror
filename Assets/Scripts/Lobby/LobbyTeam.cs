using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby
{
    public class LobbyTeam : MonoBehaviour
    {
        [SerializeField] private Image image;

        public float Hue
        {
            set
            {
                Color hsv = Color.HSVToRGB(value, 1f, 1f);
                image.color = new Color(hsv.r, hsv.g, hsv.b, .67f);
            }
        }

        public Transform Layout;
    }
}
