using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

/// <summary>
/// Class responsible for handling the shooting of the player with their pistol(s).
/// </summary>
public class PlayerPistolShootingHandler : MonoBehaviour
{
    /// <summary>
    /// Input action reference to attach the pistol shooting to.
    /// </summary>
    [SerializeField]
    private InputActionReference _triggerAction;

    /// <summary>
    /// Prefab of the bullet GameObject to spawn for shooting.
    /// </summary>
    [SerializeField]
    private GameObject _bulletPrefab;

    /// <summary>
    /// Position of the barrel to shoot from.
    /// </summary>
    [SerializeField]
    private Transform _barrelPosition;

    /// <summary>
    /// Audio Source to play the gunshot from.
    /// </summary>
    [SerializeField]
    private AudioSource _gunshotAudioSource;

    /// <summary>
    /// Particle System to spawn when shooting for visual effect.
    /// </summary>
    [SerializeField]
    private ParticleSystem _muzzleFlashParticleSystem;

    /// <summary>
    /// Damage inflicted per shot.
    /// </summary>
    [SerializeField]
    private float damagePerShot = 10;

    public IObjectPool<Bullet> Pool;

    public void Start()
    {
        Pool = new ObjectPool<Bullet>(
            createFunc: CreateBullet,
            actionOnGet: OnTakeBulletFromPool,
            actionOnRelease: OnReleasedToPool,
            actionOnDestroy: OnDestroyPoolObject,
            defaultCapacity: 30);
    }

    /// <summary>
    /// Adds the shooting function to the trigger action.
    /// </summary>
    public void ActivateGunplay()
    {
        _triggerAction.action.started += Shoot;
    }

    /// <summary>
    /// Removes the shooting function from the trigger action.
    /// </summary>
    public void DeactivateGunplay()
    {
        _triggerAction.action.started -= Shoot;
    }

    /// <summary>
    /// Shoots a bullet in the direction the gun points at.
    /// </summary>
    /// <param name="obj">Callback context of the input action created by pulling the trigger.</param>
    public void Shoot(InputAction.CallbackContext obj)
    {
        // Play gunshot audio & visual effects
        _gunshotAudioSource.Play();
        _muzzleFlashParticleSystem.Play();

        // Perform raycast to check if the player hit something
        Vector3 direction = transform.forward;
        Bullet bullet = Pool.Get();
        bullet.transform.position = _barrelPosition.position;

        Physics.Raycast(_barrelPosition.position, direction, out RaycastHit hit, float.MaxValue);

        // No hit, set default endpoint for gunshot trail to have a target
        if (hit.point.Equals(Vector3.zero))
        {
            hit.point = _barrelPosition.position + (200 * direction);
        }

        // Spawn gunshot trail
        StartCoroutine(bullet.SpawnTrail(hit));

        // If an object with health was hit => decrease health
        var hitObjectHealth = hit.transform?.GetComponentInParent<Health>();
        if (hitObjectHealth != null)
        {
            hitObjectHealth?.Hit(damagePerShot, WeaponTypeTag.Gun);
        }
    }

    public Bullet CreateBullet()
    {
        var go = Instantiate(_bulletPrefab, Vector3.zero, Quaternion.identity);
        var bullet = go.GetComponent<Bullet>();
        bullet.Pool = Pool;

        return bullet;
    }

    void OnTakeBulletFromPool(Bullet b)
    {
        b.gameObject.SetActive(true);
    }

    void OnReleasedToPool(Bullet b)
    {
        b.gameObject.SetActive(false);
    }

    void OnDestroyPoolObject(Bullet b)
    {
        Destroy(b.gameObject);
    }
}
