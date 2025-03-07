using UnityEngine;

public class ShakingLight : MonoBehaviour
{
    public Vector3 rotationCenter = new Vector3(1.75f, 8.8f, 0); // 旋转中心点
    public float rotationSpeed = 45f; // 旋转速度，每秒旋转的角度
    public float maxAngle = 90f; // 每次旋转的最大角度
    private float currentAngle = 0f; // 当前旋转角度
    private bool clockwise = true; // 是否顺时针旋转，默认为顺时针

    void Update()
    {
        // 确定旋转的方向
        float direction = clockwise ? -1f : 1f;

        // 使用 RotateAround 来让物体围绕指定点旋转
        transform.RotateAround(rotationCenter, Vector3.forward, direction * rotationSpeed * Time.deltaTime);

        // 更新当前旋转角度
        currentAngle += direction * rotationSpeed * Time.deltaTime;

        // 如果当前旋转角度超过了设定的最大角度，则反转旋转方向
        if (Mathf.Abs(currentAngle) >= maxAngle)
        {
            clockwise = !clockwise;  // 切换旋转方向
            currentAngle = 0f;  // 重置旋转角度
        }
    }
}
