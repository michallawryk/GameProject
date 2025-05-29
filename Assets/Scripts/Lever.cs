using UnityEngine;

public class Lever : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        Debug.Log("DŸwignia aktywowana!");
        // animacja, zmiana stanu, uruchomienie drzwi itd.
    }
}