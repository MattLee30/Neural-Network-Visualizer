using static System.Math;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Diagnostics;
using System.Collections.Generic;


public class Layer : MonoBehaviour
{
    public readonly int numNodesIn;
    public readonly int numNodesOut;

    public readonly double[] weights;
    public readonly double[] biases;

    public readonly double[] costGradientW;
    public readonly double[] costGradientB;

    public readonly double[] weightVelocities;
    public readonly double[] biasVelocities;

    public IActivation activation;

    // Slider reference and stored number
    [SerializeField] private Slider slider;
    private double storedNumber;

    public Layer(int numNodesIn, int numNodesOut, System.Random rng)
    {
        this.numNodesIn = numNodesIn;
        this.numNodesOut = numNodesOut;
        activation = new Activation.Sigmoid();

        weights = new double[numNodesIn * numNodesOut];
        costGradientW = new double[weights.Length];
        biases = new double[numNodesOut];
        costGradientB = new double[biases.Length];

        weightVelocities = new double[weights.Length];
        biasVelocities = new double[biases.Length];

        InitializeRandomWeights(rng);
    }

    // Static counter for sequential naming
    private static Dictionary<string, int> instanceCounts = new Dictionary<string, int>();

    void Awake()
    {
        // Set custom name on instantiation
        SetCustomName();
    }

    private void SetCustomName()
    {
        // Get the original prefab name (remove "(Clone)" if present)
        string baseName = gameObject.name.Replace("(Clone)", "").Trim();
        
        // Increment counter for this base name
        if (!instanceCounts.ContainsKey(baseName))
        {
            instanceCounts[baseName] = 0;
        }
        instanceCounts[baseName]++;
        
        // Set the new name
        gameObject.name = $"{baseName}_{instanceCounts[baseName]}";
        
        UnityEngine.Debug.Log($"Created {gameObject.name}");
    }

    void Start()
    {
        // Auto-assign slider if not set
        if (slider == null)
        {
            GameObject sliderObject = GameObject.FindGameObjectWithTag("layersize");
            if (sliderObject != null)
            {
                slider = sliderObject.GetComponent<Slider>();
                if (slider != null)
                {
                    UnityEngine.Debug.Log($"Auto-assigned slider from object with tag 'layersize' to {gameObject.name}");
                }
                else
                {
                    UnityEngine.Debug.LogWarning($"Object with tag 'layersize' found but has no Slider component!");
                }
            }
            else
            {
                UnityEngine.Debug.LogWarning($"No GameObject with tag 'layersize' found for {gameObject.name}");
            }
        }
        
        // Add listener to capture slider changes
        if (slider != null)
        {
            slider.onValueChanged.AddListener(OnSliderValueChanged);
        }
    }

    private void OnSliderValueChanged(float value)
    {
        // Store the slider value as a double
        storedNumber = value;
        UnityEngine.Debug.Log($"Number stored in {gameObject.name}: {storedNumber}");
    }

    // Public method to get the stored number
    public double GetStoredNumber()
    {
        return storedNumber;
    }

    // Public method to set the Slider reference (if creating layers dynamically)
    public void SetSlider(Slider newSlider)
    {
        // Remove old listener if exists
        if (slider != null)
        {
            slider.onValueChanged.RemoveListener(OnSliderValueChanged);
        }

        slider = newSlider;

        // Add new listener
        if (slider != null)
        {
            slider.onValueChanged.AddListener(OnSliderValueChanged);
        }
    }

    // Public method to set the stored number programmatically
    public void SetStoredNumber(double number)
    {
        storedNumber = number;

        // Optionally update the slider if it exists
        if (slider != null)
        {
            slider.value = (float)number;
        }

        UnityEngine.Debug.Log($"Number set in {gameObject.name}: {storedNumber}");
    }

    public double[] CalculateOutputs(double[] inputs)
    {
        double[] weightedInputs = new double[numNodesOut];

        for (int nodeOut = 0; nodeOut < numNodesOut; nodeOut++)
        {
            double weightedInput = biases[nodeOut];

            for (int nodeIn = 0; nodeIn < numNodesIn; nodeIn++)
            {
                weightedInput += inputs[nodeIn] * GetWeight(nodeIn, nodeOut);
            }
            weightedInputs[nodeOut] = weightedInput;
        }

        double[] activations = new double[numNodesOut];
        for (int outputNode = 0; outputNode < numNodesOut; outputNode++)
        {
            activations[outputNode] = activation.Activate(weightedInputs, outputNode);
        }

        return activations;
    }

