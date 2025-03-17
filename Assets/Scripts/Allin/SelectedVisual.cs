using UnityEngine;

/// <summary>
///     选择的视觉效果
/// </summary>
public class SelectedVisual : MonoBehaviour
{
    [SerializeField] private TriggerObject selectedObject; // 选中的物体
    [SerializeField] private GameObject selectedEffect; // 选中时的特效


    //订阅playerController中的OnTriggerObjectSelected事件
    private void Start()
    {
        PlayerController.Instance.TriggerObjectSelected += IamSelected;
    }

    public void IamSelected(object sender, PlayerController.TriggerObjectSelectedEventArgs e)
    {
        if (selectedObject == e.SelectedObject)
        {
            selectedEffect.SetActive(true);
            Debug.Log($"选中了{selectedObject.name}");
            //这只是视觉效果
        }

        if (e.SelectedObject != null) return;
        selectedEffect.SetActive(false);
        Debug.Log($"取消选中{selectedObject.name}");
    }
}