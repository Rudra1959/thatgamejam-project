using UnityEngine;

public class InventoryManager : MonoBehaviour {
    public int sticks = 0;
    public int mushrooms = 0;
    public int maxItems = 50;
    public GameObject inventoryUI; 

    // This is the missing function causing the error
    public bool ConsumeSticks(int amount) {
        if (sticks >= amount) {
            sticks -= amount;
            return true;
        }
        return false;
    }

    public bool CanCarryMore() => (sticks + mushrooms) < maxItems;

    public void AddItem(ItemPickup.Type type) {
        if (!CanCarryMore()) return;
        if (type == ItemPickup.Type.Stick) sticks++;
        else if (type == ItemPickup.Type.Mushroom) mushrooms++;
    }

    public void ToggleInventory() {
        if (inventoryUI != null) inventoryUI.SetActive(!inventoryUI.activeSelf);
    }
}