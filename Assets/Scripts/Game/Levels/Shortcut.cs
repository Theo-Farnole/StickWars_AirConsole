using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shortcut : Entity
{
    #region Fields
    [Header("Shortcut")]
    [SerializeField] private GameObject _window;
    [Space]
    [SerializeField] private float _transitionDuration = 0.1f;

    private bool _isWindowOpen = false;

    // cache variables
    private Vector3 _deltaTargetPosition;
    private Image[] _images;
    #endregion

    #region Methods
    void Start()
    {
        _deltaTargetPosition = _window.transform.position - transform.position;
        _images = transform.GetComponentsInChildren<Image>();

        _window.transform.localScale = Vector3.zero;
        _window.transform.position = transform.position;
    }

    public override void GetDamage(int damage, Entity attacker)
    {
        SwitchWindowState();
    }

    private void SwitchWindowState()
    {
        _isWindowOpen = !_isWindowOpen;

        new Timer(this, _transitionDuration, (float t) =>
        {
            if (!_isWindowOpen)
            {
                t = -1 * t + 1;
            }

            _window.transform.localScale = Vector3.one * t;
            _window.transform.position = transform.position + _deltaTargetPosition * t;

            for (int i = 0; i < _images.Length; i++)
            {
                _images[i].color.SetAlpha(t);
            }
        });
    }
    #endregion
}
