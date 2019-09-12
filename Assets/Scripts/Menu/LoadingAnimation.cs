using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadingAnimation : MonoBehaviour
{
    #region Fields
    [SerializeField] private float _threshold = 1;

    private TextMeshProUGUI _text;
    #endregion

    #region Methods
    void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        StartCoroutine(TextAnimation());
    }

    IEnumerator TextAnimation()
    {
        string[] texts = new string[] { "Loading", "Loading.", "Loading..", "Loading..." };

        int index = 0;

        while (true)
        {
            _text.text = texts[index];

            yield return new WaitForSeconds(_threshold);

            index++;
            index = MyMath.InverseClamp(index, 0, texts.Length - 1);
        }
    }
    #endregion
}
