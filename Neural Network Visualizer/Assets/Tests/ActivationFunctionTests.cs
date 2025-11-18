using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using static System.Math;

public class ActivationFunctionTests
{
    private const double EPSILON = 1e-6;

    #region ReLU Tests
    
    [Test]
    public void ReLU_PositiveInput_ReturnsInput()
    {
        var relu = new Activation.ReLU();
        double[] inputs = { 5.0, 3.2, 0.1 };
        
        Assert.AreEqual(5.0, relu.Activate(inputs, 0), EPSILON);
        Assert.AreEqual(3.2, relu.Activate(inputs, 1), EPSILON);
        Assert.AreEqual(0.1, relu.Activate(inputs, 2), EPSILON);
    }
    
    [Test]
    public void ReLU_NegativeInput_ReturnsZero()
    {
        var relu = new Activation.ReLU();
        double[] inputs = { -5.0, -3.2, -0.1 };
        
        Assert.AreEqual(0.0, relu.Activate(inputs, 0), EPSILON);
        Assert.AreEqual(0.0, relu.Activate(inputs, 1), EPSILON);
        Assert.AreEqual(0.0, relu.Activate(inputs, 2), EPSILON);
    }
    
    [Test]
    public void ReLU_ZeroInput_ReturnsZero()
    {
        var relu = new Activation.ReLU();
        double[] inputs = { 0.0 };
        
        Assert.AreEqual(0.0, relu.Activate(inputs, 0), EPSILON);
    }
    
    [Test]
    public void ReLU_Derivative_PositiveInput_ReturnsOne()
    {
        var relu = new Activation.ReLU();
        double[] inputs = { 5.0, 3.2, 0.1 };
        
        Assert.AreEqual(1.0, relu.Derivative(inputs, 0), EPSILON);
        Assert.AreEqual(1.0, relu.Derivative(inputs, 1), EPSILON);
        Assert.AreEqual(1.0, relu.Derivative(inputs, 2), EPSILON);
    }
    
    [Test]
    public void ReLU_Derivative_NegativeInput_ReturnsZero()
    {
        var relu = new Activation.ReLU();
        double[] inputs = { -5.0, -3.2, -0.1 };
        
        Assert.AreEqual(0.0, relu.Derivative(inputs, 0), EPSILON);
        Assert.AreEqual(0.0, relu.Derivative(inputs, 1), EPSILON);
        Assert.AreEqual(0.0, relu.Derivative(inputs, 2), EPSILON);
    }
    
    #endregion
    
    #region Sigmoid Tests
    
    [Test]
    public void Sigmoid_ZeroInput_ReturnsHalf()
    {
        var sigmoid = new Activation.Sigmoid();
        double[] inputs = { 0.0 };
        
        Assert.AreEqual(0.5, sigmoid.Activate(inputs, 0), EPSILON);
    }
    
    [Test]
    public void Sigmoid_PositiveInput_ReturnsBetweenHalfAndOne()
    {
        var sigmoid = new Activation.Sigmoid();
        double[] inputs = { 1.0, 2.0, 5.0 };
        
        double result1 = sigmoid.Activate(inputs, 0);
        double result2 = sigmoid.Activate(inputs, 1);
        double result3 = sigmoid.Activate(inputs, 2);
        
        Assert.Greater(result1, 0.5);
        Assert.Less(result1, 1.0);
        Assert.Greater(result2, result1); // Larger input = larger output
        Assert.Greater(result3, result2);
    }
    
    [Test]
    public void Sigmoid_NegativeInput_ReturnsBetweenZeroAndHalf()
    {
        var sigmoid = new Activation.Sigmoid();
        double[] inputs = { -1.0, -2.0, -5.0 };
        
        double result1 = sigmoid.Activate(inputs, 0);
        double result2 = sigmoid.Activate(inputs, 1);
        double result3 = sigmoid.Activate(inputs, 2);
        
        Assert.Greater(result1, 0.0);
        Assert.Less(result1, 0.5);
        Assert.Less(result2, result1); // More negative = smaller output
        Assert.Less(result3, result2);
    }
    
