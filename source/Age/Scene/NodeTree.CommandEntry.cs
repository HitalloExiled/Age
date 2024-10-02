using System.Runtime.InteropServices;
using Age.Commands;
using Age.Numerics;

namespace Age.Scene;

public sealed partial class NodeTree
{
    // [StructLayout(LayoutKind.Explicit, Pack = 8)]
    public struct Command2DEntry(Command command, Matrix3x2<float> transform)
    {
        // [FieldOffset(0)]
        public Command Command = command;

        // [FieldOffset(8)]
        public Matrix3x2<float> Transform = transform;
    }

    // [StructLayout(LayoutKind.Explicit, Pack = 8)]
    public struct Command3DEntry(Command command, Matrix4x4<float> transform)
    {
        // [FieldOffset(0)]
        public Command Command = command;

        // [FieldOffset(8)]
        public Matrix4x4<float> Transform = transform;
    }

    public enum CommandEntryKind
    {
        Command2DEntry,
        Command3DEntry,
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct CommandEntry
    {
        [FieldOffset(0)]
        private readonly CommandEntryKind kind;

        [FieldOffset(8)]
        private Command2DEntry command2DEntry;

        [FieldOffset(8)]
        private Command3DEntry command3DEntry;

        public CommandEntry(Command2DEntry entry)
        {
            this.kind = CommandEntryKind.Command2DEntry;
            this.command2DEntry = entry;
        }

        public CommandEntry(Command3DEntry entry)
        {
            this.kind = CommandEntryKind.Command3DEntry;
            this.command3DEntry = entry;
        }

        public readonly bool TryGetCommand2DEntry(out Command2DEntry entry)
        {
            if (this.kind == CommandEntryKind.Command2DEntry)
            {
                entry = this.command2DEntry;

                return true;
            }

            entry = default;

            return false;
        }

        public readonly bool TryGetCommand3DEntry(out Command3DEntry entry)
        {
            if (this.kind == CommandEntryKind.Command3DEntry)
            {
                entry = this.command3DEntry;

                return true;
            }

            entry = default;

            return false;
        }

        public static implicit operator CommandEntry(Command2DEntry value) => new(value);
        public static implicit operator CommandEntry(Command3DEntry value) => new(value);


    }
}
