using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UmlHangmanPlayerSoundManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource _correctMoveSound;

    [SerializeField]
    private AudioSource _wrongMoveSound;

    [SerializeField]
    private AudioSource _winSound;

    [SerializeField]
    private AudioSource _loseSound;

    public void OnEnable()
    {
        UmlHangmanLevelValidator.OnGameFinished += PlayGameEndSound;
    }

    public void OnDisable()
    {
        UmlHangmanLevelValidator.OnGameFinished -= PlayGameEndSound;
    }

    public void PlayCorrectMoveSound()
    {
        _correctMoveSound.Play();
    }

    public void PlayWrongMoveSound()
    {
        _wrongMoveSound.Play();
    }

    public void PlayGameEndSound(bool won, float gainedXp)
    {
        if (won)
        {
            _winSound.Play();
        }
        else
        {
            _loseSound.Play();
        }
    }
}
