using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textSceneName;

    void Awake()
    {
        _textSceneName.text = SceneManager.GetActiveScene().name;
    }
}
