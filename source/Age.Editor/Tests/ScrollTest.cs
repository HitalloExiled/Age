using Age.Numerics;
using Age.Elements;
using Age.Styling;

namespace Age.Editor.Tests;

public class ScrollTest
{
    public static void Setup(Canvas canvas)
    {
        var borderSize = 2u;

        var text = """
        Lorem ipsum dolor sit amet, consectetur adipiscing elit.
        Sed et malesuada urna.
        Duis quam nisi, ultricies ut elit ac, feugiat tincidunt dolor.
        Aliquam auctor, urna id hendrerit elementum, mi elit fermentum tortor, et posuere nisi ex ac eros.
        Aenean interdum orci luctus, ornare libero in, sollicitudin tortor.
        Phasellus consequat sed eros eget consectetur.
        In in diam sem.
        Maecenas turpis arcu, porttitor sed tortor eu, dictum varius nibh.
        Duis eget felis eget tellus cursus luctus.
        Curabitur non justo nisi.
        Vivamus eleifend lorem ut purus tincidunt, id ullamcorper nunc iaculis.
        Morbi molestie ultricies sem sed imperdiet.
        Curabitur sapien sapien, volutpat consectetur neque non, dictum volutpat elit.
        Proin id enim libero.
        Lorem ipsum dolor sit amet, consectetur adipiscing elit.
        Vivamus consequat mollis magna, non ultrices erat luctus id.

        Ut ipsum nunc, pharetra nec purus ac, gravida mattis velit.
        Integer viverra rhoncus dui, eget suscipit augue rhoncus quis.
        In ultricies, sem eu malesuada accumsan, felis justo eleifend elit, ac placerat ante tellus sed massa.
        Ut ac orci eget mi laoreet interdum ac egestas lorem.
        In hac habitasse platea dictumst.
        Interdum et malesuada fames ac ante ipsum primis in faucibus.
        Nullam ut dui elit.
        Ut lacus massa, dictum ac turpis sit amet, cursus dapibus sapien.
        Morbi ut dapibus orci, vitae varius libero.
        Phasellus quis ultricies metus.
        Fusce dignissim sit amet magna ut vulputate.
        Sed laoreet, augue vitae hendrerit elementum, elit nunc pulvinar lacus, vel efficitur orci augue eget arcu.
        Aenean sodales, nisi in ornare tincidunt, leo mi pulvinar odio, in rhoncus velit tellus accumsan risus.
        Nam vitae pellentesque tortor.

        Sed interdum convallis nibh, vitae ullamcorper erat faucibus eget.
        Vivamus auctor sem nec mattis pharetra.
        Integer venenatis iaculis auctor.
        Integer iaculis maximus scelerisque.
        Nunc erat tellus, dignissim ut rutrum sed, molestie a ante.
        Aenean pulvinar porttitor magna nec malesuada.
        Curabitur a magna at urna vehicula consectetur sit amet aliquet diam.
        Nunc sodales accumsan lacus a molestie.
        Curabitur vel eros elementum metus scelerisque vulputate.
        Maecenas tellus quam, convallis nec turpis nec, lacinia viverra velit.
        Aenean tincidunt odio condimentum hendrerit pretium.
        In laoreet porttitor magna vitae vehicula.
        Proin viverra, quam ornare fermentum suscipit, nisl sem faucibus orci, non commodo libero quam vulputate nulla.

        Nam lacinia leo sagittis, elementum leo sed, condimentum nunc.
        Quisque facilisis nunc elit, id fringilla mauris imperdiet non.
        Suspendisse fermentum, diam ut iaculis venenatis, metus libero rutrum velit, quis pretium tortor nunc quis purus.
        Curabitur a est sagittis, vehicula sapien eget, porttitor justo.
        Mauris bibendum malesuada tincidunt.
        Vivamus quis accumsan mauris.
        Mauris aliquam neque id sapien dignissim ullamcorper.
        Nunc at ultrices eros.
        Nulla commodo, ipsum eu vestibulum rutrum, ante ante ultrices dui, at iaculis ante tortor vitae purus.
        Maecenas eu nisl nec orci fringilla ultricies sed nec tellus.
        Lorem ipsum dolor sit amet, consectetur adipiscing elit.
        Pellentesque semper quis augue id dictum.
        Lorem ipsum dolor sit amet, consectetur adipiscing elit.
        Etiam mollis lacus eget semper auctor.
        Curabitur sodales diam arcu, id consectetur elit cursus quis.
        Aliquam congue, eros ut ornare fringilla, nisi nunc ullamcorper lorem, a vehicula elit diam at lacus.

        Nam hendrerit et est vel mattis.
        Maecenas ullamcorper, leo eget aliquet rutrum, massa arcu rhoncus magna, a accumsan mi purus eget sem.
        Vivamus porttitor libero at lacus vestibulum, eu mattis ligula tincidunt.
        Quisque pretium pharetra turpis, at sollicitudin lacus consectetur sit amet.
        Donec faucibus efficitur convallis.
        Quisque ullamcorper eros non leo placerat porttitor.
        Donec auctor tempor lorem.
        Donec efficitur nisi et nisl iaculis, vel tincidunt eros placerat.
        Sed eleifend orci odio, porttitor facilisis massa sodales et.
        Sed iaculis nisi id ante tincidunt mollis.
        Vestibulum quis congue felis.
        Nullam nec pellentesque mi.
        Vestibulum eu iaculis risus.
        Aenean ex tortor, sodales a urna eget, tristique vestibulum lectus.
        """;

        var conteiner = new FlexBox()
        {
            Name  = "conteiner",
            Text  = text,
            Style = new()
            {
                Border   = new(borderSize, 0, Color.Red),
                Padding  = new((Pixel)10),
                Overflow = OverflowKind.Scroll,
                Size     = new((Pixel)400),
                Color    = Color.White,
            },
        };

        canvas.AppendChild(conteiner);
    }
}
