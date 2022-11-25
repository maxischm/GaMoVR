using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UmlModel : GenericSingletonClass<UmlModel>
{
    [SerializeField]
    private ShipObjectSpawner _spawner;

    [SerializeField]
    private GameObject _classPrefab;

    [SerializeField]
    private GameObject _connectionPrefab;

    [Space, SerializeField]
    private Transform _partSpawnPosition;

    [Space, SerializeField]
    public GameObject _shipBodyClass;

    [SerializeField]
    public List<Transform> _shipBodyInstances = new();

    [SerializeField]
    public GameObject _wingClass;

    [SerializeField]
    public List<Transform> _wingInstances = new();

    [SerializeField]
    public GameObject _thrusterClass;

    [SerializeField]
    public List<Transform> _thrusterInstances = new();

    [SerializeField]
    public GameObject _cannonClass;

    [SerializeField]
    public List<Transform> _cannonInstances = new();

    [Space, SerializeField]
    private GameObject _shipBodyWingConnection;

    [SerializeField]
    private GameObject _shipBodyThrusterConnection;

    [SerializeField]
    private GameObject _shipBodyCannonConnection;

    [SerializeField]
    private GameObject _wingThrusterConnection;

    [SerializeField]
    private GameObject _wingCannonConnection;

    [Space]
    public int _maxNumberOfShipBodies;

    public int _maxNumberOfWings;

    public int _maxNumberOfThrusters;

    public int _maxNumberOfCannons;

    [Space, SerializeField]
    private AudioSource _successSound;

    [SerializeField]
    private AudioSource _spawnSound;

    public delegate void UmlModelUpdate(string partName);
    public static event UmlModelUpdate OnPartDeleted;

    [SerializeField]
    private List<Transform> _classSpawnPointTransforms;

    private List<Vector3> _classSpawnPositions = new();

    public override void Awake()
    {
        base.Awake();
        _classSpawnPointTransforms.ForEach(t => _classSpawnPositions.Add(t.position));
    }

    public void AddPartInUml(bool usePreDefinedPosition, string className)
    {
        var playerComponents = LocalPlayerReference.Instance.LocalPlayer.GetComponentInChildren<XRPlayerComponents>().transform;
        var spawnPosition = usePreDefinedPosition
            ? _classSpawnPositions[0]
            : playerComponents.position + (playerComponents.forward * 2);
        spawnPosition.y -= 0.5f;
        // spawn class w/ name ship body in & setup
        var spawnedClass = Instantiate(
            original: _classPrefab,
            position: spawnPosition,
            rotation: Quaternion.identity);

        var structure = spawnedClass.GetComponent<UmlClassStructure>();
        structure.Init(className);
        structure.InitPosition = spawnPosition;

        spawnedClass.GetComponent<UmlBuildingClassHealth>().className = className;
        if (usePreDefinedPosition)
        {
            _classSpawnPositions.RemoveAt(0);
            spawnedClass.GetComponent<UmlClassStructure>().InitPosition = spawnPosition;
        }

        GameObject go;

        switch (className)
        {
            case "ShipBody":
                go = _spawner.SpawnShipBody(_partSpawnPosition.position, Quaternion.Euler(0, -90, 0), false);
                _shipBodyInstances.Add(go.transform);
                _shipBodyClass = spawnedClass;
                break;
            case "Wing":
                go = _spawner.SpawnWing(_partSpawnPosition.position, Quaternion.Euler(-90, 0, 0), false);
                _wingInstances.Add(go.transform);
                _wingClass = spawnedClass;
                break;
            case "Thruster":
                go = _spawner.SpawnThruster(_partSpawnPosition.position, Quaternion.Euler(0, -90, 0), false);
                _thrusterInstances.Add(go.transform);
                _thrusterClass = spawnedClass;
                break;
            case "Cannon":
                go = _spawner.SpawnCannon(_partSpawnPosition.position, Quaternion.Euler(0, -90, 0), false);
                _cannonInstances.Add(go.transform);
                _cannonClass = spawnedClass;
                break;
        }
    }

    public void AddPartInModel(GameObject part)
    {
        var partType = part.GetComponent<UmlBuildingPart>();
        if (partType is UmlBuildingShipBody)
        {
            _shipBodyInstances.Add(part.transform);
            if (_shipBodyClass == null)
            {
                // spawn class w/ name ship body in & setup
                _shipBodyClass = Instantiate(_classPrefab, _classSpawnPositions[0], Quaternion.identity);
                _shipBodyClass.GetComponent<UmlClassStructure>().Init("ShipBody", _classSpawnPositions[0]);
                _classSpawnPositions.RemoveAt(0);
                _shipBodyClass.GetComponent<UmlBuildingClassHealth>().className = "ShipBody";
            }
            if (_shipBodyWingConnection != null)
            {
                ApplyShipBodyWingUmlConnection();
            }
            if (_shipBodyThrusterConnection != null)
            {
                ApplyShipBodyThrusterUmlConnection();
            }
            if (_shipBodyCannonConnection != null)
            {
                ApplyShipBodyCannonUmlConnection();
            }
        }
        else if (partType is UmlBuildingWing)
        {
            _wingInstances.Add(part.transform);
            if (_wingClass == null)
            {
                // spawn class w/ name ship body in & setup
                _wingClass = Instantiate(_classPrefab, _classSpawnPositions[0], Quaternion.identity);
                _wingClass.GetComponent<UmlClassStructure>().Init("Wing", _classSpawnPositions[0]);
                _classSpawnPositions.RemoveAt(0);
                _wingClass.GetComponent<UmlBuildingClassHealth>().className = "Wing";
            }
            if (_shipBodyWingConnection != null)
            {
                ApplyShipBodyWingUmlConnection();
            }
            if (_wingThrusterConnection != null)
            {
                ApplyWingThrusterUmlConnection();
            }
            if (_wingCannonConnection != null)
            {
                ApplyWingCannonUmlConnection();
            }
        }
        else if (partType is UmlBuildingThruster)
        {
            _thrusterInstances.Add(part.transform);
            if (_thrusterClass == null)
            {
                // spawn class w/ name ship body in & setup
                _thrusterClass = Instantiate(_classPrefab, _classSpawnPositions[0], Quaternion.identity);
                _thrusterClass.GetComponent<UmlClassStructure>().Init("Thruster", _classSpawnPositions[0]);
                _classSpawnPositions.RemoveAt(0);
                _thrusterClass.GetComponent<UmlBuildingClassHealth>().className = "Thruster";
            }
            if (_shipBodyThrusterConnection != null)
            {
                ApplyShipBodyThrusterUmlConnection();
            }
            if (_wingThrusterConnection != null)
            {
                ApplyWingThrusterUmlConnection();
            }
        }
        else if (partType is UmlBuildingCannon)
        {
            _cannonInstances.Add(part.transform);
            if (_cannonClass == null)
            {
                // spawn class w/ name ship body in & setup
                _cannonClass = Instantiate(_classPrefab, _classSpawnPositions[0], Quaternion.identity);
                _cannonClass.GetComponent<UmlClassStructure>().Init("Cannon", _classSpawnPositions[0]);
                _classSpawnPositions.RemoveAt(0);
                _cannonClass.GetComponent<UmlBuildingClassHealth>().className = "Cannon";
            }
            if (_shipBodyCannonConnection != null)
            {
                ApplyShipBodyCannonUmlConnection();
            }
            if (_wingCannonConnection != null)
            {
                ApplyWingCannonUmlConnection();
            }
        }
    }

    public void SpawnConnection()
    {
        var playerComponents = LocalPlayerReference.Instance.LocalPlayer.GetComponentInChildren<XRPlayerComponents>().transform;
        var spawnPosition = playerComponents.position + playerComponents.forward;
        spawnPosition.y -= 0.5f;
        // spawn class w/ name ship body in & setup
        var spawnedConnection = Instantiate(_connectionPrefab, spawnPosition, Quaternion.identity);
        spawnedConnection.GetComponent<UmlConnectorInitializer>().InitConnector("attaches to");

        _spawnSound.Play();
    }

    public bool AddConnectionInUml(string origin, string target, GameObject connection)
    {
        if (origin == "Wing" && target == "ShipBody" && _shipBodyWingConnection == null)
        {
            UmlBuildModeRobotController.Instance.StartBuildModeIntro5();

            _shipBodyWingConnection = connection;
            ApplyShipBodyWingUmlConnection();
            _successSound.Play();
            return true;
        }
        else if (origin == "Thruster" && target == "ShipBody" && _shipBodyThrusterConnection == null)
        {
            _shipBodyThrusterConnection = connection;
            ApplyShipBodyThrusterUmlConnection();
            _successSound.Play();
            return true;
        }
        else if (origin == "Cannon" && target == "ShipBody")
        {
            _shipBodyCannonConnection = connection;
            ApplyShipBodyCannonUmlConnection();
            _successSound.Play();
            return true;
        }
        else if (origin == "Thruster" && target == "Wing")
        {
            _wingThrusterConnection = connection;
            ApplyWingThrusterUmlConnection();
            _successSound.Play();
            return true;
        }
        else if (origin == "Cannon" && target == "Wing")
        {
            _wingCannonConnection = connection;
            ApplyWingCannonUmlConnection();
            _successSound.Play();
            return true;
        }

        return false;
    }

    public void ApplyShipBodyWingUmlConnection()
    {
        if (_shipBodyWingConnection == null)
        {
            return;
        }
        foreach (var looseWing in _wingInstances.Where(wing =>
            !_shipBodyInstances.Contains(wing.parent?.parent) || wing.parent == null))
        {
            foreach (var shipBody in _shipBodyInstances)
            {
                if (shipBody.GetComponent<UmlBuildingShipBody>()._wingAttach1.childCount == 0)
                {
                    looseWing.SetParent(shipBody.GetComponent<UmlBuildingShipBody>()._wingAttach1);
                    looseWing.localPosition = Vector3.zero;
                    // looseWing.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                    looseWing.localRotation = Quaternion.identity;
                    break;
                }
                else if (shipBody.GetComponent<UmlBuildingShipBody>()._wingAttach2.childCount == 0)
                {
                    looseWing.SetParent(shipBody.GetComponent<UmlBuildingShipBody>()._wingAttach2);
                    looseWing.localPosition = Vector3.zero;
                    // looseWing.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                    looseWing.localRotation = Quaternion.identity;
                    break;
                }
            }
        }
        UmlBuildModeRobotController.Instance.PlayBuildModeExplanationSpawnAutoAttach();
    }

    public void ApplyShipBodyThrusterUmlConnection()
    {
        if (_shipBodyThrusterConnection == null)
        {
            return;
        }
        foreach (var looseThruster in _thrusterInstances.Where(thruster =>
        thruster.transform.parent == null
            || (!_shipBodyInstances.Contains(thruster.parent.parent) && !_wingInstances.Contains(thruster.parent.parent))))
        {
            foreach (var shipBody in _shipBodyInstances)
            {
                if (shipBody.GetComponent<UmlBuildingShipBody>()._thrusterAttach1.childCount == 0)
                {
                    looseThruster.SetParent(shipBody.GetComponent<UmlBuildingShipBody>()._thrusterAttach1);
                    looseThruster.localPosition = Vector3.zero;
                    looseThruster.localRotation = Quaternion.identity;
                    // looseThruster.localScale = new Vector3(0.6f, 0.6f, 0.6f);
                    break;
                }
                else if (shipBody.GetComponent<UmlBuildingShipBody>()._thrusterAttach2.childCount == 0)
                {
                    looseThruster.SetParent(shipBody.GetComponent<UmlBuildingShipBody>()._thrusterAttach2);
                    looseThruster.localPosition = Vector3.zero;
                    looseThruster.localRotation = Quaternion.identity;
                    // looseThruster.localScale = new Vector3(0.6f, 0.6f, 0.6f);
                    break;
                }
            }
        }
        UmlBuildModeRobotController.Instance.PlayBuildModeExplanationSpawnAutoAttach();
    }

    public void ApplyWingThrusterUmlConnection()
    {
        if (_wingThrusterConnection == null)
        {
            return;
        }
        foreach (var looseThruster in _thrusterInstances.Where(thruster =>
            thruster.parent == null
            || (!_shipBodyInstances.Contains(thruster.parent.parent) && !_wingInstances.Contains(thruster.parent.parent))))
        {
            foreach (var wing in _wingInstances)
            {
                if (wing.GetComponent<UmlBuildingWing>()._thrusterAttach1.childCount == 0)
                {
                    looseThruster.SetParent(wing.GetComponent<UmlBuildingWing>()._thrusterAttach1);
                    looseThruster.localPosition = Vector3.zero;
                    looseThruster.localRotation = Quaternion.identity;
                    // looseThruster.localScale = new Vector3(0.6f, 0.6f, 0.6f);
                    break;
                }
            }
        }
        UmlBuildModeRobotController.Instance.PlayBuildModeExplanationSpawnAutoAttach();
    }

    public void ApplyShipBodyCannonUmlConnection()
    {
        if (_shipBodyCannonConnection == null)
        {
            return;
        }
        foreach (var looseCannon in _cannonInstances.Where(cannon =>
            cannon.parent == null
            || (!_shipBodyInstances.Contains(cannon.parent.parent) && !_wingInstances.Contains(cannon.parent.parent))))
        {
            foreach (var shipBody in _shipBodyInstances)
            {
                if (shipBody.GetComponent<UmlBuildingShipBody>()._cannonAttach1.childCount == 0)
                {
                    looseCannon.SetParent(shipBody.GetComponent<UmlBuildingShipBody>()._cannonAttach1);
                    looseCannon.localPosition = Vector3.zero;
                    looseCannon.localRotation = Quaternion.identity;
                    // looseCannon.localScale = new Vector3(1, 1, 1);
                    break;
                }
            }
        }
        UmlBuildModeRobotController.Instance.PlayBuildModeExplanationSpawnAutoAttach();
    }

    public void ApplyWingCannonUmlConnection()
    {
        if (_wingCannonConnection == null)
        {
            return;
        }
        foreach (var looseCannon in _cannonInstances.Where(cannon =>
            cannon.parent == null
            || (!_shipBodyInstances.Contains(cannon.parent.parent) && !_wingInstances.Contains(cannon.parent.parent))))
        {
            foreach (var wing in _wingInstances)
            {
                if (wing.GetComponent<UmlBuildingWing>()._cannonAttach1.childCount == 0)
                {
                    looseCannon.SetParent(wing.GetComponent<UmlBuildingWing>()._cannonAttach1);
                    looseCannon.localPosition = Vector3.zero;
                    looseCannon.localRotation = Quaternion.identity;
                    // looseCannon.localScale = new Vector3(1, 1, 1);
                    break;
                }
            }
        }
        UmlBuildModeRobotController.Instance.PlayBuildModeExplanationSpawnAutoAttach();
    }

    public void AddConnectionInModel(UmlBuildingPart part1, UmlBuildingPart part2)
    {
        // Spawn connection & set up
        var association = Instantiate(_connectionPrefab, Vector3.zero, Quaternion.identity);
        association.GetComponent<UmlConnectorInitializer>().InitConnector("attaches to");
        if (part1 is UmlBuildingShipBody && part2 is UmlBuildingWing && _shipBodyWingConnection == null)
        {
            UmlBuildModeRobotController.Instance.StartBuildModeIntro5();
            _shipBodyWingConnection = association;
            PositionConnection(_wingClass, _shipBodyClass, association);
            // Model part already handled
        }
        else if (part1 is UmlBuildingShipBody && part2 is UmlBuildingThruster && _shipBodyThrusterConnection == null)
        {
            _shipBodyThrusterConnection = association;
            PositionConnection(_thrusterClass, _shipBodyClass, association);
        }
        else if (part1 is UmlBuildingShipBody && part2 is UmlBuildingCannon && _shipBodyCannonConnection == null)
        {
            _shipBodyCannonConnection = association;
            PositionConnection(_cannonClass, _shipBodyClass, association);
        }
        else if (part1 is UmlBuildingWing && part2 is UmlBuildingThruster && _wingThrusterConnection == null)
        {
            _wingThrusterConnection = association;
            PositionConnection(_thrusterClass, _wingClass, association);
        }
        else if (part1 is UmlBuildingWing && part2 is UmlBuildingCannon && _wingCannonConnection == null)
        {
            _wingCannonConnection = association;
            PositionConnection(_cannonClass, _wingClass, association);
        }

        _successSound.Play();
    }

    public void PositionConnection(GameObject originClass, GameObject targetClass, GameObject connection)
    {
        // Find the classes & connect them them
        var targetClassPosition = originClass.transform.position;
        var originClassPosition = targetClass.transform.position;

        var middlePoint = (targetClassPosition + originClassPosition) / 2;

        var targetClassAttachPoint = targetClass.GetComponent<BoxCollider>().ClosestPoint(middlePoint);
        var originClassAttachPoint = originClass.GetComponent<BoxCollider>().ClosestPoint(middlePoint);

        var targetGrabVolume = connection.transform.Find("TargetGrabVolume");
        targetGrabVolume.transform.position = targetClassAttachPoint;
        targetGrabVolume.GetComponent<ConnectorGrabVolume>().intersectedClass = targetClass;
        targetGrabVolume.GetComponent<ConnectorGrabVolume>().OnGrabEnd(true);

        var originGrabVolume = connection.transform.Find("OriginGrabVolume");
        originGrabVolume.transform.position = originClassAttachPoint;
        originGrabVolume.GetComponent<ConnectorGrabVolume>().intersectedClass = originClass;
        originGrabVolume.GetComponent<ConnectorGrabVolume>().OnGrabEnd(true);

        // Final position update
        targetGrabVolume.GetComponent<ConnectorGrabVolume>().connector.UpdateTransform();
    }

    public void RemoveConnectionInModel(UmlBuildingPart part1, UmlBuildingPart part2)
    {
        if (part1 is UmlBuildingShipBody && part2 is UmlBuildingWing)
        {
            bool connectionsInModelRemain = false;
            foreach (var shipBodyInstance in _shipBodyInstances)
            {
                if ((shipBodyInstance.GetComponent<UmlBuildingShipBody>()._wingAttach1.childCount > 0
                        && shipBodyInstance.GetComponent<UmlBuildingShipBody>()
                            ._wingAttach1.GetChild(0).GetComponent<UmlBuildingPart>() != part2)
                    || (shipBodyInstance.GetComponent<UmlBuildingShipBody>()._wingAttach2.childCount > 0
                        && shipBodyInstance.GetComponent<UmlBuildingShipBody>()
                            ._wingAttach2.GetChild(0).GetComponent<UmlBuildingPart>() != part2))
                {
                    connectionsInModelRemain = true;
                    break;
                }
            }
            if (!connectionsInModelRemain)
            {
                // Destroy UML _shipBodyWingConnection as well
                RemoveConnectionInUml(_shipBodyWingConnection, false);
            }
        }
        else if (part1 is UmlBuildingShipBody && part2 is UmlBuildingThruster)
        {
            bool connectionsInModelRemain = false;
            foreach (var shipBodyInstance in _shipBodyInstances)
            {
                if ((shipBodyInstance.GetComponent<UmlBuildingShipBody>()._thrusterAttach1.childCount > 0
                        && shipBodyInstance.GetComponent<UmlBuildingShipBody>()
                            ._thrusterAttach1.GetChild(0).GetComponent<UmlBuildingPart>() != part2)
                    || (shipBodyInstance.GetComponent<UmlBuildingShipBody>()._thrusterAttach2.childCount > 0
                        && shipBodyInstance.GetComponent<UmlBuildingShipBody>()
                            ._thrusterAttach2.GetChild(0).GetComponent<UmlBuildingPart>() != part2))
                {
                    connectionsInModelRemain = true;
                    break;
                }
            }
            if (!connectionsInModelRemain)
            {
                // Destroy UML _shipBodyThrusterConnection as well
                RemoveConnectionInUml(_shipBodyThrusterConnection, false);
            }
        }
        else if (part1 is UmlBuildingShipBody && part2 is UmlBuildingCannon)
        {
            bool connectionsInModelRemain = false;
            foreach (var shipBodyInstance in _shipBodyInstances)
            {
                if (shipBodyInstance.GetComponent<UmlBuildingShipBody>()._cannonAttach1.childCount > 0
                        && shipBodyInstance.GetComponent<UmlBuildingShipBody>()
                            ._cannonAttach1.GetChild(0).GetComponent<UmlBuildingPart>() != part2)
                {
                    connectionsInModelRemain = true;
                    break;
                }
            }
            if (!connectionsInModelRemain)
            {
                // Destroy UML _shipBodyCannonConnection as well
                RemoveConnectionInUml(_shipBodyCannonConnection, false);
            }
        }
        else if (part1 is UmlBuildingWing && part2 is UmlBuildingThruster)
        {
            bool connectionsInModelRemain = false;
            foreach (var wingInstance in _wingInstances)
            {
                if (wingInstance.GetComponent<UmlBuildingWing>()._thrusterAttach1.childCount > 0
                        && wingInstance.GetComponent<UmlBuildingWing>()
                            ._thrusterAttach1.GetChild(0).GetComponent<UmlBuildingPart>() != part2)
                {
                    connectionsInModelRemain = true;
                    break;
                }
            }
            if (!connectionsInModelRemain)
            {
                // Destroy UML _wingThrusterConnection as well
                RemoveConnectionInUml(_wingThrusterConnection, false);
            }
        }
        else if (part1 is UmlBuildingWing && part2 is UmlBuildingCannon)
        {
            bool connectionsInModelRemain = false;
            foreach (var wingInstance in _wingInstances)
            {
                if (wingInstance.GetComponent<UmlBuildingWing>()._cannonAttach1.childCount > 0
                        && wingInstance.GetComponent<UmlBuildingWing>()
                            ._cannonAttach1.GetChild(0).GetComponent<UmlBuildingPart>() != part2)
                {
                    connectionsInModelRemain = true;
                    break;
                }
            }
            if (!connectionsInModelRemain)
            {
                // Destroy UML _wingCannonConnection as well
                RemoveConnectionInUml(_wingCannonConnection, false);
            }
        }
        else
        {
            RemoveConnectionInModel(part2, part1);
        }

        _successSound.Play();
    }

    public void RemoveConnectionInUml(GameObject connection, bool reposition = true, bool destroyConnectionObject = true)
    {
        if (connection == null)
        {
            return;
        }
        if (_shipBodyWingConnection == connection)
        {
            if (reposition)
            {
                if (_wingInstances.Count > 0)
                {
                    _wingInstances[0].position -= _shipBodyInstances[0].right * 1.5f;
                    _wingInstances[0].parent = null;
                }
                if (_wingInstances.Count > 1)
                {
                    _wingInstances[1].position += _shipBodyInstances[0].right * 1.5f;
                    _wingInstances[1].parent = null;
                }
            }

            if (destroyConnectionObject)
            {
                // Destroy connection GameObject
                Destroy(connection);
            }
            else
            {
                _shipBodyWingConnection = null;
            }
        }
        else if (_shipBodyThrusterConnection == connection)
        {
            _thrusterInstances.ForEach(thruster =>
                {
                    if (reposition && thruster.parent?.parent.GetComponent<UmlBuildingPart>() is UmlBuildingShipBody)
                    {
                        if (thruster.parent.name == "ThrusterAttach1")
                        {
                            thruster.position += thruster.parent.parent.right * 1.5f;
                        }
                        else if (thruster.parent.name == "ThrusterAttach2")
                        {
                            thruster.position -= thruster.parent.parent.right * 1.5f;
                        }
                    }
                    thruster.parent = null;
                }
            );

            ApplyWingThrusterUmlConnection();

            if (destroyConnectionObject)
            {
                // Destroy connection GameObject
                Destroy(connection);
            }
            else
            {
                _shipBodyThrusterConnection = null;
            }
        }
        else if (_shipBodyCannonConnection == connection)
        {
            _cannonInstances.ForEach(cannon =>
                {
                    if (reposition && cannon.parent?.parent.GetComponent<UmlBuildingPart>() is UmlBuildingShipBody)
                    {
                        cannon.position += cannon.parent.parent.forward;
                        cannon.position -= cannon.parent.parent.up;
                    }
                    cannon.parent = null;
                }
            );

            ApplyWingCannonUmlConnection();

            if (destroyConnectionObject)
            {
                // Destroy connection GameObject
                Destroy(connection);
            }
            else
            {
                _shipBodyCannonConnection = null;
            }
        }
        else if (_wingThrusterConnection == connection)
        {
            _thrusterInstances.ForEach(thruster =>
                {
                    if (reposition && thruster.parent?.parent.GetComponent<UmlBuildingPart>() is UmlBuildingWing)
                    {
                        if (thruster.parent.parent.parent?.name == "WingAttach1")
                        {
                            thruster.position -= thruster.parent.parent.right;
                        }
                        else
                        {
                            thruster.position += thruster.parent.parent.right;
                        }
                    }
                    thruster.parent = null;
                }
            );

            ApplyShipBodyThrusterUmlConnection();

            if (destroyConnectionObject)
            {
                // Destroy connection GameObject
                Destroy(connection);
            }
            else
            {
                _wingThrusterConnection = null;
            }
        }
        else if (_wingCannonConnection == connection)
        {
            _cannonInstances.ForEach(cannon =>
                {
                    if (reposition && cannon.parent?.parent.GetComponent<UmlBuildingPart>() is UmlBuildingWing)
                    {
                        if (cannon.parent.parent.parent?.name == "WingAttach1")
                        {
                            cannon.position -= cannon.parent.parent.right;
                        }
                        else
                        {
                            cannon.position += cannon.parent.parent.right;
                        }
                    }
                    cannon.parent = null;
                }
            );

            ApplyShipBodyCannonUmlConnection();

            if (destroyConnectionObject)
            {
                // Destroy connection GameObject
                Destroy(connection);
            }
            else
            {
                _wingCannonConnection = null;
            }
        }
        _successSound.Play();
        UmlBuildModeRobotController.Instance.PlayBuildModeExplanationDeleteConnectionAutoAttach();
    }

    public void RemovePartInUml(string className)
    {
        switch (className)
        {
            case "ShipBody":
                if (_shipBodyClass.GetComponent<UmlClassStructure>().InitPosition != Vector3.zero)
                {
                    _classSpawnPositions.Add(_shipBodyClass.GetComponent<UmlClassStructure>().InitPosition);
                }

                RemoveConnectionInUml(_shipBodyWingConnection);
                RemoveConnectionInUml(_shipBodyThrusterConnection);
                RemoveConnectionInUml(_shipBodyCannonConnection);
                _shipBodyInstances.ForEach(shipBody => Destroy(shipBody.gameObject));
                Destroy(_shipBodyClass);

                _shipBodyClass = null;
                ApplyWingThrusterUmlConnection();
                ApplyWingCannonUmlConnection();
                _shipBodyInstances.Clear();
                break;
            case "Wing":
                if (_wingClass.GetComponent<UmlClassStructure>().InitPosition != Vector3.zero)
                {
                    _classSpawnPositions.Add(_wingClass.GetComponent<UmlClassStructure>().InitPosition);
                }

                RemoveConnectionInUml(_shipBodyWingConnection);
                RemoveConnectionInUml(_wingThrusterConnection);
                RemoveConnectionInUml(_wingCannonConnection);
                _wingInstances.ForEach(wing => Destroy(wing.gameObject));
                Destroy(_wingClass);

                _wingClass = null;
                ApplyShipBodyThrusterUmlConnection();
                ApplyShipBodyCannonUmlConnection();
                _wingInstances.Clear();
                break;
            case "Thruster":
                if (_thrusterClass.GetComponent<UmlClassStructure>().InitPosition != Vector3.zero)
                {
                    _classSpawnPositions.Add(_thrusterClass.GetComponent<UmlClassStructure>().InitPosition);
                }

                RemoveConnectionInUml(_shipBodyThrusterConnection);
                RemoveConnectionInUml(_wingThrusterConnection);
                _thrusterInstances.ForEach(thruster => Destroy(thruster.gameObject));
                Destroy(_thrusterClass);

                _thrusterClass = null;
                _thrusterInstances.Clear();
                break;
            case "Cannon":
                if (_cannonClass.GetComponent<UmlClassStructure>().InitPosition != Vector3.zero)
                {
                    _classSpawnPositions.Add(_cannonClass.GetComponent<UmlClassStructure>().InitPosition);
                }

                RemoveConnectionInUml(_shipBodyCannonConnection);
                RemoveConnectionInUml(_wingCannonConnection);
                _cannonInstances.ForEach(cannon => Destroy(cannon.gameObject));
                Destroy(_cannonClass);

                _cannonClass = null;
                _cannonInstances.Clear();
                break;
        }

        _successSound.Play();
        UmlBuildModeRobotController.Instance.PlayBuildModeExplanationDeleteClassAutoAttach();

        OnPartDeleted?.Invoke(className);
    }

    public void RemovePartInModel(UmlBuildingPart part)
    {
        if (part is UmlBuildingShipBody)
        {
            if (_shipBodyInstances.Count == 1)
            {
                RemovePartInUml("ShipBody");
            }
            else
            {
                _shipBodyInstances.Remove(part.transform);

                // Check if connections of wing must be removed in UML
                if (_shipBodyWingConnection != null) // 1. wing ship body connection
                {
                    bool shipBodyWingConExists = false;
                    foreach (var wing in _wingInstances)
                    {
                        if (wing.parent != null)
                        {
                            shipBodyWingConExists = true;
                            break;
                        }
                    }
                    if (!shipBodyWingConExists)
                    {
                        RemoveConnectionInUml(_shipBodyWingConnection);
                    }
                }

                if (_shipBodyThrusterConnection != null) // 2. ship body thruster connection
                {
                    bool shipBodyThrusterConExists = false;
                    foreach (var thruster in _thrusterInstances)
                    {
                        if (thruster.parent != null
                            && _shipBodyInstances.Contains(thruster.parent.parent)
                            && thruster.parent.parent != part.transform
                        )
                        {
                            shipBodyThrusterConExists = true;
                            break;
                        }
                    }
                    if (!shipBodyThrusterConExists)
                    {
                        RemoveConnectionInUml(_shipBodyThrusterConnection);
                    }
                }

                if (_shipBodyCannonConnection != null) // 3. ship body cannon connection
                {
                    bool shipBodyCannonConExists = false;
                    foreach (var cannon in _cannonInstances)
                    {
                        if (cannon.parent != null
                            && _shipBodyInstances.Contains(cannon.parent.parent)
                            && cannon.parent.parent != part.transform
                        )
                        {
                            shipBodyCannonConExists = true;
                        }
                    }
                    if (!shipBodyCannonConExists)
                    {
                        RemoveConnectionInUml(_shipBodyCannonConnection);
                    }
                }

                (part as UmlBuildingShipBody)?.RemoveSelf();

                // Resolve other connections
                if (_wingThrusterConnection != null)
                {
                    ApplyWingThrusterUmlConnection();
                }
                if (_wingCannonConnection != null)
                {
                    ApplyWingCannonUmlConnection();
                }
            }

            OnPartDeleted?.Invoke("ShipBody");
        }
        else if (part is UmlBuildingWing)
        {
            if (_wingInstances.Count == 1)
            {
                RemovePartInUml("Wing");
            }
            else
            {
                _wingInstances.Remove(part.transform);

                // Check if connections of wing must be removed in UML
                if (_shipBodyWingConnection != null) // 1. wing ship body connection
                {
                    bool shipBodyWingConExists = false;
                    foreach (var wing in _wingInstances)
                    {
                        if (wing.parent != null)
                        {
                            shipBodyWingConExists = true;
                            break;
                        }
                    }
                    if (!shipBodyWingConExists)
                    {
                        RemoveConnectionInUml(_shipBodyWingConnection);
                    }
                }

                if (_wingThrusterConnection != null) // 2. wing thruster connection
                {
                    bool wingThrusterConExists = false;
                    foreach (var thruster in _thrusterInstances)
                    {
                        if (thruster.parent != null
                            && _wingInstances.Contains(thruster.parent.parent)
                            && thruster.parent.parent != part.transform
                        )
                        {
                            wingThrusterConExists = true;
                            break;
                        }
                    }
                    if (!wingThrusterConExists)
                    {
                        RemoveConnectionInUml(_wingThrusterConnection);
                    }
                }

                if (_wingCannonConnection != null) // 3. wing cannon connection
                {
                    bool wingCannonConExists = false;
                    foreach (var cannon in _cannonInstances)
                    {
                        if (cannon.parent != null
                            && _wingInstances.Contains(cannon.parent.parent)
                            && cannon.parent.parent != part.transform
                        )
                        {
                            wingCannonConExists = true;
                        }
                    }
                    if (!wingCannonConExists)
                    {
                        RemoveConnectionInUml(_wingCannonConnection);
                    }
                }

                (part as UmlBuildingWing)?.RemoveSelf();

                // Resolve other existing connections
                if (_shipBodyThrusterConnection != null)
                {
                    ApplyShipBodyThrusterUmlConnection();
                }
                if (_shipBodyCannonConnection != null)
                {
                    ApplyShipBodyCannonUmlConnection();
                }
            }

            OnPartDeleted?.Invoke("Wing");
        }
        else if (part is UmlBuildingThruster)
        {
            if (_thrusterInstances.Count == 1)
            {
                RemovePartInUml("Thruster");
            }
            else
            {
                _thrusterInstances.Remove(part.transform);

                // Check if due to removing of part, connection in UML must be removed (although more instances exist)
                if (part.transform.parent != null && _shipBodyInstances.Contains(part.transform.parent.parent))
                {
                    // Check if ship body thruster connection still exists somewhere
                    bool conExists = false;
                    foreach (var thruster in _thrusterInstances)
                    {
                        if (thruster.parent != null && _shipBodyInstances.Contains(thruster.parent.parent))
                        {
                            conExists = true;
                            break;
                        }
                    }
                    if (!conExists)
                    {
                        RemoveConnectionInUml(_shipBodyThrusterConnection);
                    }
                }
                else if (part.transform.parent != null && _wingInstances.Contains(part.transform.parent.parent))
                {
                    // Check if wing thruster connection still exists somewhere
                    bool conExists = false;
                    foreach (var thruster in _thrusterInstances)
                    {
                        if (thruster.parent != null && _wingInstances.Contains(thruster.parent.parent))
                        {
                            conExists = true;
                            break;
                        }
                    }
                    if (!conExists)
                    {
                        RemoveConnectionInUml(_wingThrusterConnection);
                    }
                }
            }

            OnPartDeleted?.Invoke("Thruster");
        }
        else if (part is UmlBuildingCannon)
        {
            if (_cannonInstances.Count == 1)
            {
                RemovePartInUml("Cannon");
            }
            else
            {
                _cannonInstances.Remove(part.transform);

                // Check if due to removing of part connection in UML must be removed (although more instances exist)
                if (part.transform.parent != null && _shipBodyInstances.Contains(part.transform.parent.parent))
                {
                    // Check if ship body cannon connection still exists somewhere
                    bool conExists = false;
                    foreach (var cannon in _cannonInstances)
                    {
                        if (cannon.parent != null && _shipBodyInstances.Contains(cannon.parent.parent))
                        {
                            conExists = true;
                            break;
                        }
                    }
                    if (!conExists)
                    {
                        RemoveConnectionInUml(_shipBodyCannonConnection);
                    }
                }
                else if (part.transform.parent != null && _wingInstances.Contains(part.transform.parent.parent))
                {
                    // Check if wing cannon connection still exists somewhere
                    bool conExists = false;
                    foreach (var cannon in _cannonInstances)
                    {
                        if (cannon.parent != null && _wingInstances.Contains(cannon.parent.parent))
                        {
                            conExists = true;
                            break;
                        }
                    }
                    if (!conExists)
                    {
                        RemoveConnectionInUml(_wingCannonConnection);
                    }
                }
            }

            OnPartDeleted?.Invoke("Cannon");
        }

        _successSound.Play();

        Destroy(part.gameObject);
    }
}
