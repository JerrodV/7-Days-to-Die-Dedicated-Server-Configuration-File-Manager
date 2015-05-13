using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace SevenDaysConfigUI.Helpers.Validation
{
    public static class Validator
    {
        public static bool IsValid(DependencyObject parent)
        {
            if (System.Windows.Controls.Validation.GetHasError(parent))
                return false;

            // Validate all the bindings on the children
            for (int i = 0; i != VisualTreeHelper.GetChildrenCount(parent); ++i)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (!IsValid(child)) { return false; }
            }

            return true;
        }
    }
}
