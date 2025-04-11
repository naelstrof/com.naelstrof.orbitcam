using System;
using System.Collections;
using System.Collections.Generic;
using GraphProcessor;
using UnityEngine;

[Serializable, NodeMenuItem("OrbitCamera/Output")]
public class OrbitCameraOutput : OrbitCameraControllerNode {
    [Input(name = "Input")]
    public OrbitCameraData input;

    [NonSerialized]
    public OrbitCameraData output;

    protected override void Process() {
        output = input;
    }
}
