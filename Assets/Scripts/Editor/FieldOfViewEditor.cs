using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(FieldOfView))]
public class FieldOfViewEditor : Editor
{
    private void OnSceneGUI()
    {
        var fov = (FieldOfView)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360f, fov.Radius);

        var viewAngle01 = DirectionFromAngle(fov.transform.eulerAngles.y, -fov.Angle * 0.5f);
        var viewAngle02 = DirectionFromAngle(fov.transform.eulerAngles.y, fov.Angle * 0.5f);

        Handles.color = Color.yellow;
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngle01 * fov.Radius);
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngle02 * fov.Radius);

        if (fov.Detected)
        {
            Handles.color = Color.red;
            Handles.DrawLine(fov.transform.position, fov.Target.position);
        }
    }

    private Vector3 DirectionFromAngle(float eulerY, float angleInDegree)
    {
        angleInDegree += eulerY;
        return new Vector3(Mathf.Sin(angleInDegree * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegree * Mathf.Deg2Rad));
    }
}
#endif
