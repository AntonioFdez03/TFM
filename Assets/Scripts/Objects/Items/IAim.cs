using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IAim
{   
    void Aim();
    void Shoot();
    float GetCurrentForce();
    float GetMaxForce();
}