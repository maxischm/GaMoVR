using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ModularWorldSpaceUI;
using UnityEngine;

public enum ConnectionPlacementError
{
    /// <summary>
    /// There is no connector of that type connected to `class`.
    /// </summary>
    NoSuchConnectorConnectedToClass,

    /// <summary>
    /// The connector is only attached on one end.
    /// There is a connector of that type for `class` but with the other end.
    /// </summary>
    NoSuchConnectorWithOneEndToClass,

    /// <summary>
    /// The connector is attached with both ends.
    /// Target and Origin of connector are switched.
    /// </summary>
    NoSuchConnectorWithBothEndsToClass,

    /// <summary>
    /// There is no connection of that type connecting the two classes.
    /// </summary>
    NoConnectorBetweenTheseClasses,

    /// <summary>
    /// All connections of that type existing in the solution are already attached.
    /// </summary>
    AllConnectionsAlreadyAttachedExactMatch,

    /// <summary>
    /// All connections of that type & with `class` at the given end already exist.
    /// </summary>
    AllConnectionsOfTypeWithThatEndAlreadyAttached,
}

/// <summary>
/// Validator class used to validate player actions during the modeling games.
/// </summary>
public class UmlHangmanLevelValidator : MonoBehaviour
{
    private Dictionary<string, UmlClassStructure> _parsedClasses;

    private Dictionary<string, UmlClassStructure> _runtimeClasses;

    private Dictionary<UmlConnectorType, List<UmlConnectorStructure>> _parsedConnectors;
    private Dictionary<UmlConnectorType, List<UmlConnectorStructure>> _runtimeConnectors;

    private List<GameObject> _wrongClasses;
    private List<GameObject> _wrongConnectors;

    [SerializeField]
    private UIActionActivation _playerUI;

    [SerializeField]
    private UmlHangmanPlayerSoundManager _playerSoundManager;
    public UmlHangmanPlayerSoundManager PlayerSoundManager
    {
        get => _playerSoundManager;
    }

    #region Events

    public delegate void GameStatusUpdate(bool won, float gainedXp);
    public static event GameStatusUpdate OnGameFinished;
    public static event GameStatusUpdate OnWrongMove;

    public delegate void GameStartedUpdate(UmlHangmanGameMode gameMode, float? initialTime);
    public static event GameStartedUpdate OnGameStarted;

    public delegate void ConnectorValidationFailed(ConnectionPlacementError errorType);
    public static event ConnectorValidationFailed OnConnectorValidationFailed;

    public delegate void MethodValidationFailed();
    public static event MethodValidationFailed OnMethodValidationFailed;

    public delegate void PropertyValidationFailed();
    public static event PropertyValidationFailed OnPropertyValidationFailed;

    #endregion

    private HangmanLevel _hangmanLevel;
    public HangmanLevel HangmanLevel
    {
        get => _hangmanLevel;
    }

    #region Singleton

    private static UmlHangmanLevelValidator _instance;

