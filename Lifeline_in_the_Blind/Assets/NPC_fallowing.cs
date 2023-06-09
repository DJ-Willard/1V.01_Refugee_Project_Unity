using UnityEngine;

public class NPCController : MonoBehaviour
{
    public GameObject playerArmature;
    public float spottingDistance = 35f;
    public float recognitionDistance = 25f;
    public float followDistance = 5f;
    public float moveSpeed = 3f;
    public Animator animator;
    public Animation crouchingIdleAnimation;
    public Animation crouchToStandAnimation;
    public Animation wavingAnimation;
    public Animation walkingAnimation;
    public Animation idleAnimation;
    public Animation prayingAnimation;
    public Collider missionEndingCollider;

    private bool isFollowing = false;
    private bool hasReachedPlayer = false;
    private bool hasStartedPraying = false;


    private void Update()
    {
        Vector3 directionToPlayer = playerArmature.transform.position - transform.position;
        float distance = directionToPlayer.magnitude;

        // Rotate NPC to face the player at all times
        if (distance > 0f)
        {
            transform.rotation = Quaternion.LookRotation(directionToPlayer);
        }

        if (distance > spottingDistance)
        {
            // Player is greater than spottingDistance away, play Crouching Idle animation
            SetAllAnimationStates(false);
            animator.SetBool("IsCrouchingIdle", true);
            crouchingIdleAnimation.Play();
        }
        else if (distance > recognitionDistance)
        {
            // Player is within spottingDistance but greater than recognitionDistance, transition to Crouch To Stand -> Waving repeating animations
            SetAllAnimationStates(false);
            animator.SetBool("IsCrouchToStand", true);
            animator.SetBool("IsWaving", true);
            crouchToStandAnimation.Play();
            wavingAnimation.Play();
        }
        else if (distance > followDistance && !isFollowing)
        {
            // Player is within recognitionDistance, transition to Idle animation and set NPC to follow the player
            SetAllAnimationStates(false);
            animator.SetBool("IsIdle", true);
            idleAnimation.Play();

            isFollowing = true;
        }

        if (isFollowing)
        {
            if (distance > followDistance)
            {
                // Player is out of follow range, start walking animation to catch up to the player
                SetAllAnimationStates(false);
                animator.SetBool("IsWalking", true);
                walkingAnimation.Play();

                Vector3 direction = directionToPlayer.normalized;
                transform.position += direction * moveSpeed * Time.deltaTime;
            }
            else if (distance <= followDistance && !hasReachedPlayer)
            {
                // Player is within followDistance, play Idle animation while standing by the player
                SetAllAnimationStates(false);
                animator.SetBool("IsIdle", true);
                idleAnimation.Play();
            }
        }
    }

    private void SetAllAnimationStates(bool state)
    {
        animator.SetBool("IsCrouchingIdle", state);
        animator.SetBool("IsCrouchToStand", state);
        animator.SetBool("IsWaving", state);
        animator.SetBool("IsIdle", state);
        animator.SetBool("IsWalking", state);
        animator.SetBool("IsPraying", state);
    }

    private void OnTriggerEnter(Collider other)
    {
        // NPC has hit the mission ending collider, start praying animation
        if (other == missionEndingCollider)
        {
            isFollowing = false;
            hasReachedPlayer = false;
            hasStartedPraying = false;
            SetAllAnimationStates(false);
            animator.SetBool("IsWalking", true);
            animator.SetBool("IsPraying", true);
            prayingAnimation.Play();
        }
    }
}