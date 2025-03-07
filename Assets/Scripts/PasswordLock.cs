using UnityEngine;
using UnityEngine.UI;

public class PasswordLock : MonoBehaviour
{
    [SerializeField] private ScrollRect[] digitScrolls;    // 4个ScrollRect
    [SerializeField] private RawImage[] digitImages;      // 使用RawImage
    [SerializeField] private Texture numberStrip;         // 使用Texture
    [SerializeField] private int[] correctPassword = {1, 2, 3, 4};
    [SerializeField] private float snapSpeed = 10f;

    private int[] currentValues;
    private float[] targetScrollPositions;
    private float numberHeight;
    private const int TOTAL_NUMBERS = 10;

    void Start()
    {
        currentValues = new int[4];
        targetScrollPositions = new float[4];
        numberHeight = numberStrip.height / TOTAL_NUMBERS;

        for (int i = 0; i < digitScrolls.Length; i++)
        {
            int index = i;
            digitImages[i].texture = numberStrip;
            digitScrolls[i].onValueChanged.AddListener((value) => OnScrollChanged(index, value));
            SetDigitDisplay(i, 0);
        }
    }

    void Update()
    {
        for (int i = 0; i < digitScrolls.Length; i++)
        {
            float currentPos = digitScrolls[i].verticalNormalizedPosition;
            float targetPos = targetScrollPositions[i];
            digitScrolls[i].verticalNormalizedPosition = Mathf.Lerp(currentPos, targetPos, Time.deltaTime * snapSpeed);

            int number = Mathf.RoundToInt((1f - digitScrolls[i].verticalNormalizedPosition) * (TOTAL_NUMBERS - 1));
            currentValues[i] = Mathf.Clamp(number, 0, TOTAL_NUMBERS - 1);
            UpdateDigitDisplay(i);
        }

        CheckPassword();
    }

    void OnScrollChanged(int index, Vector2 scrollValue)
    {
        float scrollPos = scrollValue.y;
        int nearestNumber = Mathf.RoundToInt((1f - scrollPos) * (TOTAL_NUMBERS - 1));
        nearestNumber = Mathf.Clamp(nearestNumber, 0, TOTAL_NUMBERS - 1);
        targetScrollPositions[index] = 1f - (float)nearestNumber / (TOTAL_NUMBERS - 1);
    }

    void SetDigitDisplay(int index, int number)
    {
        number = Mathf.Clamp(number, 0, TOTAL_NUMBERS - 1);
        targetScrollPositions[index] = 1f - (float)number / (TOTAL_NUMBERS - 1);
        UpdateDigitDisplay(index);
    }

    void UpdateDigitDisplay(int index)
    {
        float uvHeight = 1f / TOTAL_NUMBERS;
        float uvY = (TOTAL_NUMBERS - 1 - currentValues[index]) * uvHeight;

        digitImages[index].uvRect = new Rect(0, uvY, 1, uvHeight);
    }

    void CheckPassword()
    {
        bool isCorrect = true;
        for (int i = 0; i < 4; i++)
        {
            if (currentValues[i] != correctPassword[i])
            {
                isCorrect = false;
                break;
            }
        }

        if (isCorrect)
        {
            Debug.Log("密码正确！箱子已解锁");
        }
    }
}