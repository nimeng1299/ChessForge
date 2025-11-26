using Avalonia;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessForge.Controls
{
    public class Board: Avalonia.Controls.Control
    {
        public override void Render(DrawingContext context)
        {
            base.Render(context);
            // 绘制的是红色边框(用于参考)
            var rect = new Rect(Bounds.Size);
            var pen = new Pen(Brushes.Red, 3);
            context.DrawRectangle(null, pen, rect);
        }
    }
}
