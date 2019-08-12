using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    public enum Direction
    {
        Left = -1,
        Right = 1
    }
    #region Fields
    public static readonly int ANGLE_OFFSET = 15;

    [SerializeField] private Color _textColor = Color.white;
    [Space]
    [SerializeField] private float _hitForce = 200;
    [SerializeField] private float _lifetime = 0.8f;
    [Header("Components linking")]
    [SerializeField] private TextMeshPro _textMeshPro;

    [System.NonSerialized] public string text;
    [System.NonSerialized] public Direction direction;
    #endregion

    #region Methods
    void Start()
    {
        // find random angle
        int minValue = 0;
        int maxValue = 180;

        switch (direction)
        {
            case Direction.Left:
                minValue = 90;
                maxValue = 180;
                break;
            case Direction.Right:
                minValue = 0;
                maxValue = 90;
                break;
        }

        float angle = Random.Range(minValue + ANGLE_OFFSET, maxValue - ANGLE_OFFSET) * Mathf.Deg2Rad;

        // apply force
        Vector2 forceAngle = MyMath.AngleToVector2(angle);
        GetComponent<Rigidbody2D>().AddForce(forceAngle * _hitForce);

        // fade out
        _textMeshPro.text = text;
        _textMeshPro.Fade(FadeType.FadeOut, _lifetime);

        Destroy(gameObject, _lifetime);
    }
    #endregion
}
