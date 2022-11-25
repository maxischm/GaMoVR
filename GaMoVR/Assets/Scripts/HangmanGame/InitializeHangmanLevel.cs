using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitializeHangmanLevel : MonoBehaviour
{
    #region ModelingPrefabs

    [SerializeField]
    private GameObject classPrefab;

    [SerializeField]
    private GameObject aggregationPrefab;

    [SerializeField]
    private GameObject directedAssociationPrefab;

    [SerializeField]
    private GameObject inheritancePrefab;

    [SerializeField]
    private GameObject undirectedAssociationPrefab;

    [SerializeField]
    private GameObject compositionPrefab;

    [SerializeField]
    private GameObject classFeaturePrefab;

    #endregion

    [SerializeField]
    private UmlHangmanLevelDataObject hangmanLevels;

    private List<HangmanLevel> _completeLevelList = new();

    [SerializeField]
    private TextMeshProUGUI playerUITaskDescriptionField;

    private Dictionary<string, UmlClassStructure> _parsedClasses;
    private Dictionary<string, UmlClassStructure> _runtimeClasses;

    private List<UmlClassFeatureStructure> _parsedFeatures;

    private Dictionary<UmlConnectorType, List<UmlConnectorStructure>> _parsedConnectors;
    private Dictionary<UmlConnectorType, List<UmlConnectorStructure>> _runtimeConnectors;

    private List<GameObject> _wrongConnectors = new();
    private List<GameObject> _wrongClasses = new();

    private Dictionary<string, List<string>> _propertiesPerClassToKeep = new();
    private Dictionary<string, List<string>> _methodsPerClassToKeep = new();

    #region Spawn Positions

    private List<Vector3> _classFeatureSpawnPositions = new() {
        new Vector3(0, 1.75f, 10),
        new Vector3(0, 1, 10),
        new Vector3(0, 1.25f, 10),
        new Vector3(-1, 1.75f, 10),
        new Vector3(-1, 1, 10),
        new Vector3(-1, 1.25f, 10),
        new Vector3(-2, 1.75f, 10),
        new Vector3(-2, 1, 10),
        new Vector3(-2, 1.25f, 10),
        new Vector3(-3, 1.75f, 10),
        new Vector3(-3, 1, 10),
        new Vector3(-3, 1.25f, 10),
    };

    private List<Vector3> _classFeatureSpawnPositionsCopy;

    private List<Vector3> _connectorSpawnPositions = new() {
        new Vector3(-8, 1.75f, 8),
        new Vector3(-8, 1.25f, 8),
        new Vector3(-8, 1.75f, 6),
        new Vector3(-8, 1.25f, 6),
        new Vector3(-8, 1.75f, 4),
        new Vector3(-8, 1.25f, 4),
        new Vector3(-8, 1.75f, 2),
        new Vector3(-8, 1.25f, 2),
    };

    private List<Vector3> _connectorSpawnPositionsCopy;

    #endregion

    public void Awake()
    {
        _completeLevelList = hangmanLevels.classicLevels
            .Concat<HangmanLevel>(hangmanLevels.againstTheClockLevel)
            .Concat<HangmanLevel>(hangmanLevels.testModeLevels).ToList();
    }

    public void OnEnable()
    {
        SceneManager.sceneLoaded += InitLevel;
        _classFeatureSpawnPositionsCopy = System.ObjectExtensions.Copy(_classFeatureSpawnPositions);
        _connectorSpawnPositionsCopy = System.ObjectExtensions.Copy(_connectorSpawnPositions);
    }

    public void OnDisable()
    {
        SceneManager.sceneLoaded -= InitLevel;
    }

    // Start is called before the first frame update
    public void InitLevel(Scene scene, LoadSceneMode mode)
    {
        if (
            scene.name == "HangmanClassic"
            || scene.name == "HangmanAgainstTheClock"
            || scene.name == "HangmanClassicTutorial"
            || scene.name == "HangmanTestMode"
        )
        {
            _parsedClasses = new();
            _runtimeClasses = new();
            _parsedFeatures = new();
            _parsedConnectors = new();
            _runtimeConnectors = new();


            var levelDefinition = new XmlDocument();
            levelDefinition.LoadXml(_completeLevelList[StartHangmanLevel.Instance.SelectedLevel].xmlModelString);

            // Spawn classes & feature objects
            foreach (XmlNode c in levelDefinition.GetElementsByTagName("Classes")[0].ChildNodes)
            {
                // Create object in room
                var initPosition = new Vector3(
                        float.Parse(c.Attributes["xPosition"].Value, CultureInfo.InvariantCulture),
                        float.Parse(c.Attributes["yPosition"].Value, CultureInfo.InvariantCulture),
                        float.Parse(c.Attributes["zPosition"].Value, CultureInfo.InvariantCulture));

                var instantiatedClass = Instantiate(classPrefab, initPosition, Quaternion.identity);

                if (c.Attributes["isCorrect"]?.Value == "false")
                {
                    _wrongClasses.Add(instantiatedClass);
                }

                if (scene.name == "HangmanTestMode")
                {
                    instantiatedClass.GetComponent<XROffsetGrabbable>().enabled = false;
                }
                else if (scene.name == "HangmanAgainstTheClock")
                {
                    instantiatedClass.GetComponent<UmlElementHealth>().enabled = false;
                }

                // Add definition of class to add to dictionary for later validation
                var parsedClass = instantiatedClass.GetComponent<UmlClassStructure>();
                parsedClass.Init(c.Attributes["className"].Value);
                parsedClass.InitPosition = initPosition;

                var playerCreatedClass = new UmlClassStructure(c.Attributes["className"].Value);

                foreach (XmlNode child in c.ChildNodes)
                {
                    if (child.Name.Equals("Property"))
                    {
                        if (child.Attributes["isCreated"]?.Value == "true")
                        {
                            // add property to existing class if specified
                            instantiatedClass.GetComponent<UmlClassStructure>().AddClassFeature(
                                child.Attributes["propertyName"].Value,
                                ClassFeatureType.Property);

                            if (!_propertiesPerClassToKeep.ContainsKey(c.Attributes["className"].Value))
                            {
                                _propertiesPerClassToKeep.Add(c.Attributes["className"].Value, new());
                            }
                            _propertiesPerClassToKeep[c.Attributes["className"].Value].Add(child.Attributes["propertyName"].Value);
                        }
                        else
                        {
                            var propInitPositionIndex = Random.Range(0, _classFeatureSpawnPositionsCopy.Count);
                            var propInitPosition = _classFeatureSpawnPositionsCopy[propInitPositionIndex];
                            _classFeatureSpawnPositionsCopy.RemoveAt(propInitPositionIndex);

                            var prop = Instantiate(classFeaturePrefab, propInitPosition, Quaternion.identity);
                            prop.GetComponent<ClassFeatureManager>().Init(
                                child.Attributes["propertyName"].Value,
                                ClassFeatureType.Property);

                            playerCreatedClass.PropertyListCompleted = false;
                            parsedClass.Properties.Add(child.Attributes["propertyName"].Value);
                            var feat = prop.GetComponent<UmlClassFeatureStructure>();
                            feat.InitPosition = propInitPosition;
                            _parsedFeatures.Add(feat);
                        }
                    }
                    else if (child.Name.Equals("Method"))
                    {
                        if (child.Attributes["isCreated"]?.Value == "true")
                        {
                            // add method to existing class if specified
                            instantiatedClass.GetComponent<UmlClassStructure>().AddClassFeature(
                                child.Attributes["methodName"].Value,
                                ClassFeatureType.Method);

                            if (!_methodsPerClassToKeep.ContainsKey(c.Attributes["className"].Value))
                            {
                                _methodsPerClassToKeep.Add(c.Attributes["className"].Value, new());
                            }
                            _methodsPerClassToKeep[c.Attributes["className"].Value].Add(child.Attributes["methodName"].Value);
                        }
                        else
                        {
                            var methodInitPositionIndex = Random.Range(0, _classFeatureSpawnPositionsCopy.Count);
                            var methodInitPosition = _classFeatureSpawnPositionsCopy[methodInitPositionIndex];
                            _classFeatureSpawnPositionsCopy.RemoveAt(methodInitPositionIndex);
                            var method = Instantiate(classFeaturePrefab, methodInitPosition, Quaternion.identity);
                            method.GetComponent<ClassFeatureManager>().Init(
                                child.Attributes["methodName"].Value,
                                ClassFeatureType.Method);

                            playerCreatedClass.MethodListCompleted = false;
                            parsedClass.Methods.Add(child.Attributes["methodName"].Value);
                            var feat = method.GetComponent<UmlClassFeatureStructure>();
                            feat.InitPosition = methodInitPosition;
                            _parsedFeatures.Add(feat);
                        }
                    }
                }
                _parsedClasses.Add(c.Attributes["className"].Value, parsedClass);
                _runtimeClasses.Add(c.Attributes["className"].Value, playerCreatedClass);
            }

            // Spawn Directed Associations
            _parsedConnectors.Add(UmlConnectorType.DirectedAssociation, new List<UmlConnectorStructure>());
            _runtimeConnectors.Add(UmlConnectorType.DirectedAssociation, new List<UmlConnectorStructure>());
            foreach (XmlNode c in levelDefinition.GetElementsByTagName("DirectedAssociation"))
            {
                SpawnConnector(UmlConnectorType.DirectedAssociation, directedAssociationPrefab, c);
            }

            // Spawn Aggregations
            _parsedConnectors.Add(UmlConnectorType.Aggregation, new List<UmlConnectorStructure>());
            _runtimeConnectors.Add(UmlConnectorType.Aggregation, new List<UmlConnectorStructure>());
            foreach (XmlNode c in levelDefinition.GetElementsByTagName("Aggregation"))
            {
                SpawnConnector(UmlConnectorType.Aggregation, aggregationPrefab, c);
            }

            // Spawn Inheritances
            _parsedConnectors.Add(UmlConnectorType.Inheritance, new List<UmlConnectorStructure>());
            _runtimeConnectors.Add(UmlConnectorType.Inheritance, new List<UmlConnectorStructure>());
            foreach (XmlNode c in levelDefinition.GetElementsByTagName("Inheritance"))
            {
                SpawnConnector(UmlConnectorType.Inheritance, inheritancePrefab, c);
            }

            // Spawn Compositions
            _parsedConnectors.Add(UmlConnectorType.Composition, new List<UmlConnectorStructure>());
            _runtimeConnectors.Add(UmlConnectorType.Composition, new List<UmlConnectorStructure>());
            foreach (XmlNode c in levelDefinition.GetElementsByTagName("Composition"))
            {
                SpawnConnector(UmlConnectorType.Composition, compositionPrefab, c);
            }

            // Init validator after level was parsed
            if (_completeLevelList[StartHangmanLevel.Instance.SelectedLevel].gameMode != UmlHangmanGameMode.TestMode)
            {
                UmlHangmanLevelValidator.Instance.InitValidatorForConstructiveLevel(
                    _parsedClasses,
                    System.ObjectExtensions.Copy(_runtimeClasses),
                    _parsedConnectors,
                    System.ObjectExtensions.Copy(_runtimeConnectors),
                    _completeLevelList[StartHangmanLevel.Instance.SelectedLevel]
                );
            }
            else
            {
                UmlHangmanLevelValidator.Instance.InitValidatorForDestructiveLevel(
                    System.ObjectExtensions.Copy(_wrongClasses),
                    System.ObjectExtensions.Copy(_wrongConnectors),
                    _completeLevelList[StartHangmanLevel.Instance.SelectedLevel]
                );
            }

            playerUITaskDescriptionField.SetText(_completeLevelList[StartHangmanLevel.Instance.SelectedLevel].levelDescription);
        }
    }

    private void SpawnConnector(UmlConnectorType type, GameObject prefab, XmlNode classDescriptionNode)
    {
        var conInitPositionIndex = Random.Range(0, _connectorSpawnPositionsCopy.Count);
        var conInitPosition = _connectorSpawnPositionsCopy[conInitPositionIndex];
        // Spawn connection
        var con = Instantiate(prefab, conInitPosition, Quaternion.Euler(0, -90, 0));
        con.GetComponent<UmlConnectorInitializer>().InitConnector(classDescriptionNode.Attributes["description"].Value);

        var structure = con.GetComponentInChildren<UmlConnectorStructure>();
        structure.InitPosition = conInitPosition;

        if (classDescriptionNode.Attributes["isCreated"]?.Value == "true")
        {
            foreach (XROffsetGrabbable comp in con.GetComponentsInChildren<XROffsetGrabbable>())
            {
                comp.enabled = false;
            }

            structure.Init(connectionType: type,
                description: classDescriptionNode.Attributes["description"].Value,
                isCorrect: classDescriptionNode.Attributes["isCreated"]?.Value == "true" ? true : false,
                isCreated: true,
                originClassName: classDescriptionNode.Attributes["origin"].Value,
                targetClassName: classDescriptionNode.Attributes["target"].Value,
                getTransform: true
                );

            var targetGrabVolume = con.transform.Find("TargetGrabVolume");
            targetGrabVolume.transform.position = new Vector3(
                float.Parse(classDescriptionNode.Attributes["targetAttachXPosition"].Value, CultureInfo.InvariantCulture),
                float.Parse(classDescriptionNode.Attributes["targetAttachYPosition"].Value, CultureInfo.InvariantCulture),
                float.Parse(classDescriptionNode.Attributes["targetAttachZPosition"].Value, CultureInfo.InvariantCulture)
            );

            // Attach connection
            targetGrabVolume.GetComponent<ConnectorGrabVolume>().intersectedClass =
                _parsedClasses[classDescriptionNode.Attributes["target"].Value].gameObject;
            targetGrabVolume.GetComponent<ConnectorGrabVolume>().OnGrabEnd(true);

            con.transform.Find("OriginGrabVolume").transform.position = new Vector3(
                float.Parse(classDescriptionNode.Attributes["originAttachXPosition"].Value, CultureInfo.InvariantCulture),
                float.Parse(classDescriptionNode.Attributes["originAttachYPosition"].Value, CultureInfo.InvariantCulture),
                float.Parse(classDescriptionNode.Attributes["originAttachZPosition"].Value, CultureInfo.InvariantCulture)
            );

            // Final position update
            targetGrabVolume.GetComponent<ConnectorGrabVolume>().connector.UpdateTransform();

            // Collect "wrong" connections to for later destruction & evaluation
            if (classDescriptionNode.Attributes["isCorrect"]?.Value == "false")
            {
                _wrongConnectors.Add(con);
            }
        }
        else
        {
            _connectorSpawnPositionsCopy.RemoveAt(conInitPositionIndex);

            structure.Init(connectionType: type,
                description: classDescriptionNode.Attributes["description"].Value,
                originClassName: classDescriptionNode.Attributes["origin"].Value,
                targetClassName: classDescriptionNode.Attributes["target"].Value,
                getTransform: true);

            _parsedClasses[classDescriptionNode.Attributes["origin"].Value]
                .connections[type][UmlConnectionSide.Origin]
                .Add(classDescriptionNode.Attributes["target"].Value);
            _parsedClasses[classDescriptionNode.Attributes["target"].Value]
                .connections[type][UmlConnectionSide.Target]
                .Add(classDescriptionNode.Attributes["origin"].Value);
        }

        _parsedConnectors[type].Add(structure);
    }

    public void ResetLevel()
    {
        _classFeatureSpawnPositionsCopy = System.ObjectExtensions.Copy(_classFeatureSpawnPositions);

        if (_completeLevelList[StartHangmanLevel.Instance.SelectedLevel] is HangmanClassicLevel)
        {
            StartCoroutine(
                GamificationEngineConnection.Instance.StartNewLevel(
                    LocalPlayerReference.Instance.Nickname,
                    _completeLevelList[StartHangmanLevel.Instance.SelectedLevel].levelId,
                    (_completeLevelList[StartHangmanLevel.Instance.SelectedLevel] as HangmanClassicLevel).initialLives,
                    (_) => StartCoroutine(GamificationEngineConnection.Instance.FetchPlayerState(null))
                )
            );
        }
        else if (_completeLevelList[StartHangmanLevel.Instance.SelectedLevel] is HangmanTestModeLevel)
        {
            StartCoroutine(
                GamificationEngineConnection.Instance.StartNewLevel(
                    LocalPlayerReference.Instance.Nickname,
                    _completeLevelList[StartHangmanLevel.Instance.SelectedLevel].levelId,
                    (_completeLevelList[StartHangmanLevel.Instance.SelectedLevel] as HangmanTestModeLevel).initialLives,
                    (_) => StartCoroutine(GamificationEngineConnection.Instance.FetchPlayerState(null))
                )
            );
        }

        // TODO: what about starting other types of levels?

        // Reset game objects' positions
        foreach (var parsedClass in _parsedClasses.Values)
        {
            parsedClass.GetComponent<UmlClassStructure>().ResetToStart(
                _methodsPerClassToKeep.ContainsKey(parsedClass.Name)
                    ? _methodsPerClassToKeep[parsedClass.Name].ToArray() : new string[0],
                _propertiesPerClassToKeep.ContainsKey(parsedClass.Name)
                    ? _propertiesPerClassToKeep[parsedClass.Name].ToArray() : new string[0]
            );
        }

        foreach (var feature in _parsedFeatures)
        {
            feature.ResetToStart();
        }

        foreach (var connectorTypeList in _parsedConnectors.Values)
        {
            foreach (var connector in connectorTypeList)
            {
                connector.ResetToStart(!connector.IsCreated);
            }
        }

        if (_completeLevelList[StartHangmanLevel.Instance.SelectedLevel].gameMode != UmlHangmanGameMode.TestMode)
        {
            UmlHangmanLevelValidator.Instance.InitValidatorForConstructiveLevel(
                _parsedClasses,
                System.ObjectExtensions.Copy(_runtimeClasses),
                _parsedConnectors,
                System.ObjectExtensions.Copy(_runtimeConnectors),
                _completeLevelList[StartHangmanLevel.Instance.SelectedLevel]
            );
        }
        else
        {
            UmlHangmanLevelValidator.Instance.InitValidatorForDestructiveLevel(
                System.ObjectExtensions.Copy(_wrongClasses),
                System.ObjectExtensions.Copy(_wrongConnectors),
                _completeLevelList[StartHangmanLevel.Instance.SelectedLevel]
            );
        }
    }
}
