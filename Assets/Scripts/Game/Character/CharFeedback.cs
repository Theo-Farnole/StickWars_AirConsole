using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharFeedback : MonoBehaviour
{
    public enum Particle
    {
        SlidingWall
    }

    [Serializable]
    public struct ParticleWrapper
    {
        public ParticleSystem left;
        public ParticleSystem right;
    }

    #region Fields
    //[EnumNamedArray(typeof(Particle))]
    [SerializeField] private ParticleWrapper[] _prefabsParticleLeft = new ParticleWrapper[Enum.GetValues(typeof(Particle)).Length];
    #endregion

    #region Methods
    void Start()
    {
        for (int i = 0; i < _prefabsParticleLeft.Length; i++)
        {
            _prefabsParticleLeft[i].left.Stop();
            _prefabsParticleLeft[i].right.Stop();
        }
    }

    public void PlayParticle(bool active, Particle particle, CharController.Orientation orientation)
    {
        if (active)
        {
            switch (orientation)
            {
                case CharController.Orientation.Left:
                    _prefabsParticleLeft[(int)particle].left.Play();
                    _prefabsParticleLeft[(int)particle].right.Stop();
                    break;

                case CharController.Orientation.Right:
                    _prefabsParticleLeft[(int)particle].left.Stop();
                    _prefabsParticleLeft[(int)particle].right.Play();
                    break;
            }
        }
        else
        {
            _prefabsParticleLeft[(int)particle].left.Stop();
            _prefabsParticleLeft[(int)particle].right.Stop();
        }
    }
    #endregion
}
