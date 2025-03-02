using Managers;
using UnityEngine;

public class ItemsManagerUI : MonoBehaviour
{
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private Transform itemsContainer;


    void Start()
    {
        UpdateVisual();
    }



    public void UpdateVisual()
    {
        // foreach(Transform child in itemsContainer)
        // {
        //     Destroy(child.gameObject);
        // }
        //
        // foreach(Items_SO itemsSO in InventoryManager.Instance.items)
        // {
        //   
        //   
        // }
        
    }

}
