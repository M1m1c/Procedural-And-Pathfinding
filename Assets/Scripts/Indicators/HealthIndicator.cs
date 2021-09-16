using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthIndicator : MonoBehaviour
{
    public UnityEvent EntityHasDied;

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

        SetHealthIndicatorVisivbility(false);

        var persistentHealth = PersistentScript.CurrentHealth;
        if (persistentHealth < currentHealth)
        {
            while (currentHealth != persistentHealth)
            {
                ChangeHealth(false, false);
            }
        }
    }
    private void OnDestroy()
    {
        PersistentScript.CurrentHealth = currentHealth;
    }
    private void OnDisable()
    {
        PersistentScript.CurrentHealth = currentHealth;
    }

    private void SetHealthIndicatorVisivbility(bool visibility)
    {
        foreach (var renderer in HealthSpriteSlots)
        {
            renderer.enabled = visibility;
        }
    }

    public void TakeDamage(SpriteRenderer entitysRenderer)
    {
        if (invunruable == true) { return; }
        invunruable = true;
        StartCoroutine(DisplayHealth());
        ChangeHealth(false,true);
        StartCoroutine(InvunurableTimer(entitysRenderer));

        if (currentHealth == 0)
        {
            EntityHasDied.Invoke();
        }
    }

    private void ChangeHealth(bool positiveOrNegative, bool shouldBlink)
    {
        var changeValue = positiveOrNegative ? 1 : -1;

        var oldHealth = currentHealth;
        var indexAdjustment = oldHealth;
        if (oldHealth > 0) { indexAdjustment--; }

        if (indexAdjustment < 0 || indexAdjustment > HealthSpriteSlots.Length) { return; }

        currentHealth = Mathf.Clamp(currentHealth + changeValue, 0, HealthSpriteSlots.Length);

        var renderer = HealthSpriteSlots[indexAdjustment];
        renderer.color = positiveOrNegative ? defaultColor : InactiveColor;

        if (!shouldBlink) { return; }
        StartCoroutine(BlinkTimer(renderer, renderer.color, true));
    }
    private IEnumerator DisplayHealth()
    {
        SetHealthIndicatorVisivbility(true);
        yield return new WaitForSeconds(3f);
        SetHealthIndicatorVisivbility(false);
    }

    private IEnumerator InvunurableTimer(SpriteRenderer entitysRenderer)
    {
        yield return StartCoroutine(BlinkTimer(entitysRenderer, entitysRenderer.color, true));
        invunruable = false;
    }
    private IEnumerator BlinkTimer(SpriteRenderer entitysRenderer, Color originalColor, bool isRenderBlink)
    {
        var localBlink = maxBlinks;
        while (localBlink >= 0)
        {
            Color colorBlink = Color.white;

            if (localBlink % 2 == 1) { colorBlink = InactiveColor; }

            yield return StartCoroutine(BlinkRenderer(entitysRenderer, colorBlink, isRenderBlink));
            localBlink--;
        }
        entitysRenderer.color = originalColor;
        entitysRenderer.enabled = true;
        yield return null;
    }

    private IEnumerator BlinkRenderer(SpriteRenderer entitysRenderer, Color colorBlink, bool isRenderBlink)
    {
        if (isRenderBlink) { entitysRenderer.enabled = !entitysRenderer.enabled; }
        else { entitysRenderer.color = colorBlink; }
        yield return new WaitForSeconds(blinkInterval);
    }
}
