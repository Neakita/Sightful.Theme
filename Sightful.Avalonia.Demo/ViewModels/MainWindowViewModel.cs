using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Sightful.Avalonia.Demo.ViewModels;

public sealed partial class MainWindowViewModel : ViewModel
{
	[ObservableProperty] public partial double Zoom { get; set; } = 1;
	[ObservableProperty, NotifyPropertyChangedFor(nameof(Offset))] public partial double OffsetX { get; set; }
	[ObservableProperty, NotifyPropertyChangedFor(nameof(Offset))] public partial double OffsetY { get; set; }
	public Vector Offset => new(OffsetX, OffsetY);
}