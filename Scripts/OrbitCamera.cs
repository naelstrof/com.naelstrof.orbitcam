using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Transform = UnityEngine.Transform;
#if UNITY_EDITOR
using GraphProcessor;
using UnityEditor;

[CustomEditor(typeof(OrbitCamera))]
public class OrbitCameraEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        var orbitCamera = (OrbitCamera)target;
        var config = orbitCamera.GetConfiguration();
        var configSerialized = new SerializedObject(config);
        if (config == null) return;
        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("Exposed parameters:");
        Undo.RecordObject(config, "Orbit Camera");
        var exposedParameters = configSerialized.FindProperty("exposedParameters");
        var parameterCount = exposedParameters.arraySize;
        for (int i = 0; i < parameterCount; i++) {
            var exposedParameter = exposedParameters.GetArrayElementAtIndex(i);
            var hidden = exposedParameter.FindPropertyRelative("settings").FindPropertyRelative("isHidden").boolValue;
            var parameterName = exposedParameter.FindPropertyRelative("name").stringValue;
            if (!hidden) {
                EditorGUILayout.PropertyField(exposedParameter.FindPropertyRelative("val"), new GUIContent(parameterName));
            }
        }
        configSerialized.ApplyModifiedProperties();
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
