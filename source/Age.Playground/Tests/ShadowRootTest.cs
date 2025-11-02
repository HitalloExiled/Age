using Age.Components;
using Age.Elements;
using Age.Numerics;
using Age.Scenes;
using Age.Styling;

namespace Age.Playground.Tests;

public static class ShadowRootTest
{
    public class Host : Element
    {
        public override string NodeName => nameof(Host);

        private readonly Element slotDefault;
        private readonly Element slotS1;
        private readonly Element slotS2;

        public Element? SlotDefault { get; set => ReplaceSlot(this.slotDefault, ref field, value); }
        public Element? SlotS1      { get; set => ReplaceSlot(this.slotS1,      ref field, value); }
        public Element? SlotS2      { get; set => ReplaceSlot(this.slotS2,      ref field, value); }

        public Host()
        {
            var buttonStyle = new Style
            {
                Size = new(Unit.Pc(100), null),
            };

            Button toggleSlotDefaultButton;
            Button toggleSlotS1Button;
            Button toggleSlotS2Button;

            var shadowRoot = new FlexBox
            {
                Style =
                {
                    StackDirection  = StackDirection.Vertical,
                    //Size   = new((Pixel)100),
                    Border = new(1, 0, Color.Red),
                },
                Children =
                [
                    new FlexBox
                    {
                        InnerText = "### Shadow Element ###",
                        Style     = new()
                        {
                            Border = new(1, 0, Color.Red),
                            Color  = Color.White,
                            Size   = new(Unit.Pc(100), null),
                        }
                    },
                    new FlexBox
                    {
                        Style = new()
                        {
                            StackDirection = StackDirection.Vertical,
                            Size           = new(Unit.Pc(100), null),
                        },
                        Children =
                        [
                            toggleSlotDefaultButton = new Button { InnerText = "Toggle Slot default", Variant = ButtonVariant.Text, Style = buttonStyle },
                            toggleSlotS1Button      = new Button { InnerText = "Toggle Slot s-1",     Variant = ButtonVariant.Text, Style = buttonStyle },
                            toggleSlotS2Button      = new Button { InnerText = "Toggle Slot s-2",     Variant = ButtonVariant.Text, Style = buttonStyle },
                        ],
                    },
                    this.slotDefault = new FlexBox
                    {
                        //Name    = "1",
                        InnerText = "Default",
                        Style     = new()
                        {
                            Border = new(1, 0, Color.Red),
                            Color  = Color.White,
                        }
                    },
                    this.slotS1 = new FlexBox
                    {
                        InnerText = "Default S-1 Content",
                        Name      = "s-1",
                        Style     = new()
                        {
                            Border = new(1, 0, Color.Red),
                            Color  = Color.White,
                            //Size   = new((Pixel)200, null),
                        }
                    },
                    this.slotS2 = new FlexBox
                    {
                        InnerText = "Default S-2 Content",
                        Name      = "s-2",
                        Style     = new()
                        {
                            Border = new(1, 0, Color.Red),
                            Color  = Color.White,
                        }
                    },
                ]
            };

            this.AttachShadowRoot(shadowRoot);

            void toogle(Node node)
            {
                using var scope = this.CreateUnsealedScope();

                if (node.Parent == null)
                {
                    shadowRoot.AppendChild(node);
                }
                else
                {
                    shadowRoot.DetachChild(node);
                }
            }

            this.slotDefault.Detach();
            this.slotS1.Detach();
            this.slotS2.Detach();

            toggleSlotDefaultButton.Clicked += (in _) => toogle(this.slotDefault);
            toggleSlotS1Button.Clicked += (in _) => toogle(this.slotS1);
            toggleSlotS2Button.Clicked += (in _) => toogle(this.slotS2);

            this.Seal();
        }
    }
    public static void Setup(Canvas canvas)
    {
        Button moveL1toSlotDefaultButton;
        Button moveL1toSlotS1Button;
        Button moveL1toSlotS2Button;
        Button moveL2toSlotDefaultButton;
        Button moveL2toSlotS1Button;
        Button moveL2toSlotS2Button;

        var buttonStyle = new Style
        {
            Size = new(Unit.Pc(100), null),
        };

        Host host;

        var l1 = new FlexBox
        {
            InnerText = "L1",
            Name      = "light-1",
            Style     =
            {
                Border   = new(1, 0, Color.Green),
                Color    = Color.White,
                //Size     = new(Unit.Pc(10), null),
                Overflow = Overflow.Clipping,
            }
        };

        var l2 = new FlexBox
        {
            InnerText = "L2",
            Name      = "light-2",
            Style     =
            {
                Border    = new(1, 0, Color.Blue),
                Color     = Color.White,
                //Size      = new(Unit.Pc(10), null),
                //Alignment = AlignmentKind.End,
                Overflow  = Overflow.Clipping,
            }
        };

        canvas.Children =
        [
            new FlexBox
            {
                Style = new()
                {
                    StackDirection = StackDirection.Vertical,
                },
                Children =
                [
                    moveL1toSlotDefaultButton = new Button { InnerText = "Move L1 to Slot default", Style = buttonStyle },
                    moveL1toSlotS1Button      = new Button { InnerText = "Move L1 to Slot s-1",     Style = buttonStyle },
                    moveL1toSlotS2Button      = new Button { InnerText = "Move L1 to Slot s-2",     Style = buttonStyle },
                    moveL2toSlotDefaultButton = new Button { InnerText = "Move L2 to Slot default", Style = buttonStyle },
                    moveL2toSlotS1Button      = new Button { InnerText = "Move L2 to Slot s-1",     Style = buttonStyle },
                    moveL2toSlotS2Button      = new Button { InnerText = "Move L2 to Slot s-2",     Style = buttonStyle },
                ],
            },
            host = new Host
            {
                Name = "host"
            }
        ];

        void swap<T>(Action<Host> setter, T value) where T : Node
        {
            if (host.SlotDefault == value)
            {
                host.SlotDefault = null;
            }

            if (host.SlotS1 == value)
            {
                host.SlotS1 = null;
            }

            if (host.SlotS2 == value)
            {
                host.SlotS2 = null;
            }

            setter.Invoke(host);
        }

        moveL1toSlotDefaultButton.Clicked += (in _) => swap(x => x.SlotDefault = l1, l1);
        moveL1toSlotS1Button.Clicked += (in _)      => swap(x => x.SlotS1      = l1, l1);
        moveL1toSlotS2Button.Clicked += (in _)      => swap(x => x.SlotS2      = l1, l1);

        moveL2toSlotDefaultButton.Clicked += (in _) => swap(x => x.SlotDefault = l2, l2);
        moveL2toSlotS1Button.Clicked += (in _)      => swap(x => x.SlotS1      = l2, l2);
        moveL2toSlotS2Button.Clicked += (in _)      => swap(x => x.SlotS2      = l2, l2);
    }
}
