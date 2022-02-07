using System;
using System.Collections.Generic;
using System.Linq;

using static System.Math;
using static System.Diagnostics.Debug;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using GIDOO_space;

namespace GNPXcore{
    using sysWin=System.Windows;

    public partial class NuPz_Win{  //Puzzle Transform
        private PuzzleTrans pPTrans{ get=> GNP00.PTrans; }
        private int         noPChg = -1;

       #region Number change
        private void btnNumChange_Click( object sender, RoutedEventArgs e ){
        }
        private void btnNumChangeDone_Click( object sender, RoutedEventArgs e ){
            GNP00.GSmode = "tabACreate";
            noPChg = -1;
            _Display_GB_GBoard();
            PB_GBoard.MouseDown -= new MouseButtonEventHandler(PTrans_PB_GBoard_MouseLeftButtonDown);
        }
        private void PTrans_PB_GBoard_MouseLeftButtonDown( object sender, MouseButtonEventArgs e ){  
            int rcX = _Get_PB_GBoardRCNum();
            if(rcX<0) btnNumChangeDone_Click(this,new RoutedEventArgs());
            else{
                _Change_PB_GBoardNum(rcX);
                _Display_GB_GBoard();
            }
        }
        private int  _Get_PB_GBoardRCNum( ){
            int cSz=GNP00.cellSize, LWid=GNP00.lineWidth;
            sysWin.Point pt = Mouse.GetPosition(PB_GBoard);
            int cn=(int)pt.X-2, rn=(int)pt.Y-2;

            cn = cn - cn/cSz - cn/(cSz*3+LWid)*LWid;
            cn /= cSz;
            if(cn<0 || cn>=9) return -1;
            
            rn = rn - rn/cSz - rn/(cSz*3+LWid)*LWid;
            rn /= cSz;
            if(rn<0 || rn>=9) return -1;
            return (rn*9+cn);
        }

        private void _Change_PB_GBoardNum(int rcX){
            if(rcX<0) return;
            int noP=Abs(pGP.BDL[rcX].No);
            if(noP==0)  return;
            if(noP>noPChg){
                foreach(var q in pGP.BDL.Where(r=>r.No!=0)){                 
                    int nm=q.No, nmAbs=Abs(nm), nmSgn=Sign(nm);
                    if(nmAbs<noPChg) continue;
                    else if(nmAbs==noP) q.No = nmSgn * noPChg;
                    else if(nmAbs<noP)  q.No = nmSgn * (nmAbs+1);
                }
            }
            else if(noP<noPChg){          
                foreach(var q in pGP.BDL.Where(r=>r.No!=0)){                 
                    int nm=q.No, nmAbs=Abs(nm), nmSgn=Sign(nm);
                    if(nmAbs<noP) continue;
                    else if(nmAbs==noP)    q.No = nmSgn * noPChg;
                    else if(nmAbs<=noPChg) q.No = nmSgn * (nmAbs-1);
                }           
            }
            _SetScreenProblem();
            noPChg++;
            if(noPChg>9) btnNumChangeDone_Click(this,new RoutedEventArgs());
            return;
        }
      #endregion Number change  
    }
}