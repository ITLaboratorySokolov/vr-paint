using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCollisionProcessor : MonoBehaviour
{
    /// <summary> Display wall object </summary>
    [SerializeField]
    GameObject displaywall;

    /// <summary> Player tag </summary>
    [SerializeField]
    string playerTag;

    [SerializeField]
    GameObject[] borderWalls;

    /// <summary> Padding </summary>
    float mov = 0.5f;

    private void Start()
    {
        for (int i = 0; i < borderWalls.Length; i++)
        {
            // inject self to walls
            WallCollisionDetector wcd = borderWalls[i].GetComponent<WallCollisionDetector>();
            wcd.SetWCP(this);
        }


    }

    public void SetWallPosition(float wX, float wZ)
    {
        // move walls to be wX/2 and wZ/2
        for (int i = 0; i < borderWalls.Length; i++)
        {
            Vector3 f = borderWalls[i].transform.forward;

            if (borderWalls[i].transform.rotation.eulerAngles.y % 180 == 0)
            {
                Vector3 oldP = borderWalls[i].transform.position;
                borderWalls[i].transform.position = new Vector3(oldP.x, oldP.y, (-f.z) * wZ/2.0f);
            }
            else
            {
                Vector3 oldP = borderWalls[i].transform.position;
                borderWalls[i].transform.position = new Vector3((-f.x) * wX/2.0f, oldP.y, oldP.z);
            }
        }
    }

    internal void MoveWallPosition(Vector3 center)
    {
        for (int i = 0; i < borderWalls.Length; i++)
        {
            Vector3 oldP = borderWalls[i].transform.position;
            Vector3 newP = oldP + center;

            borderWalls[i].transform.position = newP;
        }
    }

    public void CenterRoom(Vector3 center)
    {
        // move the whole room so it is centered to current camera pos

        // move the collision walls
        // move the teleportation area

    }

    /// <summary>
    /// Display a warning wall upon collision
    /// </summary>
    /// <param name="colider"> Wall that was collided with </param>
    /// <param name="colidee"> Player </param>
    /// <param name="keepCoords"> Which coordinates should be kept </param>
    /// <returns></returns>
    internal GameObject HandleCollision(Transform colider, Transform colidee, Vector3 keepCoords)
    {
        Debug.Log(colidee.tag);

        if (colidee.tag.Equals(playerTag))
        {
            keepCoords = new Vector3(Math.Abs(keepCoords.x), Math.Abs(keepCoords.y), Math.Abs(keepCoords.z));

            Debug.Log(colider.transform.rotation.eulerAngles);
            Debug.Log(displaywall.transform.rotation.eulerAngles);

            Vector3 wallpos = new Vector3(keepCoords.x * colider.position.x, keepCoords.y * colider.position.y, keepCoords.z * colider.position.z)
                           + (new Vector3((1 - keepCoords.x) * colidee.position.x, (1 - keepCoords.y) * colidee.position.y, (1 - keepCoords.z) * colidee.position.z))
                           - mov * new Vector3(keepCoords.x * colider.forward.x, keepCoords.y * colider.forward.y, keepCoords.z * colider.forward.z);
            GameObject c = Instantiate(displaywall, wallpos, Quaternion.Euler(colider.transform.rotation.eulerAngles + displaywall.transform.rotation.eulerAngles));
            return c;
        }

        return null;
    }

    /// <summary>
    /// Delete warning wall upon player exit
    /// </summary>
    /// <param name="reaction"> Warning wall </param>
    /// <param name="colidee"> Player </param>
    internal void HandleDelete(GameObject reaction, Transform colidee)
    {
        if (colidee.tag.Equals(playerTag))
            Destroy(reaction);
    }

    /// <summary>
    /// Move warning wall with player
    /// </summary>
    /// <param name="reaction"> Warning wall </param>
    /// <param name="colider"> Wall that was collided with </param>
    /// <param name="colidee"> Player </param>
    /// <param name="keepCoords"> Which coordinates should be kept </param>
    internal void HandleStay(GameObject reaction, Transform colider, Transform colidee, Vector3 keepCoords)
    {
        if (colidee.tag.Equals(playerTag))
        {
            keepCoords = new Vector3(Math.Abs(keepCoords.x), Math.Abs(keepCoords.y), Math.Abs(keepCoords.z));

            Vector3 wallpos = new Vector3(keepCoords.x * colider.position.x, keepCoords.y * colider.position.y, keepCoords.z * colider.position.z)
               + (new Vector3((1 - keepCoords.x) * colidee.position.x, (1 - keepCoords.y) * colidee.position.y, (1 - keepCoords.z) * colidee.position.z))
               - mov * new Vector3(keepCoords.x * colider.forward.x, keepCoords.y * colider.forward.y, keepCoords.z * colider.forward.z); ;
            reaction.transform.position = wallpos;
        }
    }
}
