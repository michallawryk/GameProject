using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneInitializer : MonoBehaviour
{
    [SerializeField] private GameObject voltPrefab;
    [SerializeField] private GameObject corePrefab;

    private void Start()
    {
        // Gracz 1
        var voltSpawn = GameObject.Find("VoltSpawnPoint").transform;
        var volt = Instantiate(voltPrefab, voltSpawn.position, Quaternion.identity);

        // Gracz 2
        var coreSpawn = GameObject.Find("CoreSpawnPoint").transform;
        var core = Instantiate(corePrefab, coreSpawn.position, Quaternion.identity);

        DataHandler.Instance.GameData.currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
    }
}
