namespace Senko.Arguments
{
    public readonly struct RemainingString
    {
        public RemainingString(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public bool Equals(RemainingString other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            return obj is RemainingString other && Equals(other);
        }

        public override int GetHashCode()
        {
            return (Value != null ? Value.GetHashCode() : 0);
        }
        
        public static implicit operator string(RemainingString value)
        {
            return value.Value;
        }
        
        public static implicit operator RemainingString(string value)
        {
            return new RemainingString(value);
        }
    }
}