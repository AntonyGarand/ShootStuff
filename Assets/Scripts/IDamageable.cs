using UnityEngine;

public interface IDamageable
{
    void Takehit(float damage, Vector3 hitPoint, Vector3 hitDirection);
    void TakeDamage(float damage);
}