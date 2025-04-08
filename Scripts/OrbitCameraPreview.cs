using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using System.Net;
using UnityEditor.Overlays;

[Overlay(typeof(SceneView), "Orbit Camera Preview", false)]
public class OrbitCameraPreview : Overlay {
    private static RenderTexture renderTexture;
    private static Camera tempCamera;
    private static VisualTreeAsset visualTree;
    private static VisualElement root;
    private static OrbitCameraPivotBase lastPivot;

    public override void OnWillBeDestroyed() {
        base.OnWillBeDestroyed();
        if (tempCamera != null) {
            Object.DestroyImmediate(tempCamera.gameObject, true);
        }
        if (renderTexture != null) {
            Object.DestroyImmediate(renderTexture);
        }
        if (visualTree != null) {
            Object.DestroyImmediate(visualTree);
        }
        renderTexture = null;
        tempCamera = null;
        visualTree = null;
        root = null;
    }

    private static RenderTexture GetRenderTexture() {
        if (renderTexture == null) {
            renderTexture = new RenderTexture(256, 256, 32, RenderTextureFormat.ARGB32);
        }
        return renderTexture;
    }

    private VisualTreeAsset GetVisualTreeAsset() {
        if (visualTree == null) {
            var visualTreePath = AssetDatabase.GUIDToAssetPath("2a11c5c36a921194599e1cae66f5f2c6");
            visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(visualTreePath);
        }
        return visualTree;
    }
    public static void RenderPreview(OrbitCameraPivotBase pivot) {
        lastPivot = pivot;
        if (root == null) {
            return;
        }
        if (tempCamera == null) {
            var cameraObj = new GameObject("OrbitCameraPreview", typeof(Camera)) {
                hideFlags = HideFlags.HideAndDontSave
            };
            tempCamera = cameraObj.GetComponent<Camera>();
        }
        var data = pivot.GetData(tempCamera);
        data.ApplyTo(tempCamera);
        
        RenderPipeline.StandardRequest request = new RenderPipeline.StandardRequest();
        if (RenderPipeline.SupportsRenderRequest(tempCamera, request)) {
            request.destination = GetRenderTexture();
            RenderPipeline.SubmitRenderRequest(tempCamera, request);
            root.Q<Label>("info").text = "Adjust sliders to adjust which direction the player is looking.";
            root.Q<VisualElement>("image").style.backgroundImage = Background.FromRenderTexture(GetRenderTexture());
        } else {
            root.Q<Label>("info").text = "This render pipeline doesn't support render requests... Sorry.";
        }
    }

    public override VisualElement CreatePanelContent() {
        root = new VisualElement();
        GetVisualTreeAsset().CloneTree(root);
        root.Q<Label>("info").text = "Select a pivot to preview.";
        root.Q<VisualElement>("image").style.backgroundImage = Background.FromRenderTexture(GetRenderTexture());
        root.Q<Slider>("pitch").RegisterValueChangedCallback((v) => {
            var aim = OrbitCamera.GetPlayerIntendedScreenAim();
            aim.y = -v.newValue;
            OrbitCamera.SetPlayerIntendedScreenAim(aim);
            if (lastPivot != null) {
                RenderPreview(lastPivot);
            }
        });
        root.Q<Slider>("yaw").RegisterValueChangedCallback((v) => {
            var aim = OrbitCamera.GetPlayerIntendedScreenAim();
            aim.x = v.newValue;
            OrbitCamera.SetPlayerIntendedScreenAim(aim);
            if (lastPivot != null) {
                RenderPreview(lastPivot);
            }
        });
        return root;
    }
}

#endif