﻿#pragma checksum "..\..\..\Pages\EnrollmentPage.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "39B2BC2D125E13E18AEF0EB40CEE400602A57F43"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using ScoreManager.Pages;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.Chromes;
using Xceed.Wpf.Toolkit.Core.Converters;
using Xceed.Wpf.Toolkit.Core.Input;
using Xceed.Wpf.Toolkit.Core.Media;
using Xceed.Wpf.Toolkit.Core.Utilities;
using Xceed.Wpf.Toolkit.Panels;
using Xceed.Wpf.Toolkit.Primitives;
using Xceed.Wpf.Toolkit.PropertyGrid;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using Xceed.Wpf.Toolkit.PropertyGrid.Commands;
using Xceed.Wpf.Toolkit.PropertyGrid.Converters;
using Xceed.Wpf.Toolkit.PropertyGrid.Editors;
using Xceed.Wpf.Toolkit.Zoombox;


namespace ScoreManager.Pages {
    
    
    /// <summary>
    /// EnrollmentPage
    /// </summary>
    public partial class EnrollmentPage : System.Windows.Controls.Page, System.Windows.Markup.IComponentConnector {
        
        
        #line 11 "..\..\..\Pages\EnrollmentPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid dgStudents;
        
        #line default
        #line hidden
        
        
        #line 51 "..\..\..\Pages\EnrollmentPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnRefresh;
        
        #line default
        #line hidden
        
        
        #line 57 "..\..\..\Pages\EnrollmentPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnBrowse;
        
        #line default
        #line hidden
        
        
        #line 63 "..\..\..\Pages\EnrollmentPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnSave;
        
        #line default
        #line hidden
        
        
        #line 71 "..\..\..\Pages\EnrollmentPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Xceed.Wpf.Toolkit.WatermarkTextBox tbStudentName;
        
        #line default
        #line hidden
        
        
        #line 72 "..\..\..\Pages\EnrollmentPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Xceed.Wpf.Toolkit.WatermarkTextBox tbProgram;
        
        #line default
        #line hidden
        
        
        #line 73 "..\..\..\Pages\EnrollmentPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Xceed.Wpf.Toolkit.WatermarkTextBox tbStudentNo;
        
        #line default
        #line hidden
        
        
        #line 74 "..\..\..\Pages\EnrollmentPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnOk;
        
        #line default
        #line hidden
        
        
        #line 75 "..\..\..\Pages\EnrollmentPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnRemove;
        
        #line default
        #line hidden
        
        
        #line 76 "..\..\..\Pages\EnrollmentPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Xceed.Wpf.Toolkit.WatermarkTextBox tbEmail;
        
        #line default
        #line hidden
        
        
        #line 78 "..\..\..\Pages\EnrollmentPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ProgressBar progressBar;
        
        #line default
        #line hidden
        
        
        #line 79 "..\..\..\Pages\EnrollmentPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock txtStatus;
        
        #line default
        #line hidden
        
        
        #line 80 "..\..\..\Pages\EnrollmentPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock txtSheetName;
        
        #line default
        #line hidden
        
        
        #line 82 "..\..\..\Pages\EnrollmentPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock txtExamTerm;
        
        #line default
        #line hidden
        
        
        #line 83 "..\..\..\Pages\EnrollmentPage.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnCancelEdit;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/ScoreManager;component/pages/enrollmentpage.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Pages\EnrollmentPage.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.dgStudents = ((System.Windows.Controls.DataGrid)(target));
            
            #line 11 "..\..\..\Pages\EnrollmentPage.xaml"
            this.dgStudents.MouseDoubleClick += new System.Windows.Input.MouseButtonEventHandler(this.DgStudents_MouseDoubleClick);
            
            #line default
            #line hidden
            return;
            case 2:
            this.btnRefresh = ((System.Windows.Controls.Button)(target));
            
            #line 51 "..\..\..\Pages\EnrollmentPage.xaml"
            this.btnRefresh.Click += new System.Windows.RoutedEventHandler(this.BtnRefresh_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.btnBrowse = ((System.Windows.Controls.Button)(target));
            
            #line 57 "..\..\..\Pages\EnrollmentPage.xaml"
            this.btnBrowse.Click += new System.Windows.RoutedEventHandler(this.BtnBrowse_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.btnSave = ((System.Windows.Controls.Button)(target));
            
            #line 63 "..\..\..\Pages\EnrollmentPage.xaml"
            this.btnSave.Click += new System.Windows.RoutedEventHandler(this.BtnSave_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.tbStudentName = ((Xceed.Wpf.Toolkit.WatermarkTextBox)(target));
            return;
            case 6:
            this.tbProgram = ((Xceed.Wpf.Toolkit.WatermarkTextBox)(target));
            return;
            case 7:
            this.tbStudentNo = ((Xceed.Wpf.Toolkit.WatermarkTextBox)(target));
            return;
            case 8:
            this.btnOk = ((System.Windows.Controls.Button)(target));
            
            #line 74 "..\..\..\Pages\EnrollmentPage.xaml"
            this.btnOk.Click += new System.Windows.RoutedEventHandler(this.BtnOk_Click);
            
            #line default
            #line hidden
            return;
            case 9:
            this.btnRemove = ((System.Windows.Controls.Button)(target));
            
            #line 75 "..\..\..\Pages\EnrollmentPage.xaml"
            this.btnRemove.Click += new System.Windows.RoutedEventHandler(this.BtnRemove_Click);
            
            #line default
            #line hidden
            return;
            case 10:
            this.tbEmail = ((Xceed.Wpf.Toolkit.WatermarkTextBox)(target));
            return;
            case 11:
            this.progressBar = ((System.Windows.Controls.ProgressBar)(target));
            return;
            case 12:
            this.txtStatus = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 13:
            this.txtSheetName = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 14:
            this.txtExamTerm = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 15:
            this.btnCancelEdit = ((System.Windows.Controls.Button)(target));
            
            #line 83 "..\..\..\Pages\EnrollmentPage.xaml"
            this.btnCancelEdit.Click += new System.Windows.RoutedEventHandler(this.BtnCancelEdit_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}
