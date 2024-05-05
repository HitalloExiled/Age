// #define DUMP_IMAGES
using Age.Rendering.Drawing.Elements;

namespace Age.Rendering.Interfaces;

internal interface ITextService : IDisposable
{
    void DrawText(TextNode textNode, string text);
}
