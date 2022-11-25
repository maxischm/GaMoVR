using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LosingHeartVisualsManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _heartIcon;

    [SerializeField]
    private GameObject _particleTrail;

    [SerializeField]
    private Transform _rightHandTransform;

    [SerializeField]
    private Transform _playerCameraPositionTransform;

    [SerializeField]
    private Animator _heartCrackAnimator;

    [SerializeField]
    private List<MeshRenderer> _heartMeshRenderer = new();

    public void OnEnable()
    {
        UmlHangmanLevelValidator.OnConnectorValidationFailed += StartHeartLossVisuals;
        UmlHangmanLevelValidator.OnMethodValidationFailed += StartHeartLossVisuals;
        UmlHangmanLevelValidator.OnPropertyValidationFailed += StartHeartLossVisuals;
        UmlHangmanLevelValidator.OnWrongMove += StartHeartLossVisuals;
    }

    public void OnDisable()
    {
        UmlHangmanLevelValidator.OnConnectorValidationFailed -= StartHeartLossVisuals;
        UmlHangmanLevelValidator.OnMethodValidationFailed -= StartHeartLossVisuals;
        UmlHangmanLevelValidator.OnPropertyValidationFailed -= StartHeartLossVisuals;
        UmlHangmanLevelValidator.OnWrongMove -= StartHeartLossVisuals;
    }

    public void Update()
    {
        if (_playerCameraPositionTransform == null)
        {
            _playerCameraPositionTransform = LocalPlayerReference.Instance.LocalPlayer
                .GetComponentInChildren<NonXRPlayerComponents>().CameraAttachmentPosition;
        }
        if (_rightHandTransform == null)
        {
            _rightHandTransform = LocalPlayerReference.Instance.LocalPlayer
                .GetComponentInChildren<XRPlayerComponents>()
                .RightHandInteractors
                .GetComponentInChildren<XRDirectInteractor>()
                .transform;
        }
    }

    public IEnumerator ShrinkHeart()
    {
        _heartIcon.SetActive(true);
        foreach (var renderer in _heartMeshRenderer)
        {
            renderer.enabled = true;
        }

        float elapsedTime = 0;
        var startScale = new Vector3(0.3f, 0.3f, 0.3f);
        var targetScale = new Vector3(0.1f, 0.1f, 0.1f);

        _heartIcon.transform.position = _playerCameraPositionTransform.position + _playerCameraPositionTransform.forward * 0.5f;
        _heartIcon.transform.localScale = startScale;
        _heartIcon.transform.LookAt(_playerCameraPositionTransform.position);
        _heartIcon.transform.Rotate(90, 0, 0);

        const float timeTakes = 0.5f;

        while (elapsedTime < timeTakes)
        {
            _heartIcon.transform.localScale = Vector3.Lerp(
                _heartIcon.transform.localScale,
                targetScale,
                elapsedTime / timeTakes
            );

            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        _heartCrackAnimator.enabled = true;
        _heartCrackAnimator.SetTrigger("Break");

        yield return new WaitForSeconds(1);

        // Reset animator
        foreach (var renderer in _heartMeshRenderer)
        {
            renderer.enabled = false;
        }
        _heartCrackAnimator.SetTrigger("Reset");
        yield return new WaitForSeconds(1);
        _heartCrackAnimator.enabled = false;
        _heartIcon.SetActive(false);
    }

    public IEnumerator MoveParticleTrail()
    {
        var startScale = new Vector3(0.04f, 0.04f, 0.04f);
        var targetScale = new Vector3(0.01f, 0.01f, 0.01f);
        _particleTrail.transform.position = _playerCameraPositionTransform.position + _playerCameraPositionTransform.forward * 0.5f;
        _particleTrail.transform.localScale = startScale;

        yield return new WaitForSeconds(1.25f);

        _particleTrail.SetActive(true);

        var startPosition = _playerCameraPositionTransform.position + _playerCameraPositionTransform.forward * 0.5f;
        const float timeTakes = 1f;
        float elapsedTime = 0;

        while (elapsedTime < timeTakes)
        {
            _particleTrail.transform.position = Vector3.Lerp(
                startPosition,
                _rightHandTransform.position,
                elapsedTime / timeTakes
            );
            _particleTrail.transform.localScale = Vector3.Lerp(startScale, targetScale, elapsedTime / timeTakes);

            elapsedTime += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

        _particleTrail.SetActive(false);
    }

    public void StartHeartLossVisuals(bool won, float gainedXp)
    {
        StartCoroutine(ShrinkHeart());
        StartCoroutine(MoveParticleTrail());
    }

    public void StartHeartLossVisuals(ConnectionPlacementError errorType)
    {
        StartCoroutine(ShrinkHeart());
        StartCoroutine(MoveParticleTrail());
    }

    public void StartHeartLossVisuals()
    {
        StartCoroutine(ShrinkHeart());
        StartCoroutine(MoveParticleTrail());
    }
}
