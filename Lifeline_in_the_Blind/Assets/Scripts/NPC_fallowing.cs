using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_fallowing : MonoBehaviour
{
    public GameObject ThePlayer;
    public float TargetDistance;
    public float AllowedDistance = 5;
    public GameObject NPC;
    public float FollowSpeed;
    public RaycastHit Shot;


    // Update is called once per frame 
    //DJW
    void Update()
    {
        transform.LookAt(ThePlayer.transform);
        if(Physics.Raycast(transform.position,transform.TransformDirection(Vector3.forward),out Shot))
        {
            TargetDistance = Shot.distance;
            //NPC is of range of player
            if(TargetDistance >= AllowedDistance)
            {
                FollowSpeed = .1f;
                NPC.GetComponent<Animation>().Play("Walking");
                transform.position = Vector3.MoveTowards(transform.position, ThePlayer.transform.position, FollowSpeed);
            }
            //NPC in next to player
            else 
            {
                FollowSpeed = 0;
                NPC.GetComponent<Animation>().Play("Idle");
            }
        }
    }
}
