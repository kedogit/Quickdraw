using System.Collections;
using UnityEngine;

public abstract class EnemyMeleeArmed : Enemy
{
    [SerializeField] private TrailRenderer m_weaponTrail;
    [SerializeField] private BoxCollider m_weaponHitbox;

    public void EnableWeaponHitbox(SFX soundFX)
    {
        //enables hitbox and plays sfx (called by animevent)
        m_weaponTrail.enabled = true;
        AudioManager.GetInstance().PlaySFX(soundFX, transform.position);
        m_audioSource.Play();
        m_weaponHitbox.enabled = true;
    }

    protected IEnumerator DisableWeaponHitbox(float hitboxDuration)
    {
        yield return new WaitForSeconds(hitboxDuration);
        m_weaponHitbox.enabled = false;
        m_weaponTrail.enabled = false;
    }

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
