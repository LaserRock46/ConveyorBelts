using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ConveyorSystem
{
    public class ConveyorInstantiator : MonoBehaviour
    {
        #region Temp
        [Header("Temporary Things", order = 0)]
        int conIndex;
        #endregion

        #region Fields
        [Header("Fields", order = 1)]
        private ConveyorConstructorConditions _conditions;
        [SerializeField] private Transform _buildingsRoot;



        public ConveyorConnectionData connectionDataStart;
        public ConveyorConnectionData connectionDataEnd;

        [HideInInspector] public Mesh lastCreatedMesh;
        #endregion

        #region Functions
        Mesh GetMeshCopy(Mesh original, bool setUvs3 = true)
        {
            Mesh copy = new Mesh();
            copy.SetVertices(original.vertices);
            copy.SetUVs(0, original.uv);
            if (setUvs3)
            {
                List<Vector2> uvs3 = new List<Vector2>();
                original.GetUVs(3, uvs3);
                copy.SetUVs(3, uvs3);
            }
            copy.SetTriangles(original.triangles, 0);
            copy.SetColors(original.colors32);
            copy.RecalculateBounds();
            copy.RecalculateNormals();
            copy.RecalculateTangents();

            return copy;
        }
        public GameObject SpawnPillar(GameObject previewPillar, GameObject pillarPrefab)
        {
            return Instantiate(pillarPrefab, previewPillar.transform.position, previewPillar.transform.rotation, _buildingsRoot);
        }
        public GameObject SpawnConveyor(Transform previewTransform, GameObject conveyorPrefab)
        {
            return Instantiate(conveyorPrefab, previewTransform.position, previewTransform.rotation, _buildingsRoot);
        }

        GameObject GetConveyor(Transform previewTransform, GameObject conveyorPrefab)
        {
            GameObject newConveyor = null;

            newConveyor = SpawnConveyor(previewTransform, conveyorPrefab);

            lastCreatedMesh = GetMeshCopy(previewTransform.GetComponent<MeshFilter>().mesh);
            newConveyor.GetComponent<MeshFilter>().mesh = lastCreatedMesh;
            MeshCollider meshCollider = newConveyor.GetComponent<MeshCollider>();
            meshCollider.sharedMesh = GetMeshCopy(previewTransform.GetComponent<MeshCollider>().sharedMesh, false);
            return newConveyor;
        }
        GameObject GetPillar(GameObject previewPillar, GameObject pillarPrefab, int index)
        {
            GameObject newPillar = null;
            if (previewPillar.activeSelf)
            {
                newPillar = SpawnPillar(previewPillar, pillarPrefab);
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
        IConveyorItemGate GetConsecutiveItemGate(Pillar start, Pillar end)
        {
            IConveyorItemGate conveyorItemGate = null;
            if (connectionDataStart.conveyorSide == ConveyorConnectionData.ConveyorDirection.Output)
            {
                if (connectionDataStart.occupiedPillarSide == ConveyorConnectionData.PillarSide.Front)
                {
                    conveyorItemGate = start.backConveyor;
                }
                else
                {
                    conveyorItemGate = start.frontConveyor;
                }
            }
            else if (connectionDataEnd.conveyorSide == ConveyorConnectionData.ConveyorDirection.Output)
            {
                if (connectionDataEnd.occupiedPillarSide == ConveyorConnectionData.PillarSide.Front)
                {
                    conveyorItemGate = end.backConveyor;
                }
                else
                {
                    conveyorItemGate = end.frontConveyor;
                }
            }

            return conveyorItemGate;
        }
        #endregion



        #region Methods
        public void Initialize(ConveyorConstructorConditions conditions)
        {
            _conditions = conditions;
        }
        public void ResetData()
        {
            connectionDataEnd = new ConveyorConnectionData(ConveyorConnectionData.ConveyorDirection.Input);
            connectionDataEnd = new ConveyorConnectionData(ConveyorConnectionData.ConveyorDirection.Output);
        }
        public void Instantiate(OrientedPoints orientedPoints, Transform previewTransform, GameObject[] previewStartPillars, GameObject[] previewEndPillars, ConveyorAsset conveyorAsset)
        {
            GameObject newConveyor = GetConveyor(previewTransform, conveyorAsset.conveyorPrefab);
            conIndex++;
            newConveyor.name = conIndex.ToString();
            ConveyorController conveyorController = newConveyor.GetComponent<ConveyorController>();

            Pillar topPillarStart = null;
            Pillar topPillarEnd = null;

            if (NeedNewPillars(previewStartPillars, previewEndPillars))
            {
                for (int i = 0; i < previewStartPillars.Length; i++)
                {
                    GameObject startPillar = GetPillar(previewStartPillars[i], conveyorAsset.pillarPrefab, i);
                    GameObject endPillar = GetPillar(previewEndPillars[i], conveyorAsset.pillarPrefab, i);

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

            SetupConveyor(conveyorController, orientedPoints, topPillarStart, topPillarEnd, conveyorAsset.conveyorSpeed);
            IConveyorItemGate conveyorItemGate = conveyorController.GetComponent<IConveyorItemGate>();
            SetupAlignedPillar(topPillarStart, connectionDataStart, conveyorItemGate);
            SetupAlignedPillar(topPillarEnd, connectionDataEnd, conveyorItemGate);
        }
        void SetupConveyor(ConveyorController conveyorController, OrientedPoints orientedPoints, Pillar start, Pillar end, float speed)
        {
            bool isDirectionReversed = connectionDataStart.conveyorSide == ConveyorConnectionData.ConveyorDirection.Output;

            IConveyorItemGate consecutiveFactoryOrConveyor = GetConsecutiveItemGate(start, end);
            conveyorController.Setup(isDirectionReversed, orientedPoints, consecutiveFactoryOrConveyor, speed);
        }
        void SetupAlignedPillar(Pillar pillar, ConveyorConnectionData connectionData, IConveyorItemGate conveyorItemGate)
        {
            pillar.Setup(connectionData.conveyorSide, connectionData.occupiedPillarSide, conveyorItemGate);
        }
        #endregion

    }
}
