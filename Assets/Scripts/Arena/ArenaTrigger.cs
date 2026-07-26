using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaTrigger : MonoBehaviour
{
    [SerializeField] private List<EnemySpawner> m_spawners;
    [SerializeField] private ArenaWaveData m_waveData;
    [SerializeField] private GameObject m_doors;
    [SerializeField] bool m_isFinalArena = false;

    private int m_currentWave = 0;
    private int m_currentWaveKillCount = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            m_doors.SetActive(true);
            GetComponent<BoxCollider>().enabled = false;
            SpawnWave(m_currentWave);
        }
    }

    private void SpawnWave(int waveIndex)
    {
        //fetch the current wave;
        ArenaWaveData.Wave currentWave = m_waveData.waves[waveIndex];

        //send info to custom editor window
        QuickdrawTools.GetWindow().UpdateArenaWave(true, m_currentWave + 1, m_waveData.waves[m_currentWave].waveEnemies.Count);

        //for each wave enemy in the current wave
        foreach (ArenaWaveData.WaveEnemy enemy in currentWave.waveEnemies)
        {
            //failsafe for wrong index in arena data
            if (enemy.SpawnerIndex < m_spawners.Count)
            {
                m_spawners[enemy.SpawnerIndex].SpawnMonster(enemy.enemyPrefab, this);
                Debug.Log("Spawning enemy at " + enemy.SpawnerIndex);
            }
            else
            {
                Debug.Log("Invalid enemy spawner index: " + enemy.SpawnerIndex);
            }
        }
    }

    private void NextWave()
    {
        m_currentWave++;
        SpawnWave(m_currentWave);
    }

    public void RegisterKill()
    {
        QuickdrawTools.GetWindow().DecreaseArenaEnemyCount();

        m_currentWaveKillCount++;
        Debug.Log("wave kill, kill count at " + m_currentWaveKillCount);
        if (m_currentWaveKillCount >= m_waveData.waves[m_currentWave].waveEnemies.Count)
        {
            if (m_currentWave < m_waveData.waves.Count - 1)
            {
                NextWave();
                m_currentWaveKillCount = 0;
            }
            else
            {
                if (!m_isFinalArena)
                {
                    QuickdrawTools.GetWindow().UpdateArenaWave(false, 0, 0);
                    m_doors.SetActive(false);
                }
                else
                {
                    StartCoroutine(EndOfLevelDelay());
                }
            }
        }
    }

    private IEnumerator EndOfLevelDelay()
    {
        yield return new WaitForSeconds(2f);
        Observer.GetInstance().TriggerEvent(EVENT.ON_LEVEL_COMPLETE);
    }
}
