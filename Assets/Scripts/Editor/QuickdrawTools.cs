using UnityEditor;
using UnityEngine;

public class QuickdrawTools : EditorWindow
{
    private static QuickdrawTools m_window = null;

    private int m_selectedTab = 0;
    private string[] m_toolTabs = { "Cheats", "Arena Viewer" };

    private bool m_godMode = false;
    private bool m_instakill = false;

    private bool m_isArenaActive = false;
    private int m_waveCount = 0;
    private int m_enemyCount = 0;
    

    [MenuItem("Window/Quickdraw/Tools")]
    public static void ShowWindow()
    {
        if (m_window == null)
        {
            m_window = GetWindow<QuickdrawTools>();
            m_window.titleContent = new GUIContent("Quickdraw Tools");
        }
    }

    public static QuickdrawTools GetWindow()
    {
        return m_window;
    }

    private void OnGUI()
    {
        m_selectedTab = GUILayout.Toolbar(m_selectedTab, m_toolTabs);
        switch(m_selectedTab)
        {
            case 0:
                ShowCheats();
                break;
            case 1:
                ShowArenaViewer();
                break;
        }
    }

    //resets the bools on game start
    private void OnEnable()
    {
        m_godMode = false;
        m_instakill = false;
        m_isArenaActive = false;
        m_window = this;

        //subscribe to the static events from the arena trigger script
        ArenaTrigger.m_onArenaEnable += SetArenaActive;
        ArenaTrigger.m_onWaveChange += SetWaveCount;
        ArenaTrigger.m_onKillCountChange += DecreaseArenaEnemyCount;
    }

    private void OnDisable()
    {
        ArenaTrigger.m_onArenaEnable -= SetArenaActive;
        ArenaTrigger.m_onWaveChange -= SetWaveCount;
        ArenaTrigger.m_onKillCountChange -= DecreaseArenaEnemyCount;
    }

    private void SetArenaActive(bool activeState)
    {
        m_isArenaActive = activeState;
        Repaint();
    }

    private void SetWaveCount(int currentWave, int enemyCount)
    {
        m_waveCount = currentWave;
        m_enemyCount = enemyCount;
        Repaint();
    }

    public void DecreaseArenaEnemyCount()
    {
        m_enemyCount--;
        Repaint();
    }

    private void ShowCheats()
    {
        if (Application.isPlaying)
        {
            //kill all button: fetches all enemies and goes through their hurt function to kill them and fire off everything appropriately
            if (GUILayout.Button("Kill all enemies"))
            {
                GameObject[] allEnemies = GameObject.FindGameObjectsWithTag("Enemy");

                foreach (GameObject gameObject in allEnemies)
                {
                    gameObject.GetComponent<Enemy>().Hurt(999f, BodyPart.BODY);
                }
            }

            //god mode toggle: calls an observer event to tell the player script to turn on godmode
            bool godModeToggle = GUILayout.Toggle(m_godMode, "God Mode");
            if (godModeToggle != m_godMode)
            {
                m_godMode = godModeToggle;
                OnGodToggle();
            }

            //insta kill toggle: calls an observer event to tell the player script to turn on instakill
            bool instakillToggle = GUILayout.Toggle(m_instakill, "Instakill");
            if (instakillToggle != m_instakill)
            {
                m_instakill = instakillToggle;
                OnInstakillToggle();
            }
        }
        else
        {
            GUILayout.Label("Game is not currently running. Start the game first!");
        }
    }

    private void OnGodToggle()
    {
        Observer.GetInstance().TriggerEvent(EVENT.ON_CHEAT_GODMODE);
    }

    private void OnInstakillToggle()
    {
        Observer.GetInstance().TriggerEvent(EVENT.ON_CHEAT_INSTAKILL);
    }

    private void ShowArenaViewer()
    {
        if (m_isArenaActive && Application.isPlaying)
        {
            GUILayout.Label("Arena is ACTIVE.");
            GUILayout.Label("Wave " + m_waveCount);
            GUILayout.Label("Enemies left: " + m_enemyCount);
        }
        else
        {
            GUILayout.Label("No arenas currently active.");
        }
    }
}
