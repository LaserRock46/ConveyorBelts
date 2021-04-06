using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorConstructorVisuals : MonoBehaviour
{
    #region Temp
    //[Header("Temporary Things", order = 0)]
    #endregion

    #region Fields
    [Header("Fields", order = 1)]
    private bool _isPlaying = false;
    private bool _isConveyorDirectionReversed;
    [SerializeField] private Transform _arrowStart;
    [SerializeField] private Transform _arrowEnd;
    [SerializeField] private Vector3 _arrowScale = new Vector3(0.2f, 0.5f, 0.2f);
    [SerializeField] private float _revealSpeed = 2;
    [SerializeField] private float _revealProgress;
    [SerializeField] private float _revealTarget;
    private OrientedPoint _orientedPoints;
    [SerializeField] private MeshRenderer _conveyorRenderer;
    [SerializeField] private MeshFilter _conveyorMeshFilter;
    [SerializeField] private Material _revealMaterial;
    [SerializeField] private Transform _conveyorTransform;
    [SerializeField] private Transform _revealEffectTransform;

    private Vector3[] _positions;
    private Quaternion[] _rotations;
    [SerializeField] private ItemTransmission _revealEffectTransmission;

    [SerializeField] private string _shaderRevealPropertyString;
    private int _shaderRevealPropertyID;
    #endregion

    #region Functions

    #endregion



    #region Methods
    private void Awake()
    {
        Initialize();
    }
    private void Update()
    {
        UpdateRevealEffect();
    }
    private void Initialize()
    {    
        _shaderRevealPropertyID = Shader.PropertyToID(_shaderRevealPropertyString);
        _revealEffectTransmission.AddItem(_revealEffectTransform);
    }
    public void UpdateArrowsDirection(bool isConveyorDirectionReversed)
    {
        if (isConveyorDirectionReversed)
        {
            _arrowStart.localScale = -_arrowScale;
            _arrowEnd.localScale = -_arrowScale;
        }
        else
        {
            _arrowStart.localScale = _arrowScale;
            _arrowEnd.localScale = _arrowScale;
        }
    }
    public void PlayRevealEffect(OrientedPoint orientedPoints, Transform previewTransform, Mesh lastCreatedMesh,bool isConveyorDirectionReversed)
    {
        _isPlaying = true;
        _isConveyorDirectionReversed = isConveyorDirectionReversed;
        _revealProgress = 0;
        _revealTarget = orientedPoints.totalDistance;
        _conveyorTransform.gameObject.SetActive(true);
        _revealEffectTransform.gameObject.SetActive(true);
        _orientedPoints = orientedPoints;

        _conveyorTransform.SetPositionAndRotation(previewTransform.position, previewTransform.rotation);
        _conveyorMeshFilter.sharedMesh = lastCreatedMesh;

        Vector3[] worldPositions = ConveyorController.PositionsLocalToWorld(orientedPoints.positions, _conveyorTransform);
        
        _revealEffectTransmission.CreatePath(isConveyorDirectionReversed, _revealSpeed, worldPositions,orientedPoints.segmentDistanceForward,orientedPoints.totalDistance);
        _revealEffectTransmission.SetItemProgress(0, isConveyorDirectionReversed ? orientedPoints.totalDistance : 0);
    }
    void UpdateRevealEffect()
    {
        if (!_isPlaying) return;

        _revealProgress += Time.deltaTime * _revealSpeed;
           
        _revealEffectTransmission.Update();
        _revealMaterial.SetFloat(_shaderRevealPropertyID,_revealProgress);     

        if(_revealProgress >= _revealTarget)
        {
            _isPlaying = false;
            _conveyorTransform.gameObject.SetActive(false);
            _revealEffectTransform.gameObject.SetActive(false);
        }

    }
    #endregion

}
