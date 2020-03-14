using System.Windows;

namespace FromSoftwareGameSaves.Utils
{
    public static class MessageBoxHelper
    {
        public static MessageBoxResult ShowMessage(string message, string caption, MessageBoxButton button, MessageBoxImage image)
        {
            return Application.Current.MainWindow != null 
                ? MessageBox.Show(Application.Current.MainWindow, message, caption, button, image) 
                : MessageBox.Show(message, caption, button, image);
        }
    }
}
