
using Unity.VisualScripting;

public interface IFlying
{
    public float VerticalSpeed { get; set; }
    public float CurrentVerticalSpeed { get; set; }
    public float HorizontalSpeed { get; set; }
    public float SmoothDeltaSpeed { get; set; }

    public void FlyForward();
}
