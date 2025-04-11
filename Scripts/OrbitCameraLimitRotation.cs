using System.Collections;
using System.Collections.Generic;
using GraphProcessor;
using UnityEngine;

[System.Serializable, NodeMenuItem("OrbitCamera/Limit Rotation")]
public class OrbitCameraLimitRotation : OrbitCameraControllerNode {
    [Input("Input")]
    public OrbitCameraData input;
    [SerializeField, Input("Angle Freedom")]
    public float angleFreedom = 30f;
    [SerializeField, Input("Target Rotation")]
    public Quaternion targetRotation;

    [Output("Output")]
    public OrbitCameraData output;
    private Quaternion GetRotation(Quaternion camRotation) {
        var rotation = targetRotation;
        float angle = Quaternion.Angle(camRotation, rotation);
        if (angle > angleFreedom) {
            return Quaternion.RotateTowards(camRotation, rotation, angle - angleFreedom);
        }
        return camRotation;
    }

    protected override void Process() {
        var orbitCameraData = input;
        orbitCameraData.rotation = GetRotation(orbitCameraData.rotation);
        output = orbitCameraData;
    }
}
