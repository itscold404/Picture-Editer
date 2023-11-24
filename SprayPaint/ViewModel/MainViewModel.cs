using System.Windows.Media;
using System.Windows.Ink;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Win32;
using System.Windows.Media.Imaging;
using System.Windows;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Controls;


public partial class MainViewModel : ObservableObject
{
    public MainViewModel()
    {
        PenAttributes = new DrawingAttributes
        {
            Color = Colors.Black,
            Height = 2,
            Width = 2
        };

        isEraserPoint = true;
    }

    private bool isEraserPoint;

    [ObservableProperty]
    private DrawingAttributes _penAttributes;

    [ObservableProperty]
    private ImageBrush _canvasBackground;

    [ObservableProperty]
    private InkCanvasEditingMode _editMode;

    [ObservableProperty]
    private string _textt; // for debugging

    [ObservableProperty]
    private bool _isPen;

    [ObservableProperty]
    private bool _isSpray;

    [ObservableProperty]
    private bool _isEraser;

    [ObservableProperty]
    private int _thickValue;
    public int ThickValue
    {
        get => _thickValue;
        set
        {
            if (_thickValue != value)
            {
                _thickValue = value;
                OnPropertyChanged(nameof(ThickValue));
                UpdatePenThickness(value); // Update pen attributes here
            }
        }
    }

    private void UpdatePenThickness(int thickness)
    {
        PenAttributes.Width = thickness;
        PenAttributes.Height = thickness;
    }


    [ObservableProperty]
    private int _opacityValue;

    public enum EditingMode
    {
        Pen, SprayPaint, Eraser
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
            BitmapImage myBitmapImage = new BitmapImage();
            CanvasBackground = new ImageBrush();

            myBitmapImage.BeginInit();
            myBitmapImage.UriSource = new Uri(f_path);
            myBitmapImage.EndInit();

            CanvasBackground.ImageSource = myBitmapImage;
        }
    }

    [RelayCommand]
    private void SelectPen()
    {
        SetEditingMode(EditingMode.Pen);
    }

    [RelayCommand]
    private void SelectSprayPaint()
    {
        SetEditingMode(EditingMode.SprayPaint);
    }

    [RelayCommand]
    private void SelectEraser()
    {
        SetEditingMode(EditingMode.Eraser);
    }

    [RelayCommand]
    private void SelectEraserPoint()
    {
        isEraserPoint = true;

        if (IsEraser)
            EditMode = InkCanvasEditingMode.EraseByPoint;
    }

    [RelayCommand]
    private void SelectEraserFullStroke()
    {
        isEraserPoint = false;

        if (IsEraser)
            EditMode = InkCanvasEditingMode.EraseByStroke;
    }

    [RelayCommand]
    private void SetEditingMode(EditingMode mode)
    {
        IsPen = false;
        IsSpray = false;
        IsEraser = false;

        switch (mode)
        {
            case EditingMode.Pen:
                IsPen = true;
                EditMode = InkCanvasEditingMode.Ink;
                break;

            case EditingMode.SprayPaint:
                IsSpray = true; 
                break;

            case EditingMode.Eraser:    
                IsEraser = true;

                EditMode = isEraserPoint ? InkCanvasEditingMode.EraseByPoint : InkCanvasEditingMode.EraseByStroke;
                break;

            default:
                break;
        }
    }
}


