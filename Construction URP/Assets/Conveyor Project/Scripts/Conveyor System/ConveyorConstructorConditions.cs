using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ConveyorSystem
{
    [System.Serializable]
    public class ConveyorConstructorConditions
    {
        #region Temp
        //[Header("Temporary Things", order = 0)]
        #endregion

        #region Fields
        [Header("Fields", order = 1)]

        private ConveyorConstructor _conveyorConstructor;

        public ConveyorConstructorConditions(ConveyorConstructor conveyorConstructor)
        {
            _conveyorConstructor = conveyorConstructor;
        }
        #endregion

        #region Functions
        public bool IsPointerOverUI()
        {
            return EventSystem.current.IsPointerOverGameObject();
        }
        private Vector3 _prevHitPoint = new Vector3();
        public bool NeedUpdateAfterMove()
        {
            if (_conveyorConstructor.raycastPosition != _prevHitPoint)
            {
                _prevHitPoint = _conveyorConstructor.raycastPosition;
                return true;
            }
            return false;
        }
        private Vector3 _prevCameraPosition = new Vector3();
        public bool NeedUpdateAfterCameraMove()
        {
            if (_conveyorConstructor.playerCameraTransform.position != _prevCameraPosition)
            {
                _prevCameraPosition = _conveyorConstructor.playerCameraTransform.position;
                return true;
            }
            return false;
        }
        private int _prevRotationStep = 0;
        private bool _prevRotationIsAuto = false;
        public bool NeedUpdateAfterRotation()
        {
            if (_conveyorConstructor.rotationStep != _prevRotationStep)
            {
                _prevRotationStep = _conveyorConstructor.rotationStep;
                return true;
            }
            if (_conveyorConstructor.rotationIsAuto != _prevRotationIsAuto)
            {
                _prevRotationIsAuto = _conveyorConstructor.rotationIsAuto;
                return true;
            }
            return false;
        }
        private int _prevStartStack = 0;
        private int _prevEndStack = 0;
        public bool NeedUpdateAfterPillarHeightChanged()
        {
            if (_prevStartStack != _conveyorConstructor.startPillarsStackCount)
            {
                _prevStartStack = _conveyorConstructor.startPillarsStackCount;
                return true;
            }
            if (_prevEndStack != _conveyorConstructor.endPillarsStackCount)
            {
                _prevEndStack = _conveyorConstructor.endPillarsStackCount;
                return true;
            }
            return false;
        }
        public bool CanUpdatePath()
        {      
            return _conveyorConstructor.buildingStage != ConveyorConstructor.BuildingStage.None;
        }
        public bool IsEndPointRotationAuto()
        {
            return _conveyorConstructor.rotationIsAuto;
        }
        public bool IsStartPillarsEnabled()
        {
            return _conveyorConstructor.startPillarsStack[0].activeSelf;
        }
        public bool IsEndPillarsEnabled()
        {
            return _conveyorConstructor.endPillarsStack[0].activeSelf;
        }
        public bool CanInitializeBuildingProcess()
        {
            if (_conveyorConstructor.buildingStage == ConveyorConstructor.BuildingStage.None && _conveyorConstructor.isConveyorConstructorEnabled.value == true)
            {
                return true;
            }
            return false;
        }
        public bool CanUpdateBuildingProcess()
        {
            return _conveyorConstructor.isConveyorConstructorEnabled.value;
        }
        public bool CanSetStart()
        {
            if (_conveyorConstructor.buildingStage == ConveyorConstructor.BuildingStage.InitializedPreview)
            {
                return true;
            }
            return false;
        }
        public bool CanSetEnd()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) && _conveyorConstructor.buildingStage == ConveyorConstructor.BuildingStage.SetStart)
            {
                return true;
            }
            return false;
        }
        public bool CanFinishAndCreate()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) && _conveyorConstructor.buildingStage == ConveyorConstructor.BuildingStage.SetEnd)
            {
                return true;
            }
            return false;
        }
        public bool CanAbort()
        {
            if (_conveyorConstructor.isConveyorConstructorEnabled.value == false && _conveyorConstructor.buildingStage != ConveyorConstructor.BuildingStage.None)
            {
                return true;
            }
            return false;
        }
        public bool CanIncreaseStartPillarCount()
        {
            return Input.GetKeyDown(KeyCode.Period) && _conveyorConstructor.startPillarsStackCount < _conveyorConstructor.pillarsStackCountMax && _conveyorConstructor.buildingStage == ConveyorConstructor.BuildingStage.SetStart && IsStartPillarsEnabled();
        }
        public bool CanDecreaseStartPillarCount()
        {
            return Input.GetKeyDown(KeyCode.Comma) && _conveyorConstructor.startPillarsStackCount > 1 && _conveyorConstructor.buildingStage == ConveyorConstructor.BuildingStage.SetStart && IsStartPillarsEnabled();
        }
        public bool CanIncreaseEndPillarCount()
        {
            return Input.GetKeyDown(KeyCode.Period) && _conveyorConstructor.endPillarsStackCount < _conveyorConstructor.pillarsStackCountMax && _conveyorConstructor.buildingStage == ConveyorConstructor.BuildingStage.SetEnd && IsEndPillarsEnabled();
        }
        public bool CanDecreaseEndPillarCount()
        {
            return Input.GetKeyDown(KeyCode.Comma) && _conveyorConstructor.endPillarsStackCount > 1 && _conveyorConstructor.buildingStage == ConveyorConstructor.BuildingStage.SetEnd && IsEndPillarsEnabled();
        }
        public bool CanHidePreview()
        {
            if (_conveyorConstructor.buildingStage == ConveyorConstructor.BuildingStage.None && _conveyorConstructor.previewGameObject.activeSelf && !_conveyorConstructor.isConveyorConstructorEnabled.value)
            {
                return true;
            }
            return false;
        }
        public bool CanResetPreview()
        {
            if (_conveyorConstructor.buildingStage == ConveyorConstructor.BuildingStage.None && _conveyorConstructor.previewGameObject.activeSelf)
            {
                return true;
            }
            if (_conveyorConstructor.buildingStage == ConveyorConstructor.BuildingStage.None && !_conveyorConstructor.previewGameObject.activeSelf && _conveyorConstructor.isConveyorConstructorEnabled.value)
            {
                return true;
            }
            return false;
        }
        public bool CanEnablePreview()
        {
            if (_conveyorConstructor.buildingStage == ConveyorConstructor.BuildingStage.InitializedPreview && !_conveyorConstructor.previewGameObject.activeSelf)
            {
                return true;
            }
            return false;
        }
        public bool CanChangeRotationMode()
        {
            return Input.GetKeyDown(KeyCode.Mouse2);
        }
        public bool CanRotateOneStepLeft()
        {
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                return true;
            }
            return false;
        }
        public bool CanRotateOneStepRight()
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                return true;
            }
            return false;
        }
        public bool CanMoveConveyorStart()
        {
            if (_conveyorConstructor.buildingStage == ConveyorConstructor.BuildingStage.SetStart)
            {
                return true;
            }
            return false;
        }
        public bool CanMoveConveyorEnd()
        {
            if (_conveyorConstructor.buildingStage == ConveyorConstructor.BuildingStage.SetEnd)
            {
                return true;
            }
            return false;
        }
        private const int GROUND_LAYER_INDEX = 6;
        public bool CanAlignToGround()
        {
            if (_conveyorConstructor.raycastGameObject != null && _conveyorConstructor.raycastGameObject.layer == GROUND_LAYER_INDEX)
            {
                return true;
            }
            return false;
        }

        public bool CanAlignToPillar()
        {
            if (_conveyorConstructor.raycastGameObject != null && _conveyorConstructor.raycastGameObject.TryGetComponent(out Pillar pillar))
            {
                if (IsThisSidePillarFront() && !pillar.IsOccupiedFront())
                {
                    return true;
                }
                if (IsThisSidePillarBack() && !pillar.IsOccupiedBack())
                {
                    return true;
                }
            }
            return false;
        }
        public bool IsThisSidePillarFront()
        {
            if (_conveyorConstructor.raycastGameObject != null && _conveyorConstructor.raycastGameObject.TryGetComponent(out Pillar pillar))
            {
                if (pillar.self.InverseTransformPoint(_conveyorConstructor.raycastPosition).z > 0)
                {
                    return true;
                }
            }
            return false;
        }
        public bool IsThisSidePillarBack()
        {
            if (_conveyorConstructor.raycastGameObject != null && _conveyorConstructor.raycastGameObject.TryGetComponent(out Pillar pillar))
            {
                if (pillar.self.InverseTransformPoint(_conveyorConstructor.raycastPosition).z < 0)
                {
                    return true;
                }
            }
            return false;
        }    
        public bool ConstructionMeetsRequirements()
        {
            return _conveyorConstructor.conveyorRequirements.result.meetAllRequirements;
        }
        #endregion
        #region Methods

        #endregion
    }
}