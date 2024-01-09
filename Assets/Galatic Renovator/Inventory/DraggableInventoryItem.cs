using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace GR.Inventory
{
	public class DraggableInventoryItem : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
	{
		private Image ItemImage = null;

		private Vector2 clickOffset = Vector2.zero;

		private bool dragging;
		private Transform dragFrom = null;


		public void OnPointerDown(PointerEventData mouse)
		{
			dragging = true;
			clickOffset = (Vector2)transform.position - mouse.position;

			ItemImage = GetComponent<Image>();

			dragFrom = transform.parent;
			ItemImage.raycastTarget = false;

			transform.SetParent(transform.root);
		}


		public void OnPointerUp(PointerEventData mouse)
		{
			dragging = false;
			transform.SetParent(dragFrom);
			ItemImage.rectTransform.position = Vector3.zero;
			ItemImage.raycastTarget = true;
		}

		public void OnDrag(PointerEventData mouse)
		{
			if (dragging)
				transform.position = clickOffset + mouse.position;
		}
	}
}