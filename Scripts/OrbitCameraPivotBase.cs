using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OrbitCameraPivotBase : MonoBehaviour {
    public abstract OrbitCameraData GetData(Camera cam);
    private void OnDrawGizmosSelected() {
        OrbitCameraPreview.RenderPreview(this);
    }
}
