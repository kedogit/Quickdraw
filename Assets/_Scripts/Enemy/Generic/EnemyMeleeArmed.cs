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
        m_weaponTrail.enabled = false;
        m_weaponHitbox.enabled = false;
    }
}
