using Age.Components;
using Age.Elements;
using Age.Numerics;
using Age.Scene;
using Age.Styling;

namespace Age.Playground.Tests;

public class Host : Element
{
    public override string NodeName { get; } = nameof(Host);

    public Host()
    {
        this.AttachShadowTree();

        Button toggleSlotDefaultButton;
        Button toggleSlotS1Button;
        Button toggleSlotS2Button;

        Slot slotDefault;
        Slot slotS1;
        Slot slotS2;

        var buttonStyle = new Style
        {
            Size = new((Percentage)100, null),
        };

        this.ShadowTree.Children =
        [
            new FlexBox
            {
                Text  = "### Shadow Element ###",
                Style = new()
                {
                    Border = new(1, 0, Color.Red),
                    Color  = Color.White,
                    Size   = new((Percentage)100, null),
                }
            },
            new FlexBox
            {
                Style = new()
                {
                    Stack = StackKind.Vertical,
                    Size  = new((Percentage)100, null),
                },
                Children =
                [
                    toggleSlotDefaultButton = new Button { Text = "Toggle Slot default", Variant = ButtonVariant.Text, Style = buttonStyle },
                    toggleSlotS1Button      = new Button { Text = "Toggle Slot s-1",     Variant = ButtonVariant.Text, Style = buttonStyle },
                    toggleSlotS2Button      = new Button { Text = "Toggle Slot s-2",     Variant = ButtonVariant.Text, Style = buttonStyle },
                ],
            },
            slotDefault = new Slot
            {
                //Name  = "1",
                Text  = "Default",
                Style = new()
                {
                    Border = new(1, 0, Color.Red),
                    Color  = Color.White,
                }
            },
            slotS1 = new Slot
            {
                Text  = "Default S-1 Content",
                Name  = "s-1",
                Style = new()
                {
                    Border = new(1, 0, Color.Red),
                    Color  = Color.White,
                    //Size   = new((Pixel)200, null),
                }
            },
            slotS2 = new Slot
            {
                Text  = "Default S-2 Content",
                Name  = "s-2",
                Style = new()
                {
                    Border = new(1, 0, Color.Red),
                    Color  = Color.White,
                }
            },
        ];

        void toogle(Node node)
        {
            if (node.Parent == null)
            {
                this.ShadowTree!.AppendChild(node);
            }
            else
            {
                this.ShadowTree!.RemoveChild(node);
            }
        }

        slotDefault.Detach();
        slotS1.Detach();
        slotS2.Detach();

        toggleSlotDefaultButton.Clicked += (in MouseEvent _) => toogle(slotDefault);
        toggleSlotS1Button.Clicked += (in MouseEvent _) => toogle(slotS1);
        toggleSlotS2Button.Clicked += (in MouseEvent _) => toogle(slotS2);
    }
}

public class ShadowTreeTest
{
    public static void Setup(Canvas canvas)
    {
        Button moveL1toSlotDefaultButton;
        Button moveL1toSlotS1Button;
        Button moveL1toSlotS2Button;
        Button moveL2toSlotDefaultButton;
        Button moveL2toSlotS1Button;
        Button moveL2toSlotS2Button;
        FlexBox l1;
        FlexBox l2;

        var buttonStyle = new Style
        {
            Size = new((Percentage)100, null),
        };

        canvas.Children =
        [
            new FlexBox
            {
                Style    = new()
                {
                    Stack = StackKind.Vertical,
                },
                Children =
                [
                    moveL1toSlotDefaultButton = new Button { Text = "Move L1 to Slot default", Style = buttonStyle },
                    moveL1toSlotS1Button      = new Button { Text = "Move L1 to Slot s-1",     Style = buttonStyle },
                    moveL1toSlotS2Button      = new Button { Text = "Move L1 to Slot s-2",     Style = buttonStyle },
                    moveL2toSlotDefaultButton = new Button { Text = "Move L2 to Slot default", Style = buttonStyle },
                    moveL2toSlotS1Button      = new Button { Text = "Move L2 to Slot s-1",     Style = buttonStyle },
                    moveL2toSlotS2Button      = new Button { Text = "Move L2 to Slot s-2",     Style = buttonStyle },
                ],
            },
            new Host
            {
                Name  = "host",
                Style = new()
                {
                    Stack  = StackKind.Vertical,
                    //Size   = new((Pixel)100),
                    Border = new(1, 0, Color.Red),
                },
                Children =
                [
                    l1 = new FlexBox
                    {
                        Text  = "L1",
                        Name  = "light-1",
                        Slot  = "s-1",
                        Style = new()
                        {
                            Border   = new(1, 0, Color.Green),
                            Color    = Color.White,
                            //Size     = new((Percentage)10, null),
                            Overflow = OverflowKind.Clipping,
                        }
                    },
                    l2 = new FlexBox
                    {
                        Text  = "L2",
                        Name  = "light-2",
                        Slot  = "s-2",
                        Style = new()
                        {
                            Border    = new(1, 0, Color.Blue),
                            Color     = Color.White,
                            //Size      = new((Percentage)10, null),
                            //Alignment = AlignmentKind.End,
                            Overflow  = OverflowKind.Clipping,
                        }
                    }
                ]
            }
        ];

        moveL1toSlotDefaultButton.Clicked += (in MouseEvent _) => l1.Slot = null;
        moveL1toSlotS1Button.Clicked += (in MouseEvent _) => l1.Slot = "s-1";
        moveL1toSlotS2Button.Clicked += (in MouseEvent _) => l1.Slot = "s-2";

        moveL2toSlotDefaultButton.Clicked += (in MouseEvent _) => l2.Slot = null;
        moveL2toSlotS1Button.Clicked += (in MouseEvent _) => l2.Slot = "s-1";
        moveL2toSlotS2Button.Clicked += (in MouseEvent _) => l2.Slot = "s-2";
    }
}
