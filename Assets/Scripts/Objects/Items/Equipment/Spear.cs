using System;
using Unity.Mathematics;
using UnityEngine;

public class Spear : WeaponBehaviour, IAim
{   
    private bool shooted = false;
    private float baseForce = 13;
    private float currentForce = 0f;
    private float maxForce = 3f;
    private float minForce = 1f;

    void Update()
    {
        if(!ArmController.instance.IsAiming())
            currentForce = 0;
    }

    void FixedUpdate()
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        if (!shooted || rb.isKinematic)
            return;

        if (rb.linearVelocity.sqrMagnitude < 0.01f)
            return;

        Vector3 dir = rb.linearVelocity.normalized;

        Quaternion targetRotation =
            Quaternion.LookRotation(dir) *
            Quaternion.Euler(90f, 0f, 0f);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            12f * Time.fixedDeltaTime
        );
    }

    public float GetCurrentForce() => currentForce;
    public float GetMaxForce() => maxForce;

    public override void Attack(ArmController arm)
    {
        base.Attack(arm);

        if (!canUse)
            return;

        //canUse = false;
        StartCoroutine(UseCooldown());

        arm.StartCoroutine(arm.SpearAttackCR());
    }

    public void Aim()
    {
        if(!ArmController.instance.IsMoving())
        {   
            currentForce = math.clamp(currentForce + Time.deltaTime * 2, 0, maxForce);
            ArmController.instance.SetAiming(true);
        }
    }

    public void Shoot()
    {   
        if(currentForce >= minForce)
        {
            shooted = true;

            InventoryController inventory = InventoryController.instance;
            Rigidbody rb = GetComponent<Rigidbody>();
            Collider co = GetComponent<Collider>();

            if (rb == null) return;

            // salir de la mano
            transform.SetParent(InventoryController.instance.GetItemsParent());
            HotBarController.instance.SetHandItem(null);
            inventory.SetItem(HotBarController.instance.GetSelectedIndex(), null);

            // activar física
            rb.isKinematic = false;
            rb.detectCollisions = true;
            co.enabled = true;

            Vector3 dir = CameraController.instance.transform.forward;

            // velocidad directa (más consistente que force)
            rb.linearVelocity = dir * baseForce * currentForce;
        }

        ArmController.instance.SetAiming(false);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (shooted)
        {
            if (!collision.gameObject.CompareTag("Player"))
            {
                GetComponent<Rigidbody>().isKinematic = true;
                GetComponentInChildren<Collider>().isTrigger = true;
                float damage = Mathf.Round(
                    Mathf.Clamp(
                        weaponData.damage * currentForce,
                        weaponData.damage,
                        weaponData.damage * maxForce
                    )
                );

                ApplyDamage(collision.gameObject, damage);
                transform.SetParent(collision.transform, true);
            }
        }
    }
}