    [Test]
    public void Sigmoid_KnownValues_ReturnsCorrectResults()
    {
        var sigmoid = new Activation.Sigmoid();
        double[] inputs = { 0.0, 2.0 };
        
        // sigmoid(0) = 0.5
        Assert.AreEqual(0.5, sigmoid.Activate(inputs, 0), EPSILON);
        
        // sigmoid(2) ≈ 0.8807970779778823
        Assert.AreEqual(0.8807970779778823, sigmoid.Activate(inputs, 1), EPSILON);
    }
    
    [Test]
    public void Sigmoid_Derivative_ZeroInput_ReturnsQuarter()
    {
        var sigmoid = new Activation.Sigmoid();
        double[] inputs = { 0.0 };
        
        // Derivative at 0 is sigmoid(0) * (1 - sigmoid(0)) = 0.5 * 0.5 = 0.25
        Assert.AreEqual(0.25, sigmoid.Derivative(inputs, 0), EPSILON);
    }
    
    [Test]
    public void Sigmoid_Derivative_MatchesFormula()
    {
        var sigmoid = new Activation.Sigmoid();
        double[] inputs = { 1.0, -2.0 };
        
        for (int i = 0; i < inputs.Length; i++)
        {
            double a = sigmoid.Activate(inputs, i);
            double expectedDerivative = a * (1 - a);
            double actualDerivative = sigmoid.Derivative(inputs, i);
            
            Assert.AreEqual(expectedDerivative, actualDerivative, EPSILON);
        }
    }
    
    #endregion
    
    #region Softmax Tests
    
    [Test]
    public void Softmax_OutputsSumToOne()
    {
        var softmax = new Activation.Softmax();
        double[] inputs = { 1.0, 2.0, 3.0, 4.0 };
        
        double sum = 0.0;
        for (int i = 0; i < inputs.Length; i++)
        {
            sum += softmax.Activate(inputs, i);
        }
        
        Assert.AreEqual(1.0, sum, EPSILON);
    }
    
    [Test]
    public void Softmax_EqualInputs_ReturnsEqualProbabilities()
    {
        var softmax = new Activation.Softmax();
        double[] inputs = { 2.0, 2.0, 2.0, 2.0 };
        
        double expected = 0.25; // 1/4
        for (int i = 0; i < inputs.Length; i++)
        {
            Assert.AreEqual(expected, softmax.Activate(inputs, i), EPSILON);
        }
    }
    
    [Test]
    public void Softmax_LargerInputGivesLargerOutput()
    {
        var softmax = new Activation.Softmax();
        double[] inputs = { 1.0, 2.0, 3.0 };
        
        double output1 = softmax.Activate(inputs, 0);
        double output2 = softmax.Activate(inputs, 1);
        double output3 = softmax.Activate(inputs, 2);
        
        Assert.Less(output1, output2);
        Assert.Less(output2, output3);
    }
    
    // [Test]
    // public void Softmax_HandlesLargeValues_NumericalStability()
    // {
    //     var softmax = new Activation.Softmax();
    //     double[] inputs = { 1000.0, 1001.0, 1002.0 };
        
    //     // Should not overflow or return NaN/Infinity
    //     double sum = 0.0;
    //     for (int i = 0; i < inputs.Length; i++)
    //     {
    //         double output = softmax.Activate(inputs, i);
    //         Assert.IsFalse(double.IsNaN(output), $"Output at index {i} is NaN");
    //         Assert.IsFalse(double.IsInfinity(output), $"Output at index {i} is Infinity");
    //         sum += output;
    //     }
        
    //     Assert.AreEqual(1.0, sum, EPSILON);
    // }
    
