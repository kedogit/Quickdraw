using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    private float m_weaponDamage = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Player playerScript = other.gameObject.GetComponent<Player>();
            playerScript.Hurt(m_weaponDamage);
        }
    }

    //public func to centralize weapon damage variable to enemy script
    public void SetWeaponDamage(float weaponDamage)
    {
        m_weaponDamage = weaponDamage;
    }
}
