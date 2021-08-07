using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultInfo : NetworkBehaviour
{
    [SerializeField] private Image hueImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI killsText;
    [SerializeField] private TextMeshProUGUI deathsText;
    [SerializeField] private TextMeshProUGUI assistsText;
    
    private ResultList _resultList;

    [SyncVar]
    private float _hue;
    public float Hue
    {
        set
        {
            _hue = value;
            hueImage.color = Color.HSVToRGB(value, 1f, 1f);
        }
    }

    [SyncVar]
    private string _name;
    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            nameText.text = value;
        }
    }

    [field: SyncVar(hook = nameof(OnKillsChanged))]
    public int Kills { get; set; } = 0;

    [field: SyncVar(hook = nameof(OnDeathsChanged))]
    public int Deaths { get; set; } = 0;

    [field: SyncVar(hook = nameof(OnAssistsChanged))]
    public int Assists { get; set; } = 0;

    private void OnKillsChanged(int oldKills, int newKills)
    {
        killsText.text = newKills.ToString();
        _resultList.UpdateInfoOrder();
    }

    private void OnDeathsChanged(int oldDeaths, int newDeaths) => deathsText.text = newDeaths.ToString();
    private void OnAssistsChanged(int oldAssists, int newAssists) => assistsText.text = newAssists.ToString();
    public override void OnStartClient()
    {
        base.OnStartClient();
        Debug.Log("ON START CLIENT RES");

        _resultList = FindObjectOfType<ResultList>();
        if(!isServer)
            _resultList.ResultInfos.Add(this);
        transform.SetParent(_resultList.layoutRoot, false);

        hueImage.color = Color.HSVToRGB(_hue, 1f, 1f);
        nameText.text = _name;
    }
}
