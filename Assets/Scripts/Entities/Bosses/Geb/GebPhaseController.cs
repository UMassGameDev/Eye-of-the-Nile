using UnityEngine;

/** \brief
This script handles Geb's phases.
It keeps track of the current phase and detects when a new phase should be started.
When a new phase is started, it tells the other Geb scripts.

Documentation updated 12/28/2024
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
                if (phase != GebPhase.Defeated)
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

    /// Start phase 1 and tell all of the other Geb scripts that phase 1 has started.
    public void StartGebBossfight()
    {
        phase = GebPhase.Phase1;

        bossController.GebBossfightStarted();
        roomController.GebBossfightStarted();

        Debug.Log("Phase 1 started!"); // This line can be deleted, but nothing else marks the phase changes yet.
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

        Debug.Log("Phase 3 started!"); // This line can be deleted, but nothing else marks the phase changes at the moment.
    }
    
    /// Set the phase to defeated and tell all of the other Geb scripts that Geb has been defeated.
    public void TriggerGebDefeated()
    {
        phase = GebPhase.Defeated;

        bossController.GebDefeated();
        roomController.GebDefeated();

        Debug.Log("Boss defeated!");
    }
}
