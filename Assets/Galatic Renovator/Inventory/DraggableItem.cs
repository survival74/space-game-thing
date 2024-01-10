using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GR.Inventory
{
	public class DraggableItem : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
	{
		public Item item { get; private set; }
		public TMP_Text countText;

		[SerializeField] private Image image = null;
		[SerializeField] public int itemCount { get; private set; } = 0;


		// Dragging variables
		private Vector2 clickOffset = Vector2.zero;
		private bool dragging;
		private Transform dragFrom = null;


		#region Drag Handling
		// Handle dragging
		public void OnPointerDown(PointerEventData mouse)
		{
			dragging = true;
			clickOffset = (Vector2)transform.position - mouse.position;

			image = GetComponent<Image>();
			image.raycastTarget = false;

			dragFrom = transform.parent;
			transform.SetParent(transform.root);
		}


		public void OnPointerUp(PointerEventData mouse) => SetDropParent();

		private void Update()
		{
			if (dragging)
			{
				transform.position = clickOffset + (Vector2)Input.mousePosition;
				InventoryManager.instance.dragging = this;
			}
		}
		#endregion Dragging

		private void Start() => RefreshCount();

		public void RefreshCount()
		{
			countText.text = itemCount.ToString();
			countText.gameObject.SetActive(itemCount > 1);
		}


		public void SetItem(Item inItem, int amount = 1)
		{
			image.sprite = inItem.Icon;
			item = inItem;

			itemCount = amount;
			RefreshCount();
		}

		public int StackAvailable()
		{
			if (item == null) return 0;
			return item.MaxStack - itemCount;
		}

		public void Add(int amount = 1)
		{
			itemCount = Math.Clamp(itemCount + amount, 0, item.MaxStack);
			if (itemCount <= 0)
				Destroy(gameObject);
			else if (itemCount > item.MaxStack)
				InventoryManager.instance.SpawnItem(Player.localPlayer, item, amount - (itemCount - item.MaxStack));

			RefreshCount();
		}


		public void SetDropParent(Transform slot = null)
		{
			if (slot != null) dragFrom = slot;

			InventoryManager.instance.dragging = null;
			dragging = false;
			transform.SetParent(dragFrom);
			image.rectTransform.position = dragFrom.position;
			image.raycastTarget = true;
		}

		public void OnDrag(PointerEventData eventData)
		{
		}
	}
}