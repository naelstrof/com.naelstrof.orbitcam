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
            OnGraphChanged(null);
            onGraphChanges += OnGraphChanged;
        }
        _graphProcessor.Run();
    }

    private void OnGraphChanged(GraphChanges obj) {
        _graphProcessor.UpdateComputeOrder();
    }

    public OrbitCameraData GetData() {
        foreach (var node in graphOutputs) {
            if (node is not OrbitCameraOutput output) {
                continue;
            }
            return output.output;
        }

        return new OrbitCameraData() {
            rotation = Quaternion.identity,
        };
    }
}
