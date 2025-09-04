using UnityEngine;
using UnityEngine.InputSystem;

public class AAAMovement1 : MonoBehaviour
{
    public float moveSpeed = 5f;

    void Update()
    {
        Vector3 move = Vector3.zero;

        // 上下（Y 轴）
        if (Keyboard.current.wKey.isPressed)
            move += Vector3.up;    // Y+
        if (Keyboard.current.sKey.isPressed)
            move += Vector3.down;  // Y-

        // 左右（X 轴）
        if (Keyboard.current.aKey.isPressed)
            move += Vector3.left;  // X-
        if (Keyboard.current.dKey.isPressed)
            move += Vector3.right; // X+

        if (move != Vector3.zero)
        {
            move.Normalize();
            transform.position += move * moveSpeed * Time.deltaTime;
        }
    }
}



