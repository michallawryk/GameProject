using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorSceneLoader : MonoBehaviour, IInteractable
{
    [Header("Animacja")]
    [SerializeField] private Animator animator;
    private static readonly int IsOpen = Animator.StringToHash("IsOpen");

    [Header("Scene to load")]
    [SerializeField] private string sceneToLoad;

    // Lista lub zbiór obiektów Player, które aktualnie stoj¹ w triggerze.
    private HashSet<GameObject> _playersInside = new HashSet<GameObject>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Volt") || other.CompareTag("Core"))
        {
            _playersInside.Add(other.gameObject);
            if (animator != null && _playersInside.Count >= 2) animator.SetBool(IsOpen, true);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Volt") || other.CompareTag("Core"))
        {
            _playersInside.Remove(other.gameObject);
            if (animator != null && _playersInside.Count < 2) animator.SetBool(IsOpen, false);
        }
    }

    public void Interact(VoltController player)
    {
        if (_playersInside.Count < 2)
        {
            Debug.Log($"[{name}] Potrzebne s¹ obie postacie, by otworzyæ drzwi. Aktualnie w triggerze: {_playersInside.Count}");
            return;
        }

        if (string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.LogWarning($"[{name}] Nie podano nazwy sceny!");
            return;
        }
        SceneManager.LoadScene(sceneToLoad);
    }
}
