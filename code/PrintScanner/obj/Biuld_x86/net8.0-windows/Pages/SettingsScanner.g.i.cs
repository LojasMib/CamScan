﻿#pragma checksum "..\..\..\..\Pages\SettingsScanner.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "ABD3AE511D197DC22F138286047D0A2551F90B6A"
//------------------------------------------------------------------------------
// <auto-generated>
//     O código foi gerado por uma ferramenta.
//     Versão de Tempo de Execução:4.0.30319.42000
//
//     As alterações ao arquivo poderão causar comportamento incorreto e serão perdidas se
//     o código for gerado novamente.
// </auto-generated>
//------------------------------------------------------------------------------

using PrintScanner.Pages;
using PrintScanner.Router;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace PrintScanner.Pages {
    
    
    /// <summary>
    /// SettingsScanner
    /// </summary>
    public partial class SettingsScanner : System.Windows.Controls.Page, System.Windows.Markup.IComponentConnector {
        
        
        #line 18 "..\..\..\..\Pages\SettingsScanner.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Grid SetSelect;
        
        #line default
        #line hidden
        
        
        #line 28 "..\..\..\..\Pages\SettingsScanner.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal PrintScanner.Router.FolderSearch DriverScanner;
        
        #line default
        #line hidden
        
        
        #line 29 "..\..\..\..\Pages\SettingsScanner.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal PrintScanner.Router.FolderSearch FolderDocClientes;
        
        #line default
        #line hidden
        
        
        #line 30 "..\..\..\..\Pages\SettingsScanner.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal PrintScanner.Router.FolderSearch ConfissaoDivida;
        
        #line default
        #line hidden
        
        
        #line 31 "..\..\..\..\Pages\SettingsScanner.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal PrintScanner.Router.FolderSearch Despesas;
        
        #line default
        #line hidden
        
        
        #line 32 "..\..\..\..\Pages\SettingsScanner.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal PrintScanner.Router.FolderSearch Outros;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "8.0.4.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/CamScan;V1.0.0.0;component/pages/settingsscanner.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\Pages\SettingsScanner.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "8.0.4.0")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal System.Delegate _CreateDelegate(System.Type delegateType, string handler) {
            return System.Delegate.CreateDelegate(delegateType, this, handler);
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "8.0.4.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.SetSelect = ((System.Windows.Controls.Grid)(target));
            return;
            case 2:
            this.DriverScanner = ((PrintScanner.Router.FolderSearch)(target));
            return;
            case 3:
            this.FolderDocClientes = ((PrintScanner.Router.FolderSearch)(target));
            return;
            case 4:
            this.ConfissaoDivida = ((PrintScanner.Router.FolderSearch)(target));
            return;
            case 5:
            this.Despesas = ((PrintScanner.Router.FolderSearch)(target));
            return;
            case 6:
            this.Outros = ((PrintScanner.Router.FolderSearch)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}
