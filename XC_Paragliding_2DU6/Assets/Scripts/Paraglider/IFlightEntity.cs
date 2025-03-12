using UnityEngine;

public interface IFlightEntity
{
    public Rigidbody2D EntityRB2D { get; set; }
    public void SetLiftForce(float newLiftForce);
    public void SetWindForce(float newWindForce);
}
