using UnityEngine;
using Unity.InferenceEngine;

public class VGG16Inference : MonoBehaviour
{
    [SerializeField] private ModelAsset vgg16ModelAsset;
    private Model model;
    private Worker worker;
    
    void Start()
    {
        // Load VGG16 model from ONNX file
        model = ModelLoader.Load(vgg16ModelAsset);
        
        // Create worker for inference (use GPU for better performance)
        worker = new Worker(model, BackendType.GPUCompute);
        
        Debug.Log("VGG16 model loaded successfully!");
        Debug.Log($"Model has {model.inputs.Count} inputs and {model.outputs.Count} outputs");
    }
    
    public void RunInference(Texture2D inputTexture)
    {
        // VGG16 expects 224x224x3 input
        Tensor<float> inputTensor = new Tensor<float>(new TensorShape(1, 3, 224, 224));
        
        // Convert texture to tensor
        // Note: You may need to resize the texture to 224x224 first
        TextureConverter.ToTensor(inputTexture, inputTensor);
        
        // Execute the model
        worker.Schedule(inputTensor);
        
        // Get output (VGG16 outputs 1000 class probabilities)
        var outputTensor = worker.PeekOutput() as Tensor<float>;
        
        // Process output - find the class with highest probability
        if (outputTensor != null)
        {
            // Download tensor data to array (blocking call)
            float[] outputData = outputTensor.DownloadToArray();
            
            int maxIndex = 0;
            float maxValue = outputData[0];
            
            for (int i = 1; i < outputData.Length; i++)
            {
                if (outputData[i] > maxValue)
                {
                    maxValue = outputData[i];
                    maxIndex = i;
                }
            }
            
            Debug.Log($"Predicted class: {maxIndex} with confidence: {maxValue}");
        }
        
        // Clean up
        inputTensor.Dispose();
    }
    
    // Alternative: Async version to avoid blocking
    public async void RunInferenceAsync(Texture2D inputTexture)
    {
        // VGG16 expects 224x224x3 input
        Tensor<float> inputTensor = new Tensor<float>(new TensorShape(1, 3, 224, 224));
        
        // Convert texture to tensor
        TextureConverter.ToTensor(inputTexture, inputTensor);
        
        // Execute the model
        worker.Schedule(inputTensor);
        
        // Get output
        var outputTensor = worker.PeekOutput() as Tensor<float>;
        
        if (outputTensor != null)
        {
            // Async readback - non-blocking
            var clonedTensor = await outputTensor.ReadbackAndCloneAsync();
            float[] outputData = clonedTensor.DownloadToArray();
            
            int maxIndex = 0;
            float maxValue = outputData[0];
            
            for (int i = 1; i < outputData.Length; i++)
            {
                if (outputData[i] > maxValue)
                {
                    maxValue = outputData[i];
                    maxIndex = i;
                }
            }
            
            Debug.Log($"Predicted class: {maxIndex} with confidence: {maxValue}");
            
            clonedTensor.Dispose();
        }
        
        // Clean up
        inputTensor.Dispose();
    }
    
    // Alternative: Read directly from CPU tensor if available
    public void RunInferenceDirectRead(Texture2D inputTexture)
    {
        Tensor<float> inputTensor = new Tensor<float>(new TensorShape(1, 3, 224, 224));
        TextureConverter.ToTensor(inputTexture, inputTensor);
        
        worker.Schedule(inputTensor);
        
        var outputTensor = worker.PeekOutput() as Tensor<float>;
        
        if (outputTensor != null)
        {
            // Check if tensor is on CPU and ready to read
            if (outputTensor.backendType == BackendType.CPU && outputTensor.IsReadbackRequestDone())
            {
                // Direct access using span (more efficient)
                var span = outputTensor.AsReadOnlySpan();
                
                int maxIndex = 0;
                float maxValue = span[0];
                
                for (int i = 1; i < span.Length; i++)
                {
                    if (span[i] > maxValue)
                    {
                        maxValue = span[i];
                        maxIndex = i;
                    }
                }
                
                Debug.Log($"Predicted class: {maxIndex} with confidence: {maxValue}");
            }
            else
            {
                // Fall back to download
                float[] outputData = outputTensor.DownloadToArray();
                // ... process as above
            }
        }
        
        inputTensor.Dispose();
    }
    
    void OnDestroy()
    {
        worker?.Dispose();
    }
}