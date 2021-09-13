using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthIndicator : MonoBehaviour
{
    public SpriteRenderer[] HealthSpriteSlots = new SpriteRenderer[3];

    public Color InactiveColor;

    private Color defaultColor;

    private int currentHealth;

    private int maxBlinks = 8;

    private float blinkInterval = 0.2f;

    private bool invunruable = false;

    private void Awake()
    {
        if (HealthSpriteSlots.Length == 0) { return; }
        defaultColor = HealthSpriteSlots[0].color;

        currentHealth = HealthSpriteSlots.Length;
    }

    public void TakeDamage(SpriteRenderer entitysRenderer)
    {
        if (invunruable == true) { return; }
        invunruable = true;
        ChangeHealth(false);
        StartCoroutine(InvunurableTimer(entitysRenderer));
    }

    private void ChangeHealth(bool positiveOrNegative)
    {
        var changeValue = positiveOrNegative ? 1 : -1;

        var oldHealth = currentHealth;
        var indexAdjustment = oldHealth;
        if (oldHealth > 0) { indexAdjustment--; }

        if (indexAdjustment < 0 || indexAdjustment > HealthSpriteSlots.Length) { return; }

        currentHealth = Mathf.Clamp(currentHealth + changeValue, 0, HealthSpriteSlots.Length);
        HealthSpriteSlots[indexAdjustment].color = positiveOrNegative ? defaultColor : InactiveColor;
    }

    private IEnumerator InvunurableTimer(SpriteRenderer entitysRenderer)
    {
        var localBlink = maxBlinks;
        while (localBlink >= 0)
        {
            yield return StartCoroutine(BlinkRenderer(entitysRenderer));
            localBlink--;
        }
        entitysRenderer.enabled = true;
        invunruable = false;
        yield return null;
    }

    private IEnumerator BlinkRenderer(SpriteRenderer entitysRenderer)
    {
        entitysRenderer.enabled = !entitysRenderer.enabled;
        yield return new WaitForSeconds(blinkInterval);
    }
}
