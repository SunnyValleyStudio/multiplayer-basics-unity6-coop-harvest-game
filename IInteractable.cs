using Unity.Netcode;

public interface IInteractable
{
    void ToggleSelection(bool isSelected);
    NetworkObject NetworkObject { get; }
}
