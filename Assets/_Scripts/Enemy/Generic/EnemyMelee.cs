using System.Collections;
using UnityEngine;

public abstract class EnemyMelee : Enemy
{
    [SerializeField] private TrailRenderer m_weaponTrail;
    [SerializeField] private BoxCollider m_weaponHitbox;

    public void HandleAttackEffects(SFX soundFX, BoxCollider hitbox, float hitboxDuration)
    {
        PlaySFX(soundFX);
        StartCoroutine(ToggleHitbox(hitbox, hitboxDuration));
    }

    public void HandleAttackEffects(SFX soundFX, BoxCollider hitbox, float hitboxDuration, TrailRenderer trail, float trailDuration)
    {
        HandleAttackEffects(soundFX, hitbox, hitboxDuration);
        StartCoroutine(ToggleTrail(trail, trailDuration));
    }

    private void PlaySFX(SFX soundFX)
    {
        AudioManager.GetInstance().PlaySFX(soundFX, transform.position);
    }

    private IEnumerator ToggleHitbox(BoxCollider hitbox, float hitboxDuration)
    {
        hitbox.enabled = true;
        yield return new WaitForSeconds(hitboxDuration);
        hitbox.enabled = false;
    }

    private IEnumerator ToggleTrail(TrailRenderer trail, float trailDuration)
    {
        trail.enabled = true;
        yield return new WaitForSeconds(trailDuration);
        trail.enabled = false;
    }
}
