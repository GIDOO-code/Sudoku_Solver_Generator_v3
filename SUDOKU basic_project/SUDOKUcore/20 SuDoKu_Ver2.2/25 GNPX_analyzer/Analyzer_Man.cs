using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Media;
using System.Threading;
using static System.Diagnostics.Debug;
using static System.Math;

namespace GNPXcore{
    using pRes=Properties.Resources;
    public delegate bool dSolver();

    public class UAlgMethod{
        static private int ID0=0;
        public int        ID;
        public string     MethodName;
        public int        DifLevel; 
        public dSolver    Method;
        public bool       GenLogB;
        public int        UsedCC=0;
        public bool       IsChecked=true;
        public bool       IsEnabled=true;

        public UAlgMethod( int pid, string MethodName, int DifLevel, dSolver Method, bool GenLogB=false ){
            this.ID         = pid*1000+(ID0++); //stableSort
            this.MethodName = MethodName;
            this.DifLevel   = DifLevel;
            this.Method     = Method;
            this.GenLogB    = GenLogB;
        }
        public override string ToString(){
            string st=MethodName.PadRight(30)+"["+DifLevel+"]"+" "+UsedCC;
            st += "GeneralLogic:"+GenLogB.ToString();
            return st;
        }
    }

    public class GNPX_AnalyzerMan{
        static public event SDKSolutionEventHandler Send_Solved; 

        public GNPZ_Engin   pENGN;
        public UPuzzle      pGP{ get=>pENGN.pGP; }
		public int			GStage=0;

        public bool         Insoluble;
        public List<UCell>  pBDL{ get=>pENGN.pGP.BDL; }
        public bool         chbConfirmMultipleCells{ get=>GNPXApp000.chbConfirmMultipleCells; }
        public bool         SolInfoB{ get=>GNPZ_Engin.SolInfoB; }

        public int          SolCode{ set=>pGP.SolCode=value; get=>pGP.SolCode; }
        public string       Result{ set=>pGP.Sol_Result=value; }
        public string       ResultLong{ set=>pGP.Sol_ResultLong=value; }
        public TimeSpan     SdkExecTime;

        public List<UAlgMethod>  SolverLst0;
        private int[,]     Sol99sta{ get=> NuPz_Win.Sol99sta; } //int[,]

        public  GNPX_AnalyzerMan( GNPZ_Engin pENGN ){
            SolverLst0 = new List<UAlgMethod>();
            this.pENGN=pENGN;

          //================================================================================================
          //  UAlgMethod( int pid, string MethodName, int DifLevel, dSolver Method, bool GenLogB=false )
          //================================================================================================

            var SSingle=new SimpleSingleGen(this);
            SolverLst0.Add( new UAlgMethod( 1, "LastDigit",    1, SSingle.LastDigit ) );
            SolverLst0.Add( new UAlgMethod( 2, "NakedSingle",  1, SSingle.NakedSingle ) );
            SolverLst0.Add( new UAlgMethod( 3, "HiddenSingle", 1, SSingle.HiddenSingle ) );

            var LockedCand=new LockedCandidateGen(this);
            SolverLst0.Add( new UAlgMethod( 5, "LockedCandidate", 2, LockedCand.LockedCandidate ) );
            
            var LockedSet=new LockedSetGen(this);
            SolverLst0.Add( new UAlgMethod( 10, "LockedSet(2D)",        3, LockedSet.LockedSet2 ) );
            SolverLst0.Add( new UAlgMethod( 12, "LockedSet(3D)",        4, LockedSet.LockedSet3 ) );
            SolverLst0.Add( new UAlgMethod( 14, "LockedSet(4D)",        5, LockedSet.LockedSet4 ) );
            SolverLst0.Add( new UAlgMethod( 16, "LockedSet(5D)",       -6, LockedSet.LockedSet5 ) );//complementary to 4D
            SolverLst0.Add( new UAlgMethod( 18, "LockedSet(6D)",       -6, LockedSet.LockedSet6 ) );//complementary to 3D
            SolverLst0.Add( new UAlgMethod( 20, "LockedSet(7D)",       -6, LockedSet.LockedSet7 ) );//complementary to 2D           
            SolverLst0.Add( new UAlgMethod( 11, "LockedSet(2D)Hidden",  3, LockedSet.LockedSet2Hidden ) );           
            SolverLst0.Add( new UAlgMethod( 13, "LockedSet(3D)Hidden",  4, LockedSet.LockedSet3Hidden ) );          
            SolverLst0.Add( new UAlgMethod( 15, "LockedSet(4D)Hidden",  5, LockedSet.LockedSet4Hidden ) );
            SolverLst0.Add( new UAlgMethod( 17, "LockedSet(5D)Hidden", -6, LockedSet.LockedSet5Hidden ) );//complementary to 4D
            SolverLst0.Add( new UAlgMethod( 19, "LockedSet(6D)Hidden", -6, LockedSet.LockedSet6Hidden ) );//complementary to 3D        
            SolverLst0.Add( new UAlgMethod( 21, "LockedSet(7D)Hidden", -6, LockedSet.LockedSet7Hidden ) );//complementary to 2D

            var Fish=new FishGen(this);
            SolverLst0.Add( new UAlgMethod( 30, "XWing",            4, Fish.XWing ) );
            SolverLst0.Add( new UAlgMethod( 31, "SwordFish",        5, Fish.SwordFish ) );
            SolverLst0.Add( new UAlgMethod( 32, "JellyFish",        6, Fish.JellyFish ) );
            SolverLst0.Add( new UAlgMethod( 33, "Squirmbag",       -6, Fish.Squirmbag ) );//complementary to 4D 
            SolverLst0.Add( new UAlgMethod( 34, "Whale",           -6, Fish.Whale ) );    //complementary to 3D 
            SolverLst0.Add( new UAlgMethod( 35, "Leviathan",       -6, Fish.Leviathan ) );//complementary to 2D 

            SolverLst0.Add( new UAlgMethod( 40, "Finned XWing",     5, Fish.FinnedXWing ) );
            SolverLst0.Add( new UAlgMethod( 41, "Finned SwordFish", 6, Fish.FinnedSwordFish ) );
            SolverLst0.Add( new UAlgMethod( 42, "Finned JellyFish", 6, Fish.FinnedJellyFish ) );
            SolverLst0.Add( new UAlgMethod( 43, "Finned Squirmbag", 7, Fish.FinnedSquirmbag ) );//not complementary with fin
            SolverLst0.Add( new UAlgMethod( 44, "Finned Whale",     7, Fish.FinnedWhale ) );    //not complementary with fin
            SolverLst0.Add( new UAlgMethod( 45, "Finned Leviathan", 7, Fish.FinnedLeviathan ) );//not complementary with fin

            SolverLst0.Sort((a,b)=>(a.ID-b.ID));
        }

