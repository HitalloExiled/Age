using System.Diagnostics;
using System.Numerics;

namespace Age.Core.Math;

[DebuggerDisplay("X: {X}, Y: {Y}")]
public record struct Vector2<T>(T X, T Y) where T : IFloatingPoint<T>
{ }
