using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(FollowCameraRotation))]
public class HealthBar : MonoBehaviour
{
    public Slider slider;
    
    public void setMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;

    }
    public void SetHealth(int health)
    {
        slider.value = health;

    }
}