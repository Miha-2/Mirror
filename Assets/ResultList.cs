using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using MirrorProject.TestSceneTwo;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ResultList : NetworkBehaviour
{
    [SerializeField] private ResultInfo infoPrefab;
    public Transform layoutRoot;
    public ResultInfo AddPlayer(NetworkConnection conn, PlayerData playerData)
    {
        ResultInfo resultInfo = Instantiate(infoPrefab, layoutRoot);

        resultInfo.Hue = playerData.pHue;
        resultInfo.Name = playerData.pName;
        
        NetworkServer.Spawn(resultInfo.gameObject, conn);
        
        ResultInfos.Add(resultInfo);
        return resultInfo;
    }

    public Image image;
    public List<ResultInfo> ResultInfos { get; } = new List<ResultInfo>();

    public void UpdateInfoOrder()
    {
        ResultInfo[] orderedInfos = ResultInfos.OrderBy(x => x.Kills).ToArray();

        for (int i = 0; i < orderedInfos.Length; i++)
        {
            if (orderedInfos[i] == null)
            {
                ResultInfos.Remove(orderedInfos[i]);
                continue;
            }

            orderedInfos[i].transform.SetSiblingIndex(orderedInfos.Length - 1 - i);
        }
    }

    public override void OnStartClient()
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
