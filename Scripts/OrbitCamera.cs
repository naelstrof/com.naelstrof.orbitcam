using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Transform = UnityEngine.Transform;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(OrbitCamera))]
public class OrbitCameraEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        var orbitCamera = (OrbitCamera)target;
        var config = orbitCamera.GetConfiguration();
        if (config == null) return;
        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("Auto-detected parameters:");
        Undo.RecordObject(this, "Orbit Camera");
        foreach (var node in config.nodes.Where((a) => a is OrbitCameraControllerInput)) {
            if (node is OrbitCameraFloat f) {
                f.value = EditorGUILayout.FloatField(f.parameterName, f.value);
            }
            if (node is OrbitCameraVector3 p) {
                p.value = EditorGUILayout.Vector3Field(p.parameterName, p.value);
            }
            if (node is OrbitCameraRotation r) {
                EditorGUI.BeginChangeCheck();
                var value = EditorGUILayout.Vector3Field(r.parameterName, r.value.eulerAngles);
                if (EditorGUI.EndChangeCheck()) {
                    r.value = Quaternion.Euler(value);
                }
            }
        }
        serializedObject.ApplyModifiedProperties();
    }
}
#endif



[RequireComponent(typeof(Camera))]
public class OrbitCamera : MonoBehaviour {
    [SerializeField]
    protected OrbitCameraController configuration;

    private Camera cam;
    private bool tracking = true;
    private OrbitCameraData currentCameraData = new() {
        rotation = Quaternion.identity,
        position = Vector3.zero,
        fov = 65f,
        screenPoint = Vector2.one*0.5f,
        distance = 1f,
        cullingMask = ~0
    };
    public virtual OrbitCameraController GetConfiguration() => configuration;
    
    public void SetConfiguration(OrbitCameraController newConfig, float tweenDuration = 0.4f) {
        if (GetConfiguration() == newConfig) {
            return;
        }
        configuration = newConfig;
    }

    protected virtual void Awake() {
        cam = GetComponent<Camera>();
        var config = GetConfiguration();
        if (config != null) {
            SetOrbit(config.GetData());
        }
    }

    public void SetFloat(string parameterName, float value) {
        foreach (var node in GetConfiguration().nodes) {
            if (node is OrbitCameraFloat f && f.parameterName == parameterName) {
                f.value = value;
            }
        }
    }
    
    public void SetVector3(string parameterName, Vector3 value) {
        foreach (var node in GetConfiguration().nodes) {
            if (node is OrbitCameraVector3 f && f.parameterName == parameterName) {
                f.value = value;
            }
        }
    }
    
    public void SetRotation(string parameterName, Quaternion value) {
        foreach (var node in GetConfiguration().nodes) {
            if (node is OrbitCameraRotation f && f.parameterName == parameterName) {
                f.value = value;
            }
        }
    }

    public static Ray GetScreenRay(Camera cam, Vector2 screenPoint) {
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        Vector2 desiredScreenPosition = screenPoint*screenSize;
        return cam.ScreenPointToRay(desiredScreenPosition);
    }

    protected void SetOrbit(OrbitCameraData data) {
        data.ApplyTo(cam);
        currentCameraData = data;
    }
    protected virtual void LateUpdate() {
        var config = GetConfiguration();
        config.Process();
        SetOrbit(config.GetData());
    }
    
    public OrbitCameraData GetCurrentCameraData() => currentCameraData;

    public void Process() {
        configuration.Process();
    }
    public OrbitCameraData GetData() {
        return GetConfiguration().GetData();
    }
}
