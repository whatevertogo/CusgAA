using UnityEngine;
using UnityEngine.UI;

public class PasswordLock : MonoBehaviour
{
    // 四个滑动条，用来控制每个数字
    public Slider slider1, slider2, slider3, slider4;
    // 显示当前密码的文本
    public Text passwordDisplay;
    // 正确密码
    private string correctPassword = "1234";

    void Start()
    {
        // 初始化显示密码
        UpdatePasswordDisplay();
        
        // 监听滑动条变化
        slider1.onValueChanged.AddListener((value) => UpdatePasswordDisplay());
        slider2.onValueChanged.AddListener((value) => UpdatePasswordDisplay());
        slider3.onValueChanged.AddListener((value) => UpdatePasswordDisplay());
        slider4.onValueChanged.AddListener((value) => UpdatePasswordDisplay());
    }

    // 实时更新密码显示
    private void UpdatePasswordDisplay()
    {
        string currentPassword = Mathf.RoundToInt(slider1.value).ToString() +
                                 Mathf.RoundToInt(slider2.value).ToString() +
                                 Mathf.RoundToInt(slider3.value).ToString() +
                                 Mathf.RoundToInt(slider4.value).ToString();
        
        passwordDisplay.text = "当前密码: " + currentPassword;
    }

    // 验证密码
    public void CheckPassword()
    {
        string enteredPassword = Mathf.RoundToInt(slider1.value).ToString() +
                                 Mathf.RoundToInt(slider2.value).ToString() +
                                 Mathf.RoundToInt(slider3.value).ToString() +
                                 Mathf.RoundToInt(slider4.value).ToString();

        if (enteredPassword == correctPassword)
        {
            Debug.Log("密码正确，开锁！");
            // 开锁逻辑，比如触发动画、开启门等
        }
        else
        {
            Debug.Log("密码错误！");
        }
    }
}

