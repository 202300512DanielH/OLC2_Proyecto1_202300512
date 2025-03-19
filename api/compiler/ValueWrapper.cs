
public abstract record ValueWrapper;

public record IntValue (int Value) : ValueWrapper;

public record FloatValue (float Value) : ValueWrapper;

public record StringValue (string Value) : ValueWrapper;

public record BoolValue (bool Value) : ValueWrapper;

public record FuntionValue (Invocable invocable, string name) : ValueWrapper;

public record VoidValue : ValueWrapper;