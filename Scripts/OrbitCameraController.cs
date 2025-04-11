using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GraphProcessor;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(OrbitCameraController))]
class OrbitCameraControllerEditor : Editor {
    public override void OnInspectorGUI() {
        if (GUILayout.Button("Open Graph")) {
            OrbitCameraControllerWindow.Open(target as OrbitCameraController);
        }
    }
}
#endif

[CreateAssetMenu(fileName = "New Orbit Camera Controller", menuName = "Data/Orbit Camera/Controller")]
public class OrbitCameraController : BaseGraph {
    private ProcessGraphProcessor _graphProcessor;

    public void Process() {
        if (_graphProcessor == null) {
            _graphProcessor = new ProcessGraphProcessor(this);
            _graphProcessor.UpdateComputeOrder();
        }
        _graphProcessor.Run();
    }
    
    public OrbitCameraData GetData() {
        var node = graphOutputs.FirstOrDefault() as OrbitCameraOutput;
        return node?.output ?? new OrbitCameraData() {
            rotation = Quaternion.identity,
        };
    }
}
