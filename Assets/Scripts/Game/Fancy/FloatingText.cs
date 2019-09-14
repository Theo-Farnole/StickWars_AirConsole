using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour, IPooledObject
{
    public enum Direction
    {
        Left = -1,
        Right = 1
    }

    #region Fields
    public static readonly int ANGLE_OFFSET = 15;

    [SerializeField] private float _hitForce = 200;
    [SerializeField] private float _lifetime = 0.8f;
    [Header("Components linking")]
    [SerializeField] private TextMeshPro _text;

    [HideInInspector] public Direction direction;

    private Rigidbody2D _rb;
    #endregion

    #region Properties
    public TextMeshPro Text { get => _text; }
    #endregion

    #region Methods
    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public void OnObjectSpawn()
    {
#if UNITY_EDITOR
        DynamicsObjects.Instance.SetToParent(transform, "floating_text");
#endif

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
        _rb.AddForce(forceAngle * _hitForce);

        // fade out
        _text.Fade(FadeType.FadeOut, _lifetime);

        ObjectPooler.Instance.EnqueueGameObject("floating_text", gameObject, _lifetime);
    }

    void OnDisable()
    {
        _text.text = string.Empty;
        _text.color = Color.white;

        _rb.velocity = Vector3.zero;
    }
    #endregion
}
