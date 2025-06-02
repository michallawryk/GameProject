using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorSceneLoader : MonoBehaviour, IInteractable
{
    [Header("Scene to load")]
    [SerializeField] private string sceneToLoad;

    // Lista lub zbi�r obiekt�w Player, kt�re aktualnie stoj� w triggerze.
    private HashSet<GameObject> _playersInside = new HashSet<GameObject>();

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

    public void Interact()
    {
        if (_playersInside.Count < 2)
        {
            Debug.Log($"[{name}] Potrzebne s� obie postacie, by otworzy� drzwi. Aktualnie w triggerze: {_playersInside.Count}");
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
