using System;

internal class RangeExAttribute : Attribute
{
    private float v1;
    private float v2;
    private float v3;
    private string v4;

    public RangeExAttribute(float v1, float v2, float v3, string v4)
    {
        this.v1 = v1;
        this.v2 = v2;
        this.v3 = v3;
        this.v4 = v4;
    }
}