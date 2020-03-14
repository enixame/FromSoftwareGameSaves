using System.Windows;
using System.Windows.Controls;

namespace FromSoftwareGameSaves.Behaviors
{
    public class TextBoxBehavior
    {
        public static bool GetSelectAllTextOnFocus(TextBox textBox)
        {
            return (bool)textBox.GetValue(SelectAllTextOnFocusProperty);
        }

        public static void SetSelectAllTextOnFocus(TextBox textBox, bool value)
        {
            textBox.SetValue(SelectAllTextOnFocusProperty, value);
        }

        public static readonly DependencyProperty SelectAllTextOnFocusProperty =
            DependencyProperty.RegisterAttached(
                "SelectAllTextOnFocus",
                typeof(bool),
                typeof(TextBoxBehavior),
                new UIPropertyMetadata(false, OnSelectAllTextOnFocusChanged));

        private static void OnSelectAllTextOnFocusChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs propertyChangedEventArgs)
        {
            var textBox = dependencyObject as TextBox;
            if (textBox == null) return;

            if (propertyChangedEventArgs.NewValue is bool == false) return;

            if ((bool)propertyChangedEventArgs.NewValue)
            {
                textBox.GotFocus += OnGotFocus;
            }
            else
            {
                textBox.GotFocus -= OnGotFocus;
            }
        }

        private static void OnGotFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            var textBox = routedEventArgs.OriginalSource as TextBox;
            textBox?.SelectAll();
        } 
    }
}
