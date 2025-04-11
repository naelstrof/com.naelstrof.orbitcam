using System.Collections;
using System.Collections.Generic;
using GraphProcessor;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

public class OrbitCameraControllerWindow : BaseGraphWindow {
    public static BaseGraphWindow Open(OrbitCameraController graph) {
        // Focus the window if the graph is already opened
        var mixtureWindows = Resources.FindObjectsOfTypeAll<OrbitCameraControllerWindow>();
        foreach (var mixtureWindow in mixtureWindows)
        {
            if (mixtureWindow.graph == graph)
            {
                mixtureWindow.Show();
                mixtureWindow.Focus();
                return mixtureWindow;
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
        var graphView = new BaseGraphView(this);
        rootView.Add(graphView);
    }
    
}

#endif
