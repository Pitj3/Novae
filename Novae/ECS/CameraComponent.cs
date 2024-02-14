namespace Novae.ECS
{
    public enum CameraType
    {
        Freelook,
        Orbit
    }
    public class CameraComponent
    {
        public CameraType CameraType { get; set; }
        
        public CameraComponent(CameraType cameraType)
        {
            CameraType = cameraType;
        }
    }
}
