using System.Collections;
using System.Collections.Generic;
using GraphProcessor;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable, NodeMenuItem("Input/Float")]
public class OrbitCameraFloat : OrbitCameraControllerInput {
    [SerializeField]
    public string parameterName;
    
    [SerializeField, Output("Float")]
    public float value;
}
