using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Billboard3 : MonoBehaviour
{
    public GameObject player;
    void LateUpdate()
    {
        transform.LookAt(player.transform.position);
        // The next three lines make this work only on the horizontal axis
        Vector3 eulerAngles = transform.eulerAngles;
        eulerAngles.x = 0;
        transform.eulerAngles = eulerAngles;
    }
}