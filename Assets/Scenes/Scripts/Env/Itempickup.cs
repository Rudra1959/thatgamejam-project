using UnityEngine;

public class ItemPickup : MonoBehaviour {
    public enum Type { Stick, Mushroom }
    public Type itemType;

    public void Pickup() {
        InventoryManager inv = GameObject.FindWithTag("Player").GetComponent<InventoryManager>();
        if (inv.CanCarryMore()) {
            inv.AddItem(itemType);
            Destroy(gameObject);
        }
    }
}