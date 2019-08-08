namespace Senko.Arguments
{
    public struct Argument
    {
        public Argument(ArgumentType type, object value)
        {
            Type = type;
            Value = value;
        }

        public ArgumentType Type { get; }

        public object Value { get; }
    }
}
