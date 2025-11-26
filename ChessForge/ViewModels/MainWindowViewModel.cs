using ChessForge.Models;

namespace ChessForge.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public Chess chess = new Chess();
        public string Greeting { get
            {
                return this.chess.Greeting;
            } 
        }
    }
}
