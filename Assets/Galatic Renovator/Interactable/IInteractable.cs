namespace GR.Interactable
{
	public interface IInteractable
	{
		void Interact(Player who);
		void Focused();
		bool CanInteract();
	}
}