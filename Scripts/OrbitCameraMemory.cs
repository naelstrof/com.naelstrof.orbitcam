public class OrbitCameraMemory : IOrbitCameraDataGenerator {
    protected OrbitCameraData data;

    public OrbitCameraData Data {
        get => data;
        set => data = value;
    }
    public OrbitCameraData GetData() {
        return data;
    }
}