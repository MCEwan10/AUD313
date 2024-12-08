using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioSource audioSource2;
    public float clipDuration;
    public AudioClip clip1;
    public AudioClip clip2;
    public GameObject TestSquare;
    public GameObject TestSquare2;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = TestSquare.GetComponent<AudioSource>();
        audioSource2 = TestSquare2.GetComponent<AudioSource>();
        clipDuration = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J)) 
        {
            audioSource.clip = clip1;
            audioSource2.clip = clip2;
            clipDuration = clip1.length;
            Debug.Log("$Clip 1 loaded");
        }
        if (Input.GetKeyDown(KeyCode.K)) 
        {
            audioSource.clip = clip2;
            audioSource2.clip = clip1;
            clipDuration = clip2.length;
            Debug.Log("$Clip 2 Loaded");
        }
        if (Input.GetKeyDown(KeyCode.W)) audioSource.pitch += 0.2f;
        if (Input.GetKeyDown(KeyCode.S)) audioSource.pitch -= 0.2f;
        if (Input.GetKeyDown(KeyCode.E)) audioSource.pitch = 1f;
        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            
            audioSource.Play();//PlayOneShot(audioSource.clip, 0.5f);
            while (clipDuration>0.0f)
            {
                clipDuration -= Time.deltaTime;  
            }
            //audioSource2.Play();//PlayOneShot(audioSource2.clip, 0.5f);

        }
    }
}
