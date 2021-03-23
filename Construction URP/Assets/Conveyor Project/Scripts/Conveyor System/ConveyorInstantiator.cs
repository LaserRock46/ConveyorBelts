using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorInstantiator : MonoBehaviour
{
    #region Temp
    //[Header("Temporary Things", order = 0)]
    #endregion

    #region Fields
    [Header("Fields", order = 1)]    
    [SerializeField] private Transform _buildingsRoot;
    [SerializeField] private GameObject _conveyorPrefab;
    [SerializeField] private GameObject _pipelinePrefab;
    [SerializeField] private GameObject _pillarPrefab;
    [SerializeField] private GlobalBoolAsset _isConveyorSelected;
    [SerializeField] private GlobalBoolAsset _isPipelineSelected;

    private ConveyorConnectionData _connectionDataStart;
    private ConveyorConnectionData _connectionDataEnd;
    #endregion

    #region Functions
    Mesh GetMeshCopy(Mesh original)
    {
        Mesh copy = new Mesh();
        copy.SetVertices(original.vertices);
        copy.SetUVs(0,original.uv);
        copy.SetTriangles(original.triangles,0);
        copy.SetColors(original.colors32);
        copy.RecalculateBounds();
        copy.RecalculateNormals();
        copy.RecalculateTangents();

        return copy;
    }
    public GameObject SpawnPillar(GameObject previewPillar)
    {
        return Instantiate(_pillarPrefab,previewPillar.transform.position,previewPillar.transform.rotation,_buildingsRoot);      
    }
    public GameObject SpawnConveyor(Transform previewTransform)
    {
        return Instantiate(_conveyorPrefab, previewTransform.position, previewTransform.rotation, _buildingsRoot);
    }
    public GameObject SpawnPipeline(Transform previewTransform)
    {
        return Instantiate(_pipelinePrefab, previewTransform.position, previewTransform.rotation, _buildingsRoot);
    }
    bool IsThisTopPillar(int index)
    {
        return index == 0;
    }
    GameObject GetConveyor(Transform previewTransform)
    {
        GameObject newConveyor = null;
        if (_isConveyorSelected.value)
        {
            newConveyor = SpawnConveyor(previewTransform);
        }
        if (_isPipelineSelected.value)
        {
            newConveyor = SpawnPipeline(previewTransform);
        }
        newConveyor.GetComponent<MeshFilter>().mesh = GetMeshCopy(previewTransform.GetComponent<MeshFilter>().mesh);
        MeshCollider meshCollider = newConveyor.GetComponent<MeshCollider>();
        meshCollider.sharedMesh = GetMeshCopy(previewTransform.GetComponent<MeshCollider>().sharedMesh);
        return newConveyor;
    }
    GameObject GetPillar(GameObject previewPillar, int index)
    {
        if (previewPillar.activeSelf)
        {
            GameObject newPillar = SpawnPillar(previewPillar);
            newPillar.GetComponent<Pillar>().indexInPillarStack = previewPillar.GetComponent<Pillar>().indexInPillarStack;
            if (IsThisTopPillar(index))
            {

            }
        }
        return null;
    }
    #endregion



    #region Methods
    public void InstantiateInGameplayMode(OrientedPoint orientedPoints,Transform previewTransform,GameObject[] previewStartPillars, GameObject[] previewEndPillars)
    {
        GameObject getConveyor = GetConveyor(previewTransform);

        for (int i = 0; i < previewStartPillars.Length; i++)
        {
            GameObject startPillar = GetPillar(previewStartPillars[i],i);
            GameObject endPillar = GetPillar(previewEndPillars[i], i);
        }
    }
    public void UpdateConnectionDataStart(ConveyorConnectionData connectionDataStart)
    {
        _connectionDataStart = connectionDataStart;
    }
    public void UpdateConnectionDataEnd(ConveyorConnectionData connectionDataEnd)
    {
        _connectionDataEnd = connectionDataEnd;
    }
    public void InstantiateInSaveLoadMode()
    {

    }
    #endregion

}
