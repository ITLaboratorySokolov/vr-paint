using UnityEngine;

/// <summary>
/// Script used to set the size of the room player is allowed to move in
/// </summary>
public class RoomController : MonoBehaviour
{
    [SerializeField]
    WallCollisionProcessor irlRoomWalls;
    [SerializeField]
    GameObject player;
    [SerializeField]
    GameObject irlGround;

    /// <summary>
    /// Set room size
    /// - sets the size of the room player is allowed to move in
    /// </summary>
    /// <param name="wX"> Width in x </param>
    /// <param name="wZ"> Width in z </param>
    public void SetRoomSize(float wX, float wZ)
    {
        // set position of walls
        irlRoomWalls.SetBorderWall(wX, wZ);

        // set size of teleportation area
        float currentSizeX = irlGround.GetComponent<Renderer>().bounds.size.x;
        float currentSizeZ = irlGround.GetComponent<Renderer>().bounds.size.z;

        Vector3 scale = irlGround.transform.localScale;
        scale.x = wX * (scale.x / currentSizeX);
        scale.z = wZ * (scale.z / currentSizeZ);

        float scalingX = scale.x / irlGround.transform.localScale.x;
        float scalingZ = scale.z / irlGround.transform.localScale.z;

        irlGround.transform.localScale = scale;
    }

}
