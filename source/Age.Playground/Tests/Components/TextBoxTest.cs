using Age.Components;
using Age.Elements;
using Age.Numerics;

namespace Age.Playground.Tests.Components;

public static class TextBoxTest
{
    public static void Setup(Canvas canvas) =>
        canvas.Children =
        [
            new TextBox
            {
                Value =
                """
                One
                """,
                // """
                // 00000_00000-x-00000_00000
                // 11111_11111-x-11111_11111
                // 22222_22222-x-22222_22222
                // 33333_33333-x-33333_33333
                // 44444_44444-x-44444_44444
                // 55555_55555-x-55555_55555
                // 66666_66666-x-66666_66666
                // 77777_77777-x-77777_77777
                // 88888_88888-x-88888_88888
                // 99999_99999-x-99999_99999
                // """,
                Name  = "One",
                Style = new()
                {
                    FontFamily = "Segoe UI Emoji",
                    Overflow   = Age.Styling.Overflow.Scroll,
                    Size       = new(150)
                }
            },
            new FlexBox
            {
                InnerText =
                """
                Red
                """,
                Name  = "Red",
                Style = new()
                {
                    FontFamily = "Segoe UI Emoji",
                    Overflow   = Age.Styling.Overflow.Scroll,
                    Size       = new(150),
                    Color      = Color.Blue,
                }
            },
            new FlexBox
            {
                InnerText =
                """
                Blue
                """,
                Name  = "Blue",
                Style = new()
                {
                    FontFamily = "Segoe UI Emoji",
                    Overflow   = Age.Styling.Overflow.Scroll,
                    Size       = new(150),
                    Color      = Color.Red,
                }
            },
            // new TextBox
            // {
            //     Value = "O😹O😁",
            //     Style = new()
            //     {
            //         FontFamily = "Segoe UI Emoji",
            //     }
            // },
            // new TextBox
            // {
            //     Value =
            //         """
            //         O😹O😁
            //         O😹O😁
            //         😹O😁O
            //         O😹O😁
            //         .......
            //         """,
            //     Multiline = true,
            //     Style = new()
            //     {
            //         FontFamily = "Segoe UI Emoji",
            //     }
            // }
        ];
}
