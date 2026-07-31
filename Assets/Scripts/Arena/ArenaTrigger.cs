using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaTrigger : MonoBehaviour
{
    [SerializeField] private List<EnemySpawner> m_spawners;
    [SerializeField] private ArenaWaveData m_waveData;
    [SerializeField] private GameObject m_doors;
    [SerializeField] bool m_isFinalArena = false;

    //static events for the quickdraw tool
    public static event Action<bool> m_onArenaEnable;
    public static event Action<int, int> m_onWaveChange;
    public static event Action m_onKillCountChange;

    private int m_currentWave = 0;
    private int m_currentWaveKillCount = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            m_doors.SetActive(true);
            AudioManager.GetInstance().FadeToBattleBGM();
            GetComponent<BoxCollider>().enabled = false;
            SpawnWave(m_currentWave);
        }
    }

    private void SpawnWave(int waveIndex)
    {
        //fetch the current wave;
        ArenaWaveData.Wave currentWave = m_waveData.waves[waveIndex];

        //actions for the quickdraw editor window
        m_onArenaEnable?.Invoke(true);
        m_onWaveChange?.Invoke(m_currentWave + 1, m_waveData.waves[m_currentWave].waveEnemies.Count);

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
        //action for the quickdraw editor window
        m_onKillCountChange?.Invoke();

        m_currentWaveKillCount++;
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
                    m_doors.SetActive(false);
                    AudioManager.GetInstance().FadeToPersistentBGM();
                }
                else
                {
                    StartCoroutine(EndOfLevelDelay());
                }

                //action for the quickdraw editor window
                m_onArenaEnable?.Invoke(false);
            }
        }
    }

    private IEnumerator EndOfLevelDelay()
    {
        yield return new WaitForSeconds(2f);
        Observer.GetInstance().TriggerEvent(EVENT.ON_LEVEL_COMPLETE);
    }
}
