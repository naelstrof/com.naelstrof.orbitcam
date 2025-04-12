using System.Collections;
using System.Collections.Generic;
using GraphProcessor;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

public class OrbitCameraControllerWindow : BaseGraphWindow {
    private ToolbarView toolbarView;
    public static BaseGraphWindow Open(OrbitCameraController graph) {
        // Focus the window if the graph is already opened
        var orbitCameraWindows = Resources.FindObjectsOfTypeAll<OrbitCameraControllerWindow>();
        foreach (var orbitCameraWindow in orbitCameraWindows)
        {
            if (orbitCameraWindow.graph == graph)
            {
                orbitCameraWindow.Show();
                orbitCameraWindow.Focus();
                return orbitCameraWindow;
            }
        }

        var graphWindow = CreateWindow<OrbitCameraControllerWindow>();

        graphWindow.Show();
        graphWindow.Focus();

        graphWindow.InitializeGraph(graph);

        return graphWindow;
    }

    protected override void InitializeWindow(BaseGraph graph) {
        // Set the window title
        titleContent = new GUIContent(graph.name);
        // Here you can use the default BaseGraphView or a custom one (see section below)
        if (graphView == null) {
            graphView = new BaseGraphView(this);
            toolbarView = new OrbitCameraToolbar(graphView);
            graphView.Add(toolbarView);
        }
        rootView.Add(graphView);
    }
    
}

#endif
