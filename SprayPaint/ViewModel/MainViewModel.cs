using System.Windows.Media;
using System.Windows.Ink;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Win32;
using System.Windows.Media.Imaging;
using System.Windows;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Controls;
using System.Windows.Input;


public partial class MainViewModel : ObservableObject
{
    public MainViewModel()
    {
        Strokes = new StrokeCollection();

        // Setting default/on startup values
        PenAttributes = new DrawingAttributes
        {
            Color = Colors.Black,
            Height = 2,
            Width = 2,
        };
        ThickValue = 5;
        OpacityValue = 100;
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
    private bool _isPen;

    [ObservableProperty]
    private bool _isSpray;

    [ObservableProperty]
    private bool _isEraser;

    private int _thickValue;
    public int ThickValue
    {
        get => _thickValue;
        set
        {
            if (_thickValue != value)
            {
                try
                {
                    if (value > 50)
                        _thickValue = 50;
                    else if (value < 0)
                        _thickValue = 0;
                    else
                        _thickValue = value;

                    OnPropertyChanged(nameof(ThickValue));
                    UpdatePenThickness(value); // Update pen attributes here
                }
                catch 
                {
                    Console.WriteLine("Invalid thickness input");
                }

            }
        }
    }

    private int _opacityValue;

    public int OpacityValue
    {
        get => _opacityValue;
        set
        {
            if (_opacityValue != value)
            {
                try
                {
                    if (value > 100)
                        _opacityValue = 100;
                    else if (value < 0)
                        _opacityValue = 0;
                    else
                        _opacityValue = value;

                    OnPropertyChanged(nameof(OpacityValue));
                }
                catch
                {
                    _opacityValue = 100;
                    Console.WriteLine("Invalid opacity input");
                }
            }
        }
    }


    private Random  rand = new Random();

    [ObservableProperty]
    private StrokeCollection _strokes;

    public enum EditingMode
    {
        Pen, SprayPaint, Eraser
    }

    private void UpdatePenThickness(int thickness)
    {
        PenAttributes.Width = thickness;
        PenAttributes.Height = thickness;
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
                EditMode = InkCanvasEditingMode.None;
                break;

            case EditingMode.Eraser:    
                IsEraser = true;

                EditMode = isEraserPoint ? InkCanvasEditingMode.EraseByPoint : InkCanvasEditingMode.EraseByStroke;
                break;

            default:
                break;
        }
    }
    [RelayCommand]
    public void CreateSpray((double, double) pos)
    {
        if(IsSpray == true && OpacityValue > 0)
        {
            double x = pos.Item1;
            double y = pos.Item2;
            double density = OpacityValue / 100.0;

            StylusPointCollection strokePoints = new StylusPointCollection();
            for (int i = 0; i < ThickValue * density; i++)
            {
                int xOffset = rand.Next(-ThickValue, ThickValue);
                int yOffset = rand.Next(-ThickValue, ThickValue);

                strokePoints.Add(new StylusPoint(x + xOffset, y + yOffset));
            }

            Stroke stroke = new Stroke(strokePoints)
            {
                DrawingAttributes = new DrawingAttributes
                {
                    Color = Colors.Black, //    TODO: CHANGE WHEN COLORS IMPLEMENTED
                    Width = 1,
                    Height = 1,
                    IsHighlighter = false
                }
            };

            Strokes.Add(stroke);
        }
    }
}


