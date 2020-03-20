using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EnhancedAudioSource : MonoBehaviour
{
    public void SetToRootThenPlay()
    {
        transform.parent = null;
        GetComponent<AudioSource>().Play();
    }
}
