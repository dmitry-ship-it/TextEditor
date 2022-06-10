using System.Windows.Input;

namespace TextEditor.Extensions
{
    /// <summary>
    /// Custom application commands.
    /// </summary>
    public static class ApplicationCommandsExtension
    {
        public static readonly RoutedUICommand SaveAs = new(
            text: "Save file as",
            name: nameof(SaveAs),
            ownerType: typeof(MainWindow),
            inputGestures: new()
            {
                new KeyGesture(Key.S, ModifierKeys.Control | ModifierKeys.Shift)
            });

        public static readonly RoutedUICommand GotoSettings = new(
            text: "Open settings window",
            name: nameof(GotoSettings),
            ownerType: typeof(MainWindow));

        public static readonly RoutedUICommand ExitApplication = new(
            text: "Exit application",
            name: nameof(ExitApplication),
            ownerType: typeof(MainWindow));

        public static readonly RoutedUICommand InsertDateTime = new(
            text: "Date/Time",
            name: nameof(InsertDateTime),
            ownerType: typeof(MainWindow),
            inputGestures: new()
            {
                new KeyGesture(Key.F5)
            });
    }
}
