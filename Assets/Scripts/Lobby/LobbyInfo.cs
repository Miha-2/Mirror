using System;
using DG.Tweening;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

namespace Lobby
{
    public class LobbyInfo : NetworkBehaviour
    {
        [SerializeField] private LocalizedString readyString;
        [SerializeField] private LocalizedString notReadyString;
        [SerializeField] private Image hueImage;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI readyText;
        [SerializeField] private TextMeshProUGUI votedText;

        [field: SyncVar] public float HueInfo { get; set; }

        [field: SyncVar] public string NameInfo { get; set; }


        [HideInInspector] [SyncVar(hook = nameof(OnTeamChanged))]
        public byte Team = 0;

        [SyncVar(hook = nameof(OnVotingChanged))] private bool _isVoting = true;
        public bool IsVoting
        {
            get => _isVoting;
            set
            {
                readyText.gameObject.SetActive(!value);
                votedText.gameObject.SetActive(value);
                _isVoting = value;
            }
        }


#if !UNITY_SERVER
        public override void OnStartServer()
        {
            hueImage.color = Color.HSVToRGB(HueInfo, 1f, 1f);
            nameText.text = NameInfo;

            readyText.text = IsReady ? ready : notReady;
            readyText.color = IsReady ? Color.green : Color.red;
        }
#endif

        private LobbyList _lobbyList;

        private LobbyList LobbyList
        {
            get
            {
                if (_lobbyList == null)
                    _lobbyList = FindObjectOfType<LobbyList>();
                return _lobbyList;
            }
        }

        public override void OnStartClient()
        {
            Transform root;
            
            if (!FindObjectOfType<LobbyManager>().gamemode.isTeam)
                root = LobbyList.layoutRoot;
            else
                root = Team == 1 ? LobbyList.team1.Layout : LobbyList.team2.Layout;

            Gamemode gm = FindObjectOfType<LobbyManager>().gamemode;
            Debug.Log(root + " + "  + gm.isTeam + " -> " + gm.title);
            transform.SetParent(root, false);

            hueImage.color = Color.HSVToRGB(HueInfo, 1f, 1f);
            nameText.text = NameInfo;
            
            readyText.gameObject.SetActive(!IsVoting);
            votedText.gameObject.SetActive(IsVoting);

            readyText.text = IsReady ? ready : notReady;
            readyText.color = IsReady ? Color.green : Color.red;

            votedText.color = IsVoted ? Color.green : Color.red;
        }

        #region Ready Status
        [SyncVar(hook = nameof(OnReady))] private bool _isReady;
        public bool IsReady
        {
            get => _isReady;
            set
            {
#if !UNITY_SERVER
                OnReady(_isReady, value);
#endif
                _isReady = value;
            }
        }

        private void OnReady(bool oldReady, bool newReady)
        {
            if (oldReady != newReady)
                readyText.transform.DOScale(new Vector3(1f, 0f, 1f), .15f).onComplete += ONCompleteReady;
        }

        private void ONCompleteReady()
        {
            readyText.text = IsReady ? ready : notReady;
            readyText.color = IsReady ? Color.green : Color.red;

            readyText.transform.DOScale(Vector3.one, .15f);
        }        

        #endregion

        #region Voted Status
        [SyncVar(hook = nameof(OnVoted))] private bool _isVoted;

        public bool IsVoted
        {
            get => _isVoted;
            set
            {
#if !UNITY_SERVER
                // OnReady(_isVoted, value);
#endif
                _isVoted = value;
            }
        }

        private void OnVoted(bool oldVoted, bool newVoted)
        {
            if (oldVoted != newVoted)
                votedText.transform.DOScale(new Vector3(1f, 0f, 1f), .15f).onComplete += ONCompleteVoted;
        }

        private void ONCompleteVoted()
        {
            votedText.text = IsVoted ? "Voted" : "Not voted";
            votedText.color = IsVoted ? Color.green : Color.red;

            votedText.transform.DOScale(Vector3.one, .15f);
        }

        #endregion

        private void OnTeamChanged(byte oldTeam, byte newTeam)
        {
            Transform root = newTeam == 1 ? LobbyList.team1.Layout : LobbyList.team2.Layout;
            transform.SetParent(root, false);
        }
        private void OnVotingChanged(bool oldVoting, bool newVoting)
        {
            readyText.gameObject.SetActive(!newVoting);
            votedText.gameObject.SetActive(newVoting);
        }

        #region Localization

        private string ready = "Ready";
        private string notReady = "Not Ready";

        private void OnEnable()
        {
            readyString.StringChanged += ReadyStringChanged;
            notReadyString.StringChanged += NotReadyStringChanged;
        }

        private void ReadyStringChanged(string value)
        {
            ready = value;
            if (IsReady)
                readyText.text = value;
        }

        private void NotReadyStringChanged(string value)
        {
            notReady = value;
            if (!IsReady)
                readyText.text = value;
        }

        private void OnDisable()
        {
            readyString.StringChanged -= ReadyStringChanged;
            notReadyString.StringChanged -= NotReadyStringChanged;
        }

        #endregion
    }
}