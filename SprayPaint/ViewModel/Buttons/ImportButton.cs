using CommunityToolkit.Mvvm.ComponentModel;

namespace SprayPaint.ViewModel;

public partial class ImportButton: ObservableObject
{
    [ObservableProperty]
    private string filePath;

}
