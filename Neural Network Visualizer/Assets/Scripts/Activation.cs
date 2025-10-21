//Credit to SebLague on YouTube for the coding framework!
using System.Diagnostics;
using static System.Math;


public class ReLUActivation
{
    public enum ActivationType
    {
        Sigmoid,
        ReLU
    }

    public readonly struct Sigmoid :IActivation
    {
        public ActivationType GetActivationType()
        {
            return ActivationType.Sigmoid;
        }
    }

    public readonly struct ReLU()
    {
        public ActivationType GetActivationType()
        {
            return ActivationType.Sigmoid;
        }
    }
}
