using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterKnifeInterface : MonoBehaviour
{
    [SerializeField] private CharController _characterController;
    [SerializeField] private Image[] _knifeSprites;


    void OnEnable()
    {
        _characterController.OnProjectileAmountUpdated += OnProjectileAmountUpdated;
    }

    void OnDisable()
    {
        if (_characterController != null)
        {
            _characterController.OnProjectileAmountUpdated -= OnProjectileAmountUpdated;
        }
    }

    void OnProjectileAmountUpdated(CharController charController, int projectileAmount)
    {
        for (int i = 0; i < _knifeSprites.Length; i++)
        {
            bool shouldEnableImage = (i < projectileAmount);
            _knifeSprites[i].enabled = shouldEnableImage;
        }
    }
}
