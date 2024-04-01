using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;

namespace Sightful.Avalonia.Theme.Assists;

public static class WindowAssist
{
	private sealed class WindowCommand : ICommand
	{
		public event EventHandler? CanExecuteChanged;

		public WindowCommand(Action<Window> action)
		{
			_action = action;
		}
		
		public bool CanExecute(object? parameter)
		{
			return parameter != null;
		}

		public void Execute(object? parameter)
		{
			if (parameter == null)
				return;
			var element = (Visual)parameter;
			var topLevel = TopLevel.GetTopLevel(element);
			if (topLevel == null)
				return;
			var window = (Window)topLevel;
			_action(window);
		}
		
		private readonly Action<Window> _action;
	}

	public static ICommand MinimizeWindowCommand { get; } =
		new WindowCommand(window => window.WindowState = WindowState.Minimized);
	public static ICommand MaximizeWindowCommand { get; } =
		new WindowCommand(window => window.WindowState = WindowState.Maximized);
	public static ICommand CloseWindowCommand { get; } =
		new WindowCommand(window => window.Close());

	#region LeftContent

	public static readonly AvaloniaProperty<object?> LeftContentProperty =
		AvaloniaProperty.RegisterAttached<Window, object?>("LeftContent", typeof(WindowAssist));

	public static object? GetLeftContent(AvaloniaObject element)
	{
		return element.GetValue(LeftContentProperty);
	}

	public static void SetLeftContent(AvaloniaObject element, object? value)
	{
		element.SetValue(LeftContentProperty, value);
	}

	#endregion

	#region TopContent

	public static readonly AvaloniaProperty<object?> TopContentProperty =
		AvaloniaProperty.RegisterAttached<Window, object?>("TopContent", typeof(WindowAssist));

	public static object? GetTopContent(AvaloniaObject element)
	{
		return element.GetValue(TopContentProperty);
	}

	public static void SetTopContent(AvaloniaObject element, object? value)
	{
		element.SetValue(TopContentProperty, value);
	}

	#endregion
}