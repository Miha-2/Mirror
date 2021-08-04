using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ResultList : NetworkBehaviour
{
    [SerializeField] private ResultInfo infoPrefab;
    public Transform layoutRoot;
    public ResultInfo AddPlayer(NetworkConnection conn, ServerPlayer serverPlayer)
    {
        ResultInfo resultInfo = Instantiate(infoPrefab, layoutRoot);

        resultInfo.Hue = serverPlayer.Hue;
        resultInfo.Name = serverPlayer.PlayerName;
        
        NetworkServer.Spawn(resultInfo.gameObject, conn);
        
        _resultInfos.Add(resultInfo);
        return resultInfo;
    }

    public Image image;
    
    private readonly List<ResultInfo> _resultInfos = new List<ResultInfo>();

    public void UpdateInfoOrder()
    {
        ResultInfo[] orderedInfos = _resultInfos.OrderBy(x => x.Kills).ToArray();

        for (int i = 0; i < orderedInfos.Length; i++)
        {
            orderedInfos[i].transform.SetSiblingIndex(orderedInfos.Length - 1 - i);
            Debug.Log(orderedInfos[i].Kills);
        }
    }

    [ClientCallback]
    private void Start()
    {
        GameSystem.InputManager.PlayerInput.UiActions.ResultsMenu.performed += ResultsOnPerformed;
        GameSystem.InputManager.PlayerInput.UiActions.ResultsMenu.canceled += ResultsOnPerformed;
    }
    private void ResultsOnPerformed(InputAction.CallbackContext obj) => Activate(obj.performed);

    public void Activate(bool state)
    {
        image.enabled = state;
        layoutRoot.gameObject.SetActive(state);
    }

    [ClientCallback]
    private void OnEnable()
    {
        GameSystem.InputManager.PlayerInput.UiActions.ResultsMenu.performed += ResultsOnPerformed;
        GameSystem.InputManager.PlayerInput.UiActions.ResultsMenu.canceled += ResultsOnPerformed;
    }
    
    [ClientCallback]
    public void OnDisable()
    {
        GameSystem.InputManager.PlayerInput.UiActions.ResultsMenu.performed -= ResultsOnPerformed;
        GameSystem.InputManager.PlayerInput.UiActions.ResultsMenu.canceled -= ResultsOnPerformed;
    }
}
