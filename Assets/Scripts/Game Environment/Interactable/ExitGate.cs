using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitGate : MonoBehaviour, IInteractable
{
    [Header("Scene to load")]
    [SerializeField] private string sceneToLoad;

    public Animator animator; // lub SpriteRenderer, jeœli u¿ywasz sprite'ów
    private bool isOpen = false;

    // Lista lub zbiór obiektów Player, które aktualnie stoj¹ w triggerze.
    private HashSet<GameObject> _playersInside = new HashSet<GameObject>();

    private void Awake()
    {
        // SprawdŸ stan po za³adowaniu sceny
        if (DataHandler.Instance != null && DataHandler.Instance.AreAllLevelsCompleted())
        {
            OpenGate();
        }
        else
        {
            CloseGate();
        }
    }

    public void OpenGate()
    {
        isOpen = true;
        if (animator != null)
        {
            animator.SetBool("IsOpen", true); // za³ó¿, ¿e masz parametr "IsOpen" w Animatorze
        }
        // lub np. podmieñ sprite, collider itp.
    }

    public void CloseGate()
    {
        isOpen = false;
        if (animator != null)
        {
            animator.SetBool("IsOpen", false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Volt") || other.CompareTag("Core"))
        {
            _playersInside.Add(other.gameObject);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Volt") || other.CompareTag("Core"))
        {
            _playersInside.Remove(other.gameObject);
        }
    }

    public void Interact(VoltController player)
    {
        if (!isOpen)
        {
            Debug.Log("Nie ukoñczono wszystkich poziomów!");
            return;
        }
        if (_playersInside.Count < 2) return;

        if (string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.LogWarning($"[{name}] Nie podano nazwy sceny!");
            return;
        }
        SceneManager.LoadScene(sceneToLoad);
    }
}
