namespace GoodLuckValley.Gameplay.Player.Motor
{
    /// <summary>
    /// Represents a modification to a player's parameter, including the operation to apply,
    /// the value to modify with, and its application priority.
    /// </summary>
    public readonly struct ParameterModifier
    {
        public readonly PlayerParameter Parameter;
        public readonly ModifierOperation Operation;
        public readonly float Value;
        public readonly int Priority;

        public ParameterModifier(
            PlayerParameter parameter, 
            ModifierOperation operation,
            float value, 
            int priority = 0
        )
        {
            Parameter = parameter;
            Operation = operation;
            Value = value;
            Priority = priority;
        }
    }
}