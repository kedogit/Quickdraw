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

    public void SetWeaponDamage(float weaponDamage)
    {
        m_weaponDamage = weaponDamage;
    }
}
