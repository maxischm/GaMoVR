using UnityEngine;

namespace ModularWorldSpaceUI
{
    [RequireComponent(typeof(UIController))]
    public class UITriggerActivation : MonoBehaviour, IUIActivation
    {
        public bool toggleOnEnterAndLeave = true;

        public UITrigger enterTrigger;
        public UITrigger leaveTrigger;
        public UITrigger toggleTrigger;

        public UIController uiController;

        private bool _playerIsInTrigger;

        private void Start()
        {
            if ((toggleTrigger is null && (enterTrigger is null || leaveTrigger is null)))
                Debug.LogError("UI Trigger Activation needs triggers to activate the ui");

            uiController = GetComponent<UIController>();
            if (enterTrigger != null) enterTrigger.UITriggerActivation = this;
            if (leaveTrigger != null) leaveTrigger.UITriggerActivation = this;
            if (toggleTrigger != null) toggleTrigger.UITriggerActivation = this;
        }

        public void PlayerEnteredTrigger(NonXRPlayerComponents player, UITrigger trigger)
        {
            if (toggleOnEnterAndLeave)
            {
                if (trigger == toggleTrigger)
                {
                    uiController.ActivateUI(player);
                }
            }
            else
            {
                if (trigger == enterTrigger)
                {
                    uiController.ActivateUI(player);
                }
            }
        }

        public void PlayerLeftTrigger(NonXRPlayerComponents player, UITrigger trigger)
        {
            if (toggleOnEnterAndLeave)
            {
                if (trigger == toggleTrigger)
                {
                    uiController.DeactivateUI();
                }
            }
            else
            {
                if (trigger == leaveTrigger)
                {
                    uiController.DeactivateUI();
                }
            }
        }
    }
}