using UnityEngine;

public class DialogueController : MonoBehaviour
{
    [SerializeField] private DialogueControl dialogueControl;

    private void Start()
    {
        if (dialogueControl == null)
        {
            dialogueControl=GetComponent<DialogueControl>();
            Debug.Log("没有找到所属的dialogueControl尝试使用是否自带的component");
        }

       
        
    }

}
