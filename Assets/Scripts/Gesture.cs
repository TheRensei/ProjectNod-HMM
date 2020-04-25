using UnityEngine;

public enum GestureType
{
    BeingIdle,
    RotatingLeft,
    RotatingRight,
    TiltingUpward,
    TiltingDownward,
    LeaningLeft,
    LeaningRight,

    Shaking,
    Nodding
}

[System.Serializable]
public class GestureDataContainer
{
    public string name;
    public Vector3[] angularVelocity;
}

[System.Serializable]
public class GestureQuantizedDataContainer
{
    public int[] data;
}
