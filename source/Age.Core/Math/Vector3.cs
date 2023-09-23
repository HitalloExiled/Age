using System.Diagnostics;
using System.Numerics;

namespace Age.Core.Math;

[DebuggerDisplay("X: {X}, Y: {Y}, Z: {Z}")]
public record struct Vector3<T>(T X, T Y, T Z) where T : IFloatingPoint<T>
{ }
