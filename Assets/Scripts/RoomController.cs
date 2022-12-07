using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    [SerializeField]
    WallCollisionProcessor wallCollisionProcessor;

    [SerializeField]
    Transform roomCenter;

    [SerializeField]
    GameObject walkableGround;

    // Start is called before the first frame update
    void Start()
    {
        CenterRoom();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetRoomSize(float wX, float wZ)
    {
        // set position of walls
        wallCollisionProcessor.SetWallPosition(wX, wZ);

        // set size of teleportation area
        float currentSizeX = walkableGround.GetComponent<Renderer>().bounds.size.x;
        float currentSizeZ = walkableGround.GetComponent<Renderer>().bounds.size.z;

        Vector3 scale = walkableGround.transform.localScale;
        scale.x = wX * scale.x / currentSizeX;
        scale.z = wZ * scale.z / currentSizeZ;

        walkableGround.transform.localScale = scale;
    }

    private void CenterRoom()
    {
        Vector3 center = roomCenter.position;

        walkableGround.transform.position = center;
        wallCollisionProcessor.MoveWallPosition(center);
    }
}
