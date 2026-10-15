using UnityEngine;

public class MainMenuCamera : MonoBehaviour
{
    [SerializeField] private float m_rotationSpeed = 20f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.RotateAround(Vector3.zero, Vector3.up, m_rotationSpeed * Time.deltaTime);
    }
}
