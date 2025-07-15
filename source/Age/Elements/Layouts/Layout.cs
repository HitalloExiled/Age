// using System.Runtime.CompilerServices;
// using Age.Core;
// using Age.Numerics;
// using Age.Platforms.Display;
// using Age.Scene;
// using Age.Styling;

// namespace Age.Elements.Layouts;

// internal abstract class Layout : Disposable
// {
//     public const ushort DEFAULT_FONT_SIZE   = 16;
//     public const string DEFAULT_FONT_FAMILY = "Segoi UI";

//     public static bool IsHoveringText  { get; set; }
//     public static bool IsSelectingText { get; set; }

//     protected virtual StencilLayer? ContentStencilLayer { get; }

//     public bool IsDirty { get; private set; }

//     public int        BaseLine  { get; protected set; } = -1;
//     public Size<uint> Boundings { get; protected set; }

//     public Vector2<float> Offset { get; internal set; }

//     public uint LineHeight { get; set; }

//     public BoxLayout? Parent => this.Target.ComposedParentElement?.Layout;

//     public virtual bool          Hidden       { get; set; }
//     public virtual StencilLayer? StencilLayer { get; set; }

//     public virtual Transform2D Transform => Transform2D.CreateTranslated(this.Offset);

//     public abstract bool       IsParentDependent { get; }
//     public abstract Layoutable Target            { get; }

//     protected static StyledLayout? GetStyleSource(Node? node) =>
//         node is not ShadowTree shadowTree
//             ? (node as Element)?.Layout
//             : shadowTree.InheritsHostStyle
//                 ? shadowTree.Host.Layout
//                 : null;

//     protected abstract void OnDisposed();

//     protected void SetCursor(Cursor? cursor)
//     {
//         if (this.Target.Tree is RenderTree renderTree)
//         {
//             renderTree.Window.Cursor = cursor ?? default;
//         }
//     }

//     protected override void OnDisposed(bool disposing)
//     {
//         if (disposing)
//         {
//             this.OnDisposed();
//         }
//     }

//     public void RequestUpdate(bool affectsBoundings)
//     {
//         for (var current = this; ; current = current.Parent)
//         {
//             if (current!.IsDirty || current.Hidden)
//             {
//                 return;
//             }

//             current.MakeDirty();

//             var stopPropagation = !current.IsParentDependent && !affectsBoundings || current.Parent == null;

//             if (stopPropagation)
//             {
//                 if (current.Target.Tree is RenderTree renderTree)
//                 {
//                     renderTree.AddDeferredUpdate(current.UpdateDirtyLayout);
//                 }

//                 return;
//             }
//         }
//     }

//     public virtual void HandleTargetConnected() =>
//         this.StencilLayer = this.Parent?.ContentStencilLayer;

//     public virtual void HandleTargetDisconnected() =>
//         this.StencilLayer = null;

//     public void MakeDirty() =>
//         this.IsDirty = true;

//     public void MakePristine() =>
//         this.IsDirty = false;

//     public override string ToString() =>
//         $"{{ Target: {this.Target} }}";

//     [MethodImpl(MethodImplOptions.AggressiveInlining)]
//     public void UpdateDirtyLayout()
//     {
//         if (this.IsDirty)
//         {
//             this.Update();
//             this.MakePristine();
//         }
//     }

//     public abstract void Update();
// }