        public void SolversInitialize(){
        }
        //==========================================================

        public bool SnapSaveGP( bool saveAll=false ){
            if( SDK_Ctrl.UGPMan==null)  return false;

            try{
                if(!SDK_Ctrl.MltAnsSearch){
                    UPuzzle GPX=pGP.Copy(pGP.stageNo+1,0);
                    GPX.__SolRes = GPX.Sol_ResultLong.Replace("\r"," ");
                    SDK_Ctrl.UGPMan.MltUProbLst.Add(GPX);
                    SDK_Ctrl.UGPMan.pGPsel=GPX;
                    return false;
                }
           
                /*            
                  The following code is used in the Simple/Full version.
                  Not required in the Basic version.   

                if(SDK_Ctrl.UGPMan.MltUProbLst==null)  SDK_Ctrl.UGPMan.MltUProbLst=new List<UPuzzle>();
                if( SDK_Ctrl.UGPMan.MltUProbLst.Count>=(int)SDK_Ctrl.MltAnsOption["AllMethod"] ){
                    SDK_Ctrl.MltAnsOption["abortResult"] = pRes.msgUpperLimitBreak;
                    return false;
                }

                string __SolRes=pGP.Sol_ResultLong.Replace("\r"," ");
                if( saveAll || SDK_Ctrl.UGPMan.MltUProbLst.All(P=>(P.__SolRes!=__SolRes)) ){
                    Thread.Sleep(1);
                    int IDm=SDK_Ctrl.UGPMan.MltUProbLst.Count;
                    UPuzzle GPX=pGP.Copy(pGP.stageNo+1,IDm);
                    GPX.__SolRes = GPX.Sol_ResultLong.Replace("\r"," ");
                    SDK_Ctrl.UGPMan.MltUProbLst.Add(GPX);
                    SDK_Ctrl.UGPMan.pGPsel=GPX;
                    NuPz_Win.UPP.Add(new UProbS(GPX));
                }
                */
            }
            catch (Exception e){ WriteLine(e.Message+"\r"+e.StackTrace); }

            pBDL.ForEach(p=>p.ResetAnalysisResult());
            pGP.SolCode=-1;
            return true;
        }

