using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharFeedback : MonoBehaviour
{
    #region Enums
    public enum Particle
    {
        SlidingWall,
        Tackle
    }

    [Serializable]
    public class ParticleWrapper
    {
        public ParticleSystem left;
        public ParticleSystem right;
    }
    #endregion

    #region Fields
    [EnumNamedArray(typeof(Particle))]
    [SerializeField] private ParticleWrapper[] _prefabsParticle = new ParticleWrapper[Enum.GetValues(typeof(Particle)).Length];

    private CharController _charController;
    #endregion

    #region Methods
    void Awake()
    {
        _charController = GetComponent<CharController>();
    }

    public void PlayParticle(bool active, Particle particle)
    {
        PlayParticle(active, particle, _charController.OrientationX);
    }

    public void PlayParticle(bool active, Particle particle, CharController.Orientation orientation)
    {
        if (active)
        {
            switch (orientation)
            {
                case CharController.Orientation.Left:
                    _prefabsParticle[(int)particle].left.Play();
                    _prefabsParticle[(int)particle].right.Stop();
                    break;

                case CharController.Orientation.Right:
                    _prefabsParticle[(int)particle].right.Play();
                    _prefabsParticle[(int)particle].left.Stop();
                    break;
            }
        }
        else
        {
            _prefabsParticle[(int)particle].left.Stop();
            _prefabsParticle[(int)particle].right.Stop();
        }
    }
    #endregion
}
