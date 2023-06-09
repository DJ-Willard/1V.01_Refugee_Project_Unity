using UnityEngine;

public class NPCFollowing : MonoBehaviour
{
    public GameObject thePlayer;
    public float targetDistance;
    public float allowedDistance = 3f;
    public GameObject npc;
    public float followSpeed;
    public RaycastHit hit;
    public bool IsActivated = false; 

    public void Activate()
    {
        IsActivated = true;
    }

    private void Update()
    {
        transform.LookAt(thePlayer.transform);

        if (IsActivated)
        {
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit))
            {
                targetDistance = hit.distance;

                // NPC is out of range of the player
                if (targetDistance <= allowedDistance)
                {
                    followSpeed = 0.1f;
                    npc.GetComponent<Animation>().Play("Walking");
                    transform.position = Vector3.MoveTowards(transform.position, thePlayer.transform.position, followSpeed * Time.deltaTime);
                }
                // NPC is next to the player
                else
                {
                    followSpeed = 0f;
                    npc.GetComponent<Animation>().Play("Idle");
                }

            }
        }
        else
        {
            npc.GetComponent<Animation>().Play("Waving");
        }
            
    }
}