using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float moveSpeed = 3f;       // 移动速度
    public float moveDistance = 5f;    // 往返的最大距离

    private Vector3 startPos;          // 起始位置
    private int direction = 1;         // 1 表示往右，-1 表示往左

    void Start()
    {
        startPos = transform.position; // 记录初始位置
    }

    void Update()
    {
        // 沿着 X 轴移动
        transform.position += Vector3.right * direction * moveSpeed * Time.deltaTime;

        // 如果超过范围，就反转方向
        if (Mathf.Abs(transform.position.x - startPos.x) >= moveDistance)
        {
            direction *= -1;
        }
    }
}
