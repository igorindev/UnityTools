using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Managers")]
    [SerializeField] GameMode gameMode;
    [SerializeField] PlayerManagerController playerController;

    [Header("Events")]
    [SerializeField] UnityEvent onStart;
    [SerializeField] UnityEvent onAwake;

    void Awake()
    {
        instance = this;

        onAwake?.Invoke();
    }
    void Start()
    {
        onStart?.Invoke();
    }

    void Update()
    {
        
    }
}
