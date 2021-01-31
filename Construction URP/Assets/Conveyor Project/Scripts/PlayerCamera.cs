using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    #region Temp
    [Header("Temporary Things", order = 0)]
    public float pitchTest;
    public Vector3 inputDirTest;
    #endregion

    #region Fields
    [Header("Fields", order = 1)]
    public Transform yawAndPosition;
    public Transform pitch;
    [SerializeField] private float pitchElevationUp = 1;
    [SerializeField] private float pitchElevationDown = 250;
    [SerializeField] private float _initialPitch = -35;
    private float _pitchAccumulatedAmount;
    public float sensitivity = 150;
    public float speedHorizontal = 10;
    public float speedVertical = 5;
    [SerializeField]private Rigidbody _rigidbody;
    [SerializeField] private SphereCollider _cameraCollider;
    [SerializeField] private Transform _cameraCenter;
    #endregion

    #region Functions
    public float GetYawAmount()
    {
        float amount = Input.GetAxis("Mouse X") * sensitivity;
        return amount * Time.deltaTime;
    }
    public float GetPitchAmount()
    {     
        float amount = Input.GetAxis("Mouse Y") * sensitivity;      
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

    #endregion



    #region Methods
    void Start()
    {
        _pitchAccumulatedAmount = _initialPitch;
    }
    void Update()
    {
        Rotation();
    }
    private void FixedUpdate()
    {
        Movement();
    }
    void Rotation()
    {
        if (Input.GetKey(KeyCode.Mouse1))
        {
            yawAndPosition.Rotate(Vector3.up, GetYawAmount());
            pitch.Rotate(Vector3.right, GetPitchAmount());
            _pitchAccumulatedAmount -= GetPitchAmount();
            _cameraCollider.center = yawAndPosition.InverseTransformPoint(_cameraCenter.position);
        }
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
      
        pitchTest = pitch.localEulerAngles.x;
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
    }
    void Movement()
    {
        /*
        if(GetInputDirectionHorizontal() != Vector3.zero)
        {
            yawAndPosition.Translate(GetInputDirectionHorizontal() * speedHorizontal * Time.deltaTime,Space.Self);
            
        }
        if (GetInputDirectionVertical() != Vector3.zero)
        {
            yawAndPosition.Translate(GetInputDirectionVertical()* speedVertical * Time.deltaTime);
        }
        */
        if (GetInputDirectionHorizontal() != Vector3.zero || GetInputDirectionVertical() != Vector3.zero)
        {
          
            Vector3 targetVelocity = GetInputDirectionHorizontal();
            targetVelocity.y = GetInputDirectionVertical().y;
            _rigidbody.velocity = yawAndPosition.TransformDirection(targetVelocity) * speedHorizontal * Time.deltaTime;
        }
        if (GetInputDirectionHorizontal() == Vector3.zero && GetInputDirectionVertical() == Vector3.zero && _rigidbody.velocity != Vector3.zero)
        {
            _rigidbody.velocity = Vector3.zero;
        }
      
        inputDirTest = GetInputDirectionHorizontal();
    }
    #endregion

}
