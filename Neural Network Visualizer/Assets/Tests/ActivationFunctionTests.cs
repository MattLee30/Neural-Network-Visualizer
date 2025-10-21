using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

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
    
    [Test]
    public void Softmax_HandlesLargeValues_NumericalStability()
    {
        var softmax = new Activation.Softmax();
        double[] inputs = { 1000.0, 1001.0, 1002.0 };
        
        // Should not overflow or return NaN/Infinity
        double sum = 0.0;
        for (int i = 0; i < inputs.Length; i++)
        {
            double output = softmax.Activate(inputs, i);
            Assert.IsFalse(double.IsNaN(output), $"Output at index {i} is NaN");
            Assert.IsFalse(double.IsInfinity(output), $"Output at index {i} is Infinity");
            sum += output;
        }
        
        Assert.AreEqual(1.0, sum, EPSILON);
    }
    
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
    
    [Test]
    public void GetActivationType_ReLU_ReturnsReLUInstance()
    {
        var activation = Activation.GetActivationType(Activation.ActivationType.ReLU);
        Assert.IsInstanceOf<Activation.ReLU>(activation);
    }
    
    [Test]
    public void GetActivationType_Sigmoid_ReturnsSigmoidInstance()
    {
        var activation = Activation.GetActivationType(Activation.ActivationType.Sigmoid);
        Assert.IsInstanceOf<Activation.Sigmoid>(activation);
    }
    
    [Test]
    public void GetActivationType_Softmax_ReturnsSoftmaxInstance()
    {
        var activation = Activation.GetActivationType(Activation.ActivationType.Softmax);
        Assert.IsInstanceOf<Activation.Softmax>(activation);
    }
    
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