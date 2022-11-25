using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class InteractorStateHolder : MonoBehaviour
{
    [SerializeField] private XRBaseControllerInteractor directInteractor;
    [SerializeField] private XRBaseControllerInteractor teleportInteractor;
    [SerializeField] private XRBaseControllerInteractor rayInteractor;

    [Space, SerializeField] private bool directInteractorState;
    [SerializeField] private bool rayInteractorState;
    [SerializeField] private bool teleportInteractorState;

    public bool TeleportInteractorState
    {
        get => teleportInteractorState;
        set
        {
            teleportInteractorState = value;
            teleportInteractor.gameObject.SetActive(value);
        }
    }

    public bool DirectInteractorState
    {
        get => directInteractorState;
        //direct Interactor can be set immediately
        set
        {
            if (!(directInteractor is null))
            {
                directInteractorState = value;
                directInteractor.enabled = value;
            }
        }
    }

    private void Start()
    {
        if (!(directInteractor is null))
            directInteractorState = directInteractor.enabled;
        if (!(rayInteractor is null))
            rayInteractorState = rayInteractor.enabled;
    }

    public bool RayInteractorState
    {
        get => rayInteractorState;

        //ray interactor is only activated if teleport is not active, but it can be deactivated immediately
        set
        {
            if (!(rayInteractor is null))
            {
                rayInteractorState = value;
                if (value)
                {
                    if (!teleportInteractor.enabled)
                    {
                        rayInteractor.enabled = value;
                    }
                }
                else
                {
                    rayInteractor.enabled = false;
                }
            }
        }
    }

    public void CheckAndUpdateStates()
    {
        if (!(directInteractor is null))
            directInteractor.enabled = directInteractorState;
        if (!(rayInteractor is null))
            rayInteractor.enabled = rayInteractorState;
    }

    public void TeleportStarted()
    {
        if (!(rayInteractor is null))
            rayInteractor.enabled = false;
    }

    public void TeleportEnded()
    {
        CheckAndUpdateStates();
    }
}