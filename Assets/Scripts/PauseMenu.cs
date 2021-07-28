using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject menu;
    [SerializeField] private GameUI gameUI;
    private InputManager inputManager;

    private void Start()
    {
        inputManager = GameSystem.InputManager;
        inputManager.PlayerInput.UiActions.PauseMenu.performed += context => IsPaused = !IsPaused;

        IsPaused = false;
    }

    private bool _isPaused;
    private bool IsPaused
    {
        get => _isPaused;
        set
        {
            menu.SetActive(value);
            gameUI.gameObject.SetActive(!value);
            
            
            if (value)
            {
                inputManager.PlayerInput.ItemInteractions.Disable();
                inputManager.PlayerInput.PlayerMovement.Disable();
            }
            else
            {
                inputManager.PlayerInput.ItemInteractions.Enable();
                inputManager.PlayerInput.PlayerMovement.Enable();
            }
    
            inputManager.CursorLocked = !value;
            _isPaused = value;
            GameSystem.OnPause = value;
        }
    }
}
