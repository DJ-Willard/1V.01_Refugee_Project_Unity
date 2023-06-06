using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Capture : MonoBehaviour
{
    public Transform player;
    public AudioSource EnemyMusic;
    public AudioSource WalkingMusic;
    public AudioSource AmbientMusic;
    public float EnemyMusicRange;
    public bool PlayEnemyMusic;
    
    float minEnemyVolume = 0.0f;
    float maxEnemyVolume;
    bool m_IsPlayerInRange;

    void Awake()
    {
        if(PlayEnemyMusic)
        {
            maxEnemyVolume = EnemyMusic.volume;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.transform == player)
        {
            m_IsPlayerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.transform == player)
        {
            m_IsPlayerInRange = false;
        }
    }

    void LateUpdate()
    {
        if(m_IsPlayerInRange)
        {
            /*Vector3 direction = player.position - transform.position + Vector3.up;
            Ray ray = new Ray(transform.position, direction);
            RaycastHit raycastHit;

            if(Physics.Raycast(ray, out raycastHit))
            {
                if(raycastHit.collider.transform == player)
                {
                    SceneManager.LoadScene(0);
                }
            }*/
            // SceneManager.LoadScene(0); // this will now be handled by CanvasDeathMenu and associated code

            // todo new logic
            // write and call function from TPC script?
        }

        if(PlayEnemyMusic)
        {
            float distance = Vector3.Distance(transform.position, player.position);
            if(distance < EnemyMusicRange)
            {
                EnemyMusic.volume += 0.015f;
                if(EnemyMusic.volume > maxEnemyVolume)
                {
                    EnemyMusic.volume = maxEnemyVolume;
                }
                AmbientMusic.volume -= 0.005f;
                WalkingMusic.volume -= 0.005f;
            }
            else
            {
                if(EnemyMusic.volume < minEnemyVolume)
                {
                    EnemyMusic.volume = minEnemyVolume;
                }
            }
        }
    }
}
