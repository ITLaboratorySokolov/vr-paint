using UnityEngine;

/// <summary>
/// Class used to detect collision with player
/// </summary>
public class WallCollisionDetector : MonoBehaviour
{
    /// <summary> Which coordinates of displayed wall should be kept when the wall moves </summary>
    [SerializeField]
    Vector3 keepCoords;

    /// <summary> Reaction to collision - a displayed wall </summary>
    GameObject reaction;
    /// <summary> Player tag </summary>
    public string playerTag;
    /// <summary> Wall collision processor </summary>
    WallCollisionProcessor wcp;

    /// <summary>
    /// Set wall collision processor
    /// </summary>
    /// <param name="wcp"></param>
    public void SetWCP(WallCollisionProcessor wcp)
    {
        this.wcp = wcp;
    }

    /// <summary>
    /// Called when another game object enters trigger collider
    /// </summary>
    /// <param name="other"> Colliding game object </param>
    private void OnTriggerEnter(Collider other)
    {
        GameObject r = null;
        if (other.transform.tag.Equals(playerTag))
            r = wcp.HandleCollision(this.transform, other.transform, keepCoords);
        if (r != null)
            reaction = r;
    }

    /// <summary>
    /// Called when another game object exits trigger collider
    /// </summary>
    /// <param name="other"> Exiting game object </param>
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.tag.Equals(playerTag))
            wcp.HandleDelete(reaction, other.transform);
    }

    /// <summary>
    /// Called when another game object stays in the trigger collider
    /// </summary>
    /// <param name="other"> Staying game object </param>
    private void OnTriggerStay(Collider other)
    {
        if (other.transform.tag.Equals(playerTag))
            wcp.HandleStay(reaction, this.transform, other.transform, keepCoords);
    }
}
