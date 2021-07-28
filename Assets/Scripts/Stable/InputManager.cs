using System;
using TMPro;
using UnityEngine;


public class InputManager : MonoBehaviour
{
    public PlayerInput PlayerInput;

    private void Awake() => PlayerInput = new PlayerInput();

    private void Start()
    {
        CursorLocked = false;
        
        PlayerInput.UiActions.Enable();
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
}