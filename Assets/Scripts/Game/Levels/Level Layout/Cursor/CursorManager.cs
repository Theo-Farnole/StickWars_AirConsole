using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField] private float _speed = 3;

    void Update()
    {
        InGameCursorFollowRealCursor();
    }

    private void InGameCursorFollowRealCursor()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        MoveTo(mousePosition);
    }

    void MoveTo(Vector3 targetPosition)
    {
        // security: lock cursor position
        targetPosition.z = transform.position.z;

        Vector3 direction = targetPosition - transform.position;
        transform.position += direction * (_speed * Time.deltaTime);
    }
}
