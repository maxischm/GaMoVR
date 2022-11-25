using UnityEngine;
using UnityEngine.Events;

public class ExecuteOnStart : MonoBehaviour
{
    public UnityEvent onAwake;
    public UnityEvent onStart;

    public UnityEvent onEnable;

    // Start is called before the first frame update
    void Start()
    {
        onStart.Invoke();
    }

    private void Awake()
    {
        onAwake.Invoke();
    }

    private void OnEnable()
    {
        onEnable.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
    }
}