using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IActivateableObject
{   
    void ToggleActivation();
    bool isActive();
}