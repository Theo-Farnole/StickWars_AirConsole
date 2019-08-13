using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharAudio : MonoBehaviour
{
    public enum Sound
    {
        Tackle,
        Jump,
        HitTackle
    }

    #region Fields
    [EnumNamedArray(typeof(Sound))]
    [SerializeField] private AudioSource[] _audioSource = new AudioSource[Enum.GetValues(typeof(Sound)).Length];
    [Space]
    [SerializeField] private AudioSource[] _hitTackleAudioSource = new AudioSource[3];

    private bool _enableSound = true;
    #endregion

    #region Properties
    public bool EnableSound
    {
        set
        {
            _enableSound = value;

            if (_enableSound == false)
            {
                StopAllSounds();
            }
        }

        get
        {
            return _enableSound;
        }
    }
    #endregion

    #region Method
    public void PlaySound(Sound sound)
    {
        if (_enableSound == false)
            return;

        if (sound == Sound.HitTackle)
        {
            int randomIndex = UnityEngine.Random.Range(0, _hitTackleAudioSource.Length);
            _hitTackleAudioSource[randomIndex].Play();
        }
        else
        {
            _audioSource[(int)sound].Play();
        }
    }

    private void StopAllSounds()
    {
        for (int i = 0; i < _audioSource.Length; i++)
        {
            _audioSource[i].Stop();
        }

        for (int i = 0; i < _hitTackleAudioSource.Length; i++)
        {
            _hitTackleAudioSource[i].Stop();
        }
    }
    #endregion
}
