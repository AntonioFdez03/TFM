using System;
using System.Collections.Generic;
using UnityEngine;

public class Helicopter : MonoBehaviour
{   
    [SerializeField] private GameObject rescueRope;
    [SerializeField] private AudioClip helicopterSound;
    private AudioSource audioSource;
    private Vector3 helicopterTarget;
    private float speed = 40f;
    private float rotationSpeed = 2f;
    private float timer = 0;
    private bool ropeActive = false;
    private bool helicopterArrived = false;
    private bool leave = false;

    void Start()
    {   
        rescueRope.SetActive(false);

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

    public void Leave() => leave = true;

    void Update()
    {
        float distance = 0;
        
        if(!helicopterArrived)
            distance = Vector3.Distance(transform.position, helicopterTarget);


        if (distance > 10)
            speed = 40f;
        else if (distance > 5)
            speed = 20f;
        else
        {   
            helicopterArrived = true;
            speed = 0f;

            if (!ropeActive)
            {
                ropeActive = true;
                rescueRope.SetActive(true);
            }

            timer += Time.deltaTime;
            ObjectivesController.instance.UpdateHelicopterTime(timer);

            if(timer >= GameController.instance.GetHelicopterWaitTime())
            {
                print("Tiempo superado");
                rescueRope.GetComponent<RescueRope>().RemoveRope();
            }
        }

        if (leave)
            speed = 20f;

        transform.position += transform.forward * speed * Time.deltaTime;

        ApplyHelicopterWobble();
    }

    private void ApplyHelicopterWobble()
    {
        // Dirección hacia el objetivo
        Vector3 dir = (helicopterTarget - transform.position).normalized;

        // Rotación principal hacia el objetivo
        Quaternion lookRotation = Quaternion.LookRotation(dir);

        // Bamboleos naturales
        float roll = Mathf.Sin(Time.time * 0.8f) * 4f;     // izquierda/derecha
        float pitch = Mathf.Sin(Time.time * 1.3f) * 2f;    // adelante/atrás

        if(speed == 0)
        {
            roll = 0;
            pitch = 0;
        }

        // Movimiento vertical
        float hover = Mathf.Sin(Time.time * 1.5f) * 0.15f;

        // Aplicar rotación con balanceo
        Quaternion wobbleRotation = Quaternion.Euler(pitch, 0f, roll);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            lookRotation * wobbleRotation,
            rotationSpeed * Time.deltaTime
        );

        // Aplicar pequeña oscilación vertical
        transform.position += Vector3.up * hover * Time.deltaTime;
    }

}