    [Test]
    public void Softmax_KnownValues_ReturnsCorrectResults()
    {
        var softmax = new Activation.Softmax();
        double[] inputs = { 1.0, 2.0, 3.0 };
        
        // Manual calculation:
        // exp(1) ≈ 2.718, exp(2) ≈ 7.389, exp(3) ≈ 20.085
        // sum ≈ 30.192
        double exp1 = System.Math.Exp(1.0);
        double exp2 = System.Math.Exp(2.0);
        double exp3 = System.Math.Exp(3.0);
        double sum = exp1 + exp2 + exp3;
        
        Assert.AreEqual(exp1 / sum, softmax.Activate(inputs, 0), EPSILON);
        Assert.AreEqual(exp2 / sum, softmax.Activate(inputs, 1), EPSILON);
        Assert.AreEqual(exp3 / sum, softmax.Activate(inputs, 2), EPSILON);
    }
    
    [Test]
    public void Softmax_Derivative_ReturnsPositiveValues()
    {
        var softmax = new Activation.Softmax();
        double[] inputs = { 1.0, 2.0, 3.0 };
        
        for (int i = 0; i < inputs.Length; i++)
        {
            double derivative = softmax.Derivative(inputs, i);
            Assert.Greater(derivative, 0.0);
        }
    }
    
    #endregion
    
    #region Factory Tests
    
    // [Test]
    // public void GetActivationType_ReLU_ReturnsReLUInstance()
    // {
    //     var activation = Activation.GetActivationType(Activation.ActivationType.ReLU);
    //     Assert.IsInstanceOf<Activation.ReLU>(activation);
    // }
    
    // [Test]
    // public void GetActivationType_Sigmoid_ReturnsSigmoidInstance()
    // {
    //     var activation = Activation.GetActivationType(Activation.ActivationType.Sigmoid);
    //     Assert.IsInstanceOf<Activation.Sigmoid>(activation);
    // }
    
    // [Test]
    // public void GetActivationType_Softmax_ReturnsSoftmaxInstance()
    // {
    //     var activation = Activation.GetActivationType(Activation.ActivationType.Softmax);
    //     Assert.IsInstanceOf<Activation.Softmax>(activation);
    // }
    
    #endregion
    
    #region Edge Case Tests
    
    [Test]
    public void ReLU_ExtremelyLargePositive_HandlesCorrectly()
    {
        var relu = new Activation.ReLU();
        double[] inputs = { 1e10, 1e100 };
        
        Assert.AreEqual(1e10, relu.Activate(inputs, 0), EPSILON);
        Assert.AreEqual(1e100, relu.Activate(inputs, 1));
    }
    
    [Test]
    public void ReLU_ExtremelyLargeNegative_ReturnsZero()
    {
        var relu = new Activation.ReLU();
        double[] inputs = { -1e10, -1e100 };
        
        Assert.AreEqual(0.0, relu.Activate(inputs, 0), EPSILON);
        Assert.AreEqual(0.0, relu.Activate(inputs, 1), EPSILON);
    }
    
    [Test]
    public void Sigmoid_ExtremelyLargePositive_ApproachesOne()
    {
        var sigmoid = new Activation.Sigmoid();
        double[] inputs = { 100.0 };
        
        double result = sigmoid.Activate(inputs, 0);
        Assert.Greater(result, 0.999);
        Assert.LessOrEqual(result, 1.0);
    }
    
    [Test]
    public void Sigmoid_ExtremelyLargeNegative_ApproachesZero()
    {
        var sigmoid = new Activation.Sigmoid();
        double[] inputs = { -100.0 };
        
        double result = sigmoid.Activate(inputs, 0);
        Assert.Less(result, 0.001);
        Assert.GreaterOrEqual(result, 0.0);
    }
    
    [Test]
    public void Softmax_NegativeInputs_WorksCorrectly()
    {
        var softmax = new Activation.Softmax();
        double[] inputs = { -1.0, -2.0, -3.0 };
        
        double sum = 0.0;
        for (int i = 0; i < inputs.Length; i++)
        {
            sum += softmax.Activate(inputs, i);
        }
        
        Assert.AreEqual(1.0, sum, EPSILON);
    }
    
