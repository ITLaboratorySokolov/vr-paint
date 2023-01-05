using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCollisionDetector : MonoBehaviour
{
    WallCollisionProcessor wcp;

    [SerializeField]
    public Vector3 keepCoords;

    GameObject reaction;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetWCP(WallCollisionProcessor wcp)
    {
        this.wcp = wcp;
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Collision enter");
        GameObject r = wcp.HandleCollision(this.transform, other.transform, keepCoords);
        if (r != null)
            reaction = r;
    }

    private void OnTriggerExit(Collider other)
    {
        wcp.HandleDelete(reaction, other.transform);

    }

    private void OnTriggerStay(Collider other)
    {
        wcp.HandleStay(reaction, this.transform, other.transform, keepCoords);
    }
}
