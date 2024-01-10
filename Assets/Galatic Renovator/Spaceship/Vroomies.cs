using GR.Interactable;
using Unity.VisualScripting;
using UnityEngine;

namespace GR.Spaceship
{
	public class Vroomies : MonoBehaviour, IInteractable
	{
		[Header("Spaceship")]
		[SerializeField] private Transform viewTransform;
		private Player playerInside = null;
		public Transform exitPosition;
		public Cinemachine.CinemachineFreeLook camView;


		public bool CanInteract() => true;

		public void Focused()
		{
		}

		public void Interact(Player who)
		{
			who.SetCameraView(viewTransform);
			who.DisableCharacter(); // Who's there

			playerInside = who;
			who.vroomer = this;

			camView.enabled = true;
		}


		public void Exit()
		{
			playerInside.transform.position = exitPosition.position;

			playerInside.ResetCamera();
			playerInside.EnableCharacter(); // Knock knock
			camView.enabled = false;
		}
	}
}