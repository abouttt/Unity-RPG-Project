//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.7.0
//     from Assets/Input System/GameControls.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @GameControls: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @GameControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""GameControls"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""23eb4010-77d0-4fa7-875a-67e7e825cbf6"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""3736f2de-668b-4da4-914a-db870f128143"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Look"",
                    ""type"": ""Value"",
                    ""id"": ""3f496c14-c599-4a67-93b4-025a1ccb6da2"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": ""ScaleVector2(x=0.5,y=0.5),InvertVector2(invertX=false)"",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""dfc15563-54ec-4517-aa02-d54a26139fc0"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Sprint"",
                    ""type"": ""PassThrough"",
                    ""id"": ""f10ccb73-3b2e-441b-8aef-32be820f57f1"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Interaction"",
                    ""type"": ""Button"",
                    ""id"": ""f58e6b7d-5ba8-4723-9545-5fcc73dd1c33"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""CursorToggle"",
                    ""type"": ""Button"",
                    ""id"": ""11b76afa-69e7-4a07-ba64-c3800beea9da"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""LockOn"",
                    ""type"": ""Button"",
                    ""id"": ""7459b493-f012-4612-88d0-fb912edce1ad"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Cancel"",
                    ""type"": ""Button"",
                    ""id"": ""760664a7-dbf4-4838-912a-8e7e39bc0172"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""ItemInventory"",
                    ""type"": ""Button"",
                    ""id"": ""e564f4b7-e840-4692-ae12-5602ef2f686f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""EquipmentInventory"",
                    ""type"": ""Button"",
                    ""id"": ""1f0a41f1-ca5f-4ae4-94a4-6b21bafbbeb0"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""SkillTree"",
                    ""type"": ""Button"",
                    ""id"": ""b3c848f4-995a-4b12-826b-9bd5254fdf3f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Quest"",
                    ""type"": ""Button"",
                    ""id"": ""6317b924-7726-49c3-9884-5ed8a698f424"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Quick"",
                    ""type"": ""Button"",
                    ""id"": ""2dfeb728-f92d-4d20-8446-cecaa0ba65a6"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""21adfc22-cf38-4cd1-9a76-6dabe62ab3db"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""43a16564-f89b-40d4-afed-59fd13ca98a6"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard/Mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""39f5ad2b-afd5-4a01-bc18-0511678cfa6b"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard/Mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""793f9d1d-9f65-4bcf-9c0d-a395b6a34660"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard/Mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""8b3012a6-d389-459e-b1d9-d5b2bdf90205"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard/Mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""41d9bc2c-97a0-4bd3-91c5-1d57f91866bf"",
                    ""path"": ""<Pointer>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard/Mouse"",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""abe879d6-79f1-48d4-9490-54d37c16da7c"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard/Mouse"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7cd4b6f9-7c18-4b25-aec6-0a4674403228"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard/Mouse"",
                    ""action"": ""Sprint"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d41eb176-54bb-4d4d-91c1-7327f516a1ef"",
                    ""path"": ""<Keyboard>/tab"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard/Mouse"",
                    ""action"": ""CursorToggle"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""30e43b74-4cfa-4d78-bf0e-917831f132de"",
                    ""path"": ""<Mouse>/middleButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard/Mouse"",
                    ""action"": ""LockOn"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""54b3aacb-6344-438b-b719-da7071ed8a40"",
                    ""path"": ""<Keyboard>/i"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard/Mouse"",
                    ""action"": ""ItemInventory"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d76953dd-725f-4856-9e5e-462a806bb9f2"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard/Mouse"",
                    ""action"": ""Cancel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""28712c1a-8d4c-4072-bc9d-9c71ac2b2c25"",
                    ""path"": ""<Keyboard>/u"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard/Mouse"",
                    ""action"": ""EquipmentInventory"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""023f4445-4e1e-4711-b0d7-c9229135ff69"",
                    ""path"": ""<Keyboard>/1"",
                    ""interactions"": """",
                    ""processors"": ""Scale(factor=0)"",
                    ""groups"": ""Keyboard/Mouse"",
                    ""action"": ""Quick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""698da201-c1ee-40db-8db6-d2994596a926"",
                    ""path"": ""<Keyboard>/2"",
                    ""interactions"": """",
                    ""processors"": ""Scale"",
                    ""groups"": ""Keyboard/Mouse"",
                    ""action"": ""Quick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ee5334d5-24b5-4bea-930b-902cd39cc494"",
                    ""path"": ""<Keyboard>/3"",
                    ""interactions"": """",
                    ""processors"": ""Scale(factor=2)"",
                    ""groups"": ""Keyboard/Mouse"",
                    ""action"": ""Quick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7091e98c-d1a0-4b1c-8394-a1e50b38cc31"",
                    ""path"": ""<Keyboard>/4"",
                    ""interactions"": """",
                    ""processors"": ""Scale(factor=3)"",
                    ""groups"": ""Keyboard/Mouse"",
                    ""action"": ""Quick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4ee8373f-fcaa-4b06-ae20-5cfde215c572"",
                    ""path"": ""<Keyboard>/5"",
                    ""interactions"": """",
                    ""processors"": ""Scale(factor=4)"",
                    ""groups"": ""Keyboard/Mouse"",
                    ""action"": ""Quick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9e8bfa9b-38a6-4e49-bc84-a35c66e237ef"",
                    ""path"": ""<Keyboard>/6"",
                    ""interactions"": """",
                    ""processors"": ""Scale(factor=5)"",
                    ""groups"": ""Keyboard/Mouse"",
                    ""action"": ""Quick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""643ee8bb-7e52-4ce3-9c60-3d079f87ed6c"",
                    ""path"": ""<Keyboard>/7"",
                    ""interactions"": """",
                    ""processors"": ""Scale(factor=6)"",
                    ""groups"": ""Keyboard/Mouse"",
                    ""action"": ""Quick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6d96a762-0db0-4ec4-a40f-9781aa06781e"",
                    ""path"": ""<Keyboard>/8"",
                    ""interactions"": """",
                    ""processors"": ""Scale(factor=7)"",
                    ""groups"": ""Keyboard/Mouse"",
                    ""action"": ""Quick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d810e1d5-42a5-45c1-a394-c68078b362ea"",
                    ""path"": ""<Keyboard>/9"",
                    ""interactions"": """",
                    ""processors"": ""Scale(factor=8)"",
                    ""groups"": ""Keyboard/Mouse"",
                    ""action"": ""Quick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""75b41ba2-bb55-4d1a-9a74-524d1e6eac59"",
                    ""path"": ""<Keyboard>/0"",
                    ""interactions"": """",
                    ""processors"": ""Scale(factor=9)"",
                    ""groups"": ""Keyboard/Mouse"",
                    ""action"": ""Quick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""979aa19c-d115-4498-9004-15def5027d8f"",
                    ""path"": ""<Keyboard>/k"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard/Mouse"",
                    ""action"": ""SkillTree"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ccdd241c-235b-45ed-b7a4-a5997be9d793"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard/Mouse"",
                    ""action"": ""Interaction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1e548e01-1474-4866-96e5-8d177c4996ec"",
                    ""path"": ""<Keyboard>/j"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard/Mouse"",
                    ""action"": ""Quest"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard/Mouse"",
            ""bindingGroup"": ""Keyboard/Mouse"",
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
        m_Player_Move = m_Player.FindAction("Move", throwIfNotFound: true);
        m_Player_Look = m_Player.FindAction("Look", throwIfNotFound: true);
        m_Player_Jump = m_Player.FindAction("Jump", throwIfNotFound: true);
        m_Player_Sprint = m_Player.FindAction("Sprint", throwIfNotFound: true);
        m_Player_Interaction = m_Player.FindAction("Interaction", throwIfNotFound: true);
        m_Player_CursorToggle = m_Player.FindAction("CursorToggle", throwIfNotFound: true);
        m_Player_LockOn = m_Player.FindAction("LockOn", throwIfNotFound: true);
        m_Player_Cancel = m_Player.FindAction("Cancel", throwIfNotFound: true);
        m_Player_ItemInventory = m_Player.FindAction("ItemInventory", throwIfNotFound: true);
        m_Player_EquipmentInventory = m_Player.FindAction("EquipmentInventory", throwIfNotFound: true);
        m_Player_SkillTree = m_Player.FindAction("SkillTree", throwIfNotFound: true);
        m_Player_Quest = m_Player.FindAction("Quest", throwIfNotFound: true);
        m_Player_Quick = m_Player.FindAction("Quick", throwIfNotFound: true);
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

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Player
    private readonly InputActionMap m_Player;
    private List<IPlayerActions> m_PlayerActionsCallbackInterfaces = new List<IPlayerActions>();
    private readonly InputAction m_Player_Move;
    private readonly InputAction m_Player_Look;
    private readonly InputAction m_Player_Jump;
    private readonly InputAction m_Player_Sprint;
    private readonly InputAction m_Player_Interaction;
    private readonly InputAction m_Player_CursorToggle;
    private readonly InputAction m_Player_LockOn;
    private readonly InputAction m_Player_Cancel;
    private readonly InputAction m_Player_ItemInventory;
    private readonly InputAction m_Player_EquipmentInventory;
    private readonly InputAction m_Player_SkillTree;
    private readonly InputAction m_Player_Quest;
    private readonly InputAction m_Player_Quick;
    public struct PlayerActions
    {
        private @GameControls m_Wrapper;
        public PlayerActions(@GameControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_Player_Move;
        public InputAction @Look => m_Wrapper.m_Player_Look;
        public InputAction @Jump => m_Wrapper.m_Player_Jump;
        public InputAction @Sprint => m_Wrapper.m_Player_Sprint;
        public InputAction @Interaction => m_Wrapper.m_Player_Interaction;
        public InputAction @CursorToggle => m_Wrapper.m_Player_CursorToggle;
        public InputAction @LockOn => m_Wrapper.m_Player_LockOn;
        public InputAction @Cancel => m_Wrapper.m_Player_Cancel;
        public InputAction @ItemInventory => m_Wrapper.m_Player_ItemInventory;
        public InputAction @EquipmentInventory => m_Wrapper.m_Player_EquipmentInventory;
        public InputAction @SkillTree => m_Wrapper.m_Player_SkillTree;
        public InputAction @Quest => m_Wrapper.m_Player_Quest;
        public InputAction @Quick => m_Wrapper.m_Player_Quick;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void AddCallbacks(IPlayerActions instance)
        {
            if (instance == null || m_Wrapper.m_PlayerActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_PlayerActionsCallbackInterfaces.Add(instance);
            @Move.started += instance.OnMove;
            @Move.performed += instance.OnMove;
            @Move.canceled += instance.OnMove;
            @Look.started += instance.OnLook;
            @Look.performed += instance.OnLook;
            @Look.canceled += instance.OnLook;
            @Jump.started += instance.OnJump;
            @Jump.performed += instance.OnJump;
            @Jump.canceled += instance.OnJump;
            @Sprint.started += instance.OnSprint;
            @Sprint.performed += instance.OnSprint;
            @Sprint.canceled += instance.OnSprint;
            @Interaction.started += instance.OnInteraction;
            @Interaction.performed += instance.OnInteraction;
            @Interaction.canceled += instance.OnInteraction;
            @CursorToggle.started += instance.OnCursorToggle;
            @CursorToggle.performed += instance.OnCursorToggle;
            @CursorToggle.canceled += instance.OnCursorToggle;
            @LockOn.started += instance.OnLockOn;
            @LockOn.performed += instance.OnLockOn;
            @LockOn.canceled += instance.OnLockOn;
            @Cancel.started += instance.OnCancel;
            @Cancel.performed += instance.OnCancel;
            @Cancel.canceled += instance.OnCancel;
            @ItemInventory.started += instance.OnItemInventory;
            @ItemInventory.performed += instance.OnItemInventory;
            @ItemInventory.canceled += instance.OnItemInventory;
            @EquipmentInventory.started += instance.OnEquipmentInventory;
            @EquipmentInventory.performed += instance.OnEquipmentInventory;
            @EquipmentInventory.canceled += instance.OnEquipmentInventory;
            @SkillTree.started += instance.OnSkillTree;
            @SkillTree.performed += instance.OnSkillTree;
            @SkillTree.canceled += instance.OnSkillTree;
            @Quest.started += instance.OnQuest;
            @Quest.performed += instance.OnQuest;
            @Quest.canceled += instance.OnQuest;
            @Quick.started += instance.OnQuick;
            @Quick.performed += instance.OnQuick;
            @Quick.canceled += instance.OnQuick;
        }

        private void UnregisterCallbacks(IPlayerActions instance)
        {
            @Move.started -= instance.OnMove;
            @Move.performed -= instance.OnMove;
            @Move.canceled -= instance.OnMove;
            @Look.started -= instance.OnLook;
            @Look.performed -= instance.OnLook;
            @Look.canceled -= instance.OnLook;
            @Jump.started -= instance.OnJump;
            @Jump.performed -= instance.OnJump;
            @Jump.canceled -= instance.OnJump;
            @Sprint.started -= instance.OnSprint;
            @Sprint.performed -= instance.OnSprint;
            @Sprint.canceled -= instance.OnSprint;
            @Interaction.started -= instance.OnInteraction;
            @Interaction.performed -= instance.OnInteraction;
            @Interaction.canceled -= instance.OnInteraction;
            @CursorToggle.started -= instance.OnCursorToggle;
            @CursorToggle.performed -= instance.OnCursorToggle;
            @CursorToggle.canceled -= instance.OnCursorToggle;
            @LockOn.started -= instance.OnLockOn;
            @LockOn.performed -= instance.OnLockOn;
            @LockOn.canceled -= instance.OnLockOn;
            @Cancel.started -= instance.OnCancel;
            @Cancel.performed -= instance.OnCancel;
            @Cancel.canceled -= instance.OnCancel;
            @ItemInventory.started -= instance.OnItemInventory;
            @ItemInventory.performed -= instance.OnItemInventory;
            @ItemInventory.canceled -= instance.OnItemInventory;
            @EquipmentInventory.started -= instance.OnEquipmentInventory;
            @EquipmentInventory.performed -= instance.OnEquipmentInventory;
            @EquipmentInventory.canceled -= instance.OnEquipmentInventory;
            @SkillTree.started -= instance.OnSkillTree;
            @SkillTree.performed -= instance.OnSkillTree;
            @SkillTree.canceled -= instance.OnSkillTree;
            @Quest.started -= instance.OnQuest;
            @Quest.performed -= instance.OnQuest;
            @Quest.canceled -= instance.OnQuest;
            @Quick.started -= instance.OnQuick;
            @Quick.performed -= instance.OnQuick;
            @Quick.canceled -= instance.OnQuick;
        }

        public void RemoveCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IPlayerActions instance)
        {
            foreach (var item in m_Wrapper.m_PlayerActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_PlayerActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public PlayerActions @Player => new PlayerActions(this);
    private int m_KeyboardMouseSchemeIndex = -1;
    public InputControlScheme KeyboardMouseScheme
    {
        get
        {
            if (m_KeyboardMouseSchemeIndex == -1) m_KeyboardMouseSchemeIndex = asset.FindControlSchemeIndex("Keyboard/Mouse");
            return asset.controlSchemes[m_KeyboardMouseSchemeIndex];
        }
    }
    public interface IPlayerActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnLook(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnSprint(InputAction.CallbackContext context);
        void OnInteraction(InputAction.CallbackContext context);
        void OnCursorToggle(InputAction.CallbackContext context);
        void OnLockOn(InputAction.CallbackContext context);
        void OnCancel(InputAction.CallbackContext context);
        void OnItemInventory(InputAction.CallbackContext context);
        void OnEquipmentInventory(InputAction.CallbackContext context);
        void OnSkillTree(InputAction.CallbackContext context);
        void OnQuest(InputAction.CallbackContext context);
        void OnQuick(InputAction.CallbackContext context);
    }
}
