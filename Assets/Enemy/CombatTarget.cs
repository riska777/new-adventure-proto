using UnityEngine;

public class CombatTarget : MonoBehaviour
{
    public DamageText damageTextPrefab;
    public float damageTextOffset = 2f;

    public void TakeDamage(float damage)
    {
        Debug.Log($"DMG {gameObject}   {damage}");
        if (damageTextPrefab)
        {
            // Instantiate the damage text object and display it
            DamageText damageText = Instantiate(damageTextPrefab, transform.position, Quaternion.identity, transform);
            damageText.ShowDamageText(damageTextOffset, damage);
        }
    }
}