    public double[] CalculateOutputs(double[] inputs, LayerLearnData learnData)
    {
        learnData.inputs = inputs;

        for (int nodeOut = 0; nodeOut < numNodesOut; nodeOut++)
        {
            double weightedInput = biases[nodeOut];
            for (int nodeIn = 0; nodeIn < numNodesIn; nodeIn++)
            {
                weightedInput += inputs[nodeIn] * GetWeight(nodeIn, nodeOut);
            }
            learnData.weightedInputs[nodeOut] = weightedInput;
        }

        for (int i = 0; i < learnData.activations.Length; i++)
        {
            learnData.activations[i] = activation.Activate(learnData.weightedInputs, i);
        }

        return learnData.activations;
    }


    public void ApplyGradients(double learnRate, double regularization, double momentum)
    {
        double weightDecay = (1 - regularization * learnRate);

        for (int i = 0; i < weights.Length; i++)
        {
            double weight = weights[i];
            double velocity = weightVelocities[i] * momentum - costGradientW[i] * learnRate;
            weightVelocities[i] = velocity;
            weights[i] = weight * weightDecay + velocity;
            costGradientW[i] = 0;
        }


        for (int i = 0; i < biases.Length; i++)
        {
            double velocity = biasVelocities[i] * momentum - costGradientB[i] * learnRate;
            biasVelocities[i] = velocity;
            biases[i] += velocity;
            costGradientB[i] = 0;
        }
    }

    public void CalculateOutputLayerNodeValues(LayerLearnData layerLearnData, double[] expectedOutputs, ICost cost)
    {
        for (int i = 0; i < layerLearnData.nodeValues.Length; i++)
        {
            double costDerivative = cost.CostDerivative(layerLearnData.activations[i], expectedOutputs[i]);
            double activationDerivative = activation.Derivative(layerLearnData.weightedInputs, i);
            layerLearnData.nodeValues[i] = costDerivative * activationDerivative;
        }
    }

    public void CalculateHiddenLayerNodeValues(LayerLearnData layerLearnData, Layer oldLayer, double[] oldNodeValues)
    {
        for (int newNodeIndex = 0; newNodeIndex < numNodesOut; newNodeIndex++)
        {
            double newNodeValue = 0;
            for (int oldNodeIndex = 0; oldNodeIndex < oldNodeValues.Length; oldNodeIndex++)
            {
                double weightedInputDerivative = oldLayer.GetWeight(newNodeIndex, oldNodeIndex);
                newNodeValue += weightedInputDerivative * oldNodeValues[oldNodeIndex];
            }
            newNodeValue *= activation.Derivative(layerLearnData.weightedInputs, newNodeIndex);
            layerLearnData.nodeValues[newNodeIndex] = newNodeValue;
        }

    }

    public void UpdateGradients(LayerLearnData layerLearnData)
    {
        lock (costGradientW)
        {
            for (int nodeOut = 0; nodeOut < numNodesOut; nodeOut++)
            {
                double nodeValue = layerLearnData.nodeValues[nodeOut];
                for (int nodeIn = 0; nodeIn < numNodesIn; nodeIn++)
                {
                    double derivativeCostWrtWeight = layerLearnData.inputs[nodeIn] * nodeValue;

                    costGradientW[GetFlatWeightIndex(nodeIn, nodeOut)] += derivativeCostWrtWeight;
                }
            }
        }

        lock (costGradientB)
        {
            for (int nodeOut = 0; nodeOut < numNodesOut; nodeOut++)
            {
                double derivativeCostWrtBias = 1 * layerLearnData.nodeValues[nodeOut];
                costGradientB[nodeOut] += derivativeCostWrtBias;
            }
        }
    }

    public double GetWeight(int nodeIn, int nodeOut)
    {
        int flatIndex = nodeOut * numNodesIn + nodeIn;
        return weights[flatIndex];
    }

    public int GetFlatWeightIndex(int inputNeuronIndex, int outputNeuronIndex)
    {
        return outputNeuronIndex * numNodesIn + inputNeuronIndex;
    }

    public void SetActivationFunction(IActivation activation)
    {
        this.activation = activation;
    }

    public void InitializeRandomWeights(System.Random rng)
    {
        for (int i = 0; i < weights.Length; i++)
        {
            weights[i] = RandomInNormalDistribution(rng, 0, 1) / Sqrt(numNodesIn);
        }

        double RandomInNormalDistribution(System.Random rng, double mean, double standardDeviation)
        {
            double x1 = 1 - rng.NextDouble();
            double x2 = 1 - rng.NextDouble();

            double y1 = Sqrt(-2.0 * Log(x1)) * Cos(2.0 * PI * x2);
            return y1 * standardDeviation + mean;
        }
    }

    void OnDestroy()
    {
        // Clean up listener
        if (slider != null)
        {
            slider.onValueChanged.RemoveListener(OnSliderValueChanged);
        }
    }
}