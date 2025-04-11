using System.Collections;
using System.Collections.Generic;
using GraphProcessor;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;

public static class OrbitCameraOpenGraph {
    [OnOpenAsset(0)]
    public static bool OnBaseGraphOpened(int instanceID, int line) {
        var asset = EditorUtility.InstanceIDToObject(instanceID) as OrbitCameraController;
        if (asset != null) {
            OrbitCameraControllerWindow.Open(asset);
            return true;
        }
        return false;
    }
}

#endif
