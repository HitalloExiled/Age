using System.Runtime.CompilerServices;
using Age.Elements;
using Age.Numerics;

namespace Age.Tests.Age.Elements;

internal static class ElementAccessor
{
    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "content")]
    internal static extern ref Size<uint> GetContent(Element element);

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "size")]
    internal static extern ref Size<uint> GetSize(Element element);

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "border")]
    internal static extern ref RectEdges GetBorder(Element element);

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "margin")]
    internal static extern ref RectEdges GetMargin(Element element);

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "padding")]
    internal static extern ref RectEdges GetPadding(Element element);

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "<Boundings>k__BackingField")]
    internal static extern ref Size<uint> GetBoundings(Layoutable layoutable);

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "dependents")]
    internal static extern ref List<Element> GetDependents(Element element);

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "childsChanged")]
    internal static extern ref bool GetChildsChanged(Element element);

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "contentDependencies")]
    internal static extern ref Element.Dependency GetContentDependencies(Element element);

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "parentDependencies")]
    internal static extern ref Element.Dependency GetParentDependencies(Element element);

    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "MakeDirty")]
    internal static extern void MakeDirty(Layoutable layoutable);

    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "MakePristine")]
    internal static extern void MakePristine(Layoutable layoutable);
}
