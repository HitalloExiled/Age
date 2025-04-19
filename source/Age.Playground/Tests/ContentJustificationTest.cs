using Age.Numerics;
using Age.Elements;
using Age.Styling;

namespace Age.Playground.Tests;

public class ContentJustificationTest
{
    public static void Setup(Canvas canvas)
    {
        var borderSize = 10u;
        var marginSize = 4u;

        var horizontal_a_container = new FlexBox()
        {
            Name  = "horizontal_a_container",
            Style = new()
            {
                Border               = new(borderSize, 0, Color.Margenta),
                Color                = Color.White,
                FontSize             = 32,
                Size                 = new((Pixel)200, null),
                ContentJustification = ContentJustification.Start,
            }
        };

        var horizontal_a_item_01 = new FlexBox()
        {
            Name  = "horizontal_a_item_01",
            Text  = "01",
            Style = new()
            {
                Border = new(borderSize, 0, Color.Red),
                Color  = Color.White,
            }
        };
        var horizontal_a_item_02 = new FlexBox()
        {
            Name  = "horizontal_a_item_02",
            Text  = "02",
            Style = new()
            {
                Border = new(borderSize, 0, Color.Green),
                Color  = Color.White,
                Margin = new((Pixel)marginSize)
            }
        };
        var horizontal_a_item_03 = new FlexBox()
        {
            Name  = "horizontal_a_item_03",
            Text  = "03",
            Style = new()
            {
                Border = new(borderSize, 0, Color.Blue),
                Color  = Color.White,
            }
        };

        var horizontal_b_container = new FlexBox()
        {
            Name  = "horizontal_b_container",
            Style = new()
            {
                Border               = new(borderSize, 0, Color.Margenta),
                Color                = Color.White,
                FontSize             = 32,
                Size                 = new((Pixel)200, null),
                ContentJustification = ContentJustification.Center,
            }
        };

        var horizontal_b_item_01 = new FlexBox()
        {
            Name  = "horizontal_b_item_01",
            Text  = "01",
            Style = new()
            {
                Border = new(borderSize, 0, Color.Red),
                Color  = Color.White,
            }
        };

        var horizontal_b_item_02 = new FlexBox()
        {
            Name  = "horizontal_b_item_02",
            Text  = "02",
            Style = new()
            {
                Border = new(borderSize, 0, Color.Green),
                Color  = Color.White,
                Margin = new((Pixel)marginSize)
            }
        };

        var horizontal_b_item_03 = new FlexBox()
        {
            Name  = "horizontal_b_item_03",
            Text  = "03",
            Style = new()
            {
                Border = new(borderSize, 0, Color.Blue),
                Color  = Color.White,
            }
        };

        var horizontal_c_container = new FlexBox()
        {
            Name  = "horizontal_c_container",
            Style = new()
            {
                Border               = new(borderSize, 0, Color.Margenta),
                Color                = Color.White,
                FontSize             = 32,
                Size                 = new((Pixel)200, null),
                ContentJustification = ContentJustification.End,
            }
        };

        var horizontal_c_item_01 = new FlexBox()
        {
            Name  = "horizontal_c_item_01",
            Text  = "01",
            Style = new()
            {
                Border = new(borderSize, 0, Color.Red),
                Color  = Color.White,
            }
        };

        var horizontal_c_item_02 = new FlexBox()
        {
            Name  = "horizontal_c_item_02",
            Text  = "02",
            Style = new()
            {
                Border = new(borderSize, 0, Color.Green),
                Color  = Color.White,
                Margin = new((Pixel)marginSize),
            }
        };

        var horizontal_c_item_03 = new FlexBox()
        {
            Name  = "horizontal_c_item_03",
            Text  = "03",
            Style = new()
            {
                Border = new(borderSize, 0, Color.Blue),
                Color  = Color.White,
            }
        };

        var horizontal_d_container = new FlexBox()
        {
            Name  = "horizontal_d_container",
            Style = new()
            {
                Border               = new(borderSize, 0, Color.Margenta),
                Color                = Color.White,
                FontSize             = 32,
                Size                 = new((Pixel)200, null),
                ContentJustification = ContentJustification.SpaceAround,
            }
        };

        var horizontal_d_item_01 = new FlexBox()
        {
            Name  = "horizontal_d_item_01",
            Text  = "01",
            Style = new()
            {
                Border = new(borderSize, 0, Color.Red),
                Color  = Color.White,
            }
        };

        var horizontal_d_item_02 = new FlexBox()
        {
            Name  = "horizontal_d_item_02",
            Text  = "02",
            Style = new()
            {
                Border = new(borderSize, 0, Color.Green),
                Color  = Color.White,
                Margin = new((Pixel)marginSize),
            }
        };

        var horizontal_d_item_03 = new FlexBox()
        {
            Name  = "horizontal_d_item_03",
            Text  = "03",
            Style = new()
            {
                Border = new(borderSize, 0, Color.Blue),
                Color  = Color.White,
            }
        };

        var horizontal_e_container = new FlexBox()
        {
            Name  = "horizontal_e_container",
            Style = new()
            {
                Border               = new(borderSize, 0, Color.Margenta),
                Color                = Color.White,
                FontSize             = 32,
                Size                 = new((Pixel)200, null),
                ContentJustification = ContentJustification.SpaceBetween,
            }
        };

        var horizontal_e_item_01 = new FlexBox()
        {
            Name  = "horizontal_e_item_01",
            Text  = "01",
            Style = new()
            {
                Border = new(borderSize, 0, Color.Red),
                Color  = Color.White,
            }
        };

        var horizontal_e_item_02 = new FlexBox()
        {
            Name  = "horizontal_e_item_02",
            Text  = "02",
            Style = new()
            {
                Border = new(borderSize, 0, Color.Green),
                Color  = Color.White,
                Margin = new((Pixel)marginSize),
            }
        };

        var horizontal_e_item_03 = new FlexBox()
        {
            Name  = "horizontal_e_item_03",
            Text  = "03",
            Style = new()
            {
                Border = new(borderSize, 0, Color.Blue),
                Color  = Color.White,
            }
        };

        var horizontal_f_container = new FlexBox()
        {
            Name  = "horizontal_f_container",
            Style = new()
            {
                Border               = new(borderSize, 0, Color.Margenta),
                Color                = Color.White,
                FontSize             = 32,
                Size                 = new((Pixel)200, null),
                ContentJustification = ContentJustification.SpaceEvenly,
            }
        };

        var horizontal_f_item_01 = new FlexBox()
        {
            Name  = "horizontal_f_item_01",
            Text  = "01",
            Style = new()
            {
                Border = new(borderSize, 0, Color.Red),
                Color  = Color.White,
            }
        };

        var horizontal_f_item_02 = new FlexBox()
        {
            Name  = "horizontal_f_item_02",
            Text  = "02",
            Style = new()
            {
                Border = new(borderSize, 0, Color.Green),
                Color  = Color.White,
                Margin = new((Pixel)marginSize),
            }
        };

        var horizontal_f_item_03 = new FlexBox()
        {
            Name  = "horizontal_f_item_03",
            Text  = "03",
            Style = new()
            {
                Border = new(borderSize, 0, Color.Blue),
                Color  = Color.White,
            }
        };

        var vertical_a_container = new FlexBox()
        {
            Name  = "vertical_a_container",
            Style = new()
            {
                Border               = new(borderSize, 0, Color.Cyan),
                Color                = Color.White,
                ContentJustification = ContentJustification.Start,
                FontSize             = 32,
                Size                 = new(null, (Pixel)200),
                StackDirection       = StackDirection.Vertical,
            }
        };

        var vertical_a_item_01 = new FlexBox()
        {
            Name  = "vertical_a_item_01",
            Text  = "01",
            Style = new()
            {
                Border = new(borderSize, 0, Color.Red),
                Color  = Color.White,
            }
        };
        var vertical_a_item_02 = new FlexBox()
        {
            Name  = "vertical_a_item_02",
            Text  = "02",
            Style = new()
            {
                Border = new(borderSize, 0, Color.Green),
                Color  = Color.White,
                Margin = new((Pixel)marginSize),
            }
        };
        var vertical_a_item_03 = new FlexBox()
        {
            Name  = "vertical_a_item_03",
            Text  = "03",
            Style = new()
            {
                Border = new(borderSize, 0, Color.Blue),
                Color  = Color.White,
            }
        };

        var vertical_b_container = new FlexBox()
        {
            Name  = "vertical_b_container",
            Style = new()
            {
                Border               = new(borderSize, 0, Color.Cyan),
                Color                = Color.White,
                ContentJustification = ContentJustification.Center,
                FontSize             = 32,
                Size                 = new(null, (Pixel)200),
                StackDirection       = StackDirection.Vertical,
            }
        };

        var vertical_b_item_01 = new FlexBox()
        {
            Name  = "vertical_b_item_01",
            Text  = "01",
            Style = new()
            {
                Border = new(borderSize, 0, Color.Red),
                Color  = Color.White,
            }
        };

        var vertical_b_item_02 = new FlexBox()
        {
            Name  = "vertical_b_item_02",
            Text  = "02",
            Style = new()
            {
                Border = new(borderSize, 0, Color.Green),
                Color  = Color.White,
                Margin = new((Pixel)marginSize),
            }
        };

        var vertical_b_item_03 = new FlexBox()
        {
            Name  = "vertical_b_item_03",
            Text  = "03",
            Style = new()
            {
                Border = new(borderSize, 0, Color.Blue),
                Color  = Color.White,
            }
        };

        var vertical_c_container = new FlexBox()
        {
            Name  = "vertical_c_container",
            Style = new()
            {
                Border               = new(borderSize, 0, Color.Cyan),
                Color                = Color.White,
                ContentJustification = ContentJustification.End,
                FontSize             = 32,
                Size                 = new(null, (Pixel)200),
                StackDirection       = StackDirection.Vertical,
            }
        };

        var vertical_c_item_01 = new FlexBox()
        {
            Name  = "vertical_c_item_01",
            Text  = "01",
            Style = new()
            {
                Border = new(borderSize, 0, Color.Red),
                Color  = Color.White,
            }
        };

        var vertical_c_item_02 = new FlexBox()
        {
            Name  = "vertical_c_item_02",
            Text  = "02",
            Style = new()
            {
                Border = new(borderSize, 0, Color.Green),
                Color  = Color.White,
                Margin = new((Pixel)marginSize),
            }
        };

        var vertical_c_item_03 = new FlexBox()
        {
            Name  = "vertical_c_item_03",
            Text  = "03",
            Style = new()
            {
                Border = new(borderSize, 0, Color.Blue),
                Color  = Color.White,
            }
        };

        var vertical_d_container = new FlexBox()
        {
            Name  = "vertical_d_container",
            Style = new()
            {
                Border               = new(borderSize, 0, Color.Cyan),
                Color                = Color.White,
                ContentJustification = ContentJustification.SpaceAround,
                FontSize             = 32,
                Size                 = new(null, (Pixel)200),
                StackDirection       = StackDirection.Vertical,
            }
        };

        var vertical_d_item_01 = new FlexBox()
        {
            Name  = "vertical_d_item_01",
            Text  = "01",
            Style = new()
            {
                Border = new(borderSize, 0, Color.Red),
                Color  = Color.White,
            }
        };

        var vertical_d_item_02 = new FlexBox()
        {
            Name  = "vertical_d_item_02",
            Text  = "02",
            Style = new()
            {
                Border = new(borderSize, 0, Color.Green),
                Color  = Color.White,
            }
        };

        var vertical_d_item_03 = new FlexBox()
        {
            Name  = "vertical_d_item_03",
            Text  = "03",
            Style = new()
            {
                Border = new(borderSize, 0, Color.Blue),
                Color  = Color.White,
            }
        };

        var vertical_e_container = new FlexBox()
        {
            Name  = "vertical_e_container",
            Style = new()
            {
                Border               = new(borderSize, 0, Color.Cyan),
                Color                = Color.White,
                ContentJustification = ContentJustification.SpaceBetween,
                FontSize             = 32,
                Size                 = new(null, (Pixel)200),
                StackDirection       = StackDirection.Vertical,
            }
        };

        var vertical_e_item_01 = new FlexBox()
        {
            Name  = "vertical_e_item_01",
            Text  = "01",
            Style = new()
            {
                Border = new(borderSize, 0, Color.Red),
                Color  = Color.White,
            }
        };

        var vertical_e_item_02 = new FlexBox()
        {
            Name  = "vertical_e_item_02",
            Text  = "02",
            Style = new()
            {
                Border = new(borderSize, 0, Color.Green),
                Color  = Color.White,
                Margin = new((Pixel)marginSize),
            }
        };

        var vertical_e_item_03 = new FlexBox()
        {
            Name  = "vertical_e_item_03",
            Text  = "03",
            Style = new()
            {
                Border = new(borderSize, 0, Color.Blue),
                Color  = Color.White,
            }
        };

        var vertical_f_container = new FlexBox()
        {
            Name  = "vertical_f_container",
            Style = new()
            {
                Border               = new(borderSize, 0, Color.Cyan),
                Color                = Color.White,
                ContentJustification = ContentJustification.SpaceEvenly,
                FontSize             = 32,
                Size                 = new(null, (Pixel)200),
                StackDirection       = StackDirection.Vertical,
            }
        };

        var vertical_f_item_01 = new FlexBox()
        {
            Name  = "vertical_f_item_01",
            Text  = "01",
            Style = new()
            {
                Border = new(borderSize, 0, Color.Red),
                Color  = Color.White,
            }
        };

        var vertical_f_item_02 = new FlexBox()
        {
            Name  = "vertical_f_item_02",
            Text  = "02",
            Style = new()
            {
                Border = new(borderSize, 0, Color.Green),
                Color  = Color.White,
                Margin = new((Pixel)marginSize)
            }
        };

        var vertical_f_item_03 = new FlexBox()
        {
            Name  = "vertical_f_item_03",
            Text  = "03",
            Style = new()
            {
                Border = new(borderSize, 0, Color.Blue),
                Color  = Color.White,
            }
        };

        canvas.AppendChild(horizontal_a_container);
            horizontal_a_container.AppendChild(horizontal_a_item_01);
            horizontal_a_container.AppendChild(horizontal_a_item_02);
            horizontal_a_container.AppendChild(horizontal_a_item_03);

        canvas.AppendChild(horizontal_b_container);
            horizontal_b_container.AppendChild(horizontal_b_item_01);
            horizontal_b_container.AppendChild(horizontal_b_item_02);
            horizontal_b_container.AppendChild(horizontal_b_item_03);

        canvas.AppendChild(horizontal_c_container);
            horizontal_c_container.AppendChild(horizontal_c_item_01);
            horizontal_c_container.AppendChild(horizontal_c_item_02);
            horizontal_c_container.AppendChild(horizontal_c_item_03);

        canvas.AppendChild(horizontal_d_container);
            horizontal_d_container.AppendChild(horizontal_d_item_01);
            horizontal_d_container.AppendChild(horizontal_d_item_02);
            horizontal_d_container.AppendChild(horizontal_d_item_03);

        canvas.AppendChild(horizontal_e_container);
            horizontal_e_container.AppendChild(horizontal_e_item_01);
            horizontal_e_container.AppendChild(horizontal_e_item_02);
            horizontal_e_container.AppendChild(horizontal_e_item_03);

        canvas.AppendChild(horizontal_f_container);
            horizontal_f_container.AppendChild(horizontal_f_item_01);
            horizontal_f_container.AppendChild(horizontal_f_item_02);
            horizontal_f_container.AppendChild(horizontal_f_item_03);

        canvas.AppendChild(vertical_a_container);
            vertical_a_container.AppendChild(vertical_a_item_01);
            vertical_a_container.AppendChild(vertical_a_item_02);
            vertical_a_container.AppendChild(vertical_a_item_03);

        canvas.AppendChild(vertical_b_container);
            vertical_b_container.AppendChild(vertical_b_item_01);
            vertical_b_container.AppendChild(vertical_b_item_02);
            vertical_b_container.AppendChild(vertical_b_item_03);

        canvas.AppendChild(vertical_c_container);
            vertical_c_container.AppendChild(vertical_c_item_01);
            vertical_c_container.AppendChild(vertical_c_item_02);
            vertical_c_container.AppendChild(vertical_c_item_03);

        canvas.AppendChild(vertical_d_container);
            vertical_d_container.AppendChild(vertical_d_item_01);
            vertical_d_container.AppendChild(vertical_d_item_02);
            vertical_d_container.AppendChild(vertical_d_item_03);

        canvas.AppendChild(vertical_e_container);
            vertical_e_container.AppendChild(vertical_e_item_01);
            vertical_e_container.AppendChild(vertical_e_item_02);
            vertical_e_container.AppendChild(vertical_e_item_03);

        canvas.AppendChild(vertical_f_container);
            vertical_f_container.AppendChild(vertical_f_item_01);
            vertical_f_container.AppendChild(vertical_f_item_02);
            vertical_f_container.AppendChild(vertical_f_item_03);
    }
}
