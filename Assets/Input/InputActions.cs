//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.4.4
//     from Assets/Input/InputActions.inputactions
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

namespace InputActions
{
    public partial class @InputActions : IInputActionCollection2, IDisposable
    {
        public InputActionAsset asset { get; }
        public @InputActions()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputActions"",
    ""maps"": [
        {
            ""name"": ""TurnBasedEncounter"",
            ""id"": ""8ab82065-3ee3-40d3-9e48-080ac0b3ecd6"",
            ""actions"": [
                {
                    ""name"": ""Select"",
                    ""type"": ""Button"",
                    ""id"": ""1fe5049d-6907-4b32-a14c-83c2e1c595a5"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""2fa2a869-f2bd-4713-9455-6ed54e046448"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KBM"",
                    ""action"": ""Select"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""KBM"",
            ""bindingGroup"": ""KBM"",
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
            // TurnBasedEncounter
            m_TurnBasedEncounter = asset.FindActionMap("TurnBasedEncounter", throwIfNotFound: true);
            m_TurnBasedEncounter_Select = m_TurnBasedEncounter.FindAction("Select", throwIfNotFound: true);
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

        // TurnBasedEncounter
        private readonly InputActionMap m_TurnBasedEncounter;
        private ITurnBasedEncounterActions m_TurnBasedEncounterActionsCallbackInterface;
        private readonly InputAction m_TurnBasedEncounter_Select;
        public struct TurnBasedEncounterActions
        {
            private @InputActions m_Wrapper;
            public TurnBasedEncounterActions(@InputActions wrapper) { m_Wrapper = wrapper; }
            public InputAction @Select => m_Wrapper.m_TurnBasedEncounter_Select;
            public InputActionMap Get() { return m_Wrapper.m_TurnBasedEncounter; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(TurnBasedEncounterActions set) { return set.Get(); }
            public void SetCallbacks(ITurnBasedEncounterActions instance)
            {
                if (m_Wrapper.m_TurnBasedEncounterActionsCallbackInterface != null)
                {
                    @Select.started -= m_Wrapper.m_TurnBasedEncounterActionsCallbackInterface.OnSelect;
                    @Select.performed -= m_Wrapper.m_TurnBasedEncounterActionsCallbackInterface.OnSelect;
                    @Select.canceled -= m_Wrapper.m_TurnBasedEncounterActionsCallbackInterface.OnSelect;
                }
                m_Wrapper.m_TurnBasedEncounterActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @Select.started += instance.OnSelect;
                    @Select.performed += instance.OnSelect;
                    @Select.canceled += instance.OnSelect;
                }
            }
        }
        public TurnBasedEncounterActions @TurnBasedEncounter => new TurnBasedEncounterActions(this);
        private int m_KBMSchemeIndex = -1;
        public InputControlScheme KBMScheme
        {
            get
            {
                if (m_KBMSchemeIndex == -1) m_KBMSchemeIndex = asset.FindControlSchemeIndex("KBM");
                return asset.controlSchemes[m_KBMSchemeIndex];
            }
        }
        public interface ITurnBasedEncounterActions
        {
            void OnSelect(InputAction.CallbackContext context);
        }
    }
}
