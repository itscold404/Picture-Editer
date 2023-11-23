using Microsoft.Win32;
using System.Windows;
using System.Windows.Media.Imaging;

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
        }
        private void btnFire_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Title = "Select an Image File";
            fileDialog.Filter = "Image Files|*.png; *.bmp; *.jpeg; *.jpeg; *.tiff; *.tiff; *.gif; *.ico";
            
            bool? success = fileDialog.ShowDialog();

            if(success == true)
            {
                string f_path = fileDialog.FileName;
                info.Text = f_path;
                BitmapImage myBitmapImage = new BitmapImage();

                myBitmapImage.BeginInit();
                myBitmapImage.UriSource = new Uri(f_path);
                myBitmapImage.DecodePixelWidth = 200;
                myBitmapImage.EndInit();

                Image.Source = myBitmapImage;
            }
        }
    }
}