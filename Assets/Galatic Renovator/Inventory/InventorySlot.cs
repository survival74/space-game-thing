using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace GR.Inventory
{
	public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDropHandler
	{
		[Header("Slot Properties")]
		[SerializeField] private Color HoverColor;
		[SerializeField] private Color SelectedColor;
		[SerializeField] private Color NormalColor;

		[HideInInspector] public bool selected = false;
		private Image image;

		private void Awake()
		{
			image = GetComponent<Image>();
			image.color = NormalColor;
		}


		public DraggableItem GetItem()
		{
			if (transform.childCount == 0
				|| !transform.GetChild(0).TryGetComponent(out DraggableItem itemInSlot))
				return null;
			return itemInSlot;
		}


		#region Mouse/pointer handling
		public void OnPointerEnter(PointerEventData eventData)
		{
			image.color = HoverColor;
			InventoryManager.instance.hoveredSlot = this;
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			if (selected)
				image.color = SelectedColor;
			else
				image.color = NormalColor;
			InventoryManager.instance.hoveredSlot = null;
		}

		public void OnDrop(PointerEventData mouse)
		{
			DraggableItem dragItem;
			if (!mouse.pointerDrag.TryGetComponent(out dragItem)) { return; }

			DraggableItem slotItem = GetItem();
			if (transform.childCount != 0 && slotItem != dragItem)
			{
				if (dragItem.item == slotItem.item)
				{
					int maxStack = slotItem.item.MaxStack;
					if (slotItem.itemCount + dragItem.itemCount <= maxStack)
					{
						slotItem.Add(dragItem.itemCount);
						Destroy(dragItem.gameObject);
					}
				}
				return;
			}
			
			dragItem.SetDropParent(transform);
		}
		#endregion
	}
}