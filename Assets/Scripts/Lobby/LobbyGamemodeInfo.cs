using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Lobby
{
    public class LobbyGamemodeInfo : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI descriptionText;


        public Gamemode Gamemode
        {
            set
            {
                Title = value.title;
                Description = value.description;
            }
        }
        
        private string Title
        {
            set => titleText.text = value;
        }
        private string Description
        {
            set => descriptionText.text = value;
        }
    }
}

