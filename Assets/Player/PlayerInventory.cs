using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public int hotbarSize = 10;
    public HotbarSlot hotbarTemplate;
    HotbarSlot[] hotbarSlots;
    Item[] items;
    GameObject activeItemGM;
    Item activeItem;

    int currentItem;

    void Awake()
    {
        
    }
    
    void Start()
    {
        items = new Item[hotbarSize];
        hotbarSlots = new HotbarSlot[hotbarSize];
        for (int i = 0; i < hotbarSize; i++)
        {
            hotbarSlots[i] = Instantiate(hotbarTemplate, hotbarTemplate.transform.parent);
            hotbarSlots[i].index.text = (i + 1).ToString();
        }
        hotbarTemplate.gameObject.SetActive(false);
    }
    
    void Update()
    {
        // Set ouline enabled of current item
        for (int i = 0; i < hotbarSize; i++)
        {
            hotbarSlots[i].SetOutlineHighlight(i == currentItem);
            if (items[i]){
                hotbarSlots[i].icon.sprite = items[i].itemImage;
                hotbarSlots[i].icon.gameObject.SetActive(true);
            } else{
                hotbarSlots[i].icon.gameObject.SetActive(false);
            }
        }

        if(Input.GetKeyDown(KeyCode.Alpha1)){
            SetActiveHotbarSlot(0);
        }
        if(Input.GetKeyDown(KeyCode.Alpha2)){
            SetActiveHotbarSlot(1);
        }
        if(Input.GetKeyDown(KeyCode.Alpha3)){
            SetActiveHotbarSlot(2);
        }
        if(Input.GetKeyDown(KeyCode.Alpha4)){
            SetActiveHotbarSlot(3);
        }
        if(Input.GetKeyDown(KeyCode.Alpha5)){
            SetActiveHotbarSlot(4);
        }
        if(Input.GetKeyDown(KeyCode.Alpha6)){
            SetActiveHotbarSlot(5);
        }
        if(Input.GetKeyDown(KeyCode.Alpha7)){
            SetActiveHotbarSlot(6);
        }
        if(Input.GetKeyDown(KeyCode.Alpha8)){
            SetActiveHotbarSlot(7);
        }
        if(Input.GetKeyDown(KeyCode.Alpha9)){
            SetActiveHotbarSlot(8);
        }
        if(Input.GetKeyDown(KeyCode.Alpha0)){
            SetActiveHotbarSlot(9);
        }
    }

    void SetActiveHotbarSlot(int slot){
        if (slot < 0 || slot >= hotbarSize){
            return;
        }
        currentItem = slot;
        UpdateActiveItem();
    }

    public bool AddItem(Item item){
        // Prefer active slot
        if (items[currentItem] == null){
            items[currentItem] = item;
            UpdateActiveItem();
            return true;
        }
        for (int i = 0; i < hotbarSize; i++)
        {
            if (items[i] == null){
                items[i] = item;
                UpdateActiveItem();
                return true;
            }
        }
        return false;
    }

    void UpdateActiveItem(){
        // Ignore if we are already correct
        if (activeItem == items[currentItem]){
            return;
        }
        activeItem = items[currentItem];
        if (activeItemGM){
            Destroy(activeItemGM);
        }
        if (items[currentItem]){
            if(activeItem.gun){
                activeItemGM = Instantiate(items[currentItem].gun.gameObject, transform);
            } else{
                print("Not supported active item");
            }
            activeItemGM.transform.parent = GetComponent<PlayerMovement>().head.transform;
            activeItemGM.transform.localRotation = Quaternion.identity;
            activeItemGM.transform.localPosition = Vector3.zero;
        }
    }
}