    public static UmlHangmanLevelValidator Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<UmlHangmanLevelValidator>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(UmlHangmanLevelValidator).Name;
                    _instance = obj.AddComponent<UmlHangmanLevelValidator>();
                }
            }

            return _instance;
        }
    }

    #endregion

    public virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #region Constructive Modeling Modes Validation

    /// <summary>
    /// Initializes the validator for the player actions after the Hangman level was loaded.
    /// </summary>
    /// <param name="parsedClasses">Dictionary of parsed classes to validate against.</param>
    /// <param name="playerCreatedClasses">Dictionary of classes created by the player or during play.</param>
    /// <param name="parsedConnectors">Dictionary of connectors to validate against. Indexed by connection type.</param>
    /// <param name="runtimeConnectors">Dictionary of connectors created/handled during play. Indexed by connection type.</param>
    /// <param name="level">Level-describing object.</param>
    public void InitValidatorForConstructiveLevel(
            Dictionary<string, UmlClassStructure> parsedClasses,
            Dictionary<string, UmlClassStructure> playerCreatedClasses,
            Dictionary<UmlConnectorType, List<UmlConnectorStructure>> parsedConnectors,
            Dictionary<UmlConnectorType, List<UmlConnectorStructure>> runtimeConnectors,
            HangmanLevel level
        )
    {
        _runtimeClasses = playerCreatedClasses;
        _parsedClasses = parsedClasses;
        _runtimeConnectors = runtimeConnectors;
        _parsedConnectors = parsedConnectors;
        _hangmanLevel = level;

        if (_hangmanLevel.gameMode == UmlHangmanGameMode.AgainstTheClock)
        {
            OnGameStarted?.Invoke(level.gameMode, (level as HangmanAgainstTheClockLevel)?.initialTime);
        }
        else if (_hangmanLevel.gameMode == UmlHangmanGameMode.Classic)
        {
            OnGameStarted?.Invoke(level.gameMode, (level as HangmanClassicLevel)?.initialLives);
        }
    }

    /// <summary>
    /// Validates attachment of a UML connection created by the player during play.
    /// </summary>
    /// <param name="connectorType">Type of the UML connector.</param>
    /// <param name="updatedSide">Whether the origin side or target side of the connector has been changed.</param>
    /// <param name="description">Description/label of the UML connector.</param>
    /// <param name="originClassName">Name of the UML class the origin of the connector attached to.</param>
    /// <param name="targetClassName">Name of the UMl class the target of the connector attached to.</param>
    /// <returns>Wether the attachment by the player was valid or not.</returns>
    public bool ValidateConnectorAttachment(
        UmlConnectorType connectorType,
        UmlConnectionSide updatedSide,
        string description,
        string originClassName = null,
        string targetClassName = null
    )
    {
        // Filter connections to only those matching what the user currently wants to connect
        List<UmlConnectorStructure> parsedConnections = _parsedConnectors[connectorType].Where(con =>
        {
            if (originClassName != null && targetClassName != null)
            {
                return con.OriginClassName == originClassName
                    && con.TargetClassName == targetClassName
                    && description == con.Description;
            }
            else if (originClassName != null)
            {
                return con.OriginClassName == originClassName && description == con.Description;
            }
            else
            {
                return con.TargetClassName == targetClassName && description == con.Description;
            }
        }).ToList();

        if (parsedConnections.Count == 0)
        {
            // ERROR in player action
            Debug.Log($"No matches for connection of type {connectorType} to class {targetClassName} from {originClassName}");

            // Check which type of error was produced for custom feedback for the player
            if (originClassName != null && targetClassName != null &&
                _parsedConnectors[connectorType].Count(con => con.OriginClassName == targetClassName
                    && con.TargetClassName == originClassName
                    && description == con.Description) > 0)
            {
                OnConnectorValidationFailed?.Invoke(ConnectionPlacementError.NoSuchConnectorWithBothEndsToClass);
            }
            else if (originClassName != null &&
                _parsedConnectors[connectorType].Count(con =>
                    con.OriginClassName != originClassName
                    && con.TargetClassName == originClassName
                    && description == con.Description) > 0)
            {
                OnConnectorValidationFailed?.Invoke(ConnectionPlacementError.NoSuchConnectorWithOneEndToClass);
            }
            else if (targetClassName != null &&
                _parsedConnectors[connectorType].Count(con =>
                    con.TargetClassName != targetClassName
                    && con.OriginClassName == targetClassName
                    && description == con.Description) > 0)
            {
                OnConnectorValidationFailed?.Invoke(ConnectionPlacementError.NoSuchConnectorWithOneEndToClass);
            }
            else if (
                _parsedConnectors[connectorType].Count(con =>
                    (con.TargetClassName == targetClassName
                    || con.OriginClassName == targetClassName
                    || con.TargetClassName == originClassName)
                    && description == con.Description) == 0)
            {
                OnConnectorValidationFailed?.Invoke(ConnectionPlacementError.NoSuchConnectorConnectedToClass);
            }

            FailedValidation();
            return false;
        }

        if (originClassName != null && targetClassName != null)
        {
            // Both connector ends being attached => check if this connection already exists in runtime connections
            if (_runtimeConnectors[connectorType].FindIndex(con =>
                con.OriginClassName == originClassName && con.TargetClassName == targetClassName && con.Description == description) == -1
            )
            {
                // Does not exist => create and store
                Debug.Log($"Connection of type {connectorType} to class {targetClassName} from {originClassName} does not yet exist.");

                // Take connector where !updatedSide is set but updatedSide == null & set updatedSide
                UmlConnectorStructure availableConnector;
                if (updatedSide == UmlConnectionSide.Origin)
                {
                    availableConnector = _runtimeConnectors[connectorType]
                        .Find(con => con.TargetClassName == targetClassName && con.OriginClassName == null);
                    availableConnector.OriginClassName = originClassName;
                }
                else
                {
                    availableConnector = _runtimeConnectors[connectorType]
                        .Find(con => con.TargetClassName == null && con.OriginClassName == originClassName);
                    availableConnector.OriginClassName = originClassName;
                }

                _runtimeClasses[originClassName].connections[connectorType][UmlConnectionSide.Origin].Add(targetClassName);
                _runtimeClasses[targetClassName].connections[connectorType][UmlConnectionSide.Target].Add(originClassName);

                CheckLevelFinished();

                return true;
            }
            else
            {
                // ERROR, no such connection exists
                Debug.Log($"Connection of type {connectorType} to class {targetClassName} from {originClassName} already exists.");
                OnConnectorValidationFailed?.Invoke(ConnectionPlacementError.AllConnectionsAlreadyAttachedExactMatch);
                FailedValidation();

                return false;
            }
        }
        else if (originClassName != null && updatedSide == UmlConnectionSide.Origin)
        {
            // target not attached => check if for `className` not all connectors of type `connectorType` with `originClassName` == `connector.OriginClassName` are connected yet

            int numberOfConnectorsInRuntimeModel = _runtimeConnectors[connectorType].Count(con =>
                con.OriginClassName == originClassName && con.Description == description);
            if (numberOfConnectorsInRuntimeModel < parsedConnections.Count)
            {
                // Not all connectors exist
                Debug.Log($"Not all connectors of type {connectorType} exist with {originClassName} as origin.");
                // Create & add connection
                _runtimeConnectors[connectorType].Add(
                    new UmlConnectorStructure(
                        connectionType: connectorType,
                        isCorrect: true,
                        description: description,
                        originClassName: originClassName,
                        getTransform: false
                    )
                );

                return true;
            }
            else
            {
                Debug.Log($"There are already all connectors of type {connectorType} with origin {originClassName} attached.");
                OnConnectorValidationFailed?.Invoke(ConnectionPlacementError.AllConnectionsOfTypeWithThatEndAlreadyAttached);
                FailedValidation();

                return false;
            }
        }
        else if (targetClassName != null && updatedSide == UmlConnectionSide.Target)
        {
            // origin not attached => check if for `className` not all connectors of type `connectorType` with `targetClassName` == `connector.TargetClassName` are connected yet
            int numberOfConnectorsInRuntimeModel = _runtimeConnectors[connectorType].Count(con =>
                con.TargetClassName == targetClassName && con.Description == description);
            if (numberOfConnectorsInRuntimeModel < parsedConnections.Count)
            {
                // Not all connectors exist
                Debug.Log($"Not all connectors of type {connectorType} exist with {targetClassName} as target.");
                // Create & add connection
                _runtimeConnectors[connectorType].Add(
                    new UmlConnectorStructure(
                        connectionType: connectorType,
                        isCorrect: true,
                        description: description,
                        targetClassName: targetClassName,
                        getTransform: false
                    )
                );

                return true;
            }
            else
            {
                Debug.Log($"There are already all connectors of type {connectorType} with target {targetClassName} attached.");
                OnConnectorValidationFailed?.Invoke(ConnectionPlacementError.AllConnectionsOfTypeWithThatEndAlreadyAttached);
                FailedValidation();
                return false;
            }
        }
        else
        {
            // Something major went wrong; fallback case
            Debug.LogError($"Same side to attach was null! {connectorType}, from {originClassName} to {targetClassName}");

            return false;
        }
    }

    /// <summary>
    /// Validates the assignment of properties to classes.
    /// </summary>
    /// <param name="propertyName">Name of the property the player tries to attach.</param>
    /// <param name="className">Name of the class the player tries to attach to.</param>
    /// <returns>Whether the assignment was valid or not.</returns>
    public bool ValidatePropertyAssignment(string propertyName, string className)
    {
        if (_parsedClasses[className].Properties.Contains(propertyName)
            && !_runtimeClasses[className].Properties.Contains(propertyName))
        {
            _runtimeClasses[className].Properties.Add(propertyName);

            // Check if class is finished
            if (_runtimeClasses[className].Properties.SetEquals(_parsedClasses[className].Properties))
            {
                _runtimeClasses[className].PropertyListCompleted = true;
            }
            CheckLevelFinished();

            return true;
        }
        else
        {
            Debug.Log($"Error when adding property {propertyName} to class {className}.");
            OnPropertyValidationFailed?.Invoke();
            FailedValidation();
            return false;
        }
    }

    /// <summary>
    /// Validates the assignment of method names to classes.
    /// </summary>
    /// <param name="methodName">Name of the method the player tries to attach.</param>
    /// <param name="className">Name of the class the player tries to attach to.</param>
    /// <returns>Whether the assignment was valid or not.</returns>
    public bool ValidateMethodAssignment(string methodName, string className)
    {
        if (_parsedClasses[className].Methods.Contains(methodName) && !_runtimeClasses[className].Methods.Contains(methodName))
        {
            _runtimeClasses[className].Methods.Add(methodName);

            // Check if class is finished
            if (_runtimeClasses[className].Methods.SetEquals(_parsedClasses[className].Methods))
            {
                _runtimeClasses[className].MethodListCompleted = true;
            }
            CheckLevelFinished();

            return true;
        }
        else
        {
            Debug.Log($"Error when adding method {methodName} to class {className}.");
            OnMethodValidationFailed?.Invoke();
            FailedValidation();
            return false;
        }
    }

    /// <summary>
    /// Checks whether the level was finished or not and invokes the respective events if necessary.
    /// </summary>
    public void CheckLevelFinished()
    {
        // 1. Check all classes if they are completed
        foreach (var c in _runtimeClasses.Values)
        {
            if (!c.MethodListCompleted || !c.PropertyListCompleted)
            {
                // Found a class where method list or property list is not finished yet.
                return;
            }
        }

        // 2. Check all connectors if they are completed
        bool stoppedCheck = false;
        foreach (var c in _parsedClasses)
        {
            if (!stoppedCheck)
            {
                foreach (var connectionType in c.Value.connections.Keys)
                {
                    if (!stoppedCheck)
                    {
                        foreach (var connectionSide in c.Value.connections[connectionType].Keys)
                        {
                            var otherClassNames = c.Value.connections[connectionType][connectionSide];
                            if (
                                _runtimeClasses[c.Key].connections[connectionType] != null
                                && _runtimeClasses[c.Key].connections[connectionType][connectionSide] != null
                                && !_runtimeClasses[c.Key].connections[connectionType][connectionSide].SetEquals(otherClassNames)
                            )
                            {
                                // Found class where connections not yet finished.
                                stoppedCheck = true;
                            }
                        }
                    }
                }
            }
            else
            {
                // Found at least one connection in parsed classes that does not exist in runtime classes
                // => do not continue to calling FinishGame
                return;
            }
        }

        // No missing parts of the level found => game successfully completed.
        GameWon();
    }

    #endregion

    /// <summary>
    /// Handles failed validation, i.e. reduction of player's lives. Updating of player state in gamification engine and locally.
    /// </summary>
    private void FailedValidation()
    {
        if (_hangmanLevel.gameMode == UmlHangmanGameMode.Classic || _hangmanLevel.gameMode == UmlHangmanGameMode.TestMode) // Only run this routine if classic game mode or test mode
        {
            StartCoroutine(GamificationEngineConnection.Instance.ReduceLives(
            LocalPlayerReference.Instance.Nickname,
            (_) =>
                {
                    if (_hangmanLevel.gameMode == UmlHangmanGameMode.TestMode)
                    {
                        OnWrongMove?.Invoke(false, 0);
                    }
                    if (GamificationEngineConnection.Instance.PlayerLives <= 1)
                    {
                        // Game lost
                        OnGameFinished?.Invoke(false, _hangmanLevel.xpReward / 2);
                        StartCoroutine(GamificationEngineConnection.Instance.FinishGame(
                            LocalPlayerReference.Instance.Nickname,
                            _hangmanLevel.levelId,
                            false,
                            _hangmanLevel.xpReward / 2,
                            0,
                            (_) => StartCoroutine(GamificationEngineConnection.Instance.FetchPlayerState(() =>
                                _playerUI.UIActionInvoked(new UnityEngine.InputSystem.InputAction.CallbackContext()))))
                        );
                    }
                    else
                    {
                        StartCoroutine(GamificationEngineConnection.Instance.FetchPlayerState(null));
                    }
                }
            ));
        }
    }

    /// <summary>
    /// Calls finished game routine to update the player state in the gamification engine (i.e. add XP, update list of finished levels,...)
    /// </summary>
    private void GameWon()
    {
        StartCoroutine(GamificationEngineConnection.Instance.FinishGame(
            LocalPlayerReference.Instance.Nickname,
            _hangmanLevel.levelId,
            true,
            _hangmanLevel.xpReward,
            GamificationEngineConnection.Instance.PlayerLives,
            (_) =>
            {
                OnGameFinished?.Invoke(true, _hangmanLevel.xpReward);
                StartCoroutine(GamificationEngineConnection.Instance.FetchPlayerState(() =>
                    _playerUI.UIActionInvoked(new UnityEngine.InputSystem.InputAction.CallbackContext())));
            }
        ));
    }

    #region Destructive Modeling Modes Validation

    /// <summary>
    /// Initializes validator for levels where the player has to destroy objects in order to fix the given UML models.
    /// </summary>
    /// <param name="wrongClasses">List of classes the player has to destroy during the game.</param>
    /// <param name="wrongConnectors">List of wrong connectors the player has to destroy during the game.</param>
    /// <param name="level">Level-describing object.</param>
    public void InitValidatorForDestructiveLevel(List<GameObject> wrongClasses, List<GameObject> wrongConnectors, HangmanLevel level)
    {
        _wrongClasses = wrongClasses;
        _wrongConnectors = wrongConnectors;
        _hangmanLevel = level;
        // essentially clear the list of elements up if they are destroyed

        OnGameStarted?.Invoke(level.gameMode, (level as HangmanTestModeLevel)?.initialLives);
    }

    /// <summary>
    /// Validates whether the destruction of the given object is correct or not.
    /// </summary>
    /// <param name="objectToDestroy">Object the player wants to destroy.</param>
    /// <returns>Whether the destruction of the object is valid or not.</returns>
    public bool ValidateObjectDestruction(GameObject objectToDestroy)
    {
        // Check if player destroys a class or a UML connector.
        var classStructure = objectToDestroy.GetComponent<UmlClassStructure>();
        if (classStructure is not null)
        {
            var indexToRemove = -1;
            foreach (var wrongClass in _wrongClasses)
            {
                if (wrongClass.GetComponent<UmlClassStructure>().Name == classStructure.Name)
                {
                    indexToRemove = _wrongClasses.IndexOf(wrongClass);
                }
            }
            if (indexToRemove != -1)
            {
                _wrongClasses.RemoveAt(indexToRemove);
            }
            else
            {
                PlayerSoundManager.PlayWrongMoveSound();
                FailedValidation();

                return false;
            }
        }
        else
        {
            var conStructure = objectToDestroy.GetComponentInChildren<UmlConnectorStructure>();
            var indexToRemove = -1;
            foreach (var con in _wrongConnectors)
            {
                var wrongConStructure = con.GetComponentInChildren<UmlConnectorStructure>();
                if (wrongConStructure.ConnectionType == conStructure.ConnectionType
                    && wrongConStructure.Description == conStructure.Description
                    && wrongConStructure.OriginClassName == conStructure.OriginClassName
                    && wrongConStructure.TargetClassName == conStructure.TargetClassName
                )
                {
                    indexToRemove = _wrongConnectors.IndexOf(con);
                }
            }
            if (indexToRemove != -1)
            {
                _wrongConnectors.RemoveAt(indexToRemove);
            }
            else
            {
                PlayerSoundManager.PlayWrongMoveSound();
                FailedValidation();

                return false;
            }
        }

        if (_wrongClasses.Count == 0 && _wrongConnectors.Count == 0)
        {
            GameWon();
        }

        PlayerSoundManager.PlayCorrectMoveSound();

        return true;
    }

    #endregion

    /// <summary>
    /// Routine called when the player loses the game. Handles updating the player state in the gamification engine.
    /// </summary>
    public void LoseGame()
    {
        OnGameFinished?.Invoke(false, _hangmanLevel.xpReward / 2);
        StartCoroutine(GamificationEngineConnection.Instance.FinishGame(
            LocalPlayerReference.Instance.Nickname,
            _hangmanLevel.levelId,
            false,
            _hangmanLevel.xpReward / 2,
            0,
            (_) => StartCoroutine(GamificationEngineConnection.Instance.FetchPlayerState(() =>
                _playerUI.UIActionInvoked(new UnityEngine.InputSystem.InputAction.CallbackContext()))))
        );
    }
}
