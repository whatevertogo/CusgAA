using Managers;
using UnityEngine;
using UnityEngine.UI;

public class ItemButton : MonoBehaviour
{
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => InventoryManager.Instance.ShowInventory());
    }
    
    
}
