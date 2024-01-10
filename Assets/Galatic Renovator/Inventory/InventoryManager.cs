using UnityEngine;

namespace GR.Inventory
{
	public class InventoryManager : MonoBehaviour
	{
		public static InventoryManager instance { get; private set; }
		public GameObject inventoryView;

		public InventorySlot[] itemSlots;
		public GameObject DraggablePrefab;

		[HideInInspector] public InventorySlot selectedSlot;
		[HideInInspector] public InventorySlot hoveredSlot;
		[HideInInspector] public DraggableItem dragging;


		private void Awake() => instance = this;

		public void Select(InventorySlot slot)
		{
			selectedSlot.selected = false;

			selectedSlot = slot;
			slot.selected = true;
		}


		public bool AddItem(Item item, int amount = 1)
		{
			foreach (InventorySlot slot in itemSlots)
			{
				DraggableItem itemFrame = slot.GetItem();
				if (itemFrame == null)
				{
					SetItem(item, slot, amount);
					return true;
				}
				else if (itemFrame.item == item && itemFrame.StackAvailable() >= amount)
				{
					itemFrame.Add(amount);
					return true;
				}
			}

			return false;
		}

		public void SetItem(Item item, InventorySlot slot, int amount = 1)
		{
			DraggableItem inItem = slot.GetItem();
			if (inItem != null) Destroy(inItem);

			GameObject dragItem = Instantiate(DraggablePrefab, slot.transform);
			
			inItem = dragItem.GetComponent<DraggableItem>();
			inItem.SetItem(item, amount);
		}


		public void SpawnItem(Player fromPlayer, Item item, int amount = 1)
		{
			GameObject itemGo = Instantiate(item.Prefab);
			itemGo.transform.position = fromPlayer.transform.position;

			if (itemGo.TryGetComponent(out Pickupable sc))
			{
				sc.itemCount = amount;
				sc.Drop(fromPlayer);
			}
		}

		public void DropItem(Player who)
		{
			DraggableItem itemFrame = null;
			if (dragging != null)
				itemFrame = dragging;
			else if (hoveredSlot != null)
				itemFrame = hoveredSlot.GetItem();
			else if (selectedSlot != null)
				itemFrame = selectedSlot.GetItem();

			if (itemFrame != null)
			{
				SpawnItem(who, itemFrame.item);
				itemFrame.Add(-1);
			}
		}



		private static bool wasProcessEnabled = false;
		private static bool wasCursorLocked = false;
		public static void Toggle()
		{
			GameObject inv = instance.inventoryView;
			inv.SetActive(!inv.activeSelf);

			if (inv.activeSelf)
			{
				wasProcessEnabled = Player.localPlayer.processUpdate;
				wasCursorLocked = Cursor.lockState != CursorLockMode.None;

				Player.localPlayer.processUpdate = false;
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
			}
			else
			{

				Player.localPlayer.processUpdate = wasProcessEnabled;
				if (wasCursorLocked)
				{
					Cursor.lockState = CursorLockMode.Locked;
					Cursor.visible = false;
				}

				wasProcessEnabled = false;
				wasCursorLocked = false;

				instance.hoveredSlot?.OnPointerExit(null);
			}
		}
	}
}
