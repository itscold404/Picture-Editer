using System.Windows.Media;
using System.Windows.Ink;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Win32;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Controls;
using System.Windows.Input;
using System.Diagnostics;
using System.IO;

/// <summary>
/// Class <c>MainViewModel</c> primary view model to enable image editing tools
/// </summary>
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

    // Flag to determine eraser mode (point or stroke)
    private bool isEraserPoint;

    // The attributes of the pen
    [ObservableProperty]
    private DrawingAttributes _penAttributes;

    // The background image as ImageBrush
    [ObservableProperty]
    private ImageBrush _canvasBackground;

    // The editng mode of the ink canvas
    [ObservableProperty]
    private InkCanvasEditingMode _editMode;

    // Flags to determine the current selected tool
    [ObservableProperty]
    private bool _isPen;

    [ObservableProperty]
    private bool _isSpray;

    [ObservableProperty]
    private bool _isEraser;

    // The value of the thickness of the pen
    private int _thickValue;
    public int ThickValue
    {
        get => _thickValue;
        set
        {
            if (_thickValue != value)
            {
                // If value is an integer
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
                // If input is invalid
                catch 
                {
                    Debug.WriteLine("Invalid thickness input");
                }

            }
        }
    }

    // The value of the opasity/density of the spray paint
    private int _opacityValue;
    public int OpacityValue
    {
        get => _opacityValue;
        set
        {
            if (_opacityValue != value)
            {
                // If value is an integer
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
                // If input is invalid
                catch
                {
                    _opacityValue = 100;
                    Debug.WriteLine("Invalid opacity input");
                }
            }
        }
    }

    // Strokes that make up the InkCanvas
    [ObservableProperty]
    private StrokeCollection _strokes;

    // Random number generator
    private Random rand = new Random();

    public enum EditingMode
    {
        Pen, SprayPaint, Eraser
    }

    /// <summary>
    /// Updates the pen thickness 
    /// </summary>
    /// <param name="thickness">The thickness to set the pen attributes to.</param>
    private void UpdatePenThickness(int thickness)
    {
        PenAttributes.Width = thickness;
        PenAttributes.Height = thickness;
    }

    /// <summary>
    /// Loads an image that the user selects upon the load image button press.
    /// </summary>
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

            try
            {
                myBitmapImage.BeginInit();
                myBitmapImage.UriSource = new Uri(f_path);
                myBitmapImage.EndInit();
                CanvasBackground.ImageSource = myBitmapImage;
            }
            catch 
            {
                Debug.WriteLine("Bad file or invalid file type");
            }
        }
    }

    /// <summary>
    /// Enables the pen tool upon respective button press.
    /// </summary>
    [RelayCommand]
    private void SelectPen()
    {
        SetEditingMode(EditingMode.Pen);
    }

    /// <summary>
    /// Enables the spray paint tool upon respective button press.
    /// </summary>
    [RelayCommand]
    private void SelectSprayPaint()
    {
        SetEditingMode(EditingMode.SprayPaint);
    }

    /// <summary>
    /// Enables the eraser tool upon respective button press.
    /// </summary>
    [RelayCommand]
    private void SelectEraser()
    {
        SetEditingMode(EditingMode.Eraser);
    }

    /// <summary>
    /// Selects the eraser type to be point.
    /// </summary>
    [RelayCommand]
    private void SelectEraserPoint()
    {
        isEraserPoint = true;

        if (IsEraser)
            EditMode = InkCanvasEditingMode.EraseByPoint;
    }

    /// <summary>
    /// Selects the eraser type to be stroke.
    /// </summary>
    [RelayCommand]
    private void SelectEraserFullStroke()
    {
        isEraserPoint = false;

        if (IsEraser)
            EditMode = InkCanvasEditingMode.EraseByStroke;
    }

    /// <summary>
    /// Helper function to enable the selected tool (pen, spray paint, eraser). 
    /// </summary>
    /// <param name="mode">The tool to be enabled.</param>
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

    /// <summary>
    /// Adds spray paint to the ink canvas 
    /// </summary>
    [RelayCommand]
    public void CreateSpray((double, double) pos)
    {
        if(IsSpray == true && OpacityValue > 0)
        {
            double x = pos.Item1;
            double y = pos.Item2;
            double density = OpacityValue / 100.0 / 2;

            StylusPointCollection strokePoints = new StylusPointCollection();

            // Use the random number generator to generate random points to 
            // mimic spray paint
            for (int i = 0; i < ThickValue * density; i++)
            {
                int xOffset = rand.Next(-ThickValue, ThickValue);
                int yOffset = rand.Next(-ThickValue, ThickValue);

                strokePoints.Add(new StylusPoint(x + xOffset, y + yOffset));
            }

            // Create a stroke and add it to ink canva's stroke collection
            Stroke stroke = new Stroke(strokePoints)
            {
                DrawingAttributes = new DrawingAttributes
                {
                    Color = PenAttributes.Color, 
                    Width = 1,
                    Height = 1,
                    IsHighlighter = false
                }
            };
            Strokes.Add(stroke);
        }
    }

    /// <summary>
    /// Change the color of the drawing tool based on what color was 
    /// selected
    /// </summary>
    /// <param name="brushColor">The color of the Background.</param>
    [RelayCommand]
    private void SelectColor(SolidColorBrush brushColor)
    {
        Color color = brushColor.Color;
        string colorName = color.ToString();

        switch (colorName)
        {
            case "#FF000000":
                PenAttributes.Color = Colors.Black;
                break;
            case "#FF808080":
                PenAttributes.Color = Colors.Gray;
                break;
            case "#FFFFFFFF":
                PenAttributes.Color = Colors.White;
                break;
            case "#FF0000FF":
                PenAttributes.Color = Colors.Blue;
                break;
            case "#FF90EE90":
                PenAttributes.Color = Colors.LightGreen;
                break;
            case "#FF800080":
                PenAttributes.Color = Colors.Purple;
                break;
            case "#FFFF0000":
                PenAttributes.Color = Colors.Red;
                break;
            case "#FFFFA500":
                PenAttributes.Color = Colors.Orange;
                break;
            case "#FFFFFF00":
                PenAttributes.Color = Colors.Yellow;
                break;
        }        
    }

    /// <summary>
    /// Saves the image and edits as a PNG file.
    /// Currently does not save file properly.
    /// </summary>
    [RelayCommand]
    private void SaveImage()
    {
        SaveFileDialog dialog = new SaveFileDialog();
        dialog.Filter = "PNG format | *.PNG";
        dialog.Title = "Save Image";

        bool? success = dialog.ShowDialog();

        if (success == true)
        {
            FileStream fs = File.Open(dialog.FileName, FileMode.Create);
            Strokes.Save(fs);
            fs.Close();
        }
    }
}


