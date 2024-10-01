using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ControlsSystem = System.Windows.Controls;

namespace CamScan.Components
{
    public class ButtonSave: ControlsSystem.Control
    {
        static ButtonSave()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ButtonSave), new FrameworkPropertyMetadata(typeof(ButtonSave)));
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(ButtonSave), new PropertyMetadata(null));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
    }
}
