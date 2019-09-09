using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlitchController : Singleton<GlitchController>
{
    #region Fields
    private GlitchEffect _glitchEffect;
    #endregion

    #region Methods
    void Awake()
    {
        _glitchEffect = GetComponent<GlitchEffect>();
        _glitchEffect.enabled = false;
    }

    public void StartGlitch()
    {
        _glitchEffect.enabled = true;
    }

    public void StopGlitch()
    {
        _glitchEffect.enabled = false;
    }
    #endregion
}
