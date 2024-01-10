using GR.Interactable;
using UnityEngine;

namespace GR.Inventory
{
	public class Pickupable : MonoBehaviour, IInteractable
	{
		[Header("Item")]
		public Item item;
		public int itemCount = 1;

		[Header("Pickup/Drop Properties")]
		[SerializeField] private float dropForce = 10.0f;
		[SerializeField] private float throwForce = 2.0f;
		
		[Space]
		[SerializeField] private float torqueForce = 10.0f;

		private Rigidbody rb;
		private Collider col;
		private bool wasColliderTrigger = false;

		private void Awake()
		{
			rb = GetComponent<Rigidbody>();
			col = GetComponent<Collider>();
		}

		public bool CanInteract() => true;

		public void Focused()
		{
		}

		public void Interact(Player who)
		{
			//if (isLocalPlayer)
			//{
			InventoryManager inventory = InventoryManager.instance;
			inventory.AddItem(item, itemCount);
			OnPickedUp(who);
			//}
		}

		public void Pickup(Player who) => Interact(who);

		public void Drop(Player who, bool wasSpawned = false)
		{
			if (wasSpawned)
				wasColliderTrigger = col.isTrigger;
			OnDropped(who);
		}


		protected void OnPickedUp(Player who)
		{
			if (rb != null) rb.isKinematic = true;
			if (col != null && col.isTrigger)
			{
				wasColliderTrigger = true;
				col.isTrigger = false;
			}

			Destroy(gameObject);
		}

		protected void OnDropped(Player who)
		{
			transform.position = who.cam.transform.position + who.cam.transform.forward * 0.5f;
			transform.SetParent(null);

			if (col != null && wasColliderTrigger)
			{
				col.isTrigger = true;
				wasColliderTrigger = false;
			}

			if (rb != null)
			{
				rb.isKinematic = false;
				if (who.TryGetComponent(out Rigidbody playerRb))
					rb.velocity = playerRb.velocity;
			}

			Transform cam = who.cam.transform;
			rb.AddForce(cam.forward * dropForce, ForceMode.Impulse);
			rb.AddForce(cam.up * throwForce, ForceMode.Impulse);

			float rndTq = Random.Range(-torqueForce, torqueForce);
			rb.AddTorque(new Vector3(rndTq * (-1.0f * rndTq % 2), rndTq * (1.0f * rndTq % 2), rndTq * (-1.0f * rndTq % 2)));
		}
	}
}
