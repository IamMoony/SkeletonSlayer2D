using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHealthManagement
{
    int Health_Base { get; set; }
    int Health_Current { get; set; }

    void AddHealth(int amount);

    void SubtractHealth(int amount);
}
