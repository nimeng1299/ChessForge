using Avalonia;
using Avalonia.Media;
using ChessForge.Models;
using Python.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ChessForge.Scripts
{
    /// <summary>
    /// 用于提供渲染方法
    /// </summary>
    public class ScriptRender
    {
        public Action<DrawingContext, Rect>? DrawBoard { get; set; } = null;

        private Pen pen = new();

        public Rect _bound;

        public ScriptRender()
        {
            DrawBoard += SetBound;
        }

        public void Clear() { 
            DrawBoard = null;
            DrawBoard += SetBound;
        }
        /// <summary>
        /// 修改画笔颜色
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="a"></param>
        public void ChangePenColor(int r, int g, int b, int a)
        {
            pen.SetCurrentValue(Pen.BrushProperty, new SolidColorBrush(Color.FromArgb((byte)a, (byte)r, (byte)g, (byte)b)));
        }
        /// <summary>
        /// 修改画笔粗细
        /// </summary>
        /// <param name="thickness"></param>
        public void ChangePenThickness(double thickness)
        {
            pen.Thickness = thickness;
        }
        /// <summary>
        /// 修改画笔虚线样式, [5, 2] 表示 5像素实线 + 2像素空白
        /// </summary>
        /// <param name="dashArray"></param>
        public void ChangePenDashStyle(double[] dashArray)
        {
            pen.DashStyle = new DashStyle(dashArray, 0);
        }
        /// <summary>
        /// 修改线帽样式
        /// </summary>
        /// <param name="cap">
        /// 0 = Flat, 1 = Round, 2 = Square
        /// </param>
        public void ChangePenRoundCap(int mode)
        {
            if (mode == 0)
                pen.LineCap = PenLineCap.Flat;
            else if (mode == 1)
                pen.LineCap = PenLineCap.Round;
            else if (mode == 2)
                pen.LineCap = PenLineCap.Square;
        }

        public void SetBound(DrawingContext _, Rect bound)
        {
            _bound = bound;
        }

        public double GetBoundTop()
        {
            return _bound.Top;
        }
        public double GetBoundLeft()
        {
            return _bound.Left;
        }
        public double GetBoundWidth()
        {
            return _bound.Width;
        }
        public double GetBoundHeight()
        {
            return _bound.Height;
        }

        public void DrawRectangle(double x, double y, double w, double h)
        {
            DrawBoard += (DrawingContext cx, Rect bound) =>
            {
                var rect = new Rect(x, y, w, h);
                cx.DrawRectangle(null, pen, rect);
            };
        }

        public void DrawLine(double x1, double y1, double x2, double y2)
        {
            DrawBoard += (DrawingContext cx, Rect bound) =>
            {
                cx.DrawLine(pen, new Point(x1, y1), new Point(x2, y2));
            };
        }

        public void DrawEllipse(double r, double g, double b, double a, double center_x, double center_y, double r1, double r2)
        {
            DrawBoard += (DrawingContext cx, Rect bound) =>
            {
                var color = new SolidColorBrush(Color.FromArgb((byte)a, (byte)r, (byte)g, (byte)b));
                cx.DrawEllipse(color, pen, new Point(center_x, center_y),r1, r2);
            };
        }

        public void DrawCircle(double r, double g, double b, double a, double center_x, double center_y, double radius)
        {
            DrawEllipse(r, g, b, a, center_x, center_y, radius, radius);
        }
    }
}
