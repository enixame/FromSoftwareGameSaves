using System.Windows;
using System.Windows.Controls;

namespace FromSoftwareGameSaves.Template
{
    public sealed class ViewDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate InstallationViewDataTemplate { get; set; }
        public DataTemplate ApplicationViewDataTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return base.SelectTemplate(item, container);
        }
    }
}
