using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveController : MonoBehaviour
{
    public Vector3 modPos;
    public Transform parent;

    // Start is called before the first frame update
    void Start()
    {
        
    }

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
