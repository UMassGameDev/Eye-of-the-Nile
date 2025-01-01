using UnityEngine;

/** \brief
This script handles Geb's phases.
It keeps track of the current phase or when a cutscene is playing, and detects when a new phase should be started.
When a new phase is started, it tells the Geb's other scripts.

Documentation updated 12/31/2024
\author Alexander Art
*/
public class GebPhaseController : MonoBehaviour
{
    /// Reference to Geb's health script, used for changing the phase when Geb reaches certain health thresholds.
    protected BossHealth bossHealth;
    /// References to all of the other Geb-specific scripts.
    protected GebBossController bossController;
    protected GebRoomController roomController;

    /// The percentage that Geb's health needs to drop below for phase 2 to start.
    public float phase2Threshold = 2f/3f;
    /// The percentage that Geb's health needs to drop below for phaes 3 to start.
    public float phase3Threshold = 1f/3f;

    /// Keep track of the current phase.
    public GebPhase phase { get; private set; } = GebPhase.Inactive;
    /// Used for checking when the health of the boss changes.
    private int previousHealth;

    /// Set references to Geb's health script and the Geb-specific scripts.
    void Awake()
    {
        bossHealth = GetComponent<BossHealth>();
        bossController = GetComponent<GebBossController>();
        roomController = GetComponent<GebRoomController>();
    }

    void Update()
    {
        // Runs when Geb's health changes.
        if (previousHealth != bossHealth.currentHealth)
        {
            previousHealth = bossHealth.currentHealth;

            // When Geb's health is in a certain range, a new phase will be triggered unless if Geb is already in that phase.
            if (bossHealth.currentHealth <= 0)
            {
                if (phase != GebPhase.ClosingCutscene && phase != GebPhase.Defeated)
                {
                    TriggerGebDefeated();
                }
            }
            else if (bossHealth.currentHealth < bossHealth.GetMaxHealth() * phase3Threshold)
            {
                if (phase != GebPhase.Phase3)
                {
                    StartGebPhase3();
                }
            }
            else if (bossHealth.currentHealth < bossHealth.GetMaxHealth() * phase2Threshold)
            {
                if (phase != GebPhase.Phase2)
                {
                    StartGebPhase2();
                }
            }
        }
    }

    /// Start Geb's opening cutscene and tell all of the other Geb scripts that the opening cutscene has started.
    public void StartGebOpeningCutscene()
    {
        phase = GebPhase.OpeningCutscene;

        bossController.GebOpeningCutsceneStarted();
        roomController.GebOpeningCutsceneStarted();

        Debug.Log("Opening cutscene started!"); // These lines can be deleted, but not much marks the phase changes yet.
    }

    /// Start phase 1 (do this after the cutscene) and tell all of the other Geb scripts that phase 1 has started.
    public void StartGebBossfight()
    {
        phase = GebPhase.Phase1;

        bossController.GebPhase1Started();
        roomController.GebPhase1Started();

        Debug.Log("Phase 1 started!");
    }

    /// Start phase 2 and tell all of the other Geb scripts that phase 2 has started.
    public void StartGebPhase2()
    {
        phase = GebPhase.Phase2;

        bossController.GebPhase2Started();
        roomController.GebPhase2Started();

        Debug.Log("Phase 2 started!");
    }

    /// Start phase 3 and tell all of the other Geb scripts that phase 3 has started.
    public void StartGebPhase3()
    {
        phase = GebPhase.Phase3;

        bossController.GebPhase3Started();
        roomController.GebPhase3Started();

        Debug.Log("Phase 3 started!");
    }
    
    /// Set the phase to closing cutscene and tell all of the other Geb scripts that the cutscene is playing.
    public void TriggerGebDefeated()
    {
        phase = GebPhase.ClosingCutscene;

        bossController.GebClosingCutsceneStarted();
        roomController.GebClosingCutsceneStarted();

        Debug.Log("Boss defeated! Closing cutscene started!");
    }

    /// Set the phase to defeated and tell all of the other Geb scripts that the closing cutscene is over.
    public void ClosingCutsceneEnded()
    {
        phase = GebPhase.Defeated;

        bossController.GebDefeated();
        roomController.GebDefeated();

        Debug.Log("Closing cutscene over!");
    }
}
