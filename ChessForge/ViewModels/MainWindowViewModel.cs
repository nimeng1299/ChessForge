using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using ChessForge.Models;
using ChessForge.services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq.Dynamic.Core.Tokenizer;
using System.Threading.Tasks;

namespace ChessForge.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {

        public Chess chess = new();
        public string Greeting { get
            {
                return this.chess.Greeting;
            } 
        }
        public Action<DrawingContext, Rect>? DrawBoard { get
            {
                return chess.Script?.Render();
            }
        }

        public async void OpenScriptDialog()
        {
            try
            {
                var filesService = App.Current?.Services?.GetService<FilesService>();
                if (filesService is null) throw new NullReferenceException("Missing File Service instance.");

                var file = await filesService.OpenFileAsync();
                if (file is null) return;

                chess.LoadScript(file.Path);

            }
            catch (Exception e)
            {
                Console.WriteLine("OpenScriptDialog exception is {0}", e);
            }
        }

        public void Close()
        {
            chess.Close();
        }
    }
}
