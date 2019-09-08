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
        HitGround,
        Death,
        Footstep
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
            MuteAllSounds(_enableSound);
        }

        get
        {
            return _enableSound;
        }
    }
    #endregion

    #region Method
    public void PlaySound(Sound sound, bool forcePlay = false)
    {
        if (_enableSound == false)
            return;

        if (_audioSource[(int)sound].isPlaying == false || forcePlay)
        {
            _audioSource[(int)sound].Play();
        }
    }

    public void StopSound(Sound sound)
    {
        _audioSource[(int)sound].Stop();
    }

    public void PlayHitTackle()
    {
        int randomIndex = UnityEngine.Random.Range(0, _hitTackleAudioSource.Length);
        _hitTackleAudioSource[randomIndex].Play();
    }

    private void MuteAllSounds(bool isMuted)
    {
        for (int i = 0; i < _audioSource.Length; i++)
        {
            _audioSource[i].mute = isMuted;
        }

        for (int i = 0; i < _hitTackleAudioSource.Length; i++)
        {
            _hitTackleAudioSource[i].mute = isMuted;
        }
    }
    #endregion
}
