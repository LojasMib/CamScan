﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PrintScanner.Router
{
    public class Navigator : ButtonBase
    {
        static Navigator()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Navigator), new FrameworkPropertyMetadata(typeof(Navigator)));
        }

        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(Navigator), new PropertyMetadata(null));

        public static readonly DependencyProperty TextProperty = 
            DependencyProperty.Register("Text", typeof(string), typeof(Navigator), new PropertyMetadata(null));

        public static readonly DependencyProperty NavUriProperty =
            DependencyProperty.Register("NavUri", typeof(Uri), typeof(Navigator), new PropertyMetadata(null));

        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }
         
        public string Text
        {
            get { return (string)GetValue(TextProperty);}
            set { SetValue(TextProperty, value); }
        }

        
        public Uri NavUri
        {
            get { return (Uri)GetValue(NavUriProperty); }
            set { SetValue(NavUriProperty, value); }
        }
    }

    public class FolderSearch : Control
    {
        static FolderSearch()
        {   
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FolderSearch), new FrameworkPropertyMetadata(typeof(FolderSearch)));
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(FolderSearch), new PropertyMetadata(null));

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(FolderSearch), new PropertyMetadata(null));

        public static readonly DependencyProperty ButtonProperty =
            DependencyProperty.Register("Button", typeof(Button), typeof(FolderSearch), new PropertyMetadata(null));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public Button Button
        {
            get { return (Button)GetValue(ButtonProperty); }
            set { SetValue(ButtonProperty, value); }
        }


    }

}