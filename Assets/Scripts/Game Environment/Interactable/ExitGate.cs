using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitGate : MonoBehaviour, IInteractable
{
    [Header("Scene to load")]
    [SerializeField] private string sceneToLoad;

    public Animator animator; 
    private bool isOpen = false;

    private HashSet<GameObject> _playersInside = new HashSet<GameObject>();

    [SerializeField] private List<LevelBulb> bulbs;

    private void Awake()
    {
        bool[] levelCompleted = DataHandler.Instance.GetCompletedLevels();

        for (int i = 0; i < levelCompleted.Length; i++)
        {
            bulbs[i].SetState(levelCompleted[i]);
        }

        foreach (var level in levelCompleted)
        {
            if (!level)
            {
                CloseGate();
                return;
            }
        }

        OpenGate();
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
