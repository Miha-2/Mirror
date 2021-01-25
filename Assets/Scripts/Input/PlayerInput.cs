// GENERATED AUTOMATICALLY FROM 'Assets/Scripts/Input/PlayerInput.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @PlayerInput : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerInput()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerInput"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""a11ba36d-54cc-411a-8768-8eafe2fd609d"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""Value"",
                    ""id"": ""6ccbeea2-d6fb-4937-bb5a-878451fe308f"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""ee48c077-9208-4e2c-9cc2-c23d99a6454a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press""
                },
                {
                    ""name"": ""MouseDelta"",
                    ""type"": ""Value"",
                    ""id"": ""75575e85-4452-4cab-b92c-469ab68c0d6d"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ResetPosition"",
                    ""type"": ""Button"",
                    ""id"": ""bd189e9d-4750-4245-8474-eab84ca39599"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WASD"",
                    ""id"": ""9ddbe932-d23d-4904-8eb8-22eff326bba2"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""7ecbe5c9-efce-4873-a771-552e11178a86"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""MouseKeyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""35010490-feda-4ff6-a6a5-e5becc0dca82"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""MouseKeyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""6c72226b-feb3-4aac-abe5-57adc9938b20"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""MouseKeyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""3e00cbb5-5ea3-41bb-872d-c6651085e794"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""MouseKeyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""0ab667f8-954e-4042-8414-3a0cb0786443"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""MouseKeyboard"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""66a1f9d0-1f39-48eb-911e-4c2dc507b3f4"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""MouseKeyboard"",
                    ""action"": ""MouseDelta"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8614ad69-b819-4965-b5fe-b2aed6c06f2a"",
                    ""path"": ""<Keyboard>/l"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""MouseKeyboard"",
                    ""action"": ""ResetPosition"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Item Interactions"",
            ""id"": ""3c829ea7-6c86-401e-956f-8b1ef94c2cb6"",
            ""actions"": [
                {
                    ""name"": ""PrimaryAction"",
                    ""type"": ""Button"",
                    ""id"": ""c8aaaab3-20c3-4f46-abe5-a1250473892f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""SecondaryAction"",
                    ""type"": ""Button"",
                    ""id"": ""e3b13b9e-37e4-4e2b-9d51-6619f623f75e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Reload"",
                    ""type"": ""Button"",
                    ""id"": ""db45e67f-54d9-449d-b93c-21bdbcce7579"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""c594b70c-f762-4cb8-b81c-100922a32a4d"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""MouseKeyboard"",
                    ""action"": ""PrimaryAction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2ecce48d-60ff-4a04-a51f-d7c737379dfc"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""MouseKeyboard"",
                    ""action"": ""SecondaryAction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""55314347-3094-4951-9a01-eebaacf1b6c6"",
                    ""path"": ""<Keyboard>/r"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""MouseKeyboard"",
                    ""action"": ""Reload"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Ui Actions"",
            ""id"": ""459ce1c4-a583-42c7-a0a6-dfd6b0832d59"",
            ""actions"": [
                {
                    ""name"": ""Pause Menu"",
                    ""type"": ""Button"",
                    ""id"": ""b45bf13b-f7ed-4f1a-8a57-40ab03532bc0"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ToggleCursor"",
                    ""type"": ""Button"",
                    ""id"": ""1bdfad27-d66b-46ef-87ce-d626059d097a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""c5dcdf31-036d-4575-ae11-079bd487570f"",
                    ""path"": ""<Keyboard>/p"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""MouseKeyboard"",
                    ""action"": ""Pause Menu"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""cad67777-3cf4-4b7c-aa28-848dfbdf7a5c"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""MouseKeyboard"",
                    ""action"": ""ToggleCursor"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""MouseKeyboard"",
            ""bindingGroup"": ""MouseKeyboard"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_Movement = m_Player.FindAction("Movement", throwIfNotFound: true);
        m_Player_Jump = m_Player.FindAction("Jump", throwIfNotFound: true);
        m_Player_MouseDelta = m_Player.FindAction("MouseDelta", throwIfNotFound: true);
        m_Player_ResetPosition = m_Player.FindAction("ResetPosition", throwIfNotFound: true);
        // Item Interactions
        m_ItemInteractions = asset.FindActionMap("Item Interactions", throwIfNotFound: true);
        m_ItemInteractions_PrimaryAction = m_ItemInteractions.FindAction("PrimaryAction", throwIfNotFound: true);
        m_ItemInteractions_SecondaryAction = m_ItemInteractions.FindAction("SecondaryAction", throwIfNotFound: true);
        m_ItemInteractions_Reload = m_ItemInteractions.FindAction("Reload", throwIfNotFound: true);
        // Ui Actions
        m_UiActions = asset.FindActionMap("Ui Actions", throwIfNotFound: true);
        m_UiActions_PauseMenu = m_UiActions.FindAction("Pause Menu", throwIfNotFound: true);
        m_UiActions_ToggleCursor = m_UiActions.FindAction("ToggleCursor", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Player
    private readonly InputActionMap m_Player;
    private IPlayerActions m_PlayerActionsCallbackInterface;
    private readonly InputAction m_Player_Movement;
    private readonly InputAction m_Player_Jump;
    private readonly InputAction m_Player_MouseDelta;
    private readonly InputAction m_Player_ResetPosition;
    public struct PlayerActions
    {
        private @PlayerInput m_Wrapper;
        public PlayerActions(@PlayerInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement => m_Wrapper.m_Player_Movement;
        public InputAction @Jump => m_Wrapper.m_Player_Jump;
        public InputAction @MouseDelta => m_Wrapper.m_Player_MouseDelta;
        public InputAction @ResetPosition => m_Wrapper.m_Player_ResetPosition;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
            {
                @Movement.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMovement;
                @Movement.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMovement;
                @Movement.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMovement;
                @Jump.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
                @MouseDelta.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMouseDelta;
                @MouseDelta.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMouseDelta;
                @MouseDelta.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMouseDelta;
                @ResetPosition.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnResetPosition;
                @ResetPosition.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnResetPosition;
                @ResetPosition.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnResetPosition;
            }
            m_Wrapper.m_PlayerActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @MouseDelta.started += instance.OnMouseDelta;
                @MouseDelta.performed += instance.OnMouseDelta;
                @MouseDelta.canceled += instance.OnMouseDelta;
                @ResetPosition.started += instance.OnResetPosition;
                @ResetPosition.performed += instance.OnResetPosition;
                @ResetPosition.canceled += instance.OnResetPosition;
            }
        }
    }
    public PlayerActions @Player => new PlayerActions(this);

    // Item Interactions
    private readonly InputActionMap m_ItemInteractions;
    private IItemInteractionsActions m_ItemInteractionsActionsCallbackInterface;
    private readonly InputAction m_ItemInteractions_PrimaryAction;
    private readonly InputAction m_ItemInteractions_SecondaryAction;
    private readonly InputAction m_ItemInteractions_Reload;
    public struct ItemInteractionsActions
    {
        private @PlayerInput m_Wrapper;
        public ItemInteractionsActions(@PlayerInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @PrimaryAction => m_Wrapper.m_ItemInteractions_PrimaryAction;
        public InputAction @SecondaryAction => m_Wrapper.m_ItemInteractions_SecondaryAction;
        public InputAction @Reload => m_Wrapper.m_ItemInteractions_Reload;
        public InputActionMap Get() { return m_Wrapper.m_ItemInteractions; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(ItemInteractionsActions set) { return set.Get(); }
        public void SetCallbacks(IItemInteractionsActions instance)
        {
            if (m_Wrapper.m_ItemInteractionsActionsCallbackInterface != null)
            {
                @PrimaryAction.started -= m_Wrapper.m_ItemInteractionsActionsCallbackInterface.OnPrimaryAction;
                @PrimaryAction.performed -= m_Wrapper.m_ItemInteractionsActionsCallbackInterface.OnPrimaryAction;
                @PrimaryAction.canceled -= m_Wrapper.m_ItemInteractionsActionsCallbackInterface.OnPrimaryAction;
                @SecondaryAction.started -= m_Wrapper.m_ItemInteractionsActionsCallbackInterface.OnSecondaryAction;
                @SecondaryAction.performed -= m_Wrapper.m_ItemInteractionsActionsCallbackInterface.OnSecondaryAction;
                @SecondaryAction.canceled -= m_Wrapper.m_ItemInteractionsActionsCallbackInterface.OnSecondaryAction;
                @Reload.started -= m_Wrapper.m_ItemInteractionsActionsCallbackInterface.OnReload;
                @Reload.performed -= m_Wrapper.m_ItemInteractionsActionsCallbackInterface.OnReload;
                @Reload.canceled -= m_Wrapper.m_ItemInteractionsActionsCallbackInterface.OnReload;
            }
            m_Wrapper.m_ItemInteractionsActionsCallbackInterface = instance;
            if (instance != null)
            {
                @PrimaryAction.started += instance.OnPrimaryAction;
                @PrimaryAction.performed += instance.OnPrimaryAction;
                @PrimaryAction.canceled += instance.OnPrimaryAction;
                @SecondaryAction.started += instance.OnSecondaryAction;
                @SecondaryAction.performed += instance.OnSecondaryAction;
                @SecondaryAction.canceled += instance.OnSecondaryAction;
                @Reload.started += instance.OnReload;
                @Reload.performed += instance.OnReload;
                @Reload.canceled += instance.OnReload;
            }
        }
    }
    public ItemInteractionsActions @ItemInteractions => new ItemInteractionsActions(this);

    // Ui Actions
    private readonly InputActionMap m_UiActions;
    private IUiActionsActions m_UiActionsActionsCallbackInterface;
    private readonly InputAction m_UiActions_PauseMenu;
    private readonly InputAction m_UiActions_ToggleCursor;
    public struct UiActionsActions
    {
        private @PlayerInput m_Wrapper;
        public UiActionsActions(@PlayerInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @PauseMenu => m_Wrapper.m_UiActions_PauseMenu;
        public InputAction @ToggleCursor => m_Wrapper.m_UiActions_ToggleCursor;
        public InputActionMap Get() { return m_Wrapper.m_UiActions; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(UiActionsActions set) { return set.Get(); }
        public void SetCallbacks(IUiActionsActions instance)
        {
            if (m_Wrapper.m_UiActionsActionsCallbackInterface != null)
            {
                @PauseMenu.started -= m_Wrapper.m_UiActionsActionsCallbackInterface.OnPauseMenu;
                @PauseMenu.performed -= m_Wrapper.m_UiActionsActionsCallbackInterface.OnPauseMenu;
                @PauseMenu.canceled -= m_Wrapper.m_UiActionsActionsCallbackInterface.OnPauseMenu;
                @ToggleCursor.started -= m_Wrapper.m_UiActionsActionsCallbackInterface.OnToggleCursor;
                @ToggleCursor.performed -= m_Wrapper.m_UiActionsActionsCallbackInterface.OnToggleCursor;
                @ToggleCursor.canceled -= m_Wrapper.m_UiActionsActionsCallbackInterface.OnToggleCursor;
            }
            m_Wrapper.m_UiActionsActionsCallbackInterface = instance;
            if (instance != null)
            {
                @PauseMenu.started += instance.OnPauseMenu;
                @PauseMenu.performed += instance.OnPauseMenu;
                @PauseMenu.canceled += instance.OnPauseMenu;
                @ToggleCursor.started += instance.OnToggleCursor;
                @ToggleCursor.performed += instance.OnToggleCursor;
                @ToggleCursor.canceled += instance.OnToggleCursor;
            }
        }
    }
    public UiActionsActions @UiActions => new UiActionsActions(this);
    private int m_MouseKeyboardSchemeIndex = -1;
    public InputControlScheme MouseKeyboardScheme
    {
        get
        {
            if (m_MouseKeyboardSchemeIndex == -1) m_MouseKeyboardSchemeIndex = asset.FindControlSchemeIndex("MouseKeyboard");
            return asset.controlSchemes[m_MouseKeyboardSchemeIndex];
        }
    }
    public interface IPlayerActions
    {
        void OnMovement(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnMouseDelta(InputAction.CallbackContext context);
        void OnResetPosition(InputAction.CallbackContext context);
    }
    public interface IItemInteractionsActions
    {
        void OnPrimaryAction(InputAction.CallbackContext context);
        void OnSecondaryAction(InputAction.CallbackContext context);
        void OnReload(InputAction.CallbackContext context);
    }
    public interface IUiActionsActions
    {
        void OnPauseMenu(InputAction.CallbackContext context);
        void OnToggleCursor(InputAction.CallbackContext context);
    }
}
