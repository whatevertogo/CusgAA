using UnityEngine;
using UnityEngine.UI;

public class DialoguePannelClicked : MonoBehaviour
{
    private Button button1;
    [SerializeField] private DialogueControl dialogueControl;

    private void Start()
    {
        button1 = GetComponent<Button>();
        button1.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        dialogueControl.SkipDialogueLines();
    }
    
    
    
}
