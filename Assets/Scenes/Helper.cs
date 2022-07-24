using UnityEngine;

public class Helper : MonoBehaviour
{
    static public Vector3 getScaleForSize(GameObject objectForMeasure, Vector3 newSize)
    {
        float minSize = 0.0000001f;

        Vector3 size = objectForMeasure.GetComponent<Renderer>().bounds.size;

        Vector3 localScale = objectForMeasure.transform.localScale;

        float realSizeX = size.x / localScale.x;
        float realSizeY = size.y / localScale.y;
        float realSizeZ = size.z / localScale.z;

        return new Vector3(
            Mathf.Abs(realSizeX) < minSize ? 1 : (newSize.x / realSizeX),
            Mathf.Abs(realSizeY) < minSize ? 1 : (newSize.y / realSizeY),
            Mathf.Abs(realSizeZ) < minSize ? 1 : (newSize.z / realSizeZ)
        );
    }

    static public float limitFloat(float min, float value, float max)
    {
        if (value < min)
        {
            return min;
        }

        if (value > max)
        {
            return max;
        }

        return value;
    }
}