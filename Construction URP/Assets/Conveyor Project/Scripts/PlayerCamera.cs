using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    #region Temp
    [Header("Temporary Things", order = 0)]
  
    #endregion

    #region Fields
    [Header("Fields", order = 1)]
    public Transform yawAndPosition;
    public Transform pitch;
    [SerializeField] private float pitchElevationUp = 1;
    [SerializeField] private float pitchElevationDown = 250;
    [SerializeField] private float _initialPitch = -35;
    private float _pitchAccumulatedAmount;
    public float sensitivityPitch = 0.4f;
    public float sensitivityYaw = 200;
    public float speed = 10;
  
    [SerializeField]private Rigidbody _rigidbody;
    [SerializeField] private SphereCollider _cameraCollider;
    [SerializeField] private Transform _cameraCenter;
    private bool _isPivotBlocked;
    #endregion

    #region Functions
    public float GetYawAmount()
    {
        float amount = Input.GetAxis("Mouse X") * sensitivityYaw;
        return amount * Time.deltaTime;
    }
    public float GetPitchAmount()
    {     
        float amount = Input.GetAxis("Mouse Y") * sensitivityPitch;      
        return -amount * Time.deltaTime;
    }
    Vector3 GetInputDirectionHorizontal()
    {
        Vector3 inputDirection = new Vector3();
        if (Input.GetKey(KeyCode.W))
        {
            inputDirection.z = 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            inputDirection.z = -1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            inputDirection.x = -1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            inputDirection.x = 1;
        }   
        return inputDirection.normalized;
    }
    Vector3 GetInputDirectionVertical()
    {
        Vector3 inputDirection = new Vector3();
        if (Input.GetKey(KeyCode.LeftShift))
        {
            inputDirection.y = -1;
        }
        if (Input.GetKey(KeyCode.Space))
        {
            inputDirection.y = 1;
        }
        return inputDirection;
    }
    private Vector3 _obstacleCheckBox = new Vector3(3f,0.5f,3f);
    bool IsStoppedByObstacle()
    {       
        if (Physics.CheckBox(yawAndPosition.position,_obstacleCheckBox,yawAndPosition.rotation))
        {
            return true;
        }
        return false;
    }

    #endregion



    #region Methods
    void Start()
    {
        InitializeRotation();
    }
    void Update()
    {
        Rotation();
    }
    private void FixedUpdate()
    {
        Movement();
    }
    void InitializeRotation()
    {
        float linearPitch = Mathf.InverseLerp(pitchElevationDown, pitchElevationUp, _initialPitch);
        _pitchAccumulatedAmount = linearPitch;
        float angle = Mathf.LerpAngle(pitchElevationDown, pitchElevationUp, _pitchAccumulatedAmount);
        pitch.localEulerAngles = new Vector3(angle, 0, 0);
    }
    void Rotation()
    {
        if (Input.GetKey(KeyCode.Mouse1))
        {
            yawAndPosition.Rotate(Vector3.up, GetYawAmount());
            //pitch.Rotate(Vector3.right, GetPitchAmount());
            _pitchAccumulatedAmount -= GetPitchAmount();
            _pitchAccumulatedAmount = Mathf.Clamp(_pitchAccumulatedAmount,0, 1);
            float angle = Mathf.LerpAngle(pitchElevationDown,pitchElevationUp,_pitchAccumulatedAmount);
            pitch.localEulerAngles = new Vector3(angle,0,0);
            _cameraCollider.center = yawAndPosition.InverseTransformPoint(_cameraCenter.position);
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
        /*
        if (_pitchAccumulatedAmount > pitchElevationUp)
        {
            pitch.localEulerAngles = new Vector3(pitchElevationUp, 0, 0);
            _pitchAccumulatedAmount = pitchElevationUp;
        }
        if (_pitchAccumulatedAmount < pitchElevationDown)
        {
            pitch.localEulerAngles = new Vector3(Mathf.Abs(pitchElevationDown), 0, 0);
            _pitchAccumulatedAmount = pitchElevationDown;
        }
        */
    }
    void Movement()
    {
        _isPivotBlocked = IsStoppedByObstacle();

        if (GetInputDirectionHorizontal() != Vector3.zero || GetInputDirectionVertical() != Vector3.zero)
        {
          
            Vector3 targetDirection = GetInputDirectionHorizontal();
            if (!_isPivotBlocked)
            {
                targetDirection.y = GetInputDirectionVertical().y;
            }
            else
            {
                targetDirection.y = 1;
            }
            _rigidbody.velocity = yawAndPosition.TransformDirection(targetDirection) * speed * Time.deltaTime;
        }
        if (GetInputDirectionHorizontal() == Vector3.zero && GetInputDirectionVertical() == Vector3.zero && _rigidbody.velocity != Vector3.zero)
        {
            if (!_isPivotBlocked)
            {
                _rigidbody.velocity = Vector3.zero;
            }
            else
            {
                _rigidbody.velocity = new Vector3(0, speed * Time.deltaTime, 0);
            }
        }
          
    }
    #endregion

}
