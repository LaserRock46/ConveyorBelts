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
    public bool isConveyorConstructorEnabled;
    [Header("____________________")]
    [SerializeField] private ConveyorPath _conveyorPath;
    [Header("____________________")]
    [SerializeField] private ConveyorMesh _conveyorMesh;
    private ConveyorConstructorConditions _conditions;
    [Header("____________________")]
    [SerializeField] private LayerMask _raycastTarget = new LayerMask();

    public Transform previewTransform = null;
    public GameObject previewGameObject = null;
    [SerializeField] private Transform _conveyorStartTransform = null;
    [SerializeField] private Transform _conveyorEndTransform = null;

    public enum BuildingStage { None, InitializedPreview, SetStart, SetEnd }
    public BuildingStage buildingStage = BuildingStage.None;

    [HideInInspector] public Vector3 raycastPosition;
    [HideInInspector] public GameObject raycastGameObject;

    public Transform playerCameraTransform;

    [SerializeField] private Vector3 pillarHeight = new Vector3();
    [SerializeField] private Vector3 pillarToGroundOffset = new Vector3(0, -0.1f, 0);
    [SerializeField] private Vector3 conveyorTipToPillarOffset = new Vector3();
    public int pillarStackCount = 1;
    public int pillarStackCountMax = 5;

    [HideInInspector] public int rotationStep;
    [SerializeField] private int _rotationStepAngle = 10;
    [SerializeField] private int _rotationStepCount = 35;
    public bool rotationIsAuto;

    [SerializeField] private Vector3 _conveyorEndResetPosition = Vector3.forward;


    #endregion

    #region Functions
    Vector3 GetPreviewPositionForGrounded()
    {
        return raycastPosition + (pillarHeight * pillarStackCount) - pillarToGroundOffset;
    }
    Vector3 GetPreviewPositionForPillar()
    {
        return raycastGameObject.GetComponent<Pillar>().tipAnchor.position + conveyorTipToPillarOffset;
    }
    Vector3 GetPreviewEndTransformAutoRotation()
    {
        Vector3 startPosition = _conveyorStartTransform.position;
        startPosition.y = 0;
        Vector3 endPosition = _conveyorEndTransform.position;
        endPosition.y = 0;

        return (endPosition - startPosition).normalized;
    }
    Vector3 GetPreviewTransformManualRotation()
    {
        return new Vector3(0, rotationStep * _rotationStepAngle, 0);
    }
    #endregion



    #region Methods
    void Start()
    {
        _conditions = new ConveyorConstructorConditions(this);
        _conveyorMesh.AssignOrientedPoints(_conveyorPath.orientedPoints);
    }
    void Update()
    {
        PreviewStateUpdate();
        BuildingStageUpdate();
        RotationUpdate();
        BezierAndMeshUpdate();
        FinishAndCreateUpdate();
      
        if (updateCurveManual)
        { 
            _conveyorPath.ConstructPath();
            _conveyorMesh.MeshUpdate(false);
        }
        if (isConveyorConstructorEnabled)
        {
            _conveyorPath.DebugDraw();
        }
    }
    private void FixedUpdate()
    {
        RaycastUpdate();      
    }
    public void EnableConveyorConstructor(bool enable)
    {
        isConveyorConstructorEnabled = enable;
        StartCoroutine(ChangeBuildingStage(BuildingStage.None));
    }
    void BuildingStageUpdate()
    {
        if (_conditions.CanInitializeBuildingProcess())
        {
            StartCoroutine(ChangeBuildingStage(BuildingStage.InitializedPreview));

        }
        if (_conditions.CanSetStart() && !_conditions.IsPointerOverUI())
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
            EnableConveyorConstructor(false);
        }
    }
    IEnumerator ChangeBuildingStage(BuildingStage buildingStage)
    {
        yield return new WaitForEndOfFrame();
        this.buildingStage = buildingStage;
        //Debug.Log(buildingStage);
        yield return null;
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
            previewTransform.position = GetPreviewPositionForGrounded();
        }
        if (_conditions.CanHidePreview())
        {
            previewGameObject.SetActive(false);
        }
        if (_conditions.CanEnablePreview())
        {
            previewGameObject.SetActive(true);
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
    void BezierAndMeshUpdate()
    {
        if (_conditions.NeedUpdateAfterMove() || _conditions.NeedUpdateAfterRotation()|| _conditions.NeedUpdateAfterCameraMove() || _conditions.CanResetPreview())
        {
            UpdatePreviewPrototypeAndBezierLocation();
            if (_conditions.CanUpdateBezier() || _conditions.CanResetPreview())
            {
                //_bezier.Compute();
                _conveyorPath.ConstructPath();
                _conveyorMesh.MeshUpdate(false);
            }      
        }
    } 
    void UpdatePreviewPrototypeAndBezierLocation()
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
        if (_conditions.CanMoveBeltStart())
        {
            previewTransform.position = GetPreviewPositionForGrounded();
            previewTransform.eulerAngles = GetPreviewTransformManualRotation();
        }
        if (_conditions.CanMoveBeltEnd())
        {
            _conveyorEndTransform.position = GetPreviewPositionForGrounded();

            if (_conditions.IsEndPointRotationAuto())
            {
                _conveyorEndTransform.forward = GetPreviewEndTransformAutoRotation();
            }
            else
            {
                _conveyorEndTransform.eulerAngles = GetPreviewTransformManualRotation();
            }
        }
    }
    void AlignToPillar()
    {
        if (_conditions.CanMoveBeltStart())
        {
     

        }

        if (_conditions.CanMoveBeltEnd())
        {
            
        }
    }
    void FinishAndCreateUpdate()
    {
        if (_conditions.CanFinishAndCreate() && _conditions.ConstructionMeetsRequirements())
        {
            buildingStage = BuildingStage.None;
        }
    }
    #endregion

}
