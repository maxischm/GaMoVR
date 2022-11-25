using System.Collections.Generic;
using UnityEngine;

public class InteractorSelectionTrigger : MonoBehaviour
{
    [Header("States in Trigger"), SerializeField]
    private bool directInteractorEnter;

    [SerializeField] private bool rayInteractorEnter;

    [SerializeField] private bool teleportInteractorEnter;
    
    [Header("States after leaving Trigger"), SerializeField]
    private bool directInteractorExit;

    [SerializeField] private bool rayInteractorExit;
    
    [SerializeField] private bool teleportInteractorExit;

    [Space, SerializeField] private bool leftHand;
    [SerializeField] private bool rightHand;

    [Space, SerializeField] private List<string> tagsToTrigger;

    

    private void OnTriggerEnter(Collider other)
    {
        foreach (var tag in tagsToTrigger)
        {
            if (other.CompareTag(tag))
            {
                GameObjectWithTagEnter(other.GetComponent<XRPlayerComponents>());
                break;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        foreach (var tag in tagsToTrigger)
        {
            if (other.CompareTag(tag))
            {
                GameObjectWithTagExit(other.GetComponent<XRPlayerComponents>());
                break;
            }
        }
    }

    private void GameObjectWithTagEnter(XRPlayerComponents components)
    {
        if (leftHand)
        {
            components.LeftHandInteractors.DirectInteractorState = directInteractorEnter;
            components.LeftHandInteractors.RayInteractorState = rayInteractorEnter;
            components.LeftHandInteractors.TeleportInteractorState = teleportInteractorEnter;

        }
        if (rightHand)
        {
            components.RightHandInteractors.DirectInteractorState = directInteractorEnter;
            components.RightHandInteractors.RayInteractorState = rayInteractorEnter;
            components.RightHandInteractors.TeleportInteractorState = teleportInteractorEnter;
        }
    }


    private void GameObjectWithTagExit(XRPlayerComponents components)
    {
        if (leftHand)
        {
            components.LeftHandInteractors.DirectInteractorState = directInteractorExit;
            components.LeftHandInteractors.RayInteractorState = rayInteractorExit;
            components.LeftHandInteractors.TeleportInteractorState = teleportInteractorExit;
        }
        if (rightHand)
        {
            components.RightHandInteractors.DirectInteractorState = directInteractorExit;
            components.RightHandInteractors.RayInteractorState = rayInteractorExit;
            components.RightHandInteractors.TeleportInteractorState = teleportInteractorExit;
        }
    }

    public void SimulateTriggerEnter(XRPlayerComponents components)
    {
        GameObjectWithTagEnter(components);
    }

    public void SimulateTriggerExit(XRPlayerComponents components)
    {
        GameObjectWithTagExit(components);
    }
    
}