//Credit to SebLague on YouTube for the coding framework!
using static System.Math;

public struct Activation
{
    public enum ActivationType
    {
        ReLU,
        Sigmoid,
        Softmax
    }

    public static IActivation GetActivationType(ActivationType type)
    {
        switch (type)
        {
            case ActivationType.ReLU:
                return new ReLU();
            case ActivationType.Sigmoid:
                return new Sigmoid();
            case ActivationType.Softmax:
                return new Softmax();
            default:
                return new Sigmoid();
        }
    }

    public readonly struct ReLU : IActivation
    {
        public double Activate(double[] inputs, int index)
        {
            return System.Math.Max(0, inputs[index]);
        }
        public double Derivative(double[] inputs, int index)
        {
            return (inputs[index] > 0) ? 1 : 0;
        }
        public ActivationType GetActivationType()
        {
            return ActivationType.ReLU;
        }
    }

    public readonly struct Sigmoid : IActivation
    {
        public double Activate(double[] inputs, int index)
        {
            return 1.0 / (1.0 + System.Math.Exp(-inputs[index]));
        }
        public double Derivative(double[] inputs, int index)
        {
            double a = Activate(inputs, index);
            return a * (1 - a);
        }
        public ActivationType GetActivationType()
        {
            return ActivationType.Sigmoid;
        }
    }
    public readonly struct Softmax: IActivation
    {
        public double Activate(double[] inputs, int index)
        {
            double sum = 0.0;

            for(int i = 0; i < inputs.Length; i++)
            {
                sum += System.Math.Exp(inputs[i]);
            }

            return Exp(inputs[index]) / sum;
        }
        public double Derivative(double[] inputs, int index)
        {
            double sum = 0.0;

            for (int i = 0; i < inputs.Length; i++)
            {
                sum += System.Math.Exp(inputs[i]);
            }

            double expSum = Exp(inputs[index]) / sum;

            return (sum * expSum - expSum * expSum) / (expSum * expSum);
        }
        public ActivationType GetActivationType()
        {
            return ActivationType.Softmax;
        } 
    }
}