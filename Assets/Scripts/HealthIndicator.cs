using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthIndicator : MonoBehaviour
{
    public SpriteRenderer[] HealthSpriteSlots = new SpriteRenderer[3];

    public Color InactiveColor;

    private Color defaultColor;

    private int currentHealth;

    private void Awake()
    {
        if (HealthSpriteSlots.Length == 0) { return; }
        defaultColor = HealthSpriteSlots[0].color;

        currentHealth = HealthSpriteSlots.Length;
    }

    public void ChangeHealth(bool positiveOrNegative)
    {
        var changeValue = positiveOrNegative ? 1 : -1;

        var oldHealth = currentHealth;
        var indexAdjustment = oldHealth;
        if (oldHealth > 0) { indexAdjustment--; }

        if (indexAdjustment < 0 || indexAdjustment > HealthSpriteSlots.Length) { return; }

        currentHealth = Mathf.Clamp(currentHealth + changeValue, 0, HealthSpriteSlots.Length);
        HealthSpriteSlots[oldHealth].color = positiveOrNegative ? defaultColor : InactiveColor;
    }
}
