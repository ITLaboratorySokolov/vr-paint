using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    [SerializeField]
    GameObject onlineHand;
    [SerializeField]
    GameObject offlineHand;

    public void OnlineHandDisplay()
    {
        offlineHand.SetActive(false);
        onlineHand.SetActive(true);
    }


    public void OfflineHandDisplay()
    {
        onlineHand.SetActive(false);
        offlineHand.SetActive(true);
    }
}
