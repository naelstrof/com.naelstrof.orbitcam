using GraphProcessor;

[System.Serializable, NodeMenuItem("OrbitCamera/Lerp")]
public class OrbitCameraLerp : OrbitCameraControllerNode {
    [Input("A")]
    public OrbitCameraData inputA;
    [Input("B")]
    public OrbitCameraData inputB;
    [Input("Blend")]
    public float blend = 0.5f;

    [Output("Output")]
    public OrbitCameraData output;

    protected override void Process() {
        output = OrbitCameraData.Lerp(inputA, inputB, blend);
    }
}
