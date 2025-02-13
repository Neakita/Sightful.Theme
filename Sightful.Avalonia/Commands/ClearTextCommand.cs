using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;

namespace Sightful.Avalonia.Commands;

internal sealed class ClearTextCommand : AvaloniaObject, ICommand
{
	public static readonly StyledProperty<bool> CanExecuteProperty =
		AvaloniaProperty.Register<ClearTextCommand, bool>(nameof(CanExecute));

	public static readonly StyledProperty<TextBox?> TextBoxProperty =
		AvaloniaProperty.Register<ClearTextCommand, TextBox?>(nameof(TextBox));

	public event EventHandler? CanExecuteChanged;

	public bool CanExecute
	{
		get => GetValue(CanExecuteProperty);
		set => SetValue(CanExecuteProperty, value);
	}

	public TextBox? TextBox
	{
		get => GetValue(TextBoxProperty);
		set => SetValue(TextBoxProperty, value);
	}

	public void Execute(object? parameter)
	{
		TextBox?.Clear();
	}

	bool ICommand.CanExecute(object? parameter) => CanExecute;

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		if (change.Property == CanExecuteProperty)
			CanExecuteChanged?.Invoke(this, EventArgs.Empty);
		base.OnPropertyChanged(change);
	}
}