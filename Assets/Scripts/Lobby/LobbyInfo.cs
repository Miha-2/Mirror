using DG.Tweening;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyInfo : NetworkBehaviour
{
    [SerializeField] private Image hueImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI readyText;

    [field: SyncVar]
    public float HueInfo { get; set; }

    [field: SyncVar]
    public string NameInfo { get; set; }

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

#if !UNITY_SERVER
    public override void OnStartServer()
    {
        hueImage.color = Color.HSVToRGB(HueInfo, 1f, 1f);
        nameText.text = NameInfo;
        
        readyText.text = IsReady ? "Ready" : "Not Ready";
        readyText.color = IsReady ? Color.green : Color.red;
    }
#endif

    public override void OnStartClient()
    {
        transform.SetParent(FindObjectOfType<LobbyList>().layoutRoot,false);
        hueImage.color = Color.HSVToRGB(HueInfo, 1f, 1f);
        nameText.text = NameInfo;
        
        readyText.text = IsReady ? "Ready" : "Not Ready";
        readyText.color = IsReady ? Color.green : Color.red;
    }
    private void OnReady(bool oldReady, bool newReady)
    {
        Debug.Log(nameof(oldReady) + " = " + oldReady + " --- " + nameof(newReady) + " = " + newReady);
        if (oldReady != newReady)
            readyText.transform.DOScale(new Vector3(1f, 0f, 1f), .15f).onComplete += ONComplete;
    }
    private void ONComplete()
    {
        readyText.text = IsReady ? "Ready" : "Not Ready";
        readyText.color = IsReady ? Color.green : Color.red;

        readyText.transform.DOScale(Vector3.one, .15f);
    }

}