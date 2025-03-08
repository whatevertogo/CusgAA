using UnityEngine;

public class SelectedVisual : MonoBehaviour
{
    [SerializeField] private TriggerObject selectedObject; // 选中的物体
    [SerializeField] private GameObject selectedEffect; // 选中时的特效


    //订阅playerController中的OnTriggerObjectSelected事件
    void Start()
    {
        PlayerController.Instance.OnTriggerObjectSelected += IamSelected;
    }

    public void IamSelected(object sender, PlayerController.TriggerObjectSelectedEventArgs e)
    {
        if (selectedObject == e.SelectedObject)
        {
            selectedEffect.SetActive(true);
            Debug.Log($"选中了{selectedObject.name}");
        }
        if (e.SelectedObject == null)
        {
            selectedEffect.SetActive(false);
            Debug.Log($"取消选中{selectedObject.name}");
        }
    }
    



}
