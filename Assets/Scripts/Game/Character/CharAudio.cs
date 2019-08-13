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
    #endregion

    #region Method
    public void PlaySound(Sound sound)
    {
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
    #endregion
}
