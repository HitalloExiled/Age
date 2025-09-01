using System.Runtime.CompilerServices;
using Age.Elements;
using Age.Platforms.Display;

namespace Age.Scene;

public sealed partial class RenderTree
{
    private record struct VirtualChild(Element VirtualParent, uint VirtualChildIndex)
    {
        public readonly bool IsConnected
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this.VirtualParent.IsConnected;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void HandleMouseDown(in MouseEvent mouseEvent) =>
            this.VirtualParent.HandleVirtualChildMouseDown(mouseEvent, this.VirtualChildIndex);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void HandleMouseMoved(in MouseEvent mouseEvent) =>
            this.VirtualParent.HandleVirtualChildMouseMoved(mouseEvent, this.VirtualChildIndex);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void HandleMouseOut(in MouseEvent mouseEvent) =>
            this.VirtualParent.HandleVirtualChildMouseOut(mouseEvent, this.VirtualChildIndex);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void HandleMouseOver(in MouseEvent mouseEvent) =>
            this.VirtualParent.HandleVirtualChildMouseOver(mouseEvent, this.VirtualChildIndex);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void HandleMouseRelease(in MouseEvent mouseEvent) =>
            this.VirtualParent.HandleVirtualChildMouseRelease(mouseEvent, this.VirtualChildIndex);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void HandleMouseUp(in MouseEvent mouseEvent) =>
            this.VirtualParent.HandleVirtualChildMouseUp(mouseEvent, this.VirtualChildIndex);

    }
}
