using System.Collections;
using System.Collections.Generic;
using GraphProcessor;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable, NodeMenuItem("Input/Vector3")]
public class OrbitCameraVector3 : OrbitCameraControllerInput {
    [SerializeField]
    public string parameterName;
    
    [SerializeField, Output("Vector3")]
    public Vector3 value;
}
