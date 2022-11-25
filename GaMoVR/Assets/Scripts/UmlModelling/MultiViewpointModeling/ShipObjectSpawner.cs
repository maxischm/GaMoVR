using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipObjectSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject _shipBodyPrefab;

    [SerializeField]
    private GameObject _wingPrefab;

    [SerializeField]
    private GameObject _thrusterPrefab;

    [SerializeField]
    private GameObject _cannonPrefab;

    [Space, SerializeField]
    private Transform _shipBodySpawnPosition;

    [SerializeField]
    private Transform _wingSpawnPosition;

    [SerializeField]
    private Transform _thrusterSpawnPosition;

    [SerializeField]
    private Transform _cannonSpawnPosition;

    [Space, SerializeField]
    private AudioSource _spawnSound;

    [SerializeField]
    private AudioSource _partBlockedAudioSource;

    [SerializeField]
    private AudioClip _shipBodyBlockedClip;

    [SerializeField]
    private AudioClip _wingBlockedClip;

    [SerializeField]
    private AudioClip _thrusterBlockedClip;

    [SerializeField]
    private AudioClip _cannonBlockedClip;

    [SerializeField]
    private AudioClip _classSpawnBlockedClip;

    [Space, SerializeField]
    private GameObject _shipBodySpawnButtonBlock;

    [SerializeField]
    private GameObject _wingSpawnButtonBlock;

    [SerializeField]
    private GameObject _thrusterSpawnButtonBlock;

    [SerializeField]
    private GameObject _cannonSpawnButtonBlock;

    [Space, SerializeField]
    private GameObject _shipBodyClassSpawnButtonBlock;

    [SerializeField]
    private GameObject _wingClassSpawnButtonBlock;

    [SerializeField]
    private GameObject _thrusterClassSpawnButtonBlock;

    [SerializeField]
    private GameObject _cannonClassSpawnButtonBlock;

    public void OnEnable()
    {
        UmlModel.OnPartDeleted += UpdateModelSpawnBlocks;
    }

    public void OnDestroy()
    {
        UmlModel.OnPartDeleted -= UpdateModelSpawnBlocks;
    }

    public void SpawnShipBodyAtDefaultPosition()
    {
        SpawnShipBody(_shipBodySpawnPosition.position, Quaternion.Euler(-90, 0, 90));
    }

    public GameObject SpawnShipBody(Vector3 spawnPosition, Quaternion spawnRotation, bool addPartToModel = true)
    {
        var go = Instantiate(
            _shipBodyPrefab,
            spawnPosition,
            spawnRotation
        );
        _spawnSound.Play();

        if (addPartToModel)
        {
            UmlModel.Instance.AddPartInModel(go);
        }

        if (UmlModel.Instance._shipBodyInstances.Count == UmlModel.Instance._maxNumberOfShipBodies)
        {
            _shipBodySpawnButtonBlock.SetActive(true);
        }
        if (UmlModel.Instance._shipBodyClass != null)
        {
            _shipBodyClassSpawnButtonBlock.SetActive(true);
        }

        return go;
    }

    public void SpawnWingAtDefaultPosition()
    {
        SpawnWing(_wingSpawnPosition.position, Quaternion.Euler(-90, 0, 0));
    }

    public GameObject SpawnWing(Vector3 spawnPosition, Quaternion spawnRotation, bool addPartToModel = true)
    {
        var go = Instantiate(
            _wingPrefab,
            spawnPosition,
            spawnRotation
        );
        _spawnSound.Play();
        if (addPartToModel)
        {
            UmlModel.Instance.AddPartInModel(go);
        }

        if (UmlModel.Instance._wingInstances.Count == UmlModel.Instance._maxNumberOfWings)
        {
            _wingSpawnButtonBlock.SetActive(true);
        }
        if (UmlModel.Instance._wingClass != null)
        {
            _wingClassSpawnButtonBlock.SetActive(true);
        }

        return go;
    }

    public void SpawnThrusterAtDefaultPosition()
    {
        SpawnThruster(_thrusterSpawnPosition.position, Quaternion.Euler(-90, 0, 0));
    }

    public GameObject SpawnThruster(Vector3 spawnPosition, Quaternion spawnRotation, bool addPartToModel = true)
    {
        var go = Instantiate(
            _thrusterPrefab,
            spawnPosition,
            spawnRotation
        );
        _spawnSound.Play();

        if (addPartToModel)
        {
            UmlModel.Instance.AddPartInModel(go);
        }

        if (UmlModel.Instance._thrusterInstances.Count == UmlModel.Instance._maxNumberOfThrusters)
        {
            _thrusterSpawnButtonBlock.SetActive(true);
        }
        if (UmlModel.Instance._thrusterClass != null)
        {
            _thrusterClassSpawnButtonBlock.SetActive(true);
        }

        return go;
    }

    public void SpawnCannonAtDefaultPosition()
    {
        SpawnCannon(_cannonSpawnPosition.position, Quaternion.Euler(-90, 0, 0));
    }

    public GameObject SpawnCannon(Vector3 spawnPosition, Quaternion spawnRotation, bool addPartToModel = true)
    {
        var go = Instantiate(
            _cannonPrefab,
            spawnPosition,
            spawnRotation
        );
        _spawnSound.Play();

        if (addPartToModel)
        {
            UmlModel.Instance.AddPartInModel(go);
        }

        if (UmlModel.Instance._cannonInstances.Count == UmlModel.Instance._maxNumberOfCannons)
        {
            _cannonSpawnButtonBlock.SetActive(true);
        }
        if (UmlModel.Instance._cannonClass != null)
        {
            _cannonClassSpawnButtonBlock.SetActive(true);
        }

        return go;
    }

    public void SpawnShipBodyClass()
    {
        UmlModel.Instance.AddPartInUml(false, "ShipBody");
        _spawnSound.Play();

        if (UmlModel.Instance._shipBodyInstances.Count == UmlModel.Instance._maxNumberOfShipBodies)
        {
            _shipBodySpawnButtonBlock.SetActive(true);
        }
        if (UmlModel.Instance._shipBodyClass != null)
        {
            _shipBodyClassSpawnButtonBlock.SetActive(true);
        }
    }

    public void SpawnWingClass()
    {
        UmlModel.Instance.AddPartInUml(false, "Wing");
        _spawnSound.Play();

        if (UmlModel.Instance._wingInstances.Count == UmlModel.Instance._maxNumberOfWings)
        {
            _wingSpawnButtonBlock.SetActive(true);
        }
        if (UmlModel.Instance._wingClass != null)
        {
            _wingClassSpawnButtonBlock.SetActive(true);
        }
    }

    public void SpawnEngineClass()
    {
        UmlModel.Instance.AddPartInUml(false, "Engine");
        _spawnSound.Play();

        if (UmlModel.Instance._thrusterInstances.Count == UmlModel.Instance._maxNumberOfThrusters)
        {
            _thrusterSpawnButtonBlock.SetActive(true);
        }
        if (UmlModel.Instance._thrusterClass != null)
        {
            _thrusterClassSpawnButtonBlock.SetActive(true);
        }
    }

    public void SpawnCannonClass()
    {
        UmlModel.Instance.AddPartInUml(false, "Cannon");
        _spawnSound.Play();

        if (UmlModel.Instance._cannonInstances.Count == UmlModel.Instance._maxNumberOfCannons)
        {
            _cannonSpawnButtonBlock.SetActive(true);
        }
        if (UmlModel.Instance._cannonClass != null)
        {
            _cannonClassSpawnButtonBlock.SetActive(true);
        }
    }

    public void PlayShipBodyBlockedSound()
    {
        _partBlockedAudioSource.clip = _shipBodyBlockedClip;
        _partBlockedAudioSource.Play();
    }

    public void PlayWingBlockedSound()
    {
        _partBlockedAudioSource.clip = _wingBlockedClip;
        _partBlockedAudioSource.Play();
    }

    public void PlayEngineBlockedSound()
    {
        _partBlockedAudioSource.clip = _thrusterBlockedClip;
        _partBlockedAudioSource.Play();
    }

    public void PlayCannonBlockedSound()
    {
        _partBlockedAudioSource.clip = _cannonBlockedClip;
        _partBlockedAudioSource.Play();
    }

    public void PlayClassSpawnBlockedSound()
    {
        _partBlockedAudioSource.clip = _classSpawnBlockedClip;
        _partBlockedAudioSource.Play();
    }

    public void UpdateModelSpawnBlocks(string partName)
    {
        switch (partName)
        {
            case "ShipBody":
                _shipBodySpawnButtonBlock.SetActive(false);
                if (UmlModel.Instance._shipBodyClass == null)
                {
                    _shipBodyClassSpawnButtonBlock.SetActive(false);
                }
                break;
            case "Wing":
                _wingSpawnButtonBlock.SetActive(false);
                if (UmlModel.Instance._wingClass == null)
                {
                    _wingClassSpawnButtonBlock.SetActive(false);
                }
                break;
            case "Thruster":
                _thrusterSpawnButtonBlock.SetActive(false);
                if (UmlModel.Instance._thrusterClass == null)
                {
                    _thrusterClassSpawnButtonBlock.SetActive(false);
                }
                break;
            case "Cannon":
                _cannonSpawnButtonBlock.SetActive(false);
                if (UmlModel.Instance._cannonClass == null)
                {
                    _cannonClassSpawnButtonBlock.SetActive(false);
                }
                break;
        }
    }
}
