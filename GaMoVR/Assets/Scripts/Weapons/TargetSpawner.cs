using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class TargetSpawner : MonoBehaviour
{
    // Collection checks will throw errors if we try to release an item that is already in the pool.
    public bool collectionChecks = true;

    [SerializeField]
    private GameObject _destroyableTarget;

    public IObjectPool<DestroyableTarget> m_Pool;

    [SerializeField]
    private int minXSpawn = -10;

    [SerializeField]
    private int maxXSpawn = 10;

    [SerializeField]
    private int minYSpawn = 1;

    [SerializeField]
    private int maxYSpawn = 5;

    [SerializeField]
    private int minZSpawn = -10;

    [SerializeField]
    private int maxZSpawn = 10;

    [SerializeField]
    private GallowsManager _gallowsManager;

    [SerializeField]
    private AudioSource _targetDestroyedAudioSource;

    [Space, SerializeField]
    private Transform _playerTransform;

    public IObjectPool<DestroyableTarget> Pool
    {
        get
        {
            return m_Pool;
        }
    }

    public void Awake()
    {
        m_Pool = new ObjectPool<DestroyableTarget>(
                    createFunc: CreateDestroyableTarget,
                    actionOnGet: OnTakeFromPool,
                    actionOnRelease: OnReturnToPool,
                    actionOnDestroy: OnDestroyPoolObject,
                    collectionCheck: collectionChecks,
                    defaultCapacity: 10
                );
    }

    public void Start()
    {
        InvokeRepeating("SpawnDestroyableTarget", 1, 30);
    }

    public void OnEnable()
    {
        UmlHangmanLevelValidator.OnGameFinished += StopSpawningRoutine;
    }

    public void OnDestroy()
    {
        UmlHangmanLevelValidator.OnGameFinished -= StopSpawningRoutine;
        CancelInvoke("SpawnDestroyableTarget");
    }

    public void StopSpawningRoutine(bool won, float gainedXp)
    {
        CancelInvoke("SpawnDestroyableTarget");
    }

    DestroyableTarget CreateDestroyableTarget()
    {
        var go = Instantiate(_destroyableTarget);
        var dt = go.GetComponent<DestroyableTarget>();
        dt.Pool = Pool;
        dt.GallowsManager = _gallowsManager;
        dt.TargetDestroyedAudioSource = _targetDestroyedAudioSource;
        var lookAtPlayerComponent = go.GetComponent<LookAtPlayer>();
        lookAtPlayerComponent.transformSource = _playerTransform;

        return dt;
    }

    void OnReturnToPool(DestroyableTarget target)
    {
        StartCoroutine(DelayedDeactivation(target));
    }

    void OnTakeFromPool(DestroyableTarget target)
    {
        target.gameObject.SetActive(true);
        target.transform.position = new Vector3(
            Random.Range(minXSpawn, maxXSpawn),
            Random.Range(minYSpawn, maxYSpawn),
            Random.Range(minZSpawn, maxZSpawn)
        );
        target.ResetHealth();
    }

    void OnDestroyPoolObject(DestroyableTarget target)
    {
        Destroy(target.gameObject);
    }

    public void SpawnDestroyableTarget()
    {
        Pool.Get();
    }

    private IEnumerator DelayedDeactivation(DestroyableTarget target)
    {
        yield return new WaitForEndOfFrame();
        target.gameObject.SetActive(false);
    }
}
