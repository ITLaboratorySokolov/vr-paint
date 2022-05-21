using UnityEngine;
using UnityEngine.InputSystem;

/// <summary> Class controlling the movement of the rig </summary>
public class MovementController : MonoBehaviour
{
    [Header("Action input references")]
    /// <summary> Move action </summary>
    public InputActionReference moveRef = null;
    /// <summary> Rotate action </summary>
    public InputActionReference rotRef = null;

    [Header("Game objects")]
    /// <summary> Rig </summary>
    public Transform rig;

    [Header("Controlls")]
    /// <summary> Speed of movement </summary>
    public int speed;

    /// <summary> Updates once per frame </summary>
    private void Update()
    {
        MoveAndRotateRig();
    }

    /// <summary>
    /// Move rig based on input
    /// </summary>
    private void MoveAndRotateRig()
    {
        // Compute translation
        Vector2 v = moveRef.action.ReadValue<Vector2>();
        rig.position += rig.transform.TransformVector(new Vector3(v.x, 0, v.y)) * speed * Time.deltaTime;

        // Compute rotation angle
        Vector2 w = rotRef.action.ReadValue<Vector2>();
        float angle = (Mathf.Atan2(w.y, w.x));
        if (angle != 0)
            angle = Mathf.Deg2Rad * 90 - angle;

        // Cap rotation at max +-90°
        if (System.Math.Abs(angle) > Mathf.Deg2Rad * 180)
            angle = -Mathf.Deg2Rad * 90;
        else if (System.Math.Abs(angle) > Mathf.Deg2Rad * 90)
            angle = Mathf.Deg2Rad * 90;

        // Rotate
        rig.rotation *= Quaternion.AngleAxis(angle * speed, Vector3.up);
    }

}
