using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorInstantiator : MonoBehaviour
{
    #region Temp
    //[Header("Temporary Things", order = 0)]
    public Vector2[] uvs3;
    #endregion

    #region Fields
    [Header("Fields", order = 1)]    
    [SerializeField] private Transform _buildingsRoot;
    [SerializeField] private GameObject _conveyorPrefab;
    [SerializeField] private GameObject _pipelinePrefab;
    [SerializeField] private GameObject _pillarPrefab;
    [SerializeField] private GlobalBoolAsset _isConveyorSelected;
    [SerializeField] private GlobalBoolAsset _isPipelineSelected;

    public ConveyorConnectionData connectionDataStart;
    public ConveyorConnectionData connectionDataEnd;

    public Mesh lastCreatedMesh;
    #endregion

    #region Functions
    Mesh GetMeshCopy(Mesh original)
    {
        Mesh copy = new Mesh();
        copy.SetVertices(original.vertices);
        copy.SetUVs(0,original.uv);
        if (original.vertices.Length == uvs3.Length)
        {
            //copy.SetUVs(3, uvs3);
            List<Vector2> uvs33 = new List<Vector2>();
            original.GetUVs(3,uvs33);
            copy.SetUVs(3, uvs33);
       
        }
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
        lastCreatedMesh = GetMeshCopy(previewTransform.GetComponent<MeshFilter>().mesh);
        newConveyor.GetComponent<MeshFilter>().mesh = lastCreatedMesh;
        MeshCollider meshCollider = newConveyor.GetComponent<MeshCollider>();
        meshCollider.sharedMesh = GetMeshCopy(previewTransform.GetComponent<MeshCollider>().sharedMesh);
        return newConveyor;
    }
    GameObject GetPillar(GameObject previewPillar, int index)
    {
        GameObject newPillar = null;
        if (previewPillar.activeSelf)
        {
            newPillar = SpawnPillar(previewPillar);       
            newPillar.GetComponent<Pillar>().indexInPillarStack = previewPillar.GetComponent<Pillar>().indexInPillarStack;          
        }
        return newPillar;
    }
    bool IsThisTopPillar(int index)
    {
        return index == 0;
    }
    bool NeedNewPillars(GameObject[] previewStartPillars, GameObject[] previewEndPillars)
    {
        return previewStartPillars[0].activeSelf || previewEndPillars[0].activeSelf;
    }
    #endregion



    #region Methods
    public void Instantiate(OrientedPoint orientedPoints,Transform previewTransform,GameObject[] previewStartPillars, GameObject[] previewEndPillars, Vector2[] uvs3)
    {
        this.uvs3 = uvs3;
        GameObject newConveyor = GetConveyor(previewTransform);
        ConveyorController conveyorController = newConveyor.GetComponent<ConveyorController>();
        SetupConveyor(conveyorController, orientedPoints);

        Pillar topPillarStart = null;
        Pillar topPillarEnd = null;

        if (NeedNewPillars(previewStartPillars,previewEndPillars))
        {
            for (int i = 0; i < previewStartPillars.Length; i++)
            {
                GameObject startPillar = GetPillar(previewStartPillars[i], i);
                GameObject endPillar = GetPillar(previewEndPillars[i], i);
                         
                if (startPillar && IsThisTopPillar(i))
                {
                    topPillarStart = startPillar.GetComponent<Pillar>();                    
                }
                if (endPillar && IsThisTopPillar(i))
                {
                    topPillarEnd = endPillar.GetComponent<Pillar>();
                }
            }
        }

        if (connectionDataStart.isAlignedToExistingPillar)
        {
            topPillarStart = connectionDataStart.alignedToPillar;         
        }
        if (connectionDataEnd.isAlignedToExistingPillar)
        {
            topPillarEnd = connectionDataEnd.alignedToPillar;
        }
        SetupAlignedPillar(topPillarStart,connectionDataStart);
        SetupAlignedPillar(topPillarEnd,connectionDataEnd);
    }
    void SetupConveyor(ConveyorController conveyorController,OrientedPoint orientedPoints)
    {
        bool isDirectionReversed = connectionDataStart.conveyorSide == ConveyorConnectionData.ConveyorSide.End;      
        conveyorController.Setup(isDirectionReversed,orientedPoints.positions,orientedPoints.rotations);
    }
    void SetupAlignedPillar(Pillar pillar, ConveyorConnectionData connectionData)
    {
        pillar.Setup(connectionData.conveyorSide,connectionData.occupiedPillarSide);
    }
    #endregion

}
