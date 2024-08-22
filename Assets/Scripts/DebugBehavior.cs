using UnityEngine;
using Unity.MLAgents.Policies;

public class DebugBehaviorParams : MonoBehaviour
{
    public BehaviorParameters behaviorParameters;

    void Start()
    {
        if (behaviorParameters != null)
        {
            // Print the behavior name
            Debug.Log($"Behavior Name: {behaviorParameters.BehaviorName}");

            // Print the fully qualified behavior name (includes team ID)
            Debug.Log($"Fully Qualified Behavior Name: {behaviorParameters.FullyQualifiedBehaviorName}");

            // Print the behavior type
            Debug.Log($"Behavior Type: {behaviorParameters.BehaviorType}");

            // Print team ID
            Debug.Log($"Team ID: {behaviorParameters.TeamId}");

            // Print observable attribute handling
            Debug.Log($"Observable Attribute Handling: {behaviorParameters.ObservableAttributeHandling}");

            // Print if child sensors and actuators are used
            Debug.Log($"Use Child Sensors: {behaviorParameters.UseChildSensors}");
            Debug.Log($"Use Child Actuators: {behaviorParameters.UseChildActuators}");

            // Print model and inference device if available
            if (behaviorParameters.Model != null)
            {
                Debug.Log("Model is set.");
            }
            else
            {
                Debug.Log("Model is not set.");
            }

            Debug.Log($"Inference Device: {behaviorParameters.InferenceDevice}");

            // Check if the agent is in heuristic mode
            Debug.Log($"Is In Heuristic Mode: {behaviorParameters.IsInHeuristicMode()}");
        }
        else
        {
            Debug.LogError("Behavior Parameters component is not assigned.");
        }
    }
}
