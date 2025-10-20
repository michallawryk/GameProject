using UnityEngine;

public class ControlPanel : MonoBehaviour, IInteractable
{
    [SerializeField] private ControllablePlatform platform;
    public bool isBeingUsed { get; private set; } = false;
    private VoltController activePlayer;
    
    public void Interact(VoltController player)
    {
        if (platform == null || player == null)
        {
            return;
        }

        if (!isBeingUsed && player != null)
        {
            Debug.Log("Aktywacja");
            isBeingUsed = true;
            activePlayer = player;
            player.EnterPanelControl(this);
        }
        else if (isBeingUsed && player == activePlayer)
        {
            Debug.Log("Dezaktywacja");
            StopControlling(player);
        }
    }

    public void SetPlatformInput(Vector2 input)
    {
        if (isBeingUsed && platform != null)
            platform.SetMoveInput(input);
    }

    // Trigger do wykrywania wyjœcia gracza z panelu
    private void OnTriggerExit2D(Collider2D other)
    {
        VoltController player = other.GetComponent<VoltController>();
        if (player != null && player == activePlayer)
        {
            StopControlling(player);
        }
    }

    private void StopControlling(VoltController player)
    {
        isBeingUsed = false;
        player.ExitPanelControl();
        activePlayer = null;
        platform.StopPlatform();
    }
}
