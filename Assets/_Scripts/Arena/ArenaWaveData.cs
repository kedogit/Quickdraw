using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ArenaWaveData", menuName = "Scriptable Objects/ArenaWaveData")]
public class ArenaWaveData : ScriptableObject
{
    [System.Serializable]
    public struct Wave
    {
        public List<WaveEnemy> waveEnemies;
    }

    [System.Serializable]
    public struct WaveEnemy
    {
        public GameObject enemyPrefab;
        public int SpawnerIndex;
    }

    public List<Wave> waves = new List<Wave>();
}