    #endregion
}

#region Layer Tests

[TestFixture]
public class LayerTests
{
    private System.Random rng;
    private const double EPSILON = 1e-6;

    [SetUp]
    public void Setup()
    {
        rng = new System.Random(42); // Fixed seed for reproducibility
    }

    [Test]
    public void Layer_Constructor_InitializesCorrectDimensions()
    {
        var layer = new Layer(3, 5, rng);

        Assert.AreEqual(3, layer.numNodesIn);
        Assert.AreEqual(5, layer.numNodesOut);
        Assert.AreEqual(15, layer.weights.Length); // 3 * 5
        Assert.AreEqual(5, layer.biases.Length);
        Assert.AreEqual(15, layer.costGradientW.Length);
        Assert.AreEqual(5, layer.costGradientB.Length);
    }

    [Test]
    public void Layer_Constructor_InitializesWeightsWithRandomValues()
    {
        var layer = new Layer(3, 5, rng);

        // Check that weights are initialized (not all zeros)
        bool hasNonZeroWeight = false;
        foreach (var weight in layer.weights)
        {
            if (weight != 0)
            {
                hasNonZeroWeight = true;
                break;
            }
        }
        Assert.IsTrue(hasNonZeroWeight);
    }

    [Test]
    public void Layer_Constructor_InitializesBiasesToZero()
    {
        var layer = new Layer(3, 5, rng);

        foreach (var bias in layer.biases)
        {
            Assert.AreEqual(0, bias);
        }
    }

    [Test]
    public void Layer_CalculateOutputs_ProducesCorrectShape()
    {
        var layer = new Layer(3, 5, rng);
        var inputs = new double[] { 0.5, 0.3, 0.8 };

        var outputs = layer.CalculateOutputs(inputs);

        Assert.AreEqual(5, outputs.Length);
    }

    [Test]
    public void Layer_CalculateOutputs_WithLearnData_StoresIntermediateValues()
    {
        var layer = new Layer(3, 5, rng);
        var inputs = new double[] { 0.5, 0.3, 0.8 };
        var learnData = new LayerLearnData(layer);

        var outputs = layer.CalculateOutputs(inputs, learnData);

        Assert.AreEqual(inputs, learnData.inputs);
        Assert.AreEqual(5, learnData.weightedInputs.Length);
        Assert.AreEqual(5, learnData.activations.Length);
        Assert.AreEqual(outputs, learnData.activations);
    }

    [Test]
    public void Layer_CalculateOutputs_WithZeroInputs_ProducesBiasedOutputs()
    {
        var layer = new Layer(3, 2, rng);
        layer.biases[0] = 1.0;
        layer.biases[1] = -0.5;
        var inputs = new double[] { 0, 0, 0 };

        var outputs = layer.CalculateOutputs(inputs);

        // With zero inputs, output should be activation(bias)
        // Using sigmoid activation by default
        Assert.Greater(outputs[0], 0.7); // sigmoid(1.0) ≈ 0.73
        Assert.Less(outputs[1], 0.4);    // sigmoid(-0.5) ≈ 0.38
    }

    [Test]
    public void Layer_GetWeight_ReturnsCorrectWeight()
    {
        var layer = new Layer(3, 2, rng);
        
        // Manually set a weight
        int flatIndex = layer.GetFlatWeightIndex(1, 0);
        layer.weights[flatIndex] = 0.75;

        var weight = layer.GetWeight(1, 0);

        Assert.AreEqual(0.75, weight, EPSILON);
    }

    [Test]
    public void Layer_GetFlatWeightIndex_CalculatesCorrectIndex()
    {
        var layer = new Layer(3, 2, rng);

        // For 3 inputs and 2 outputs, weights are stored as:
        // [out0_in0, out0_in1, out0_in2, out1_in0, out1_in1, out1_in2]
        Assert.AreEqual(0, layer.GetFlatWeightIndex(0, 0));
        Assert.AreEqual(1, layer.GetFlatWeightIndex(1, 0));
        Assert.AreEqual(2, layer.GetFlatWeightIndex(2, 0));
        Assert.AreEqual(3, layer.GetFlatWeightIndex(0, 1));
        Assert.AreEqual(4, layer.GetFlatWeightIndex(1, 1));
        Assert.AreEqual(5, layer.GetFlatWeightIndex(2, 1));
    }

