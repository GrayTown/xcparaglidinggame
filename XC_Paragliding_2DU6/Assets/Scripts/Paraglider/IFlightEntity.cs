using UnityEngine;

public interface IFlightEntity
{
    public void SetLiftForce(float newLiftForce);
    public void SetWindForce(float newWindForce);
    public Rigidbody2D EntityRB2D { get; set; }
}
