using UnityEngine;
using System.Collections;

public class CameraSwitcher : MonoBehaviour
{
    public Camera overheadCamera;
    public Camera agentCamera;
    public float overheadViewDuration = 3.0f; // Duration for overhead view



    private bool agentCanStart = false; // Flag to control agent's start

    void Start()
    {
        StartCoroutine(SwitchCameraAfterDelay());
    }

    public IEnumerator SwitchCameraAfterDelay()
    {
        // Enable overhead camera, disable agent camera
        overheadCamera.enabled = true;
        agentCamera.enabled = false;

        // Wait for the duration of the overhead view
        yield return new WaitForSeconds(overheadViewDuration);

        // Switch to agent camera
        overheadCamera.enabled = false;
        agentCamera.enabled = true;

        // Allow the agent to start
        agentCanStart = true;
    }

    public bool CanAgentStart()
    {
        return agentCanStart;
    }



}



