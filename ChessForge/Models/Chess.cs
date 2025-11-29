using Avalonia.Controls.Notifications;
using ChessForge.Scripts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessForge.Models
{
    public class Chess
    {
        public string Greeting { get; } = "Test Aa Bb Cc Dd";

        public IScript? Script { get; set; }

        public void LoadScript(Uri uri)
        {
            string path = uri.LocalPath;
            string extension = Path.GetExtension(path);
            if(extension == ".py")
            {
                Script = new PyScript(path);
            }
        }

        public void Close()
        {
            Script?.Close();
        }

        public bool InfoBarIsOpaque {  get; set; }
        public bool InfoBarIsCloseable { get; set; }
        public bool InfoBarIsOpen { get; set; } = false;
        public NotificationType InfoBarServerity { get; set; } = NotificationType.Success;
        public string InfoBarTitle { get; set; } = "";
        public string InfoBarMessage { get; set; } = "";

    }
}
