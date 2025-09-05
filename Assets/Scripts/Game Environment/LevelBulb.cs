using UnityEngine;

public class LevelBulb : MonoBehaviour
{
    [SerializeField] private GameObject onIcon;
    [SerializeField] private GameObject offIcon;

    public void SetState(bool completed)
    {
        if (onIcon) onIcon.SetActive(completed);
        if (offIcon) offIcon.SetActive(!completed);
    }
}
