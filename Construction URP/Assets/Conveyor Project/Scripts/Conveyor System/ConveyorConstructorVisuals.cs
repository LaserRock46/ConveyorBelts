using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ConveyorSystem
{
    public class ConveyorConstructorVisuals : MonoBehaviour
    {
        #region Temp
        //[Header("Temporary Things", order = 0)]
        #endregion

        #region Fields
        [Header("Fields", order = 1)]
        private bool _isPlaying = false;
        private bool _isMarkedError = false;

        [SerializeField] private Transform _arrowStart;
        [SerializeField] private Transform _arrowEnd;
        [SerializeField] private Vector3 _arrowScale = new Vector3(0.2f, 0.5f, 0.2f);
        [SerializeField] private float _revealSpeed = 2;
        [SerializeField] private float _revealProgress;
        [SerializeField] private float _revealTarget;
        [SerializeField] private MeshRenderer _conveyorRenderer;
        [SerializeField] private MeshRenderer _conveyorRevealRenderer;
        [SerializeField] private MeshFilter _conveyorRevealMeshFilter;
        [SerializeField] private Material _buildModeMaterial;
        [SerializeField] private Material _errorMaterial;
        private MaterialPropertyBlock _revealMaterialPropertyBlock;
        [SerializeField] private Transform _conveyorTransform;
        [SerializeField] private Transform _revealEffectTransform;

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
            _revealMaterialPropertyBlock = new MaterialPropertyBlock();
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
        public void PlayRevealEffect(OrientedPoints orientedPoints, Transform previewTransform, Mesh lastCreatedMesh, bool isConveyorDirectionReversed)
        {
            _isPlaying = true;          
            _revealProgress = 0;
            _revealTarget = orientedPoints.totalDistance;
            _conveyorTransform.gameObject.SetActive(true);
            _revealEffectTransform.gameObject.SetActive(true);

            _conveyorTransform.SetPositionAndRotation(previewTransform.position, previewTransform.rotation);
            _conveyorRevealMeshFilter.sharedMesh = lastCreatedMesh;

            Vector3[] worldPositions = ConveyorController.PositionsLocalToWorld(orientedPoints.positions, _conveyorTransform);

            _revealEffectTransmission.CreatePath(isConveyorDirectionReversed, _revealSpeed, worldPositions, orientedPoints, orientedPoints.totalDistance, clearPath: true);
            _revealEffectTransmission.SetItemProgress(0, 0);
        }
        void UpdateRevealEffect()
        {
            if (!_isPlaying) return;

            _revealProgress += Time.deltaTime * _revealSpeed;

            _revealEffectTransmission.UpdateForRevealEffect();

            _conveyorRevealRenderer.GetPropertyBlock(_revealMaterialPropertyBlock);
            _revealMaterialPropertyBlock.SetFloat(_shaderRevealPropertyID, _revealProgress);
            _conveyorRevealRenderer.SetPropertyBlock(_revealMaterialPropertyBlock);

            if (_revealProgress >= _revealTarget)
            {
                _isPlaying = false;
                _conveyorTransform.gameObject.SetActive(false);
                _revealEffectTransform.gameObject.SetActive(false);
            }

        }
        public void MarkError(bool meetAllRequirements)
        {
            if(!meetAllRequirements && !_isMarkedError)
            {
                _isMarkedError = true;
                _conveyorRenderer.material = _errorMaterial;
            }
            else if (meetAllRequirements && _isMarkedError)
            {
                _isMarkedError = false;
                _conveyorRenderer.material = _buildModeMaterial;
            }
        }
        #endregion

    }
}
