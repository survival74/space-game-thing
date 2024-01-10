using UnityEngine;

namespace GR.Inventory
{
	public enum ItemType
	{
		Resource,
		Tool
	}

	[CreateAssetMenu(menuName = "Inventory/Item")]
	public class Item : ScriptableObject
	{
		[Header("Item")]
		public string Name;
		public Sprite Icon;
		public ItemType Type;

		[Space]
		public GameObject Prefab;

		[Header("Inventory Properties")]
		public int MaxStack = 1;
	}
}