using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioClipGroup : MonoBehaviourSingleton<AudioClipGroup> 
{
    public enum SoundTypes_hoverClickSounds{
        hover,
        click
    }
    public SoundTypes_hoverClickSounds _soundTypes_hoverClickSounds;
    public AudioClip[] hoverClickSounds;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
