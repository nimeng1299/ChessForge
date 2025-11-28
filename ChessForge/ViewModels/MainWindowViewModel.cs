using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using ChessForge.Models;
using ChessForge.services;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using System;
using System.IO;
using System.Linq.Dynamic.Core.Tokenizer;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows.Input;

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
        private Action<DrawingContext, Rect>? _drawBoard;
        public Action<DrawingContext, Rect>? DrawBoard { 
            get =>_drawBoard;
            private set => this.RaiseAndSetIfChanged(ref _drawBoard, value);
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
                DrawBoard = chess.Script?.Render();

            }
            catch (Exception e)
            {
                
            }
        }


        public Action<Rect, Point>? BoardClickAction => OnBoardClick;
        public void OnBoardClick(Rect bounds, Point p)
        {
            try
            {
                chess.Script?.Click(p.X, p.Y);


                DrawBoard = chess.Script?.Render();
            }
            catch (Exception ex)
            {

            }
        }

        public void Close()
        {
            chess.Close();
        }
    }
}
