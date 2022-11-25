using System.Collections;
using UnityEngine;

public class ResetOnLevelBorderExit : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;

    [SerializeField] private bool fixCoordinatesAtStart;

    [SerializeField] private bool resetRigidbodyVelocity;

    [SerializeField] private float secondsObjectNeedsToBeOutOfBounds=1.0f;

    private Vector3 _spawnPosition;

    private Quaternion _spawnRotationQuaternion;
    private Rigidbody _rigidbody;


    private Coroutine _outOfBoundsCheck;

    // Start is called before the first frame update
    void Start()
    {
        if (!(spawnPoint is null) )
        {
            if (fixCoordinatesAtStart)
            {
                _spawnPosition = spawnPoint.position;
                _spawnRotationQuaternion = spawnPoint.rotation;
            }
        }
        else
        {
            _spawnPosition = transform.position;
            _spawnRotationQuaternion = transform.rotation;
        }

        if (resetRigidbodyVelocity)
        {
            _rigidbody = GetComponent<Rigidbody>();
            if (_rigidbody is null)
            {
                resetRigidbodyVelocity = false;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("LevelBoarder"))
        {
            
            if (_outOfBoundsCheck is null)
            {
                _outOfBoundsCheck = StartCoroutine(WaitForOutOfBoundsCheck());
            }
            
        }
    }

    private IEnumerator WaitForOutOfBoundsCheck()
    {
        yield return new WaitForSeconds(secondsObjectNeedsToBeOutOfBounds);
        ResetObject();

    }

    private void ResetObject()
    {
        //use dynamic position of spawnpoint
        if (!(spawnPoint is null) && !fixCoordinatesAtStart)
        {
            transform.position = spawnPoint.position;
            transform.rotation = spawnPoint.rotation;
        }
        //use static spawnpoint (either from own transform or external spawnPoint)
        else
        {
            transform.position = _spawnPosition;
            transform.rotation = _spawnRotationQuaternion;
        }


        if (resetRigidbodyVelocity)
        {
            _rigidbody.velocity = Vector3.zero;
        }
        Debug.Log("Resetted Position of "+name);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LevelBoarder"))
        {
            
            if (!(_outOfBoundsCheck is null))
            {
                StopCoroutine(_outOfBoundsCheck);
                _outOfBoundsCheck = null;
            }
        }
    }
}