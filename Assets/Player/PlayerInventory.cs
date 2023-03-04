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

    int activeItemIndex;

    public GameObject pickupPrefab;

    Dictionary<string, int> ammo = new Dictionary<string, int>();

    
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
            hotbarSlots[i].SetOutlineHighlight(i == activeItemIndex);
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
        // Cycle through items with the scroll wheel
        if (Input.mouseScrollDelta.y < 0){
            SetActiveHotbarSlot(activeItemIndex+1);
        }
        if (Input.mouseScrollDelta.y > 0){
            SetActiveHotbarSlot(activeItemIndex-1);
        }

        if (activeItem != null && Input.GetKeyDown(KeyCode.Q)){
            var d = Instantiate(pickupPrefab, transform.position+transform.forward*2, Quaternion.identity);
            d.GetComponent<PickupOnHit>().item = activeItem;
            items[activeItemIndex] = null;
            UpdateActiveItem();
        }
    }

    void SetActiveHotbarSlot(int slot){
        if (slot < 0){
            SetActiveHotbarSlot(slot+hotbarSize);
        } else if (slot >= hotbarSize){
            SetActiveHotbarSlot(slot-hotbarSize);
        } else{
            activeItemIndex = slot;
            UpdateActiveItem();
        }
    }

    public bool AddItem(Item item){
        // Prefer active slot
        if (items[activeItemIndex] == null){
            items[activeItemIndex] = item;
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

    public void EmptyCurrentHotbarSlot(){
        items[activeItemIndex] = null;
        UpdateActiveItem();
    }

    void UpdateActiveItem(){
        // Ignore if we are already correct
        if (activeItem == items[activeItemIndex]){
            return;
        }
        activeItem = items[activeItemIndex];
        if (activeItemGM){
            Destroy(activeItemGM);
        }
        if (items[activeItemIndex]){
            if(activeItem.gun){
                activeItemGM = Instantiate(items[activeItemIndex].gun.gameObject, transform);
            } else if(activeItem.sword){
                activeItemGM = Instantiate(items[activeItemIndex].sword.gameObject, transform);
            }else if(activeItem.toolbox){
                activeItemGM = Instantiate(items[activeItemIndex].toolbox.gameObject, transform);
            } else{
                print("Not supported active item");
            }
            activeItemGM.transform.parent = GetComponent<PlayerMovement>().head.transform;
            activeItemGM.transform.localRotation = Quaternion.identity;
            activeItemGM.transform.localPosition = Vector3.zero;
        }
    }

    public void AddAmmo(string type, int amount){
        if (ammo.ContainsKey(type)){
            ammo[type] += amount;
        } else{
            ammo[type] = amount;
        }
    }

    public bool HasAmmo(string type){
        if (ammo.ContainsKey(type)){
            return ammo[type] >= 1;
        }
        return false;
    }

    public void ConsumeAmmo(string type){
        if (ammo.ContainsKey(type)){
            ammo[type] -= 1;
        }
    }
}
