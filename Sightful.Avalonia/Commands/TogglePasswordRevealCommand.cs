using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;

namespace Sightful.Avalonia.Commands;

internal sealed class TogglePasswordRevealCommand : AvaloniaObject, ICommand
{
	public static readonly StyledProperty<TextBox?> TextBoxProperty =
		AvaloniaProperty.Register<TogglePasswordRevealCommand, TextBox?>(nameof(TextBox));
	
#pragma warning disable 67
	public event EventHandler? CanExecuteChanged;
#pragma warning restore 67

	public TextBox? TextBox
	{
		get => GetValue(TextBoxProperty);
		set => SetValue(TextBoxProperty, value);
	}

	public bool CanExecute(object? parameter)
	{
		return true;
	}

	public void Execute(object? parameter)
	{
		if (TextBox == null)
			return;
		TextBox.RevealPassword = !TextBox.RevealPassword;
	}
}