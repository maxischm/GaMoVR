using UnityEngine;

public class ModelElementSpawner : MonoBehaviour
{
    #region Public Fields

    public GameObject classPrefab;
    public GameObject undirectedAssociationPrefab;
    public GameObject directedAssociationPrefab;
    public GameObject inheritancePrefab;
    public GameObject aggregationPrefab;
    public GameObject compositionPrefab;
    public GameObject classFeature;

    #endregion

    #region Public Methods

    public void SpawnUndirectedAssociation()
    {
        SpawnConnector(undirectedAssociationPrefab);
    }

    public void SpawnDirectedAssociation()
    {
        SpawnConnector(directedAssociationPrefab);
    }

    public void SpawnInheritance()
    {
        SpawnConnector(inheritancePrefab);
    }

    public void SpawnAggregation()
    {
        SpawnConnector(aggregationPrefab);
    }

    public void SpawnComposition()
    {
        SpawnConnector(compositionPrefab);
    }

    public void SpawnClass()
    {
        Instantiate(
            classPrefab,
            transform.position + 2 * CalculateHorizontalForward() + 0.5f * Vector3.up,
            Quaternion.LookRotation(CalculateHorizontalForward(), Vector3.up)
        );
    }

    public void SpawnClassFeature()
    {
        Instantiate(
            classFeature,
            transform.position + 2 * CalculateHorizontalForward() + 0.5f * Vector3.up,
            Quaternion.LookRotation(CalculateHorizontalForward(), Vector3.up)
        );
    }

    #endregion

    #region Private Methods

    private void SpawnConnector(GameObject connectorPrefab)
    {
        Instantiate(
            connectorPrefab,
            transform.position + 0.5f * CalculateHorizontalForward() + 0.5f * CalculateHorizontalRight(),
            Quaternion.LookRotation(CalculateHorizontalForward(), Vector3.up)
        );
    }

    private Vector3 CalculateHorizontalForward()
    {
        return Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
    }

    private Vector3 CalculateHorizontalRight()
    {
        return Vector3.ProjectOnPlane(transform.right, Vector3.up).normalized;
    }

    #endregion
}
