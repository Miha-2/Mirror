using System.Collections;
using System.Collections.Generic;
using MirrorProject.TestSceneTwo;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.SceneManagement;

namespace Lobby
{
    public class LobbyButtons : MonoBehaviour
    {
        [SerializeField] private LocalizedString readyString;
        [SerializeField] private LocalizedString unReadyString;
        private LobbyPlayer _lobbyPlayer;
        [SerializeField] private TextMeshProUGUI readyText;
        public CustomButton switchTeams;

        public LobbyPlayer LobbyPlayer
        {
            get => _lobbyPlayer;
            set
            {
                readyText.text = value.IsReady ? unReady : ready;
                _lobbyPlayer = value;
            }
        }

        private bool _canSwitch = true;

        public bool CanSwitch
        {
            get => _canSwitch;
            set
            {
                _canSwitch = value;
                switchTeams.IsEnabled = _canSwitch && !LobbyPlayer.IsReady;
            }
        }

        public void ToggleReady()
        {
            LobbyPlayer.IsReady = !LobbyPlayer.IsReady;
            readyText.text = LobbyPlayer.IsReady ? unReady : ready;

            switchTeams.IsEnabled = CanSwitch && !LobbyPlayer.IsReady;
        }

        public void SwitchTeams() => LobbyPlayer.CmdChangeTeam();

        public void Leave() => FindObjectOfType<MainNetworkManager>().StopAny();

        #region Localization

        private string ready = "Ready";
        private string unReady = "Not Ready";

        private void OnEnable()
        {
            readyString.StringChanged += ReadyStringChanged;
            unReadyString.StringChanged += UnReadyStringChanged;
        }

        private void ReadyStringChanged(string value)
        {
            ready = value;
            if (LobbyPlayer == null) return;
            if (!LobbyPlayer.IsReady)
                readyText.text = value;
        }

        private void UnReadyStringChanged(string value)
        {
            unReady = value;
            if (LobbyPlayer == null) return;
            if (LobbyPlayer.IsReady)
                readyText.text = value;
        }

        private void OnDisable()
        {
            readyString.StringChanged -= ReadyStringChanged;
            unReadyString.StringChanged -= UnReadyStringChanged;
        }

        #endregion
    }
}
