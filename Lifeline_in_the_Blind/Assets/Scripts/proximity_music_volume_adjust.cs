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
    private float distance;
    private float originalVolume;
    private float newVolume;

    // Start is called before the first frame update
    void Start()
    {
        originalVolume = audio_source.volume;
    }

    // Update is called once per frame
    void Update()
    {
        // closer you get to target, quieter music gets so you can hear object audio
        distance = Vector3.Distance(transform.position, target.transform.position);
        if (distance < threshold)
        {
            newVolume = (distance / threshold) * originalVolume;
            Debug.Log("Adjusting volume from" + audio_source.volume + " to " + newVolume);
            audio_source.volume = newVolume;
        }
    }
}
