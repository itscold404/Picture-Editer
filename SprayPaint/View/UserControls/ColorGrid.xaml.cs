using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SprayPaint.View.UserControls
{
    /// <summary>
    /// Interaction logic for ColorGrid.xaml
    /// </summary>
    public partial class ColorGrid : UserControl
    {
        public ColorGrid()
        {
            InitializeComponent();
            fillGridColors();
        }

        private void fillGridColors()
        {
            // for row
            for (int r = 0; r < 3; r++)
            {
                // for column
                for (int c = 0; c < 6; c++)
                {
                    // fill the color for the current cell
                    Border currCell = new Border();
                    currCell.Background = new SolidColorBrush(
                        Color.FromRgb((byte)Math.Abs((c-r) * 42), (byte)(r * 85), (byte)(c * 42)));
                    colorGrid.Children.Add(currCell);

                    currCell.MouseDown += ColorGrid_MouseDown;
                    Grid.SetRow(currCell, r);
                    Grid.SetColumn(currCell, c);
                }
            }
        }

        private void ColorGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Border currCell = sender as Border;
            SolidColorBrush brushColor = currCell.Background as SolidColorBrush;
            Color selectedColor = brushColor.Color;

            colorSelected.Text = selectedColor.ToString();
        }
    }
}
