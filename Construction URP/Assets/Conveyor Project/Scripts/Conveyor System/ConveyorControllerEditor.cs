using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ConveyorSystem
{
    [CustomEditor(typeof(ConveyorController))]
    public class ConveyorControllerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            ConveyorController conveyorController = (ConveyorController)target;
            if (GUILayout.Button("Test Spawn Item"))
            {
                conveyorController.TestSpawnItem();
            }
        }
    }
}
