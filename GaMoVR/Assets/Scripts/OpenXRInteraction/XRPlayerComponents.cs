using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRPlayerComponents : MonoBehaviour
{
    [SerializeField] private InteractorStateHolder rightHandInteractors;
    [SerializeField] private InteractorStateHolder leftHandInteractors;

    [SerializeField] private XRDirectInteractor rightHandBaseInteractor;
    [SerializeField] private XRDirectInteractor leftHandBaseInteractor;

    [SerializeField] private NonXRPlayerComponents nonXRPlayerComponents;

    public InteractorStateHolder RightHandInteractors => rightHandInteractors;

    public InteractorStateHolder LeftHandInteractors => leftHandInteractors;

    public XRDirectInteractor RightHandBaseInteractor => rightHandBaseInteractor;

    public XRDirectInteractor LeftHandBaseInteractor => leftHandBaseInteractor;

    public NonXRPlayerComponents NonXRPlayerComponents => nonXRPlayerComponents;
}