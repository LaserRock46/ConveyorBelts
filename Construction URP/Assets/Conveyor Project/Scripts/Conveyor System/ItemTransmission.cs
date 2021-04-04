using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemTransmission
{   
    public Transform self;
    public int pathIndex;
    public float progress;
    public bool reversedTransmission;
    public float speed;
    public Vector3[] positions;

    public ItemTransmission(Transform self, float progress, bool reversedTransmission, float speed, Vector3[] positions)
    {
        this.self = self;
        this.pathIndex = reversedTransmission ? positions.Length - 1 : 0;
        this.progress = progress;
        this.reversedTransmission = reversedTransmission;
        this.speed = speed;
        this.positions = positions;
    }

    public void Update()
    {      
        float singleStep = speed * Time.deltaTime;
        int nextPathIndex = 0;

        if (self.position != positions[pathIndex])
        {
            self.position = Vector3.MoveTowards(self.position, positions[pathIndex], singleStep);
           
            progress += singleStep;
        }
        else
        {
            nextPathIndex = reversedTransmission ? -1 : +1;
            pathIndex = (pathIndex + nextPathIndex) % positions.Length;
        }

        Vector3 targetDirection = positions[pathIndex] - self.position;    
        Vector3 newDirection = Vector3.RotateTowards(self.forward, targetDirection, singleStep, 0.0f);
        self.rotation = Quaternion.LookRotation(newDirection);

        for (int i = 1; i < positions.Length; i++)
        {
            Debug.DrawLine(positions[i - 1], positions[i], Color.red);
        }
    }

}
