using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private ParticleSystem m_particles;
    private const float m_spawnDelay = 2f;

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
        m_particles.Play();
        yield return new WaitForSeconds(m_spawnDelay);
        m_particles.Stop();

        GameObject enemy = Instantiate(prefab, transform.position, transform.rotation, transform);
        enemy.GetComponent<Enemy>().SetArenaScript(arenaScript);
    }
}