    [Test]
    public void Layer_UpdateGradients_AccumulatesGradients()
    {
        var layer = new Layer(2, 2, rng);
        var learnData = new LayerLearnData(layer);
        learnData.inputs = new double[] { 0.5, 0.3 };
        learnData.nodeValues = new double[] { 0.1, 0.2 };

        layer.UpdateGradients(learnData);

        // Check that gradients are non-zero
        bool hasNonZeroWeightGradient = false;
        foreach (var grad in layer.costGradientW)
        {
            if (grad != 0)
            {
                hasNonZeroWeightGradient = true;
                break;
            }
        }
        Assert.IsTrue(hasNonZeroWeightGradient);

        bool hasNonZeroBiasGradient = false;
        foreach (var grad in layer.costGradientB)
        {
            if (grad != 0)
            {
                hasNonZeroBiasGradient = true;
                break;
            }
        }
        Assert.IsTrue(hasNonZeroBiasGradient);
    }

    [Test]
    public void Layer_UpdateGradients_AccumulatesMultipleCalls()
    {
        var layer = new Layer(2, 1, rng);
        var learnData = new LayerLearnData(layer);
        learnData.inputs = new double[] { 1.0, 1.0 };
        learnData.nodeValues = new double[] { 0.1 };

        layer.UpdateGradients(learnData);
        var firstGradient = layer.costGradientW[0];

        layer.UpdateGradients(learnData);
        var secondGradient = layer.costGradientW[0];

        // Second gradient should be double the first (accumulated)
        Assert.AreEqual(firstGradient * 2, secondGradient, EPSILON);
    }

    [Test]
    public void Layer_ApplyGradients_UpdatesWeights()
    {
        var layer = new Layer(2, 1, rng);
        var originalWeight = layer.weights[0];
        
        // Set up a gradient
        layer.costGradientW[0] = 0.1;

        layer.ApplyGradients(learnRate: 0.1, regularization: 0, momentum: 0);

        Assert.AreNotEqual(originalWeight, layer.weights[0]);
    }

    [Test]
    public void Layer_ApplyGradients_ResetsGradients()
    {
        var layer = new Layer(2, 1, rng);
        layer.costGradientW[0] = 0.5;
        layer.costGradientB[0] = 0.3;

        layer.ApplyGradients(learnRate: 0.1, regularization: 0, momentum: 0);

        Assert.AreEqual(0, layer.costGradientW[0]);
        Assert.AreEqual(0, layer.costGradientB[0]);
    }

    [Test]
    public void Layer_ApplyGradients_WithMomentum_UsesVelocity()
    {
        var layer = new Layer(2, 1, rng);
        layer.costGradientW[0] = 0.1;

        // Apply gradients with momentum
        layer.ApplyGradients(learnRate: 0.1, regularization: 0, momentum: 0.9);

        var firstVelocity = layer.weightVelocities[0];
        Assert.AreNotEqual(0, firstVelocity);

        // Apply again with same gradient
        layer.costGradientW[0] = 0.1;
        layer.ApplyGradients(learnRate: 0.1, regularization: 0, momentum: 0.9);

        var secondVelocity = layer.weightVelocities[0];
        
        // Velocity should have increased due to momentum
        Assert.Greater(Abs(secondVelocity), Abs(firstVelocity));
    }

