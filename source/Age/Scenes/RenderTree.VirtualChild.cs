using System.Runtime.CompilerServices;
using Age.Elements;
using Age.Platforms.Display;

namespace Age.Scenes;

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
        public readonly void HandleMouseDown(in WindowMouseEvent mouseEvent) =>
            this.VirtualParent.HandleVirtualChildMouseDown(mouseEvent, this.VirtualChildIndex);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void HandleMouseMoved(in WindowMouseEvent mouseEvent) =>
            this.VirtualParent.HandleVirtualChildMouseMoved(mouseEvent, this.VirtualChildIndex);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void HandleMouseOut(in WindowMouseEvent mouseEvent) =>
            this.VirtualParent.HandleVirtualChildMouseOut(mouseEvent, this.VirtualChildIndex);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void HandleMouseOver(in WindowMouseEvent mouseEvent) =>
            this.VirtualParent.HandleVirtualChildMouseOver(mouseEvent, this.VirtualChildIndex);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void HandleMouseRelease(in WindowMouseEvent mouseEvent) =>
            this.VirtualParent.HandleVirtualChildMouseRelease(mouseEvent, this.VirtualChildIndex);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void HandleMouseUp(in WindowMouseEvent mouseEvent) =>
            this.VirtualParent.HandleVirtualChildMouseUp(mouseEvent, this.VirtualChildIndex);
    }
}
