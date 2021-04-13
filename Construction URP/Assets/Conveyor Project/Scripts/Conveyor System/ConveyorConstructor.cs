using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorConstructor : MonoBehaviour
{
    #region Temp
    [Header("Temporary Things", order = 0)]
    public bool updateCurveManual;
    #endregion

    #region Fields
    [Header("Fields", order = 1)]
    private ConveyorConstructorConditions _conditions;
    public GlobalBoolAsset isConveyorConstructorEnabled;
    [Header("____________________")]
    [SerializeField] private ConveyorPath _conveyorPath;
    [Header("____________________")]
    [SerializeField] private ConveyorMesh _conveyorMesh;
    [Header("____________________")]
    [SerializeField] private ConveyorInstantiator _conveyorInstantiator;
    [Header("____________________")]
    [SerializeField] private ConveyorConstructorVisuals _conveyorConstructorVisuals;
    [Header("____________________")]
    [SerializeField] private LayerMask _raycastTarget = new LayerMask();

    public Transform previewTransform = null;
    public GameObject previewGameObject = null;
    [SerializeField] private Transform _conveyorStartTransform = null;
    [SerializeField] private Transform _conveyorEndTransform = null;
   
    public GameObject[] startPillarsStack;
    public  GameObject[] endPillarsStack;

    public enum BuildingStage { None, InitializedPreview, SetStart, SetEnd }
    public BuildingStage buildingStage = BuildingStage.None;

    [HideInInspector] public Vector3 raycastPosition;
    [HideInInspector] public GameObject raycastGameObject;

    public Transform playerCameraTransform;

    [SerializeField] private ConveyorAsset[] _conveyorAssets;
    private ConveyorAsset _selectedConveyorAsset;

    public int startPillarsStackCount = 1;
    public int endPillarsStackCount = 1;
    public int pillarsStackCountMax = 5;

    [HideInInspector] public int rotationStep;
    [SerializeField] private int _rotationStepAngle = 10;
    [SerializeField] private int _rotationStepCount = 35;
    [SerializeField] private Transform _autoRotationLookTarget;
    public bool rotationIsAuto;

    [SerializeField] private Vector3 _conveyorEndResetPosition = Vector3.forward;




    #endregion

    #region Functions
    Vector3 GetPreviewPositionForGroundedEnd()
    {
        Vector3 position = GetPreviewPositionForGroundedStart();
        position.y = 0;
        Vector3 startTransform = _conveyorStartTransform.position;
        startTransform.y = 0;

        if (Vector3.Distance(position, startTransform) < 4)
        {
            Vector3 yOffset = new Vector3();
            RaycastHit hit;

            if (Physics.Raycast(_conveyorEndTransform.position, -_conveyorEndTransform.up, out hit, Mathf.Infinity, _raycastTarget))
            {             
                yOffset = hit.point + (_selectedConveyorAsset.pillarHeight * endPillarsStackCount) - _selectedConveyorAsset.pillarToGroundOffset;
            }
            Vector3 diff = position - startTransform;
            Vector3 diffNormalized = diff.normalized * 4;
            Vector3 safePosition = diffNormalized + startTransform;
            safePosition.y = yOffset.y;
            return safePosition;
        }
        return raycastPosition + (_selectedConveyorAsset.pillarHeight * endPillarsStackCount) - _selectedConveyorAsset.pillarToGroundOffset ;
    }
    Vector3 GetPreviewPositionForGroundedStart()
    {
        
        return raycastPosition + (_selectedConveyorAsset.pillarHeight * startPillarsStackCount) - _selectedConveyorAsset.pillarToGroundOffset ;
    }
    Vector3 GetPreviewPositionAllignedToPillar()
    {
        return raycastGameObject.GetComponent<Pillar>().tipAnchor.position;
    }
    Vector3 GetPreviewRotationAllignedToPillarStart()
    {
        Vector3 direction = raycastGameObject.GetComponent<Pillar>().tipAnchor.forward;    
        return _conditions.IsThisSidePillarFront() ? direction: -direction;
    }
    Vector3 GetPreviewRotationAllignedToPillarEnd()
    {
        Vector3 direction = raycastGameObject.GetComponent<Pillar>().tipAnchor.forward;
        return _conditions.IsThisSidePillarFront() ? -direction : direction;
    }
    Vector3 GetPreviewPositionForPillarInStack(int heightMultiplier)
    {
        Vector3 pillarPosition = (-_selectedConveyorAsset.pillarHeight * heightMultiplier) - _selectedConveyorAsset.pillarToGroundOffset - _selectedConveyorAsset.conveyorTipToPillarOffset;
       
        return pillarPosition;
    }
    Vector3 GetPreviewEndTransformAutoRotation()
    {
        int lookAtPointIndex = _conveyorPath.circularArcStart.indexThisPoint;
        Vector3 startPosition = new Vector3();

        float leftToEnd = Vector3.Distance(_conveyorPath.circularArcStart.pointsLeftAnchor.position, _conveyorEndTransform.position);
        float rightToEnd = Vector3.Distance(_conveyorPath.circularArcStart.pointsRightAnchor.position, _conveyorEndTransform.position);
       
        if (leftToEnd < rightToEnd)
        {
            startPosition = _conveyorPath.circularArcStart.pointsLeft[lookAtPointIndex].position;          
        }
        else 
        {
            startPosition = _conveyorPath.circularArcStart.pointsRight[lookAtPointIndex].position;  
        }

        startPosition.y = 0;
        Vector3 endPosition = _conveyorEndTransform.position;
        endPosition.y = 0;

        return (endPosition - startPosition).normalized;
    }
    Vector3 GetPreviewTransformManualRotation()
    {
        return new Vector3(0, rotationStep * _rotationStepAngle, 0);
    }
    bool IsConveyorDirectionReversed()
    {      
       return _conveyorInstantiator.connectionDataStart.conveyorSide == ConveyorConnectionData.ConveyorSide.Output;
    }
    ConveyorConnectionData.ConveyorSide GetConveyorSideForExistingPillar(Pillar alignedPillar)
    {
        ConveyorConnectionData.ConveyorSide conveyorSide = ConveyorConnectionData.ConveyorSide.None;
        if (_conditions.CanMoveConveyorStart())
        {
            if(alignedPillar.conveyorSide == ConveyorConnectionData.ConveyorSide.None|| alignedPillar.conveyorSide == ConveyorConnectionData.ConveyorSide.Output)
            {
                conveyorSide = ConveyorConnectionData.ConveyorSide.Input;
            }
            else if(alignedPillar.conveyorSide == ConveyorConnectionData.ConveyorSide.Input)
            {
                conveyorSide = ConveyorConnectionData.ConveyorSide.Output;
            }
        }
        else if (_conditions.CanMoveConveyorEnd())
        {
            if (alignedPillar.conveyorSide == ConveyorConnectionData.ConveyorSide.None || alignedPillar.conveyorSide == ConveyorConnectionData.ConveyorSide.Input)
            {
                conveyorSide = ConveyorConnectionData.ConveyorSide.Output;
            }
            else if (alignedPillar.conveyorSide == ConveyorConnectionData.ConveyorSide.Output)
            {
                conveyorSide = ConveyorConnectionData.ConveyorSide.Input;
            }
        }
        return conveyorSide;
    }
    ConveyorConnectionData.ConveyorSide GetReversedConveyorSide(ConveyorConnectionData.ConveyorSide conveyorSide)
    {
        return conveyorSide == ConveyorConnectionData.ConveyorSide.Input ? ConveyorConnectionData.ConveyorSide.Output : ConveyorConnectionData.ConveyorSide.Input;
    }
    (ConveyorConnectionData.ConveyorSide conveyorSideStart, ConveyorConnectionData.ConveyorSide conveyorSideEnd) GetCorrectedConveyorSides(ConveyorConnectionData connectionDataStart,ConveyorConnectionData connectionDataEnd)
    {
        ConveyorConnectionData.ConveyorSide conveyorSideStart = ConveyorConnectionData.ConveyorSide.None;
        ConveyorConnectionData.ConveyorSide conveyorSideEnd = ConveyorConnectionData.ConveyorSide.None;

        bool sideStartIsUnpreferred = connectionDataStart.isAlignedToExistingPillar == false || (connectionDataStart.isAlignedToExistingPillar == true && connectionDataStart.alignedToPillar.conveyorSide == ConveyorConnectionData.ConveyorSide.None);

        if (sideStartIsUnpreferred)
        {
            if(connectionDataEnd.conveyorSide == ConveyorConnectionData.ConveyorSide.Output)
            {
                conveyorSideStart = ConveyorConnectionData.ConveyorSide.Input;
                conveyorSideEnd = ConveyorConnectionData.ConveyorSide.Output;
            }
            else
            {
                conveyorSideStart = ConveyorConnectionData.ConveyorSide.Output;
                conveyorSideEnd = ConveyorConnectionData.ConveyorSide.Input;
            }
        }
        else
        {
            conveyorSideStart = connectionDataStart.conveyorSide;
            conveyorSideEnd = connectionDataEnd.conveyorSide;
        }

        return (conveyorSideStart, conveyorSideEnd);
    }
    #endregion



    #region Methods
    void Awake()
    {
        _conditions = new ConveyorConstructorConditions(this);
        _conveyorInstantiator.Initialize(_conditions);
        _conveyorMesh.AssignOrientedPoints(_conveyorPath.orientedPoints);
    }
    void Update()
    {
        BuildingStageUpdate();
        PreviewStateUpdate();
        if (_conditions.CanUpdateBuildingProcess())
        {
            PreviewPillarsUpdate();
            RotationUpdate();
            PathAndMeshUpdate();
            FinishAndCreateUpdate();

            _conveyorPath.DebugDraw();         
        }
    }
    private void FixedUpdate()
    {
        if (_conditions.CanUpdateBuildingProcess())
        {
            RaycastUpdate();
        }
    }   
    void BuildingStageUpdate()
    {
        if (_conditions.CanInitializeBuildingProcess())
        {
            StartCoroutine(ChangeBuildingStage(BuildingStage.InitializedPreview));
            SelectConveyorAsset();
        }
        if (_conditions.CanSetStart())
        {
            StartCoroutine(ChangeBuildingStage(BuildingStage.SetStart));
        }
        if (_conditions.CanSetEnd() && !_conditions.IsPointerOverUI())
        {
            StartCoroutine(ChangeBuildingStage(BuildingStage.SetEnd));
        }
        if (_conditions.CanAbort())
        {
            StartCoroutine(ChangeBuildingStage(BuildingStage.None));        
        }
    }
    IEnumerator ChangeBuildingStage(BuildingStage buildingStage)
    {
        yield return new WaitForEndOfFrame();
        this.buildingStage = buildingStage;
        yield return null;
    }
    void SelectConveyorAsset()
    {
        for (int i = 0; i < _conveyorAssets.Length; i++)
        {
            if (_conveyorAssets[i].isSelected.value)
            {
                _selectedConveyorAsset = _conveyorAssets[i];
            }
        }
    }
    void RotationUpdate()
    {
        if (_conditions.CanChangeRotationMode())
        {
            rotationIsAuto = !rotationIsAuto;
        }
        if (_conditions.CanRotateOneStepLeft())
        {
            rotationStep--;
            if (rotationStep < 0)
            {
                rotationStep = _rotationStepCount;
            }
        }
        if (_conditions.CanRotateOneStepRight())
        {
            rotationStep++;
            if (rotationStep > _rotationStepCount)
            {
                rotationStep = 0;
            }
        }
    }
    void PreviewStateUpdate()
    {
        if (_conditions.CanResetPreview())
        {
            _conveyorEndTransform.localPosition = _conveyorEndResetPosition;
            _conveyorEndTransform.forward = _conveyorStartTransform.forward;       
            previewTransform.position = new Vector3(0,-1000,0);
            _conveyorConstructorVisuals.UpdateArrowsDirection(false);
        }
        if (_conditions.CanHidePreview())
        {
            previewGameObject.SetActive(false);
            previewTransform.position = new Vector3(0, -1000, 0);
        }
        if (_conditions.CanEnablePreview())
        {
            previewGameObject.SetActive(true);
            previewTransform.position = new Vector3(0, -1000, 0);
            EnableNeededPreviewPillars();
        }
    }
    void PreviewPillarsUpdate()
    {
        if (_conditions.CanIncreaseStartPillarCount())
        {
            startPillarsStackCount++;
            endPillarsStackCount = startPillarsStackCount;
            EnableNeededPreviewPillars();        
        }
        if (_conditions.CanDecreaseStartPillarCount())
        {
            startPillarsStackCount--;
            endPillarsStackCount = startPillarsStackCount;
            EnableNeededPreviewPillars();         
        }
        if (_conditions.CanIncreaseEndPillarCount())
        {
            endPillarsStackCount++;
            EnableNeededPreviewPillars(enableStart: _conditions.IsStartPillarsEnabled());
        }
        if (_conditions.CanDecreaseEndPillarCount())
        {
            endPillarsStackCount--;
            EnableNeededPreviewPillars(enableStart: _conditions.IsStartPillarsEnabled());
        }

        if (_conditions.CanResetPreview())
        {
            startPillarsStackCount = endPillarsStackCount;       
            EnableNeededPreviewPillars();
        }
    }
    void EnableNeededPreviewPillars(bool enableStart = true,bool enableEnd = true)
    {
        for (int i = 0; i < pillarsStackCountMax; i++)
        {
            if (buildingStage == BuildingStage.SetStart || _conditions.CanResetPreview())
            {
                startPillarsStack[i].transform.localPosition = GetPreviewPositionForPillarInStack(i + 1);
            }
            endPillarsStack[i].transform.localPosition = GetPreviewPositionForPillarInStack(i + 1);

            startPillarsStack[i].SetActive(startPillarsStackCount - 1 >= i && enableStart);
            endPillarsStack[i].SetActive(endPillarsStackCount - 1 >= i && enableEnd);
        }
    }
    void RaycastUpdate()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _raycastTarget))
        {
            if (hit.point != raycastPosition)
            {
                raycastPosition = hit.point;
            }
            if (hit.collider.gameObject != raycastGameObject)
            {
                raycastGameObject = hit.collider.gameObject;
            }         
        }
        else
        {
            if (raycastGameObject != null)
            {
                raycastGameObject = null;
            }
        }
    }
    void PathAndMeshUpdate()
    {
        if (_conditions.NeedUpdateAfterMove() || _conditions.NeedUpdateAfterRotation()|| _conditions.NeedUpdateAfterCameraMove() || _conditions.CanResetPreview() || _conditions.NeedUpdateAfterPillarHeightChanged())
        {     
            UpdatePreviewAlignment();
            if (_conditions.CanUpdatePath()||_conditions.CanResetPreview())
            {          
                _conveyorPath.ConstructPath();              
                _conveyorMesh.MeshUpdate(IsConveyorDirectionReversed(),_selectedConveyorAsset);
            }              
        }
    } 
    void UpdatePreviewAlignment()
    {     
        if (_conditions.CanAlignToGround())
        {
            AlignToGround();
        }
        if (_conditions.CanAlignToPillar())
        {    
            AlignToPillar();
        }
    }
    void AlignToGround()
    {
        if (_conditions.CanMoveConveyorStart() || _conditions.CanResetPreview())
        {
            previewTransform.position = GetPreviewPositionForGroundedStart();
            previewTransform.eulerAngles = GetPreviewTransformManualRotation();
            if (!_conditions.IsStartPillarsEnabled() || !_conditions.IsEndPillarsEnabled())
            {
                EnableNeededPreviewPillars(true,true);
            }

            _conveyorInstantiator.connectionDataStart = new ConveyorConnectionData(null,false,ConveyorConnectionData.PillarSide.Front,ConveyorConnectionData.ConveyorSide.Input);         
        }
        if (_conditions.CanMoveConveyorEnd())
        {
            _conveyorEndTransform.position = GetPreviewPositionForGroundedEnd();

            if (_conditions.IsEndPointRotationAuto())
            {
                _conveyorEndTransform.forward = GetPreviewEndTransformAutoRotation();
            }
            else
            {
                _conveyorEndTransform.eulerAngles = GetPreviewTransformManualRotation();
            }

            if (!_conditions.IsEndPillarsEnabled())
            {
                EnableNeededPreviewPillars(_conditions.IsStartPillarsEnabled(), true);
            }

            ConveyorConnectionData.ConveyorSide conveyorSide = GetReversedConveyorSide(_conveyorInstantiator.connectionDataStart.conveyorSide);
            _conveyorInstantiator.connectionDataEnd = new ConveyorConnectionData(null, false, ConveyorConnectionData.PillarSide.Back, conveyorSide);
            
      
        }
        _conveyorConstructorVisuals.UpdateArrowsDirection(IsConveyorDirectionReversed());
    }
    void AlignToPillar()
    {
        if (_conditions.CanMoveConveyorStart())
        {
            previewTransform.position = GetPreviewPositionAllignedToPillar();
            previewTransform.forward = GetPreviewRotationAllignedToPillarStart();
            if (_conditions.IsStartPillarsEnabled())
            {
                EnableNeededPreviewPillars(false, false);
            }

            Pillar pillar = raycastGameObject.GetComponent<Pillar>();
            ConveyorConnectionData.PillarSide pillarSide = _conditions.IsThisSidePillarFront() ? ConveyorConnectionData.PillarSide.Front : ConveyorConnectionData.PillarSide.Back;
            ConveyorConnectionData.ConveyorSide conveyorSide = GetConveyorSideForExistingPillar(pillar);

            _conveyorInstantiator.connectionDataStart = new ConveyorConnectionData(pillar, true, pillarSide, conveyorSide);
            _conveyorInstantiator.connectionDataEnd = new ConveyorConnectionData(GetReversedConveyorSide(conveyorSide));
           
        }

        if (_conditions.CanMoveConveyorEnd())
        {
            _conveyorEndTransform.position = GetPreviewPositionAllignedToPillar();
            _conveyorEndTransform.forward = GetPreviewRotationAllignedToPillarEnd();
            if (_conditions.IsEndPillarsEnabled())
            {
                EnableNeededPreviewPillars(_conditions.IsStartPillarsEnabled(), false);
            }

            Pillar pillar = raycastGameObject.GetComponent<Pillar>();
            ConveyorConnectionData.PillarSide pillarSide = _conditions.IsThisSidePillarFront() ? ConveyorConnectionData.PillarSide.Front : ConveyorConnectionData.PillarSide.Back;
            ConveyorConnectionData.ConveyorSide conveyorSide = GetConveyorSideForExistingPillar(pillar);

            _conveyorInstantiator.connectionDataEnd = new ConveyorConnectionData(pillar, true, pillarSide, conveyorSide);

            var corrected = GetCorrectedConveyorSides(_conveyorInstantiator.connectionDataStart, _conveyorInstantiator.connectionDataEnd);
            _conveyorInstantiator.connectionDataStart.CorrectConveyorSide(corrected.conveyorSideStart);
            _conveyorInstantiator.connectionDataEnd.CorrectConveyorSide(corrected.conveyorSideEnd);

        }
        _conveyorConstructorVisuals.UpdateArrowsDirection(IsConveyorDirectionReversed());
    }

    void FinishAndCreateUpdate()
    {
        if (_conditions.CanFinishAndCreate() && _conditions.ConstructionMeetsRequirements() && !_conditions.IsPointerOverUI())
        {
            buildingStage = BuildingStage.None;
            _conveyorMesh.BakeCollider(_selectedConveyorAsset);
            _conveyorInstantiator.Instantiate(_conveyorPath.orientedPoints,previewTransform,startPillarsStack,endPillarsStack,_selectedConveyorAsset);
            _conveyorConstructorVisuals.PlayRevealEffect(_conveyorPath.orientedPoints, previewTransform, _conveyorInstantiator.lastCreatedMesh, IsConveyorDirectionReversed());
        }
    }
    #endregion

}
