using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralWalk : MonoBehaviour
{
    public Transform[] feet;
    public Transform[] feetTargets;

    Vector3[] legPositions;
    Vector3[] lastLegPositions;
    float[] lerps;

    public float maxLegDistance = 0.5f;

    public float lerpTime = 0.25f;
    public float stepHeight = 0.15f;

    float sineTimer;

    void Awake()
    {
        legPositions = new Vector3[feet.Length];
        lastLegPositions = new Vector3[feet.Length];
        lerps = new float[feet.Length];
        for (int i = 0; i < legPositions.Length; i++) {
            legPositions[i] = DownwardsProjectedPosition(i);
            lastLegPositions[i] = legPositions[i];
            lerps[i] = 1f;
        }
    }
    
    void Start()
    {
        
    }
    
    void Update()
    {
        for (int i = 0; i < legPositions.Length; i++)
        {
            var currentValue = Mathf.Sin(sineTimer);
            var currentPeriod = currentValue > 0 ? 1 : -1;
            var iPeriod = i % 2 == 0 ? 1 : -1;
            var myTurn = currentPeriod == iPeriod && Mathf.Abs(currentValue) > 0.85f;
            if (lerps[i] < 1){
                lerps[i] += Time.deltaTime / lerpTime;
                var h = Mathf.Sin(lerps[i] * Mathf.PI) * stepHeight;
                feetTargets[i].position = Vector3.Lerp(lastLegPositions[i], legPositions[i], lerps[i])+Vector3.up*h;
            } else {
                lastLegPositions[i] = legPositions[i];
                var newPosition = DownwardsProjectedPosition(i);
                if (myTurn){// && Vector3.Distance(newPosition, legPositions[i]) > maxLegDistance){
                    legPositions[i] = (newPosition-legPositions[i]) * 1.5f + legPositions[i];
                    lerps[i] = 0;
                } else{
                    feetTargets[i].position = legPositions[i];
                }
            }
        }
        sineTimer += Time.deltaTime*Mathf.PI*2/lerpTime/2;
        //print(Mathf.Sin(sineTimer));
    }

    Vector3 DownwardsProjectedPosition(int leg){
        var hip = feet[leg].parent.parent;
        // Raycast downwards from the hip and return the hit point
        if (Physics.Raycast(hip.position, Vector3.down, out var hit, 100.0f)){
            return hit.point;
        }
        return legPositions[leg];
    }
}
