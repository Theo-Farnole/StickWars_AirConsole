using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharFeedback : MonoBehaviour
{
    #region Enums
    public enum OrientateParticle
    {
        SlidingWall,
        Tackle
    }

    public enum Particle
    {
        HitGround
    }
    #endregion

    #region Classes
    [Serializable]
    public class ParticleOrientedWrapper
    {
        public ParticleSystem left;
        public ParticleSystem right;
    }
    #endregion

    #region Fields
    [EnumNamedArray(typeof(OrientateParticle))]
    [SerializeField] private ParticleOrientedWrapper[] _orientedParticles = new ParticleOrientedWrapper[Enum.GetValues(typeof(OrientateParticle)).Length];
    [Space]
    [EnumNamedArray(typeof(Particle))]
    [SerializeField] private ParticleSystem[] _nonOrientedParticles = new ParticleSystem[Enum.GetValues(typeof(Particle)).Length];

    private CharController _charController;
    #endregion

    #region Methods
    void Awake()
    {
        _charController = GetComponent<CharController>();
    }

    public void PlayOrientedParticle(bool active, OrientateParticle particle)
    {
        PlayOrientedParticle(active, particle, _charController.OrientationX);
    }

    public void PlayOrientedParticle(bool active, OrientateParticle particle, CharController.Orientation orientation)
    {
        if (active)
        {
            switch (orientation)
            {
                case CharController.Orientation.Left:
                    _orientedParticles[(int)particle].left.Play();
                    _orientedParticles[(int)particle].right.Stop();
                    break;

                case CharController.Orientation.Right:
                    _orientedParticles[(int)particle].right.Play();
                    _orientedParticles[(int)particle].left.Stop();
                    break;
            }
        }
        else
        {
            _orientedParticles[(int)particle].left.Stop();            
            _orientedParticles[(int)particle].right.Stop();
        }
    }

    public void PlayNonOrientedParticle(bool active, Particle particle)
    {
        if (active)
        {
            _nonOrientedParticles[(int)particle].Play();
        }
        else
        {
            _nonOrientedParticles[(int)particle].Stop();
        }
    }
    #endregion
}
