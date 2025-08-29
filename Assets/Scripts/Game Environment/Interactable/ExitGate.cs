using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitGate : MonoBehaviour, IInteractable
{
    [Header("Scene to load")]
    [SerializeField] private string sceneToLoad;

    public Animator animator; // lub SpriteRenderer, je�li u�ywasz sprite'�w
    private bool isOpen = false;

    // Lista lub zbi�r obiekt�w Player, kt�re aktualnie stoj� w triggerze.
    private HashSet<GameObject> _playersInside = new HashSet<GameObject>();

    private void Awake()
    {
        // Sprawd� stan po za�adowaniu sceny
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
            animator.SetBool("IsOpen", true); // za��, �e masz parametr "IsOpen" w Animatorze
        }
        // lub np. podmie� sprite, collider itp.
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
            Debug.Log("Nie uko�czono wszystkich poziom�w!");
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