        public bool CheckTimeOut(){ // Use only time-consuming SuDoKu Algorithm
            if( !SDK_Ctrl.MltAnsSearch )  return false;
            TimeSpan ts = DateTime.Now - (DateTime)SDK_Ctrl.MltAnsOption["StartTime"];
            bool tmout = ts.TotalSeconds >= (int)SDK_Ctrl.MltAnsOption["MaxTime"];
            if(tmout)  SDK_Ctrl.MltAnsOption["abortResult"] = pRes.msgUpperLimitTimeBreak;
            return tmout;
        }

//==========================================================
        public bool AggregateCellsPZM( ref int nP, ref int nZ,ref int nM ){
            int P=0, Z=0, M=0;
            if( pBDL==null )  return false;
            pBDL.ForEach( q =>{
                if(q.No>0)      P++;
                else if(q.No<0) M++;
                else            Z++;
            } );
            nP=P; nZ=Z; nM=M;
            return pBDL.Any(q=>q.FreeB>0);
        }

        private int[] NChk=new int[27];
        public bool FixOrEliminate_SuDoKu( ref int[] eNChk ){//Confirmation process
            eNChk=null;
            if( pBDL.Any(p=>p.FixedNo>0) ){
                foreach( var P in pBDL.Where(p=>p.No==0) ){
                    int No=P.FixedNo;
                    if(No<1 || No>9) continue;
                    P.FixedNo=0; P.No=-No;
                    P.CellBgCr=Colors.Black;
                }
                
                Set_CellFreeB(false);
                foreach( var P in pBDL.Where(p=>(p.No==0 && p.FreeBC==0)) )  P.ErrorState=9;

                for(int h=0; h<27; h++ ) NChk[h]=0;
                foreach( var P in pBDL ){
                    int no=(P.No<0)? -P.No: P.No;
                    int H=(no>0)? (1<<(no-1)): P.FreeB;
                    NChk[P.r]|=H; NChk[P.c+9]|=H; NChk[P.b+18]|=H;
                }
                for(int h=0; h<27; h++ ){
                    if(NChk[h]!=0x1FF){ eNChk=NChk; SolCode=-9119; return false; }
                }
            }
            else if( pBDL.Any(p=>p.CancelB>0) ){
                foreach( var P in pBDL.Where(p=>p.CancelB>0) ){
                    int CancelB=P.CancelB^0x1FF;
                    P.FreeB &= CancelB; P.CancelB=0;       
                    P.CellBgCr=Colors.Black;
                }
            }
            else{ return false; }  //No solution
            pBDL.ForEach(P=>P.ECrLst=null);

            SolCode=-1;
            return true;
        }
        public void SetBG_OnError(int h){
            foreach(var P in pBDL.IEGetCellInHouse(h)) P.SetCellBgColor(Colors.Violet);
        }
        public void Set_CellFreeB( bool allFlag=true ){
            Insoluble=false;
            foreach( var P in pBDL ){
                P.Reset_StepInfo();
                int freeB=0;
                if(P.No==0){
                    foreach( var Q in pBDL.IEGetFixed_Pivot27(P.rc) ) freeB |= (1<<Abs(Q.No));
                    freeB=(freeB>>=1)^0x1FF; //internal expression with 1 right bit shift
                    if(!allFlag) freeB &= P.FreeB;
                    if(freeB==0){ Insoluble=true; P.ErrorState=1; }//No solution
                }
                P.FreeB=freeB;
            }
        }
        public bool VerifyRoule_SuDoKu( ){
            bool    ret=true;
            if(Insoluble){ SolCode=9; return false; }

            for(int tfx=0; tfx<27; tfx++){
                int usedB=0, errB=0;
                foreach(var P in pBDL.IEGetCellInHouse(tfx).Where(Q=>Q.No!=0)){
                    int no=Abs(P.No);
                    if((usedB&(1<<no))!=0) errB |= (1<<no);
                    usedB |= (1<<no);
                }

                if(errB==0) continue;
                foreach(var P in pBDL.IEGetCellInHouse(tfx).Where(Q=>Q.No!=0)){
                    int no=Abs(P.No);
                    if((errB&(1<<no))!=0){ P.ErrorState=8; ret=false; }
                }
            }
            SolCode = ret? 0: 9; //99:anti-rule
            return ret;
        }
        public void ResetAnalysisResult( bool clear0 ){
            if(clear0){   // true:Initial State
                foreach(var P in pBDL.Where(Q=>Q.No<=0)){ P.Reset_StepInfo(); P.FreeB=0x1FF; P.No=0; }
            }
            else{
                foreach(var P in pBDL.Where(Q=>Q.No==0)){ P.Reset_StepInfo(); P.FreeB=0x1FF; }
            }
            Set_CellFreeB();
			pGP.extRes="";
        }
    }
}