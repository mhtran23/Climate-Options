using UnityEngine;

public class ResetConfig : MonoBehaviour
{
    ProgramManager programManager;

    private void Awake()
    {
        programManager = FindObjectOfType<ProgramManager>();
    }

    public void Activate()
    {
        programManager.ResetClimateControlSystemConfig();
    }
}
