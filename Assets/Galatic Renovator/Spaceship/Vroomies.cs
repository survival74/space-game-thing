using GR.Interactable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GR.Spaceship
{
	public class Vroomies : MonoBehaviour, IInteractable
	{
		[Header("Spaceship")]
		[SerializeField] private Transform viewTransform;


		public bool CanInteract() => true;

		public void Focused()
		{
		}

		public void Interact(Player who)
		{
			who.SetCameraView(viewTransform);
		}
	}
}