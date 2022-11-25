using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ModularWorldSpaceUI
{
    [RequireComponent(typeof(UIController))]
    public class UIActionActivation : MonoBehaviour
    {
        [SerializeField] private InputActionReference activationAction;

        private UIController _uiController;

        private bool _isActivated;
        // Start is called before the first frame update
        void OnEnable()
        {
            activationAction.action.started += UIActionInvoked;
            //TODO separate activation and deactivation
        }

        private void OnDisable()
        {
            activationAction.action.started -= UIActionInvoked;
        }

        public void Start()
        {
            _uiController = GetComponent<UIController>();
        }

        public void UIActionInvoked(InputAction.CallbackContext obj)
        {
            if (!_isActivated)
            {
                _isActivated = true;
                _uiController.ActivateUI(LocalPlayerReference.Instance.LocalPlayer.GetComponentInChildren<NonXRPlayerComponents>());
            }
            else
            {
                _isActivated = false;
                _uiController.DeactivateUI();
            }
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}