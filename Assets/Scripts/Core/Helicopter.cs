using System;
using System.Collections.Generic;
using UnityEngine;

public class Helicopter : MonoBehaviour
{   
    [SerializeField] private AudioClip helicopterSound;
    private Vector3 helicopterTarget;
    private float speed = 40f;
    private float rotationSpeed = 2f;
    private Animator animator;
    private AudioSource audioSource;
    void Start()
    {   
        print("Helicoptero en marcha");

        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        audioSource.volume = 10f;
        audioSource.spatialBlend = 1;
        audioSource.clip = helicopterSound;
        audioSource.loop = true;
        audioSource.Play();
    }

    public void SetTarget(Vector3 target){
        helicopterTarget = target;
        transform.LookAt(helicopterTarget);
    }
    void Update()
    {
        float distance = Vector3.Distance(transform.position, helicopterTarget);
        
        if(distance > 10)
            speed = 40f;
        else if(distance > 5)
            speed = 20f;
        else
            speed = 0; 

        transform.position += transform.forward * speed * Time.deltaTime;

    }

}