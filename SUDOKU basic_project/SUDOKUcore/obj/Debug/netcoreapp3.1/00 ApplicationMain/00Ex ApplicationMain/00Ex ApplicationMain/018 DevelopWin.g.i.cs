﻿#pragma checksum "..\..\..\..\..\..\00 ApplicationMain\00Ex ApplicationMain\00Ex ApplicationMain\018 DevelopWin.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "E7145EFAF288105DE7D3867378BDABB758690082"
//------------------------------------------------------------------------------
// <auto-generated>
//     このコードはツールによって生成されました。
//     ランタイム バージョン:4.0.30319.42000
//
//     このファイルへの変更は、以下の状況下で不正な動作の原因になったり、
//     コードが再生成されるときに損失したりします。
// </auto-generated>
//------------------------------------------------------------------------------

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


namespace GNPXcore {
    
    
    /// <summary>
    /// DevelopWin
    /// </summary>
    public partial class DevelopWin : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 1 "..\..\..\..\..\..\00 ApplicationMain\00Ex ApplicationMain\00Ex ApplicationMain\018 DevelopWin.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal GNPXcore.DevelopWin devWin;
        
        #line default
        #line hidden
        
        
        #line 6 "..\..\..\..\..\..\00 ApplicationMain\00Ex ApplicationMain\00Ex ApplicationMain\018 DevelopWin.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label GNPXGNPX;
        
        #line default
        #line hidden
        
        
        #line 9 "..\..\..\..\..\..\00 ApplicationMain\00Ex ApplicationMain\00Ex ApplicationMain\018 DevelopWin.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button devWinClose;
        
        #line default
        #line hidden
        
        
        #line 18 "..\..\..\..\..\..\00 ApplicationMain\00Ex ApplicationMain\00Ex ApplicationMain\018 DevelopWin.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Image dev_GBoard;
        
        #line default
        #line hidden
        
        
        #line 20 "..\..\..\..\..\..\00 ApplicationMain\00Ex ApplicationMain\00Ex ApplicationMain\018 DevelopWin.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button SaveBitMap;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "5.0.10.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/SUDOKU_Basic;component/00%20applicationmain/00ex%20applicationmain/00ex%20applic" +
                    "ationmain/018%20developwin.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\..\..\00 ApplicationMain\00Ex ApplicationMain\00Ex ApplicationMain\018 DevelopWin.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "5.0.10.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.devWin = ((GNPXcore.DevelopWin)(target));
            
            #line 4 "..\..\..\..\..\..\00 ApplicationMain\00Ex ApplicationMain\00Ex ApplicationMain\018 DevelopWin.xaml"
            this.devWin.IsVisibleChanged += new System.Windows.DependencyPropertyChangedEventHandler(this.devWin_IsVisibleChanged);
            
            #line default
            #line hidden
            return;
            case 2:
            this.GNPXGNPX = ((System.Windows.Controls.Label)(target));
            return;
            case 3:
            this.devWinClose = ((System.Windows.Controls.Button)(target));
            
            #line 9 "..\..\..\..\..\..\00 ApplicationMain\00Ex ApplicationMain\00Ex ApplicationMain\018 DevelopWin.xaml"
            this.devWinClose.Click += new System.Windows.RoutedEventHandler(this.devWinClose_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.dev_GBoard = ((System.Windows.Controls.Image)(target));
            return;
            case 5:
            this.SaveBitMap = ((System.Windows.Controls.Button)(target));
            
            #line 22 "..\..\..\..\..\..\00 ApplicationMain\00Ex ApplicationMain\00Ex ApplicationMain\018 DevelopWin.xaml"
            this.SaveBitMap.Click += new System.Windows.RoutedEventHandler(this.SaveBitMap_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

