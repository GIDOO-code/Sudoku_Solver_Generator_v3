using System;
using System.Collections.Generic;
using static System.Diagnostics.Debug;
using System.Globalization;

using System.Windows;
using System.Windows.Media;
using System.Windows.Input;

using System.Resources;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace GIDOO_space{
    static public class GNPZExtender{
        static readonly int[] __BC=new int[512]; //Number of 1's in binary expression 0-511

        static GNPZExtender(){
            for(int n=0; n<512; n++ ) __BC[n] = (n+512).BitCount()-1;           //avoid recursion with "+512"
        }

        static public void Swap<T>( ref T a, ref T b ){ T w=b; b=a; a=w; }

        static public void EnumVisual( Visual myVisual, ref List<Visual> Vis ){
            for(int i=0; i<VisualTreeHelper.GetChildrenCount(myVisual); i++){
                // Retrieve child visual at specified index value.
                Visual childVisual = (Visual)VisualTreeHelper.GetChild(myVisual, i);
                Vis.Add(childVisual);
                WriteLine(childVisual.ToString());
                // Do processing of the child visual object.

                // Enumerate children of the child visual object.
                EnumVisual(childVisual, ref Vis);
            }
        }

        static public bool Inner( this MouseButtonEventArgs e, FrameworkElement Q ){
            Point pt = e.GetPosition(Q);
            if(pt.X<=0.0 || pt.Y<=0.0)  return false;
            if(pt.X>=Q.Width  || pt.Y>=Q.Height)  return false;
            return true;
        }

        static public double ToDouble( this string st ) =>  double.Parse( st );
        static public float ToFloat( this string st )   => float.Parse( st );

        static public int ToInt( this string st )       => int.Parse( st );

        static public int ToInt( this char ch )         => int.Parse( ch.ToString() );

        static public int DifSet( this int A, int B )   => (int)(A&(B^0xFFFFFFFF)); 
        static public long DifSet( this long A, long B ) => (long)(A&(B^0x7FFFFFFFFFFFFFFF));
        static public ulong DifSet( this ulong A, ulong B ) => (ulong)(A&(B^0xFFFFFFFFFFFFFFFF));

        static public string ToBitString( this int noB, int ncc ){
            string st="";
            for(int n=0; n<ncc; n++ ){
                st += (((noB>>n)&1)!=0)? (n+1).ToString(): "."; 
            }
            return st;
        } 

        static public int BitSet( this int X, int n )   => X |= (1<<n);

        static public int BitReset( this int X, int n ) => X &= (1<<n)^0x7FFFFFFF;

        static public int BitCount( this int nF ){      //by Hacker's Delight
            if( (nF&0x7FFFFE00)==0 )  return __BC[nF];  //for 9 bits or less, refer to the table. fast.
            int x = nF;
            x = (x&0x55555555) + ((x>>1)&0x55555555);
            x = (x&0x33333333) + ((x>>2)&0x33333333);
            x = (x&0x0F0F0F0F) + ((x>>4)&0x0F0F0F0F);
            x = (x&0x00FF00FF) + ((x>>8)&0x00FF00FF);
            x = (x&0x0000FFFF) + ((x>>16)&0x0000FFFF);
            return x;
        }
        static public int BitCount( this uint nF ){     //by Hacker's Delight
            if( (nF&0x7FFFFE00)==0 )  return __BC[nF];  //for 9 bits or less, refer to the table. fast.
            uint x = nF;
            x = (x&0x55555555) + ((x>>1)&0x55555555);
            x = (x&0x33333333) + ((x>>2)&0x33333333);
            x = (x&0x0F0F0F0F) + ((x>>4)&0x0F0F0F0F);
            x = (x&0x00FF00FF) + ((x>>8)&0x00FF00FF);
            x = (x&0x0000FFFF) + ((x>>16)&0x0000FFFF);
            return (int)x;
        }
        static public bool IsNumeric( this string stTarget ){ //Test whether a string is a number
            int nNullable;
            return int.TryParse( stTarget, NumberStyles.Any, null, out nNullable );
        }
        static public bool IsNumeric( this char chTarget ){ //Test whether a string is a number
            int nNullable;
            return int.TryParse( chTarget.ToString(), NumberStyles.Any, null, out nNullable );
        }
        static public List<T> GetControlsCollection<T>(object parent) where T :DependencyObject{
            //WPF - Getting a collection of all controls for a given type on a Page
            //http://stackoverflow.com/questions/7153248/wpf-getting-a-collection-of-all-controls-for-a-given-type-on-a-page
            //using: List<Button> buttons = GetControlsCollection<Button>(yourPage);
            List<T> logicalCollection = new List<T>();
            GetControlsCollection( parent as DependencyObject, logicalCollection );
            return logicalCollection;
        }
        static private void GetControlsCollection<T>( DependencyObject parent, List<T> logicalCollection ) where T: DependencyObject{
            var children = LogicalTreeHelper.GetChildren(parent);
            foreach (object child in children){
                if(child is DependencyObject){
                    DependencyObject depChild = child as DependencyObject;
                    if( child is T )logicalCollection.Add(child as T);
                    GetControlsCollection(depChild, logicalCollection);
                }
            }
        }

        static public string encapsulate( string st ){
            char[] sep=new Char[]{ ' ', ',', '\t' };
            string[] stL;
            stL=st.Split(sep);
            if( stL[0]!=st) return "\"" + st + "\"";
            else return st;
        }

        static private char[] sepDef=new Char[]{ ' ', ',', '\t' };
        static public string[] SplitEx( this string st, char[] sep=null ){
            string[] eLst = st.Split(sep,StringSplitOptions.RemoveEmptyEntries);
            for( int k=0; k<eLst.Length; k++ )  eLst[k] = eLst[k].Replace("\"","");
            return eLst;
        }


        static public void ProcessExe( string url ){
            // Process.Start for URLs on .NET Core
            // https://brockallen.com/2016/09/24/process-start-for-urls-on-net-core/
            try{  
                Process.Start(url);  
            }  
            catch{  
                if( RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ){      //for Windows  
                    url = url.Replace("&", "^&");  
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow=true });  
                }  
                else if( RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ){   //for Linux  
                    Process.Start("xdg-open", url);  
                }  
                else if( RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ){     //for Mac  
                    Process.Start("open", url);  
                }  
                else{ throw; }  
            }  

        }
    }
}