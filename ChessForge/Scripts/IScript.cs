using Avalonia;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessForge.Scripts
{
    /// <summary>
    /// 用于实现加载脚本(py...)
    /// </summary>
    public interface IScript
    {
        /// <summary>
        /// 加载脚本
        /// </summary>
        /// <param name="path"></param>
        public void Load(string path);

        public Action<DrawingContext, Rect>? Render();

        /// <summary>
        /// 用于重新加载脚本
        /// </summary>
        public void Reload();

        public void Close();

        public void Click(double x, double y);
        public int IsWin();

        public void NewGame();
    }
}
