using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DamageText : MonoBehaviour
{
    private TextMesh damageText;

    public void Awake()
    {
        damageText = GetComponent<TextMesh>();
    }

    public void ShowDamageText(float damageTextOffset, float damage)
    {
        // Set the damage value and position of the text object
        damageText.text = damage.ToString();
        transform.position += new Vector3(0f, damageTextOffset, 0f);
        // Trigger animation or any other desired effects
        // Example: Scale up the text and fade it out
        StartCoroutine(AnimateDamageText());
    }

    private IEnumerator AnimateDamageText()
    {
        float scaleFactor = 1f;
        float fadeSpeed = 1.5f;

        while (scaleFactor < 2f)
        {
            scaleFactor += Time.deltaTime;
            transform.localScale = Vector3.one * scaleFactor;

            Color textColor = damageText.color;
            textColor.a -= fadeSpeed * Time.deltaTime;
            damageText.color = textColor;

            yield return null;
        }

        Destroy(gameObject); // Remove the damage text object from the scene
    }
}
