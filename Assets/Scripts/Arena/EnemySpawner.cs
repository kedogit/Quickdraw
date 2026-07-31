using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private ParticleSystem m_particles;
    private const float m_spawnDelay = 2f;
    private const float m_aggroRange = 99f;

    private void Start()
    {
        m_particles.Stop();
    }

    public void SpawnMonster(GameObject prefab, ArenaTrigger arenaScript)
    {
        StartCoroutine(SpawnDelay(prefab, arenaScript));
    }

    private IEnumerator SpawnDelay(GameObject prefab, ArenaTrigger arenaScript)
    {
        //play smoke particles to indicate spawn
        m_particles.Play();
        yield return new WaitForSeconds(m_spawnDelay);
        m_particles.Stop();


        //instantiate enemy and set values specific to arena enemies
        GameObject enemy = Instantiate(prefab, transform.position, transform.rotation, transform);
        Enemy enemyScript = enemy.GetComponent<Enemy>();
        enemyScript.SetArenaScript(arenaScript);
        enemyScript.SetAggroRange(m_aggroRange);
    }
}
