using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IObjectHealth
{   
    void SetCurrentHealth(float health);
    float GetCurrentHealth();
}