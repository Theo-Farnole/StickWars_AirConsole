using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerWrapper : MonoBehaviour
{
    [SerializeField] private Image _avatar;
    [SerializeField] private Image  _outline;
    [Space]
    [SerializeField] private TextMeshProUGUI _name;

    public Image Avatar { get => _avatar; }
    public Image Outline { get => _outline; }
    public TextMeshProUGUI Name { get => _name; }
}
