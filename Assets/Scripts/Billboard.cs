using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Billboard : MonoBehaviour
{
    private GameObject player;

    void LateUpdate()
    {
        if(GameObject.Find("Player") != null)
        {
            player = GameObject.Find("Player");

            transform.LookAt(player.transform.position);
            // The next three lines make this work only on the horizontal axis
            Vector3 eulerAngles = transform.eulerAngles;
            eulerAngles.x = 90;
            transform.eulerAngles = eulerAngles;
        }
        else if(GameObject.Find("Shield(Clone)") != null)//temp, remove later once character rotation is figured out
        {
            player = GameObject.Find("Shield(Clone)");

            transform.LookAt(player.transform.position);
            // The next three lines make this work only on the horizontal axis
            Vector3 eulerAngles = transform.eulerAngles;
            eulerAngles.x = 90;
            transform.eulerAngles = eulerAngles;
        }
    }
}