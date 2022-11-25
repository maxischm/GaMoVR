using UnityEngine;

public class VRKeysActivationHelper : MonoBehaviour
{
    private InteractorStateHolder _interactorStateHolder;

    // Start is called before the first frame update
    void Start()
    {
        if (_interactorStateHolder is null)
        {
            _interactorStateHolder = FindObjectOfType<InteractorStateHolder>();
        }
    }


    public void Activated()
    {
        if (_interactorStateHolder is null)
        {
            _interactorStateHolder = FindObjectOfType<InteractorStateHolder>();
        }

        _interactorStateHolder.RayInteractorState = false;
    }

    public void Deactivated()
    {
        if (_interactorStateHolder is null)
        {
            _interactorStateHolder = FindObjectOfType<InteractorStateHolder>();
        }

        _interactorStateHolder.RayInteractorState = true;
    }
}