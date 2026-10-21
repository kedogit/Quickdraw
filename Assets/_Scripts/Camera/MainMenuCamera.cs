using UnityEngine;

public class MainMenuCamera : MonoBehaviour
{
    [SerializeField] private float m_rotationSpeed = 20f;

    void Update()
    {
        this.transform.RotateAround(Vector3.zero, Vector3.up, m_rotationSpeed * Time.deltaTime);
    }
}
