using System.Collections.Generic;
using UnityEngine;

namespace ModularWorldSpaceUI
{
    [RequireComponent(typeof(Collider))]
    public class UITrigger : MonoBehaviour
    {
        private UITriggerActivation _uiTriggerActivation;

        public UITriggerActivation UITriggerActivation
        {
            get => _uiTriggerActivation;
            set => _uiTriggerActivation = value;
        }

        [SerializeField] private List<string> tagsToTrigger;

        private void OnTriggerEnter(Collider other)
        {
            foreach (var triggerTag in tagsToTrigger)
            {
                if (other.CompareTag(triggerTag))
                {
                    _uiTriggerActivation.PlayerEnteredTrigger(
                        other.GetComponent<XRPlayerComponents>().NonXRPlayerComponents, this);
                    break;
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            foreach (var triggerTag in tagsToTrigger)
            {
                if (other.CompareTag(triggerTag))
                {
                    _uiTriggerActivation.PlayerLeftTrigger(
                        other.GetComponent<XRPlayerComponents>().NonXRPlayerComponents, this);
                    break;
                }
            }
        }
    }
}