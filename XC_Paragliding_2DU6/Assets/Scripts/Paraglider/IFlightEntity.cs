using UnityEngine;

public interface IFlightEntity
{
    public float CurrentHorizontalSpeed { get; set; }
    public float CurrentVerticalSpeed { get; set; }
    public Rigidbody2D EntityRB2D { get; set; }
}
