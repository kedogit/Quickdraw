using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] int m_checkpointIndex = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GetComponent<Collider>().enabled = false;

            //if this checkpoint's index is higher than the saved one, overwrite previous checkpoint
            if (m_checkpointIndex > PlayerPrefs.GetInt("CheckpointIndex"))
            {
                PlayerPrefs.SetInt("CheckpointIndex", m_checkpointIndex);
                PlayerPrefs.SetFloat("PlayerStartX", transform.position.x);
                PlayerPrefs.SetFloat("PlayerStartY", transform.position.y);
                PlayerPrefs.SetFloat("PlayerStartZ", transform.position.z);
                PlayerPrefs.Save();
            }
        }
    }
}
