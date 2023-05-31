using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_fallowing : MonoBehaviour
{
    public GameObject Player;
    public float TargetDistance;
    public float AllowedDistance = 5;
    public GameObject NPC;
    public float FollowSpeed;
    public RaycastHit Shot;


    // Update is called once per frame
    void Update()
    {
        transform.Lookat(Player.transform);
        if(Physics.Raycast(transform.position.TransformDirection(Vector3.forward),out Shot))
        {
            TargetDistance = Shot.Distance;
            //NPC is of range of player
            if(TargetDistance >= AllowedDistance)
            {
                FollowSpeed = .1f;
                NPC.GetComponent<Animation>().play("Walking");
                transform.position = Vector3.MoveTowards(transform.postion, Player.transform.position, FollowSpeed);
            }
            //NPC in next to player
            else 
            {
                FollowingSpeed = 0;
                NPC.GetComponent<Animation>().Play("Idle")
            }
        }
    }
}
