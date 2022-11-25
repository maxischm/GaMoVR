using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// Class representing a bullet & handling the effects of a shot.
/// </summary>
public class Bullet : MonoBehaviour
{
    public IObjectPool<Bullet> Pool;

    /// <summary>
    /// Particle system played on bullet impact.
    /// </summary>
    [SerializeField]
    private ParticleSystem _impactParticleSystem;

    /// <summary>
    /// Trail renderer used for displaying the bullet tracer.
    /// </summary>
    [SerializeField]
    private TrailRenderer _bulletTrail;

    /// <summary>
    /// Spawns a trail representing the bullet tracer.
    /// </summary>
    /// <param name="Hit">Hit of the gunshot.</param>
    public IEnumerator SpawnTrail(RaycastHit Hit)
    {
        float time = 0;
        Vector3 startPosition = transform.position;

        while (time < 3)
        {
            _bulletTrail.transform.position = Vector3.Lerp(startPosition, Hit.point, time);
            time += Time.deltaTime / _bulletTrail.time;

            yield return null;
        }

        _bulletTrail.transform.position = Hit.point;

        if (Hit.transform != null)
        {
            _impactParticleSystem.transform.position = Hit.point;
            _impactParticleSystem.transform.rotation = Quaternion.LookRotation(Hit.normal);

            var main = _impactParticleSystem.main;
            main.stopAction = ParticleSystemStopAction.Callback;

            _impactParticleSystem.Play();
        }
        else
        {
            Pool.Release(this);
        }
    }

    public void OnParticleSystemStopped()
    {
        _bulletTrail.transform.localPosition = Vector3.zero;
        Pool.Release(this);
    }
}
