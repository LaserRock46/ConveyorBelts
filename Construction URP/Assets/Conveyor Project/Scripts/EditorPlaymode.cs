using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class EditorPlaymode : MonoBehaviour
{
    #region Temp
    //[Header("Temporary Things", order = 0)]
    #endregion

    #region Fields
    [Header("Fields", order = 1)]
    public KeyCode firstKey = KeyCode.Space; 
    public KeyCode pause = KeyCode.A;
    public KeyCode exit = KeyCode.Escape;
    #endregion

    #region Functions
    
    #endregion

    

    #region Methods
    void Start()
    {
        
    }
   void Update()
    {
        if(Input.GetKeyDown(pause) && Input.GetKey(firstKey))
        {
            EditorApplication.isPaused = !EditorApplication.isPaused;
        }
        if (Input.GetKeyDown(exit) && Input.GetKey(firstKey))
        {
            EditorApplication.isPlaying = false;
        }
    }
    #endregion

}
