
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;

public class ONNXModelRunner : MonoBehaviour
{
    private InferenceSession _session;

    // Path to your ONNX model file
    public string modelFileName = "/Users/wesleyalvarado/ml-agents-training/results/MyFirstRun3/RobotAgent.onnx";

    void Start()    {
        try
        {
            // Determine the model path dynamically
            string modelPath = Path.Combine(Application.streamingAssetsPath, modelFileName);

            // Initialize ONNX Runtime
            var options = new SessionOptions();
            _session = new InferenceSession(modelPath, options);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to initialize ONNX model: {ex.Message}");
        }
    }

    public float[] RunInference(float[] inputData)
    {
        try
        {
            // Ensure the input data has the expected size (e.g., 24).
            int expectedInputSize = 24;

            if (inputData.Length != expectedInputSize)
            {
                Debug.LogWarning($"Adjusting input data size from {inputData.Length} to {expectedInputSize}");
                if (inputData.Length < expectedInputSize)
                {
                    inputData = inputData.Concat(new float[expectedInputSize - inputData.Length]).ToArray();
                }
                else
                {
                    inputData = inputData.Take(expectedInputSize).ToArray();
                }
            }

            // Prepare input tensor
            var inputTensor = new DenseTensor<float>(inputData, new int[] { 1, expectedInputSize });

            // Use the correct input name "input"
            var inputNamedOnnxValue = NamedOnnxValue.CreateFromTensor<float>("input", inputTensor);

            // Run inference
            var inputs = new[] { inputNamedOnnxValue };
            var results = _session.Run(inputs);

            // Process the results
            var resultTensor = results.First().AsTensor<float>();
            var resultData = resultTensor.ToArray();

            // (Optional) Reverse any normalization or scaling if needed
            for (int i = 0; i < resultData.Length; i++)
            {
                resultData[i] *= 3; // Adjust if necessary
            }

            // Log inference result
            Debug.Log("Inference result: " + string.Join(", ", resultData));

            return resultData;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error during ONNX inference: {ex.Message}");
            return null; // or some sensible fallback
        }
    }

    void OnDestroy()
    {
        _session?.Dispose(); // Properly dispose of the session when done
    }
}
