using System.Windows;
using Torsion.Apps.IconConverter.ViewModels;

namespace Torsion.Apps.IconConverter.Views
{
    public partial class MainWindow : Window
    {
        private IconViewModel VM;
        public MainWindow()
        {
            InitializeComponent();
            VM = new IconViewModel(this);
            DataContext = VM;
        }
    }
}