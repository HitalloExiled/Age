// using Age.Core.Extensions;
// using Age.Scene;
// using Age.Styling;
// using System.Diagnostics.CodeAnalysis;
// using System.Runtime.CompilerServices;

// namespace Age.Elements.Layouts;

// internal abstract partial class StyledLayout(Element target) : Layout
// {
//     public event Action<StyleProperty>? StyleChanged;

//     private static readonly StylePool stylePool = new();

//     private ElementState States
//     {
//         get;
//         set
//         {
//             if (field != value)
//             {
//                 field = value;

//                 if (this.StyleSheet != null)
//                 {
//                     this.ComputeStyle(this.ComputedStyle.Data);
//                 }
//             }
//         }
//     }

//     public Style ComputedStyle { get; } = stylePool.Get();

//     public override Element Target => target;

//     public StyleSheet? StyleSheet
//     {
//         get;
//         set
//         {
//             if (field != value)
//             {
//                 field = value;

//                 this.ComputeStyle(this.ComputedStyle.Data);
//             }
//         }
//     }

//     public Style? UserStyle
//     {
//         get;
//         set
//         {
//             if (field != value)
//             {
//                 if (field != null)
//                 {
//                     field.PropertyChanged -= this.OnPropertyChanged;
//                 }

//                 if (value != null)
//                 {
//                     value.PropertyChanged += this.OnPropertyChanged;
//                 }

//                 var previous = this.ComputedStyle.Data;

//                 field = value;

//                 this.ComputeStyle(previous);
//             }
//         }
//     }

//     private void CompareAndInvoke(in StyleData left, in StyleData right)
//     {
//         var changes = StyleData.Diff(left, right);

//         if (changes != default)
//         {
//             this.InvokeStyleChanged(changes);
//         }
//     }

//     private void ComputeStyle(in StyleData previous)
//     {
//         if (!target.IsConnected)
//         {
//             return;
//         }

//         var inheritedProperties = this.GetInheritedProperties();

//         this.ComputedStyle.Copy(inheritedProperties);

//         if (this.StyleSheet?.Base != null)
//         {
//             this.ComputedStyle.Merge(this.StyleSheet.Base);
//         }

//         if (this.UserStyle != null)
//         {
//             this.ComputedStyle.Merge(this.UserStyle);
//         }

//         if (this.StyleSheet != null)
//         {
//             merge(ElementState.Focus,    this.StyleSheet.Focus);
//             merge(ElementState.Hovered,  this.StyleSheet.Hovered);
//             merge(ElementState.Disabled, this.StyleSheet.Disabled);
//             merge(ElementState.Enabled,  this.StyleSheet.Enabled);
//             merge(ElementState.Checked,  this.StyleSheet.Checked);
//             merge(ElementState.Active,   this.StyleSheet.Active);
//         }

//         this.CompareAndInvoke(this.ComputedStyle.Data, previous);

//         [MethodImpl(MethodImplOptions.AggressiveInlining)]
//         void merge(ElementState states, Style? style)
//         {
//             if (this.States.HasFlags(states) && style != null)
//             {
//                 this.ComputedStyle.Merge(style);
//             }
//         }
//     }

//     private StyleData GetInheritedProperties() =>
//         GetStyleSource(this.Target.Parent)?.ComputedStyle is Style parentStyle
//             ? new StyleData
//             {
//                 Color         = parentStyle.Color,
//                 FontFamily    = parentStyle.FontFamily,
//                 FontSize      = parentStyle.FontSize,
//                 FontWeight    = parentStyle.FontWeight,
//                 TextSelection = parentStyle.TextSelection
//             }
//             : default;

//     private void InvokeStyleChanged(StyleProperty property)
//     {
//         this.OnStyleChanged(property);
//         StyleChanged?.Invoke(property);
//     }

//     private void OnParentStyleChanged(StyleProperty property)
//     {
//         var inheritedProperties = this.GetInheritedProperties();

//         this.ComputedStyle.Merge(inheritedProperties);
//     }

//     private void OnPropertyChanged(StyleProperty property)
//     {
//         this.ComputedStyle?.Copy(this.UserStyle!, property);
//         this.InvokeStyleChanged(property);
//     }

//     protected abstract void OnStyleChanged(StyleProperty property);

//     protected override void OnDisposed() =>
//         stylePool.Return(this.ComputedStyle);

//     public void AddState(ElementState state) =>
//         this.States |= state;

//     public void ComputeStyle() =>
//         this.ComputeStyle(default);

//     public void RemoveState(ElementState state) =>
//         this.States &= ~state;

//     public override void HandleTargetConnected()
//     {
//         base.HandleTargetConnected();

//         GetStyleSource(this.Target.Parent)?.StyleChanged += this.OnParentStyleChanged;
//     }

//     public void HandleTargetRemoved(Node parent) =>
//         GetStyleSource(parent)?.StyleChanged -= this.OnParentStyleChanged;

//     public override string ToString() =>
//         $"{{ Target: {target} }}";
// }
