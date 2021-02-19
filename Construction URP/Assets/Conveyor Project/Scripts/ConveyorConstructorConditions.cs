using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
[System.Serializable]
public class ConveyorConstructorConditions
{
    #region Temp
    //[Header("Temporary Things", order = 0)]
    #endregion

    #region Fields
    [Header("Fields", order = 1)]
   
    private ConveyorConstructor _conveyorConstructor;

    public ConveyorConstructorConditions( ConveyorConstructor conveyorConstructor)
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
        if(_conveyorConstructor.raycastPosition != _prevHitPoint)
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
            //Debug.Log("NeedUpdateAfterCameraMove()");
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
    public bool CanUpdateBezier()
    {
        if(_conveyorConstructor.buildingStage != ConveyorConstructor.BuildingStage.None)
        {
            return true;
        }
        return false;
    }
    public bool IsEndPointRotationAuto()
    {
        //Debug.Log("IsEndPointRotationAuto() " + _conveyorConstructor.rotationIsAuto);
        return _conveyorConstructor.rotationIsAuto;
    }
    public bool CanInitializeBuildingProcess()
    {
        if (_conveyorConstructor.buildingStage == ConveyorConstructor.BuildingStage.None && _conveyorConstructor.isConveyorConstructorEnabled)
        {
            //Debug.Log("CanInitializeBuildingProcess()");
            return true;
        }
        return false;
    }
    public bool CanSetStart()
    {
        if (_conveyorConstructor.buildingStage == ConveyorConstructor.BuildingStage.InitializedPreview)
        {
            //Debug.Log("CanSetStart()");
            return true;
        }
        return false;
    }
    public bool CanSetEnd()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && _conveyorConstructor.buildingStage == ConveyorConstructor.BuildingStage.SetStart)
        {
            //Debug.Log("CanSetEnd()");
            return true;
        }
        return false;
    }
    public bool CanFinishAndCreate()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0) && _conveyorConstructor.buildingStage == ConveyorConstructor.BuildingStage.SetEnd)
        {
            //Debug.Log("CanFinishAndCreate()");
            return true;
        }
        return false;
    }
    public bool CanAbort()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //Debug.Log("CanAbort()");
            return true;
        }
        return false;
    }
    public bool CanHidePreview()
    {
        if (_conveyorConstructor.buildingStage == ConveyorConstructor.BuildingStage.None  && _conveyorConstructor.previewGameObject.activeSelf && !_conveyorConstructor.isConveyorConstructorEnabled)
        {
            //Debug.Log("CanHidePreview()");
            return true;
        }
        return false;
    }
    public bool CanResetPreview()
    {
        if (_conveyorConstructor.buildingStage == ConveyorConstructor.BuildingStage.None && _conveyorConstructor.previewGameObject.activeSelf)
        {
            //Debug.Log("CanResetPreview()");
            return true;
        }
        if (_conveyorConstructor.buildingStage == ConveyorConstructor.BuildingStage.None && !_conveyorConstructor.previewGameObject.activeSelf && _conveyorConstructor.isConveyorConstructorEnabled)
        {
            //Debug.Log("CanResetPreview()");
            return true;
        }
        return false;
    }
    public bool CanEnablePreview()
    {
        if (_conveyorConstructor.buildingStage == ConveyorConstructor.BuildingStage.InitializedPreview && !_conveyorConstructor.previewGameObject.activeSelf)
        {
            //Debug.Log("CanEnablePreview()");
            return true;
        }
        return false;
    }
    public bool CanChangeRotationMode()
    {
        //Debug.Log("CanChangeRotationMode()");
        return Input.GetKeyDown(KeyCode.Mouse2);
    }
    public bool CanRotateOneStepLeft()
    {
        if(Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            //Debug.Log("CanRotateOneStepLeft()");
            return true;
        }
        return false;
    }
    public bool CanRotateOneStepRight()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            //Debug.Log("CanRotateOneStepRight()");
            return true;
        }
        return false;
    }
    public bool CanMoveBeltStart()
    {
        if(_conveyorConstructor.buildingStage == ConveyorConstructor.BuildingStage.SetStart)
        {
            //Debug.Log("CanMoveBeltStart()");
            return true;
        }
        return false;
    }
    public bool CanMoveBeltEnd()
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
        if(_conveyorConstructor.raycastGameObject != null && _conveyorConstructor.raycastGameObject.TryGetComponent(out Pillar pillar))
        {
            if(pillar.isOccupied == false)
            {
                return true;
            }
        }
        return false;
    }
    public bool CanAlignToPillarFront()
    {
        if (_conveyorConstructor.previewGameObject != null && _conveyorConstructor.raycastGameObject.TryGetComponent(out Pillar pillar))
        {
            if ( pillar.self.InverseTransformPoint(_conveyorConstructor.raycastPosition).z > 0)
            {
                return true;
            }
        }
        return false;
    }
    public bool CanAlignToPillarBack()
    {
        if (_conveyorConstructor.previewGameObject != null && _conveyorConstructor.raycastGameObject.TryGetComponent(out Pillar pillar))
        {
            if (pillar.self.InverseTransformPoint(_conveyorConstructor.raycastPosition).z < 0)
            {
                return true;
            }
        }
        return false;
    }
    public bool IsDetectedConveyorConnectorStart()
    {
        if (_conveyorConstructor.previewGameObject != null && _conveyorConstructor.raycastGameObject.TryGetComponent(out ConveyorControllerConnector conveyorControllerConnector))
        {
            if (conveyorControllerConnector.conveyorController.startConnector == _conveyorConstructor.previewGameObject)
            {
                return true;
            }
        }
        return false;
    }
    public bool IsDetectedConveyorConnectorEnd()
    {
        if (_conveyorConstructor.previewGameObject != null && _conveyorConstructor.raycastGameObject.TryGetComponent(out ConveyorControllerConnector conveyorControllerConnector))
        {
            if (conveyorControllerConnector.conveyorController.endConnector == _conveyorConstructor.previewGameObject)
            {
                return true;
            }
        }
        return false;
    }
    public bool ConstructionMeetsRequirements()
    {
        return true;
    }
    #endregion



    #region Methods
  
    #endregion

}
