using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// ReSharper disable Unity.PerformanceCriticalCodeCameraMain
// ReSharper disable Unity.PerformanceCriticalCodeInvocation

public class GlobalEventSingleton : Singleton<GlobalEventSingleton>
{
    private Camera activeCamera;
    [SerializeField] private GameObject pauseMenu = null;

    private PlayerInput PlayerInput;
    private bool isPaused = false;

    public bool IsPaused
    {
        get => isPaused;
        set
        {
            pauseMenu.SetActive(value);
            GameSystem.PauseStatusChanged.Invoke(value);
            isPaused = value;
        }
    }


    private void Start()
    {
        PlayerInput = new PlayerInput();
        PlayerInput.UiActions.Enable();
        PlayerInput.UiActions.PauseMenu.performed += context => IsPaused = !IsPaused;
        PlayerInput.UiActions.ToggleCursor.performed += context => CursorLocked = !CursorLocked;
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