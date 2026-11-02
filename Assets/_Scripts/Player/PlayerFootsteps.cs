using System.Collections.Generic;
using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    [SerializeField] private List<AudioClip> m_footstepSounds;
    [SerializeField] private float m_footstepDelay = 0.5f;

    private AudioSource m_audioSource;
    private Rigidbody m_body;
    private Player m_playerScript;
    private float m_elapsed = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_body = GetComponentInParent<Rigidbody>();
        m_playerScript = GetComponentInParent<Player>();
        m_audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_playerScript.CheckGrounded())
        {
            m_elapsed += Time.deltaTime * (m_body.linearVelocity.magnitude / m_playerScript.MoveSpeed);

            if (m_elapsed >= m_footstepDelay)
            {
                m_audioSource.clip = m_footstepSounds.GetRandomItem();
                m_audioSource.Play();
                m_elapsed = 0f;
            }
        }
    }
}
