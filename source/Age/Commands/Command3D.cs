using Age.Numerics;
using Age.Scenes;

namespace Age.Commands;

public abstract record Command3D : Command<Spatial<Command3D, Matrix4x4<float>>>;
