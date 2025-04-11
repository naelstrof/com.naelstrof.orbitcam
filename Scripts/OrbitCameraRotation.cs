using System.Collections;
using System.Collections.Generic;
using GraphProcessor;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable, NodeMenuItem("Input/Rotation")]
public class OrbitCameraRotation : OrbitCameraControllerInput {
    [SerializeField]
    public string parameterName;
    
    [SerializeField, Output("Rotation")]
    public Quaternion value = Quaternion.identity;
}
