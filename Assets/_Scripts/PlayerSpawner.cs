using Unity.Cinemachine;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject m_player;
    [SerializeField] private GameObject m_cameras;
    [SerializeField] private GameObject m_gameHUD;
    [SerializeField] private GameObject m_audioManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        //determine player start pos depending on game state
        Vector3 playerStartPos = transform.position;

        if (Observer.GetInstance()?.GameState == GAME_STATE.SAVEDGAME)
        {
            //fetches the saved location and sets the player's position
            float x = PlayerPrefs.GetFloat("PlayerStartX");
            float y = PlayerPrefs.GetFloat("PlayerStartY");
            float z = PlayerPrefs.GetFloat("PlayerStartZ");

            playerStartPos = new Vector3(x, y, z);
        }
        else
        {
            AudioManager.GetInstance()?.ChangePersistentBGM(SONG.OVERWORLD1);
            PlayerPrefs.SetInt("hasGrapple", 0);
            Observer.GetInstance()?.SetGameState(GAME_STATE.SAVEDGAME);
        }

        //create and store the GOs
        GameObject audioManager = Instantiate(m_audioManager);
        audioManager.SetActive(true);

        GameObject gameHUD = Instantiate(m_gameHUD);
        gameHUD.SetActive(true);

        GameObject player = Instantiate(m_player, playerStartPos, Quaternion.identity);
        player.SetActive(true);

        GameObject cameras = Instantiate(m_cameras);
        cameras.SetActive(true);

        //grab necessary transforms
        Transform fpsCamera = cameras.transform.Find("VCAM-FPSView");
        Transform playerBow = cameras.transform.Find("VCAM-FPSView/HandPivot/NewPlayerBow");
        Transform playerCamAnchor = player.transform.Find("CameraAnchor");

        //set the camera's original rotation to match the spawner rotation
        CinemachinePanTilt cameraPanTiltComponent = fpsCamera.GetComponent<CinemachinePanTilt>();
        cameraPanTiltComponent.TiltAxis.Value = transform.rotation.x;
        cameraPanTiltComponent.PanAxis.Value = transform.rotation.y;

        //set player script references
        player.GetComponent<Player>().SetReferences(fpsCamera, playerBow, gameHUD.GetComponent<GameHUD>());

        //set camera tracking target
        fpsCamera.gameObject.GetComponent<CinemachineCamera>().Target.TrackingTarget = playerCamAnchor;

        //die
        Destroy(this.gameObject);
    }
}
