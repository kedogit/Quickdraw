using Unity.Cinemachine;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject m_player;
    [SerializeField] private GameObject m_cameras;
    [SerializeField] private GameObject m_gameHUD;
    [SerializeField] private GameObject m_audioManager;
    [SerializeField] private GameObject m_impulseManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        //create and store the impulse manager
        GameObject impulseManager = Instantiate(m_impulseManager);
        impulseManager.SetActive(true);

        //create and store the audio manager
        GameObject audioManager = Instantiate(m_audioManager);
        audioManager.SetActive(true);

        //determine player start pos depending on game state
        Vector3 playerStartPos = transform.position;
        float startPan = transform.rotation.eulerAngles.y;

        if (Observer.GetInstance()?.GameState == GAME_STATE.SAVEDGAME)
        {
            //fetches the saved location and sets the player's position
            float x = PlayerPrefs.GetFloat("PlayerStartX");
            float y = PlayerPrefs.GetFloat("PlayerStartY");
            float z = PlayerPrefs.GetFloat("PlayerStartZ");
            playerStartPos = new Vector3(x, y, z);

            //sets start pan
            startPan = PlayerPrefs.GetFloat("PlayerPanAngle");
        }
        else
        {
            AudioManager.GetInstance()?.ChangePersistentBGM(SONG.OVERWORLD1);

            PlayerPrefs.DeleteKey("hasGrapple");
            PlayerPrefs.DeleteKey("PlayerStartX");
            PlayerPrefs.DeleteKey("PlayerStartY");
            PlayerPrefs.DeleteKey("PlayerStartZ");
            PlayerPrefs.DeleteKey("PlayerPanAngle");
            PlayerPrefs.DeleteKey("CheckpointIndex");

            Observer.GetInstance()?.SetGameState(GAME_STATE.SAVEDGAME);
        }

        //create and store the HUD
        GameObject gameHUD = Instantiate(m_gameHUD);
        gameHUD.SetActive(true);

        //create and store the player at the proper start pos
        GameObject player = Instantiate(m_player, playerStartPos, Quaternion.identity);
        player.SetActive(true);

        //create and store the cameras
        GameObject cameras = Instantiate(m_cameras);
        cameras.SetActive(true);

        //grab necessary transforms
        Transform fpsCamera = cameras.transform.Find("VCAM-FPSView");
        Transform playerBow = cameras.transform.Find("VCAM-FPSView/HandPivot/NewPlayerBow");
        Transform playerCamAnchor = player.transform.Find("CameraAnchor");

        //set the camera's original rotation to match the spawner rotation
        CinemachinePanTilt cameraPanTiltComponent = fpsCamera.GetComponent<CinemachinePanTilt>();
        cameraPanTiltComponent.TiltAxis.Value = 0f;
        cameraPanTiltComponent.PanAxis.Value = startPan;

        //set player script references
        player.GetComponent<Player>().SetReferences(fpsCamera, playerBow, gameHUD.GetComponent<GameHUD>());

        //set camera tracking target
        fpsCamera.gameObject.GetComponent<CinemachineCamera>().Target.TrackingTarget = playerCamAnchor;

        //die
        Destroy(this.gameObject);
    }
}
