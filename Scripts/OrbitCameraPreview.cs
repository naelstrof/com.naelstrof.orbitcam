using UnityEngine;
using UnityEngine.Rendering;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.EditorTools;

[EditorTool("Orbit Camera Preview Tool", typeof(OrbitCamera))]
public class OrbitCameraPreview : EditorTool {
    private static RenderTexture renderTexture;
    private static Camera tempCamera;
    private static OrbitCameraData lastData;

    private static OrbitCameraPreviewStatus status;

    enum OrbitCameraPreviewStatus {
        Okay,
        NotSupportedSRP,
    }
    
    public static Camera GetPreviewCamera() {
        if (tempCamera == null) {
            var cameraObj = new GameObject("OrbitCameraPreview", typeof(Camera)) {
                hideFlags = HideFlags.HideAndDontSave
            };
            tempCamera = cameraObj.GetComponent<Camera>();
            tempCamera.nearClipPlane = 0.03f;
        }

        return tempCamera;
    }
    
    private static RenderTexture GetRenderTexture() {
        if (renderTexture == null) {
            renderTexture = new RenderTexture(256, 256, 32, RenderTextureFormat.ARGB32);
        }
        return renderTexture;
    }
    
    private static RenderTexture RenderPreview(OrbitCameraData data) {
        lastData = data;
        var camera = GetPreviewCamera();
        lastData.ApplyTo(camera);
        RenderTexture temp = GetRenderTexture();
        
        RenderPipeline.StandardRequest request = new RenderPipeline.StandardRequest();
        if (RenderPipeline.SupportsRenderRequest(camera, request)) {
            request.destination = temp;
            RenderPipeline.SubmitRenderRequest(camera, request);
            status = OrbitCameraPreviewStatus.Okay;
        } else {
            status = OrbitCameraPreviewStatus.NotSupportedSRP;
        }
        return temp;
    }
    
    private void OnEnable() {
    }
    private void OnDisable() {
        if (tempCamera != null) {
            DestroyImmediate(tempCamera.gameObject, true);
        }
        if (renderTexture != null) {
            DestroyImmediate(renderTexture);
        }
        renderTexture = null;
        tempCamera = null;
    }

    public override void OnToolGUI(EditorWindow window) {
        if (!(window is SceneView sceneView)) {
            return;
        }
        if (target is not OrbitCamera camera) {
            return;
        }
        
        camera.Process();
        lastData = camera.GetData();

        if (Event.current.type is not EventType.Repaint) {
            return;
        }
        
        var rt = RenderPreview(lastData);
        Handles.BeginGUI();
        EditorGUI.DrawPreviewTexture(new Rect(0, 0, 256, 256), rt);
        Handles.EndGUI();
        sceneView.Repaint();
    }
    
    public override void OnActivated() {
        SceneView.lastActiveSceneView.ShowNotification(new GUIContent("Entering Orbit Camera Tool"), .1f);
    }
    
    public override void OnWillBeDeactivated() {
        SceneView.lastActiveSceneView.ShowNotification(new GUIContent("Exiting Orbit Camera Tool"), .1f);
    }
}

#endif