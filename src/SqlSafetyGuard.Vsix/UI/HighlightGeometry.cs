using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace SqlSafetyGuard.UI
{
    internal static class HighlightGeometry
    {
        // Union, rather than a GeometryGroup: shared row edges disappear from the outline.
        // TextBounds include the full rendered line height, including wrapped visual lines.
        public static Geometry Union(IEnumerable<Rect> rectangles)
        {
            Geometry result = Geometry.Empty;
            foreach (var rectangle in rectangles.Where(r => !r.IsEmpty && r.Width > 0 && r.Height > 0))
                result = Geometry.Combine(result, new RectangleGeometry(rectangle), GeometryCombineMode.Union, null);
            result.Freeze();
            return result;
        }
    }
}
