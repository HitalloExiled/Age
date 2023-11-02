namespace Age.Loaders.Wavefront;

public readonly record struct Line(int Start, int End, int Group = -1);
