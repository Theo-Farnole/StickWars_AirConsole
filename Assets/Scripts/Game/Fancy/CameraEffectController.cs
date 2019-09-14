using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEffectController : Singleton<CameraEffectController>
{
    #region Fields
    private GlitchEffect _glitchEffect;
    private SuperBlur.SuperBlur _superBlur;
    #endregion

    #region Methods
    void Awake()
    {
        _glitchEffect = GetComponent<GlitchEffect>();
        _glitchEffect.enabled = false;

        _superBlur = GetComponent<SuperBlur.SuperBlur>();
        _superBlur.enabled = false;
    }

    public void EnableGlitch(bool start)
    {
        _glitchEffect.enabled = start;
    }

    public void EnableBlur(bool enable)
    {
        _superBlur.enabled = enable;
    }
    #endregion
}
