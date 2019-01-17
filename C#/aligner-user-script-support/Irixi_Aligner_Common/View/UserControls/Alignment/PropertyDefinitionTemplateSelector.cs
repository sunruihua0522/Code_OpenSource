using Irixi_Aligner_Common.Classes.Base;
using System.Windows;
using System.Windows.Controls;

namespace Irixi_Aligner_Common.View.UserControls.AlignmentView
{
    public class PropertyDefinitionTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;

            if (element != null && item != null)
            {
                var pd = item as Property;
                var pd_name = pd.Name;

                if(string.IsNullOrEmpty(pd_name))
                {
                    pd_name = pd.CollectionName;
                }


                // find the DataTemplate named with the property name
                var res = element.TryFindResource(pd_name) as DataTemplate;

                if (res != null)
                    return res;

            }

            return element.TryFindResource("DefaultPropDefTemplate") as DataTemplate;
        }
    }
}
