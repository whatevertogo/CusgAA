using Managers;
using UnityEngine;
using UnityEngine.UI;

public class ItemButton : MonoBehaviour
{
    
    [SerializeField] private ItemsManagerUI itemsManagerUI;
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() =>itemsManagerUI.ShowInventory());
    }
    
    
}
