using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioSource audioSource2;

    public AudioClip clip1;
    public AudioClip clip2;
    public GameObject TestSquare;
    public GameObject TestSquare2;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = TestSquare.GetComponent<AudioSource>();
        audioSource2 = TestSquare2.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J)) 
        {
            audioSource.clip = clip1;
            audioSource2.clip = clip2;
            Debug.Log("$Clip 1 loaded");
        }
        if (Input.GetKeyDown(KeyCode.K)) 
        {
            audioSource.clip = clip2;
            audioSource2.clip = clip1;
            Debug.Log("$Clip 2 Loaded");
        }
        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            audioSource2.Play();
            audioSource.Play();

        }
    }
}
