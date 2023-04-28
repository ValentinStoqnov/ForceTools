using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace ForceTools.ViewModels
{
    internal class DataGridCustom : DataGrid
    {
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Return && e.KeyboardDevice.Modifiers == ModifierKeys.None)
            {
                return;
            }

            base.OnKeyDown(e);
        }
    }
}
