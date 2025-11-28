using Avalonia;
using Avalonia.Input;
using Avalonia.Media;
using ChessForge.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChessForge.Controls
{
    public class Board: Avalonia.Controls.Control
    {
        public static readonly StyledProperty<Action<DrawingContext, Rect>?> RenderActionProperty = AvaloniaProperty.Register<Board, Action<DrawingContext, Rect>?>(nameof(RenderAction));

        public Action<DrawingContext, Rect>? RenderAction
        {
            get => GetValue(RenderActionProperty);
            set => SetValue(RenderActionProperty, value);
        }

        public Board()
        {
            // 确保控件可以被命中
            IsHitTestVisible = true;
            Focusable = true;

            this.GetObservable(RenderActionProperty).Subscribe(_ => InvalidateVisual());
            this.GetObservable(DataContextProperty).Subscribe(_ => InvalidateVisual());
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);
            // 绘制的是红色边框(用于参考)
            var rect = new Rect(Bounds.Size);
            var pen = new Pen(Brushes.Red, 3);
            //一定要画一个透明的矩形，否则无法接收鼠标事件
            context.DrawRectangle(Brushes.Transparent, pen, rect);

            var action = RenderAction ?? (DataContext as MainWindowViewModel)?.DrawBoard;
            action?.Invoke(context, Bounds);
        }

        public static readonly StyledProperty<Action<Rect, Point>?> ClickActionProperty =
                   AvaloniaProperty.Register<Board, Action<Rect, Point>?>(nameof(ClickAction));

        public Action<Rect, Point>? ClickAction
        {
            get => GetValue(ClickActionProperty);
            set => SetValue(ClickActionProperty, value);
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);

            var pos = e.GetPosition(this);

            ClickAction?.Invoke(Bounds, pos);


        }

    }
}
