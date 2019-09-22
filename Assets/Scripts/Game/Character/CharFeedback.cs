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
        HitGround,
        Jump,
        Death,
        Hitted
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
    [Space]
    [SerializeField] private ParticleSystem[] _respawnParticles;

    private CharController _charController;
    private bool _canPlayHitGround = false; // disable the first call of hitGround
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
            if (particle == Particle.HitGround && !_canPlayHitGround)
            {
                _canPlayHitGround = true;
            }
            else
            {
                _nonOrientedParticles[(int)particle].Play();
            }
        }
        else
        {
            _nonOrientedParticles[(int)particle].Stop();
        }
    }

    public ParticleSystem GetNonOrientedParticle(Particle particle)
    {
        return _nonOrientedParticles[(int)particle];
    }

    public void PlayRespawnParticle()
    {
        for (int i = 0; i < _respawnParticles.Length; i++)
        {
            _respawnParticles[i].Play();
        }

        new Timer(this, 1.8f, (float t) =>
        {
            var c= _charController.SpriteRenderer.color;
            c.a = t;
            _charController.SpriteRenderer.color = c;
        });
    }
    #endregion
}
