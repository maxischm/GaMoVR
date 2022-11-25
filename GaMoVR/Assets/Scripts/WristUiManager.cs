using System.Collections;
using System.Collections.Generic;
using ProceduralNetworkPlayer.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class WristUiManager : MonoBehaviour
{
    [SerializeField]
    private GameObject player;

    [SerializeField]
    private Transform leftHandXrTransformSource;

    [SerializeField]
    private Transform rightHandXrTransformSource;

    [SerializeField]
    private MoveThisTransformLikeOther _playerWristUiTransform;

    [SerializeField]
    private GameObject _wristCanvas;

    [SerializeField]
    private TextMeshProUGUI _livesText;

    private bool _wristCanvasActive;

    public void OnEnable()
    {
        GamificationEngineConnection.OnPlayerStatusUpdate += SetLivesText;
    }

    public void OnDisable()
    {
        GamificationEngineConnection.OnPlayerStatusUpdate -= SetLivesText;
    }

    public void Start()
    {
        _playerWristUiTransform.TransformSource = rightHandXrTransformSource;
    }

    // Update is called once per frame
    public void Update()
    {
        // Activation if hand is rotated far enough
        if (rightHandXrTransformSource.localEulerAngles.z < 310
            && rightHandXrTransformSource.localEulerAngles.z > 260
            && !_wristCanvasActive
        )
        {
            _wristCanvas.SetActive(true);
            _wristCanvasActive = true;
        }
        else if (
            _wristCanvasActive &&
                (rightHandXrTransformSource.localEulerAngles.z > 320 || rightHandXrTransformSource.localEulerAngles.z < 260))
        {
            _wristCanvas.SetActive(false);
            _wristCanvasActive = false;
        }
    }

    public void SetLivesText()
    {
        _livesText.SetText(GamificationEngineConnection.Instance.PlayerLives.ToString());
    }
}
