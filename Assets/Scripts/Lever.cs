using UnityEngine;

public class Lever : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        Debug.Log("D�wignia aktywowana!");
        // animacja, zmiana stanu, uruchomienie drzwi itd.
    }
}