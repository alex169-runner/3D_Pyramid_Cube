using UnityEngine;

public class Rotation
{
    public Rotation(Vector3 axis, float duration, Centre.FACE face, bool reversed)
    {
        this.axis = axis;
        this.duration = duration;
        this.face = face;
        this.reversed = reversed;
    }
    public Centre.FACE face;
    public Vector3 axis;
    public float duration;
    public bool reversed;
}
