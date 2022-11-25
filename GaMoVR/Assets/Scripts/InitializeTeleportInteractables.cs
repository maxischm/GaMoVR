using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class InitializeTeleportInteractables : MonoBehaviour
{
    // Start is called before the first frame update
    public void Start()
    {
        TeleportationProvider teleportationProvider = GetComponentInChildren<TeleportationProvider>();
        BaseTeleportationInteractable[] teleportationInteractables =
            FindObjectsOfType<BaseTeleportationInteractable>();
        foreach (var teleportation in teleportationInteractables)
        {
            teleportation.teleportationProvider = teleportationProvider;
        }
    }
}