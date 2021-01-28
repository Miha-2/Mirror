using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

// ReSharper disable Unity.PerformanceCriticalCodeCameraMain
// ReSharper disable Unity.PerformanceCriticalCodeInvocation

public class GlobalEventSingleton : Singleton<GlobalEventSingleton>
{
    private Camera activeCamera;
    [SerializeField] private GameObject pauseMenu = null;
    public Timer Timer = null;

    public PlayerInput playerInput;
    private bool isPaused = false;
    
    //Settings (to struct? later)
    [Header("Settings")]
    public ClientSettings clientSettings;

    public ServerSettings serverSettings;

    public void UpdateDelay(TMP_InputField inputField)
    {
        serverSettings.respawnTime = float.Parse(inputField.text);
    }

    private bool IsPaused
    {
        get => isPaused;
        set
        {
            pauseMenu.SetActive(value);
            // GameSystem.PauseStatusChanged.Invoke(value);
            if (value)
            {
                print("disabling");
                playerInput.ItemInteractions.Disable();
                playerInput.PlayerMovement.Disable();
            }
            else
            {
                print("enabling");
                playerInput.ItemInteractions.Enable();
                playerInput.PlayerMovement.Enable();
            }

            CursorLocked = !value;
            isPaused = value;
            GameSystem.OnPause = value;
        }
    }

    private void Start()
    {
        playerInput = new PlayerInput();
        IsPaused = false;
        
        playerInput.UiActions.Enable();
        playerInput.UiActions.PauseMenu.performed += context => IsPaused = !IsPaused;
        playerInput.UiActions.ToggleCursor.performed += context => CursorLocked = !CursorLocked;
    }
    private bool _cursorLocked;
    
    public bool CursorLocked
    {
        get => _cursorLocked;
        set
        {
            Cursor.lockState = value ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !value;
            _cursorLocked = value;
        }
    }

    private void Update()
    {
        CheckCamera();
    }

    private void CheckCamera()
    {
        if(activeCamera == null && !Camera.current)
            activeCamera = Camera.main;
        else if (activeCamera == Camera.current) return;
        else if (!Camera.current) return;
        
        activeCamera = Camera.current;
        GameSystem.ActiveCameraChanged.Invoke(activeCamera);
    }
}