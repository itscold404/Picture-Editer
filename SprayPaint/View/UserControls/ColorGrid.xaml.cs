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
                        Color.FromRgb((byte)Math.Abs((c-r) * 85), (byte)(r * 85), (byte)(c * 85)));
                    colorGrid.Children.Add(currCell);

                    Grid.SetRow(currCell, r);
                    Grid.SetColumn(currCell, c);
                }
            }
        }
    }
}
