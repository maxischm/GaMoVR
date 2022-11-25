using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GallowsAnimator : MonoBehaviour
{
    [SerializeField]
    private Transform _verticalBeam;

    private Vector3 _verticalBeamStartPosition;
    private Vector3 _verticalBeamTargetPosition = new Vector3(-0.859f, 1.484f, 0.042f);

    private Quaternion _verticalBeamStartRotation;
    private Quaternion _verticalBeamTargetRotation = Quaternion.identity;

    [SerializeField]
    private Transform _horizontalBeam;

    private Vector3 _horizontalBeamStartPosition;
    private Vector3 _horizontalBeamTargetPosition = new Vector3(-0.159f, 2.443f, 0.041f);

    private Quaternion _horizontalBeamStartRotation;
    private Quaternion _horizontalBeamTargetRotation = Quaternion.Euler(0, 0, 90);

    [SerializeField]
    private Transform _diagonalBeam;

    private Vector3 _diagonalBeamStartPosition;
    private Vector3 _diagonalBeamTargetPosition = new Vector3(-0.542f, 2.123f, 0.039f);

    private Quaternion _diagonalBeamStartRotation;
    private Quaternion _diagonalBeamTargetRotation = Quaternion.Euler(0, 0, -45);

    [SerializeField]
    private Transform _rope;

    private Vector3 _ropeStartPosition;
    private Vector3 _ropeTargetPosition = new Vector3(0.388f, 2.189f, 0.045f);

    private Quaternion _ropeStartRotation;
    private Quaternion _ropeTargetRotation = Quaternion.identity;

    [SerializeField]
    private Transform _head;

    private Vector3 _headStartPosition;
    private Vector3 _headTargetPosition = new Vector3(0.3947f, 1.8238f, 0.042f);

    private Quaternion _headStartRotation;
    private Quaternion _headTargetRotation = Quaternion.identity;

    private float _timePerSection;
    private float _elapsedTime;

    private Coroutine _currentCoroutine;

    private string _currentlyMovingPartName;

    public void OnEnable()
    {
        _verticalBeamStartPosition = _verticalBeam.localPosition;
        _horizontalBeamStartPosition = _horizontalBeam.localPosition;
        _diagonalBeamStartPosition = _diagonalBeam.localPosition;
        _ropeStartPosition = _rope.localPosition;
        _headStartPosition = _head.localPosition;

        _verticalBeamStartRotation = _verticalBeam.localRotation;
        _horizontalBeamStartRotation = _horizontalBeam.localRotation;
        _diagonalBeamStartRotation = _diagonalBeam.localRotation;
        _ropeStartRotation = _rope.localRotation;
        _headStartRotation = _head.localRotation;

        UmlHangmanLevelValidator.OnGameFinished += StopGallowsAnimation;
    }

    public void OnDestroy()
    {
        UmlHangmanLevelValidator.OnGameFinished -= StopGallowsAnimation;
    }

    public void StartGallowsAnimation(float timePerSection)
    {
        // Set all parts to start position
        _verticalBeam.localPosition = _verticalBeamStartPosition;
        _horizontalBeam.localPosition = _horizontalBeamStartPosition;
        _diagonalBeam.localPosition = _diagonalBeamStartPosition;
        _rope.localPosition = _ropeStartPosition;
        _head.localPosition = _headStartPosition;

        _verticalBeam.localRotation = _verticalBeamStartRotation;
        _horizontalBeam.localRotation = _horizontalBeamStartRotation;
        _diagonalBeam.localRotation = _diagonalBeamStartRotation;
        _rope.localRotation = _ropeStartRotation;
        _head.localRotation = _headStartRotation;

        _elapsedTime = 0;

        if (_currentCoroutine != null)
        {
            StopCoroutine(_currentCoroutine);
        }

        _timePerSection = timePerSection;

        _currentCoroutine = StartCoroutine(MoveVerticalBeam());
    }

    public void RevertAnimation(float timeToRevert)
    {
        if (timeToRevert < _elapsedTime)
        {
            // only reset time for current object
            _elapsedTime -= timeToRevert;
        }
        else
        {
            StopCoroutine(_currentCoroutine);
            timeToRevert -= _elapsedTime;
            _elapsedTime = _timePerSection - timeToRevert;
            switch (_currentlyMovingPartName)
            {
                case "VerticalBeam":
                    _verticalBeam.localPosition = _verticalBeamStartPosition;
                    _verticalBeam.localRotation = _verticalBeamStartRotation;
                    _elapsedTime = 0;
                    _currentCoroutine = StartCoroutine(MoveVerticalBeam());
                    break;
                case "HorizontalBeam":
                    _horizontalBeam.localPosition = _horizontalBeamStartPosition;
                    _horizontalBeam.localRotation = _horizontalBeamStartRotation;
                    _currentCoroutine = StartCoroutine(MoveVerticalBeam());
                    break;
                case "DiagonalBeam":
                    _diagonalBeam.localPosition = _diagonalBeamStartPosition;
                    _diagonalBeam.localRotation = _diagonalBeamStartRotation;
                    _currentCoroutine = StartCoroutine(MoveHorizontalBeam());
                    break;
                case "Rope":
                    _rope.localPosition = _ropeStartPosition;
                    _rope.localRotation = _ropeStartRotation;
                    _currentCoroutine = StartCoroutine(MoveDiagonalBeam());
                    break;
                case "Head":
                    _head.localPosition = _headStartPosition;
                    _head.localRotation = _headStartRotation;
                    _currentCoroutine = StartCoroutine(MoveRope());
                    break;
            }
        }
    }

    public void StopGallowsAnimation(bool won, float gainedXp)
    {
        StopCoroutine(_currentCoroutine);
    }

    public IEnumerator MoveVerticalBeam()
    {
        _currentlyMovingPartName = _verticalBeam.gameObject.name;
        while (_elapsedTime < _timePerSection)
        {
            _verticalBeam.localPosition = Vector3.Lerp(
                _verticalBeamStartPosition,
                _verticalBeamTargetPosition,
                _elapsedTime / _timePerSection
            );
            _verticalBeam.localRotation = Quaternion.Lerp(
                _verticalBeamStartRotation,
                _verticalBeamTargetRotation,
                _elapsedTime / _timePerSection
            );

            _elapsedTime += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

        _elapsedTime = 0;
        _currentCoroutine = StartCoroutine(MoveHorizontalBeam());
    }

    public IEnumerator MoveHorizontalBeam()
    {
        _currentlyMovingPartName = _horizontalBeam.gameObject.name;
        while (_elapsedTime < _timePerSection)
        {
            _horizontalBeam.localPosition = Vector3.Lerp(
                _horizontalBeamStartPosition,
                _horizontalBeamTargetPosition,
                _elapsedTime / _timePerSection
            );
            _horizontalBeam.localRotation = Quaternion.Lerp(
                _horizontalBeamStartRotation,
                _horizontalBeamTargetRotation,
                _elapsedTime / _timePerSection
            );

            _elapsedTime += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

        _elapsedTime = 0;
        _currentCoroutine = StartCoroutine(MoveDiagonalBeam());
    }

    public IEnumerator MoveDiagonalBeam()
    {
        _currentlyMovingPartName = _diagonalBeam.gameObject.name;
        while (_elapsedTime < _timePerSection)
        {
            _diagonalBeam.localPosition = Vector3.Lerp(
                _diagonalBeamStartPosition,
                _diagonalBeamTargetPosition,
                _elapsedTime / _timePerSection
            );
            _diagonalBeam.localRotation = Quaternion.Lerp(
                _diagonalBeamStartRotation,
                _diagonalBeamTargetRotation,
                _elapsedTime / _timePerSection
            );

            _elapsedTime += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

        _elapsedTime = 0;
        _currentCoroutine = StartCoroutine(MoveRope());
    }

    public IEnumerator MoveRope()
    {
        _currentlyMovingPartName = _rope.gameObject.name;
        while (_elapsedTime < _timePerSection)
        {
            _rope.localPosition = Vector3.Lerp(
                _ropeStartPosition,
                _ropeTargetPosition,
                _elapsedTime / _timePerSection
            );
            _rope.localRotation = Quaternion.Lerp(
                _ropeStartRotation,
                _ropeTargetRotation,
                _elapsedTime / _timePerSection
            );

            _elapsedTime += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

        _elapsedTime = 0;
        _currentCoroutine = StartCoroutine(MoveHead());
    }

    public IEnumerator MoveHead()
    {
        _currentlyMovingPartName = _head.gameObject.name;
        _elapsedTime = 0;
        while (_elapsedTime < _timePerSection)
        {
            _head.localPosition = Vector3.Lerp(
                _headStartPosition,
                _headTargetPosition,
                _elapsedTime / _timePerSection
            );
            _head.localRotation = Quaternion.Lerp(
                _headStartRotation,
                _headTargetRotation,
                _elapsedTime / _timePerSection
            );

            _elapsedTime += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }
    }
}
