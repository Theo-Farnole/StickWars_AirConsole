using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingIconRotation : MonoBehaviour
{
    [SerializeField] private int _segmentsCount = 12;
    [SerializeField] private float _timePerSegment = 0.1f;
    [SerializeField] private bool _rotateClockwise = true;

    private float _timer = 0;

    void Update()
    {
        _timer += Time.deltaTime;

        if (_timer >= _timePerSegment)
        {
            _timer = 0;
            RotateOneSegments();
        }
    }

    void RotateOneSegments()
    {
        float angleForOneSegment = 360 / _segmentsCount;

        if (_rotateClockwise)
            angleForOneSegment *= -1; // reverse angle delta

        var eulerAngles = transform.eulerAngles;        
        eulerAngles.z += angleForOneSegment;
        transform.eulerAngles = eulerAngles;
    }
}
