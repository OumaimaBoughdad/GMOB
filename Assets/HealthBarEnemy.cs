using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarEnemy : MonoBehaviour
{
    [SerializeField] private float timeToDrain = 0.25f;
    [SerializeField] private Gradient gradient;
    public Image image;
    private float targetFillAmount = 1f;
    private Coroutine drainHealthBarCoroutine;

    private void Start()
    {
        if (image == null)
        {
            image = GetComponent<Image>();
        }
        CheckHealthBarGradientAmount();
    }

    public void UpdateHealthBar(float maxHealth, float currentHealth)
    {
        targetFillAmount = currentHealth / maxHealth;
        if (drainHealthBarCoroutine != null)
        {
            StopCoroutine(drainHealthBarCoroutine);
        }
        drainHealthBarCoroutine = StartCoroutine(DrainHealthBar());
    }

    private IEnumerator DrainHealthBar()
    {
        float initialFillAmount = image.fillAmount;
        float elapsedTime = 0f;
        while (elapsedTime < timeToDrain)
        {
            elapsedTime += Time.deltaTime;
            image.fillAmount = Mathf.Lerp(initialFillAmount, targetFillAmount, elapsedTime / timeToDrain);
            CheckHealthBarGradientAmount();
            yield return null;
        }
        image.fillAmount = targetFillAmount;
        CheckHealthBarGradientAmount();
    }

    private void CheckHealthBarGradientAmount()
    {
        float fillAmount = Mathf.Clamp01(image.fillAmount);
        image.color = gradient.Evaluate(fillAmount);
    }
}
