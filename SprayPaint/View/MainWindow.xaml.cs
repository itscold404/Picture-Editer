using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SprayPaint
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainViewModel vm = new MainViewModel();
            DataContext = vm;
        }

        private void InkCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed)
            {
                var vm = DataContext as MainViewModel;
                var pos = e.GetPosition(sender as InkCanvas);
                vm?.CreateSprayCommand.Execute((pos.X, pos.Y));
            }
        }
    }
}