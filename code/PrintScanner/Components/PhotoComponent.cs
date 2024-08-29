using CamScan.Router;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PrimitiveSystem = System.Windows.Controls.Primitives;
using ControlsSystem = System.Windows.Controls;
using System.Threading.Tasks;
using System.Windows;

namespace CamScan.Components
{
    public class ButtonOfPhoto: ControlsSystem.Control
    {
        static ButtonOfPhoto()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ButtonOfPhoto), new FrameworkPropertyMetadata(typeof(ButtonOfPhoto)));
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(ButtonOfPhoto), new PropertyMetadata(null));

         public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
    }

    public class PhotoFolderOptions : ControlsSystem.RadioButton
    {
        static PhotoFolderOptions()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PhotoFolderOptions), new FrameworkPropertyMetadata(typeof(PhotoFolderOptions)));
        }


        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(PhotoFolderOptions), new PropertyMetadata(null));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
    }

}
