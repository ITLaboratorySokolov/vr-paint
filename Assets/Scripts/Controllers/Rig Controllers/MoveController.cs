using UnityEngine;

/// <summary>
/// Script used to move game object with set parent transform
/// </summary>
public class MoveController : MonoBehaviour
{
    public Vector3 modPos;
    public Transform parent;

    // Update is called once per frame
    void Update()
    {
        if (parent != null)
        {
            this.transform.position = parent.position - modPos;
            this.transform.rotation = parent.rotation;
        }
    }
}
