using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCFollowing : MonoBehaviour
{
    public GameObject playerArmature;
    public float followDistance = 5f;
    public float moveSpeed = 3f;
    public Animator animator;
    public Animation prayingAnimation;
    public bool isFollowing = false;

    private bool isWaving = true;
    private bool hasStartedPraying = false;

    private void Update()
    {
        float distance = Vector3.Distance(transform.position, playerArmature.transform.position);

        if (isWaving)
        {
            // NPC is near the player, stop waving and start following
            if (distance <= followDistance)
            {
                isWaving = false;
                isFollowing = true;
                animator.SetBool("IsWaving", false);
                animator.SetBool("IsFollowing", true);
            }
        }

        if (isFollowing)
        {
            if (distance > followDistance)
            {
                // Player is out of follow range, stop following and start praying
                isFollowing = false;
                animator.SetBool("IsFollowing", false);
                animator.SetBool("IsPraying", true);
                prayingAnimation.Play();
                hasStartedPraying = true;
            }
            else
            {
                Vector3 direction = (playerArmature.transform.position - transform.position).normalized;
                transform.position += direction * moveSpeed * Time.deltaTime;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // NPC has hit a collider, start praying animation
        if (isFollowing && !hasStartedPraying)
        {
            isFollowing = false;
            animator.SetBool("IsFollowing", false);
            animator.SetBool("IsPraying", true);
            prayingAnimation.Play();
            hasStartedPraying = true;
        }
    }
}