using ProceduralNetworkPlayer.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class UmlModelingPlayerManager : MonoBehaviour
{
    [SerializeField]
    private GameObject player;

    [SerializeField]
    private Transform leftHandXrTransformSource;

    [SerializeField]
    private Transform rightHandXrTransformSource;

    [SerializeField]
    private MoveThisTransformLikeOther _playerAudioMoveThisTransformLikeOther;

    public void Update()
    {
        if (player == null || leftHandXrTransformSource == null || rightHandXrTransformSource == null)
        {
            player = LocalPlayerReference.Instance.LocalPlayer;

            // Cache references
            leftHandXrTransformSource =
            player
                .GetComponentInChildren<XRPlayerComponents>()
                .LeftHandInteractors
                .GetComponentInChildren<XRDirectInteractor>()
                .transform;
            rightHandXrTransformSource =
                player
                    .GetComponentInChildren<XRPlayerComponents>()
                    .RightHandInteractors
                    .GetComponentInChildren<XRDirectInteractor>()
                    .transform;

            _playerAudioMoveThisTransformLikeOther.TransformSource = rightHandXrTransformSource;
        }
    }

    public void Start()
    {
        StartCoroutine(GamificationEngineConnection.Instance.FetchPlayerState(null));
    }
}
