using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    public static GameManager GetInstanceOrCreate()
    {
        return instance == null ? new GameObject("GameManager").AddComponent<GameManager>() : instance;
    }
}
