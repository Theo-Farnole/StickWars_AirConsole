using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipleAudioSources : MonoBehaviour
{
    [SerializeField] private AudioSource[] _audioSources;

    public void PlayRandomAudioSource()
    {
        int index = Random.Range(0, _audioSources.Length);
        _audioSources[index].Play();
    }
}
