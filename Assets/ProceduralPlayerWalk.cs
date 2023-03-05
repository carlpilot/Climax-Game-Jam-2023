using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralPlayerWalk : MonoBehaviour
{
    public Transform[] feet;
    public Transform[] feetTargets;

    public float stepPeriod;

    float t;
    Vector3 lastPos;
    public float stepHeight = 0.5f;

    public float stepHeightOfffset = 0.5f;

    Vector3[] flooredLegPositions;
    Vector3[] lastFlooredLegPositions;

    void Awake()
    {
        flooredLegPositions = new Vector3[feet.Length];
        lastFlooredLegPositions = new Vector3[feet.Length];
    }
    
    void Start()
    {
        
    }
    
    void Update()
    {
        Vector3 velocity = (transform.GetChild(0).position - lastPos) / Time.deltaTime;
        lastPos = transform.GetChild(0).position;
        t += Time.deltaTime*Mathf.PI*2/stepPeriod;
        for (int i = 0; i < feet.Length; i++)
        {
            var currentValue = Mathf.Sin(t);
            var currentValueDiff = Mathf.Cos(t);
            if (i % 2 == 0) {
                currentValue = -currentValue;
                currentValueDiff = -currentValueDiff;
            }
            currentValue += stepHeightOfffset;
            if (currentValue < 0) currentValue = 0;
            Vector3 hip = feet[i].parent.parent.position;
            Vector3 floorPos = hip + Vector3.down * 1f;
            // Find the point on the floor below the hip with a raycast
            RaycastHit hit;
            if (Physics.Raycast(hip, Vector3.down, out hit, 10f))
            {
                floorPos = hit.point;
            }
            feetTargets[i].position = new Vector3(hip.x+velocity.x*stepPeriod*-0.15f*currentValueDiff, floorPos.y + currentValue * stepHeight, hip.z+velocity.z*stepPeriod*-0.15f*currentValueDiff);
            /*if (currentValue == 0){
                // On ground
                feetTargets[i].position = flooredLegPositions[i];
                lastFlooredLegPositions[i] = flooredLegPositions[i];
            } else{
                // In air
                var targetEndpoint = floorPos + Vector3.ProjectOnPlane(velocity, Vector3.up)*stepPeriod*0.5f;
                var lerp = currentValue;
                if (currentValueDiff < 0){
                    print(currentValue);
                    lerp = 2*(1+stepHeightOfffset) - currentValue;
                }
                lerp = lerp / (2*(1+stepHeightOfffset));
                var currentEndpoint = Vector3.Lerp(lastFlooredLegPositions[i], flooredLegPositions[i], lerp);
                feetTargets[i].position = currentEndpoint + Vector3.up * currentValue * stepHeight;
                flooredLegPositions[i] = targetEndpoint;
            }*/
        }
    }
}
