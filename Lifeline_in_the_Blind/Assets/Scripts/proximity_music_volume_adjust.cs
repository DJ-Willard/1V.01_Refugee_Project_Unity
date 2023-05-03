using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class proximity_music_volume_adjust : MonoBehaviour
{
    // public to expose to Inspector, followed by tooltip
    // (tooltip syntax is similar to slider range syntax)
    [Tooltip("When target object is within threshold, audio is steadily reduced in volume.")]
    public Transform target;
    [Tooltip("When target object is within threshold, audio is steadily reduced in volume.")]
    public float threshold;
    public AudioSource audio_source;
    public GlobalBoolVariable pickedUpRadio;
    private float distance;
    private float originalVolume;
    private float newVolume;
    private bool isCoroutineStarted;
    

    void Start()
    {
        originalVolume = audio_source.volume;
        isCoroutineStarted = false;
    }

    void Update()
    {
        // check for radio pickup
        // a more efficient way to do this might be an event and delegate manager
        if (!pickedUpRadio.value)
        {
            // closer you get to target, quieter music gets so you can hear object audio
            distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance < threshold)
            {
                newVolume = (distance / threshold) * originalVolume;
                Debug.Log("IF: Adjusting volume from" + audio_source.volume + " to " + newVolume);
                audio_source.volume = newVolume;
            }
        }
        else 
        // return to original volume
        {
            if (!isCoroutineStarted)
            {
                StartCoroutine(ChangeVolumeCoroutine());
                isCoroutineStarted = true;
            }
            //Debug.Log("Volume is " + audio_source.volume);
        }
        //Debug.Log("pickedUpRadio = " + pickedUpRadio.value);
    }

    private IEnumerator ChangeVolumeCoroutine()
    {
        Debug.Log("Ran coroutine");
        while (audio_source.volume < originalVolume)
        {
            audio_source.volume += 0.0025f;//0.00005f;
            yield return null;
        }
        Debug.Log("Finished coroutine");
        this.gameObject.SetActive(false);
    }
}
