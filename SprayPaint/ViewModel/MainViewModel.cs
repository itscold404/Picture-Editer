using System.Windows.Media;
using System.Windows.Ink;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Win32;
using System.Windows.Media.Imaging;
using System.Windows;
using CommunityToolkit.Mvvm.Input;


public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private DrawingAttributes _penAttributes;

    [ObservableProperty]
    private string _filePath;

    [ObservableProperty]
    private BitmapImage _imageSource;

    public MainViewModel()
    {
        PenAttributes = new DrawingAttributes
        {
            Color = Colors.Black,
            Height = 2,
            Width = 2
        };
    }

    [RelayCommand]
    private void LoadImage()
    {
        OpenFileDialog fileDialog = new OpenFileDialog();
        fileDialog.Title = "Select an Image File";
        fileDialog.Filter = "Image Files|*.png; *.bmp; *.jpeg; *.jpeg; *.tiff; *.tiff; *.gif; *.ico";

        bool? success = fileDialog.ShowDialog();

        if (success == true)
        {
            string f_path = fileDialog.FileName;
            FilePath = f_path;
            BitmapImage myBitmapImage = new BitmapImage();

            myBitmapImage.BeginInit();
            myBitmapImage.UriSource = new Uri(f_path);
            myBitmapImage.DecodePixelWidth = 200;
            myBitmapImage.EndInit();

            ImageSource = myBitmapImage;
        }
    }
}