    [Test]
    public void Layer_ApplyGradients_WithRegularization_DecaysWeights()
    {
        var layer = new Layer(2, 1, rng);
        layer.weights[0] = 1.0;
        layer.costGradientW[0] = 0; // No gradient, only regularization effect

        layer.ApplyGradients(learnRate: 0.1, regularization: 0.01, momentum: 0);

        // Weight should decay: 1.0 * (1 - 0.01 * 0.1) = 0.999
        Assert.Less(layer.weights[0], 1.0);
        Assert.AreEqual(0.999, layer.weights[0], EPSILON);
    }

    [Test]
    public void Layer_CalculateOutputLayerNodeValues_ComputesCorrectDerivatives()
    {
        var layer = new Layer(2, 2, rng);
        var learnData = new LayerLearnData(layer);
        learnData.activations = new double[] { 0.7, 0.3 };
        learnData.weightedInputs = new double[] { 0.5, -0.5 };
        
        var expectedOutputs = new double[] { 1.0, 0.0 };
        var cost = new Cost.MeanSquaredError();

        layer.CalculateOutputLayerNodeValues(learnData, expectedOutputs, cost);

        // Node values should be non-zero (they represent error * activation derivative)
        Assert.AreNotEqual(0, learnData.nodeValues[0]);
        Assert.AreNotEqual(0, learnData.nodeValues[1]);
    }

    [Test]
    public void Layer_CalculateHiddenLayerNodeValues_PropagatesErrorBackward()
    {
        var hiddenLayer = new Layer(3, 2, rng);
        var outputLayer = new Layer(2, 2, rng);
        
        var hiddenLearnData = new LayerLearnData(hiddenLayer);
        hiddenLearnData.weightedInputs = new double[] { 0.5, -0.5 };
        
        var oldNodeValues = new double[] { 0.1, 0.2 };

        hiddenLayer.CalculateHiddenLayerNodeValues(hiddenLearnData, outputLayer, oldNodeValues);

        // Node values should be computed (non-zero if weights are non-zero)
        Assert.IsNotNull(hiddenLearnData.nodeValues);
        Assert.AreEqual(2, hiddenLearnData.nodeValues.Length);
    }

    [Test]
    public void Layer_SetActivationFunction_ChangesActivation()
    {
        var layer = new Layer(2, 1, rng);
        var newActivation = new Activation.ReLU();

        layer.SetActivationFunction(newActivation);

        Assert.AreEqual(newActivation, layer.activation);
    }

    [Test]
    public void LayerLearnData_Constructor_InitializesCorrectSizes()
    {
        var layer = new Layer(3, 5, rng);
        var learnData = new LayerLearnData(layer);

        Assert.AreEqual(5, learnData.weightedInputs.Length);
        Assert.AreEqual(5, learnData.activations.Length);
        Assert.AreEqual(5, learnData.nodeValues.Length);
    }

    [Test]
    public void Layer_InitializeRandomWeights_UsesNormalDistribution()
    {
        var layer1 = new Layer(10, 10, new System.Random(1));
        var layer2 = new Layer(10, 10, new System.Random(2));

        // Weights should be different with different seeds
        Assert.AreNotEqual(layer1.weights[0], layer2.weights[0]);
        
        // Weights should be scaled by 1/sqrt(numNodesIn)
        // For 10 inputs, expected standard deviation is 1/sqrt(10) ≈ 0.316
        double sum = 0;
        foreach (var weight in layer1.weights)
        {
            sum += weight * weight;
        }
        double variance = sum / layer1.weights.Length;
        double stdDev = Sqrt(variance);
        
        // Should be roughly 1/sqrt(10)
        Assert.Less(Abs(stdDev - 0.316), 0.15);
    }

    [Test]
    public void Layer_CalculateOutputs_ProducesValidActivations()
    {
        var layer = new Layer(3, 2, rng);
        var inputs = new double[] { 0.5, 0.3, 0.8 };

        var outputs = layer.CalculateOutputs(inputs);

        // Sigmoid outputs should be between 0 and 1
        foreach (var output in outputs)
        {
            Assert.GreaterOrEqual(output, 0.0);
            Assert.LessOrEqual(output, 1.0);
        }
    }
}

#endregion