using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
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

                ShowInfoBarMessage(NotificationType.Success, "success",$"Script '{Path.GetFileName(file.Path.LocalPath)}' loaded.", true, false);

            }
            catch (Exception e)
            {
                
            }
        }

        public void NewGame()
        {
            chess.Script?.NewGame();
            DrawBoard = chess.Script?.Render();
        }

        // Board click action
        public Action<Rect, Point>? BoardClickAction => OnBoardClick;
        public void OnBoardClick(Rect bounds, Point p)
        {
            try
            {
                chess.Script?.Click(p.X, p.Y);

                DrawBoard = chess.Script?.Render();

                if(chess.Script?.IsWin() != 0)
                {
                    ShowInfoBarMessage(NotificationType.Success, "Game Over", "You win!", true, false);
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void Close()
        {
            chess.Close();
        }

        // Info bar properties
        public bool InfoBarIsOpaque
        {
            get => chess.InfoBarIsOpaque;
            set
            {
                chess.InfoBarIsOpaque = value;
                this.RaisePropertyChanged(nameof(InfoBarIsOpaque));
            }
        }
        public bool InfoBarIsCloseable
        {
            get => chess.InfoBarIsCloseable;
            set
            {
                chess.InfoBarIsCloseable = value;
                this.RaisePropertyChanged(nameof(InfoBarIsCloseable));
            }
        }
        public bool InfoBarIsOpen
        {
            get => chess.InfoBarIsOpen;
            set
            {
                chess.InfoBarIsOpen = value;
                this.RaisePropertyChanged(nameof(InfoBarIsOpen));
            }
        }
        public NotificationType InfoBarServerity
        {
            get => chess.InfoBarServerity;
            set
            {
                chess.InfoBarServerity = value;
                this.RaisePropertyChanged(nameof(InfoBarServerity));
            }
        }
        public string InfoBarTitle
        {
            get => chess.InfoBarTitle;
            set
            {
                chess.InfoBarTitle = value;
                this.RaisePropertyChanged(nameof(InfoBarTitle));
            }
        }
        public string InfoBarMessage
        {
            get => chess.InfoBarMessage;
            set
            {
                chess.InfoBarMessage = value;
                this.RaisePropertyChanged(nameof(InfoBarMessage));
            }
        }

        public async void ShowInfoBarMessage(NotificationType severity, string title, string message, bool isCloseable, bool isOpaque)
        {
            InfoBarServerity = severity;
            InfoBarMessage = message;
            InfoBarIsCloseable = isCloseable;
            InfoBarIsOpaque = isOpaque;
            InfoBarIsOpen = true;
            await Task.Delay(5000);
            InfoBarIsOpen = false;
        }
    }
}
