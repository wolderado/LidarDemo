using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;

public class GunSway : MonoBehaviour
{
    public float Smoothness = 1;
    public float RotationSway = 1;
    public float PositionSway = 1;

    void Start()
    {
        
    }
    
    void Update()
    {
        //Normally we should decouple input from this script
        float xInput = Mathf.Clamp(Input.GetAxisRaw( "Mouse X" ),-1,1);
        float yInput = Mathf.Clamp(Input.GetAxisRaw( "Mouse Y" ),-1,1);


        Quaternion xRot = Quaternion.AngleAxis( -yInput * RotationSway , Vector3.right );
        Quaternion yRot = Quaternion.AngleAxis( xInput * RotationSway, Vector3.up );

        Quaternion finalRot = xRot * yRot;

        transform.localRotation = Quaternion.Slerp( transform.localRotation, finalRot, Time.deltaTime * Smoothness );


        Vector3 finalPos = new Vector3( xInput * PositionSway, yInput * PositionSway );
        transform.localPosition = Vector3.Lerp( transform.localPosition, finalPos, Time.deltaTime * Smoothness );
    }
    
}
