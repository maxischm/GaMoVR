using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UmlClassStructure : MonoBehaviour
{
    /* Name of the current class; unique. */
    public string Name;

    /* Set of methods this class contains. */
    public HashSet<string> Methods;

    /* Set of properties this class contains. */
    public HashSet<string> Properties;

    /* Whether this class's method list is complete or not. */
    public bool MethodListCompleted;

    /* Whether this class's property list is complete or not. */
    public bool PropertyListCompleted;

    /* Initial position vector when spawning the class */
    public Vector3 InitPosition;

    public Dictionary<UmlConnectorType, Dictionary<UmlConnectionSide, HashSet<string>>> connections;

    [SerializeField]
    private UmlClassSideManager[] _classSides;

    // IDEA: store connectors per class & decide completeness of model based on bool in `UmlClassStructure` instead of having to iterate over the lists of connectors
    // allows checking of "do x many connectors of type y exist in orientation z with other class c

    public void Init(
        string name,
        HashSet<string> methods,
        HashSet<string> properties,
        Vector3 initPosition,
        bool methodListCompleted = false,
        bool propertyListCompleted = false
    )
    {
        Name = name;
        Methods = methods;
        Properties = properties;
        MethodListCompleted = methodListCompleted;
        PropertyListCompleted = propertyListCompleted;
        InitPosition = initPosition;
        InitConnectionDict();
    }

    public void Init(string name, Vector3 initPosition)
    {
        Name = name;
        InitPosition = initPosition;
        InitConnectionDict();

        _classSides = GetComponentsInChildren<UmlClassSideManager>();

        foreach (var side in _classSides)
        {
            side.SetClassName(name);
        }
    }

    public void Init(
        string name,
        bool methodListCompleted = false,
        bool propertyListCompleted = false
    )
    {
        Name = name;
        Methods = new HashSet<string>();
        Properties = new HashSet<string>();
        MethodListCompleted = methodListCompleted;
        PropertyListCompleted = propertyListCompleted;
        InitConnectionDict();

        _classSides = GetComponentsInChildren<UmlClassSideManager>();

        foreach (var side in _classSides)
        {
            side.SetClassName(name);
        }
    }

    public void Init(string name, bool hasGameObject)
    {
        Name = name;
        Methods = new HashSet<string>();
        Properties = new HashSet<string>();
        InitPosition = Vector3.zero;
        MethodListCompleted = true;
        PropertyListCompleted = true;
        InitConnectionDict();

        if (hasGameObject)
        {
            _classSides = GetComponentsInChildren<UmlClassSideManager>();

            foreach (var side in _classSides)
            {
                side.SetClassName(name);
            }
        }
    }

    public UmlClassStructure(string name)
    {
        Init(name, false);
    }

    public void ResetToStart(string[] methodsToKeep, string[] propertiesToKeep)
    {
        gameObject.SetActive(true);
        gameObject.GetComponent<UmlElementHealth>().ResetHealth();

        gameObject.transform.position = InitPosition;
        gameObject.transform.rotation = Quaternion.identity;

        foreach (var side in _classSides)
        {
            side.ResetToStart(methodsToKeep, propertiesToKeep);
        }
    }

    public void AddClassFeature(string propertyText, ClassFeatureType type)
    {
        foreach (var side in _classSides)
        {
            if (type == ClassFeatureType.Property)
            {
                side.AddClassProperty(propertyText);
            }
            else
            {
                side.AddClassMethod(propertyText);
            }
        }
    }

    private void InitConnectionDict()
    {
        connections = new Dictionary<UmlConnectorType, Dictionary<UmlConnectionSide, HashSet<string>>>()
        {
            {
                UmlConnectorType.Aggregation, new Dictionary<UmlConnectionSide, HashSet<string>>()
                {
                    { UmlConnectionSide.Origin, new HashSet<string>() },
                    { UmlConnectionSide.Target, new HashSet<string>() }
                }
            },
            {
                UmlConnectorType.DirectedAssociation, new Dictionary<UmlConnectionSide, HashSet<string>>()
                {
                    { UmlConnectionSide.Origin, new HashSet<string>() },
                    { UmlConnectionSide.Target, new HashSet<string>() }
                }
            },
            {
                UmlConnectorType.Composition, new Dictionary<UmlConnectionSide, HashSet<string>>()
                {
                    { UmlConnectionSide.Origin, new HashSet<string>() },
                    { UmlConnectionSide.Target, new HashSet<string>() }
                }
            },
            {
                UmlConnectorType.Inheritance, new Dictionary<UmlConnectionSide, HashSet<string>>()
                {
                    { UmlConnectionSide.Origin, new HashSet<string>() },
                    { UmlConnectionSide.Target, new HashSet<string>() }
                }
            },
            {
                UmlConnectorType.UndirectedAssociation, new Dictionary<UmlConnectionSide, HashSet<string>>()
                {
                    { UmlConnectionSide.Origin, new HashSet<string>() },
                    { UmlConnectionSide.Target, new HashSet<string>() }
                }
            },
        };
    }
}
