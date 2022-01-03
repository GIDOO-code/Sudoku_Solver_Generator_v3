using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Media;

using static System.Diagnostics.Debug;
using static System.Math;

using GIDOO_space;
using GIDOOCV;

namespace GNPXcore{
    public partial class GNPZ_Engin{
      
        private GNPXApp000       pGNP00;                    // main control     
        public  UPuzzle          pGP=null;                  // Problem to analyze     
        public  GNPX_AnalyzerMan AnMan;                     // analyzer(methods)
        private List<UMthdChked> pGMthdLst{ get{ return pGNP00.SolverLst2; } }  // List of algorithms to be applied.
        public  List<UAlgMethod> MethodLst_Run=new List<UAlgMethod>();          // List of successful algorithms         

        static public bool       SolInfoB;    
        static public int        retCode;                   // result
        static public string     GNPX_AnalyzerMessage="";
        static public TimeSpan   SdkExecTime;  
                       
        private bool             __ChkPrint__=false;        // for debug 
      //----------------------------------------------------------------------

        public GNPZ_Engin( GNPXApp000 pGNP00, UPuzzle pGP ){
            this.pGNP00=pGNP00;
            this.pGP   =pGP;
            AnMan=new GNPX_AnalyzerMan(this);
        }

        public void SetGP( UPuzzle pGP ){
            this.pGP=pGP;                                   // Set the Puzzle to the engine
            AnMan.Set_CellFreeB();                          // Initialize the state of the cells in Puzzle.
            GNPX_AnalyzerMessage="";
        }

        public int Set_MethodLst_Run( bool AllMthd=false, bool GenLogUse=true ){ 
            MethodLst_Run.Clear(); 
            foreach( var S in pGMthdLst ){
                if(!AllMthd && !S.IsChecked)  continue;
                if(S.Name==" GeneralLogic" && !GenLogUse)  continue;
                var Sobj = AnMan.SolverLst0.Find(P=>P.MethodName==S.Name); // List solver-object by name
                MethodLst_Run.Add(Sobj);    // List of algorithms to be applied.
            }
            return MethodLst_Run.Count;
        }
        public void AnalyzerCounterReset(){ MethodLst_Run.ForEach(P=>P.UsedCC=0); } //Clear the algorithm counter. 
        private Random rnd = new Random();

        private int[] eNChk;



        // Solve up
        public void sudokAnalyzerAuto( CancellationToken ct ){
            try{
                retCode=0;
                AnMan.Set_CellFreeB();
                Stopwatch AnalyzerLap = new Stopwatch();
                AnalyzerLap.Start();
                while(true){
                    if( ct.IsCancellationRequested ){ ct.ThrowIfCancellationRequested(); return; }
                    int ret2=0;     
                    bool ret = AnalyzerControl(ct,ref ret2,false ); // <-- 1-step solver
                    if(pGP.Insoluble)     retCode=-999;
                    if(ret2==-999888777)  retCode=ret2;
                    if(!ret)  break;
                    if( !AnMan.FixOrEliminate_SuDoKu(ref eNChk) )  retCode=-998;
                    SdkExecTime = AnalyzerLap.Elapsed;
                    if(retCode<0)  return;
                }
                AnalyzerLap.Stop();        
                int  nP=0, nZ=0, nM=0;
                AnMan.AggregateCellsPZM(ref nP,ref nZ,ref nM);
                retCode=nZ;
            }
            catch(OperationCanceledException){}
            catch(Exception e){
                WriteLine( e.Message+"\r"+e.StackTrace );
                using(var fpW=new StreamWriter("Exception_201_0.txt",true,Encoding.UTF8)){
                    fpW.WriteLine($"---{DateTime.Now} {e.Message} \r{e.StackTrace}");
                }
            }
        }





        // 1-step solver
        public bool AnalyzerControl( CancellationToken ct, ref int ret2, bool SolInfoB ){
            List<UCell> XYchainList = new List<UCell>();
            Stopwatch AnalyzerLap = new Stopwatch();
			AnMan.GStage++;

            bool ret=false;
            try{
                pGP.Sol_ResultLong = "";
                int lvlLow = SDK_Ctrl.lvlLow;
                int lvlHgh = SDK_Ctrl.lvlHgh;
                AnMan.SolversInitialize();

        #region Solve
                int  mCC=0;            
                do{
                    { //===== Verify the solution =====
                        ret = AnMan.VerifyRoule_SuDoKu();
                        if(!ret){
                            if(SolInfoB) pGP.Sol_ResultLong = "No solution";
                            ret2 = -999888777;
                            return false;
                        }
                        ret=false;
                    }



                  LblRestart:    
                    AnalyzerLap.Start();
                    DateTime MltAnsTimer=DateTime.Now;
                    UPuzzle GPpre=null;


                    // "SDK_Ctrl.MltAnsSearch" is a flag for executing multiple solution search.
                    // Since Basic version is not used, related parts can be deleted, And can be greatly simplified. 
                    // It is left for continuity with the Simple/Full version.
                    if ( SDK_Ctrl.MltAnsSearch ) GPpre=pGP.Copy(0,0);

                    try{
						if( AnMan.pBDL.All(p=>(p.FreeB==0)) ) break;                        // There are no unconfirmed cells

                        pGP.SolCode=-1;
                        bool L1SolFound=false;
                        foreach( var P in MethodLst_Run ){                                  // Sequentially execute analysis algorithms
                            if( ct!=null && ct.IsCancellationRequested ){ return false; }   // Was the task canceled?                        						    
                            int lvl = P.DifLevel; 
                            int lvlAbs = Abs(lvl);
                            if( lvlAbs>lvlHgh )  continue;

                            try{

                                // It works when searching for multiple solutions.
                                // Negative level algorithms are not needed to solve the problem.
                                if (SDK_Ctrl.MltAnsSearch){     // Multiple Solutions Analysis
                                    if( L1SolFound && lvlAbs>=2 )  break;
                                    if( lvlAbs>(int)SDK_Ctrl.MltAnsOption["MaxLevel"] ) continue;
                                    if( (string)SDK_Ctrl.MltAnsOption["abortResult"]!="" ){
                                        GNPX_AnalyzerMessage = (string)SDK_Ctrl.MltAnsOption["abortResult"];
                                        break;
                                    }
                                }
                                else{
                                    if(lvl<0) continue;     //(negative difficulty method is used only Multiple Solutions Analysis)
                                }
                                            if(__ChkPrint__) WriteLine( $"---> method{(mCC++)} :{P.MethodName}");


                                GNPX_AnalyzerMessage = P.MethodName;
                                if( ct!=null && ct.IsCancellationRequested )  return false; // Was the task canceled?                    
							    



                                // ===== Execution of Sudoku algorithm =====
                                if( (ret = P.Method()) ){     // <--- Execute
                                    if( SDK_Ctrl.UGPMan!=null &&  SDK_Ctrl.UGPMan.MltUProbLst!=null &&
                                        SDK_Ctrl.UGPMan.MltUProbLst.Any(q=>q.SolCode==1) )  L1SolFound=true;
                                    P.UsedCC++;            // Counter for the number of times the algorithm has been applied
                                    pGP.pMethod = P;        
                                    if( pGP.DifLevel<P.DifLevel ) pGP.DifLevel=P.DifLevel;  // Set the maximum level of the algorithm to the problem level
                                    if( SDK_Ctrl.UGPMan!=null && SDK_Ctrl.UGPMan.pGPsel!=null)  SDK_Ctrl.UGPMan.pGPsel.pMethod=P;

                                            if(__ChkPrint__) WriteLine( $"========================> solved {P.MethodName}" );
                                    if( !SDK_Ctrl.MltAnsSearch )  goto succeedBreak;
                                }
                                // -----------------------------------------
                            }
                            catch( Exception e ){
                                WriteLine( e.Message+"\r"+e.StackTrace );
                                            using(var fpW=new StreamWriter("Exception_201_1.txt",true,Encoding.UTF8)){      // message for debug 
                                                fpW.WriteLine($"---{DateTime.Now} {e.Message} \r{e.StackTrace}");
                                            }
                                goto LblRestart;
                            }
                        }

                        if( SDK_Ctrl.MltAnsSearch && SDK_Ctrl.UGPMan!=null && SDK_Ctrl.UGPMan.MltUProbLst!=null ){
                            if(SDK_Ctrl.UGPMan.MltUProbLst.Count>0){
                                pGP =SDK_Ctrl.UGPMan.MltUProbLst.First();
                                SDK_Ctrl.UGPMan.pGPsel=pGP;
                                ret=true;
                                goto succeedBreak;
                            }
                        }

                                            if(__ChkPrint__) WriteLine( "========================> can not solve");
                        if(SolInfoB)  pGP.Sol_ResultLong = "can not solve";
                        ret2 = -999888777;
                        return false;
                    }
                    catch(OperationCanceledException){}
                    catch(Exception e){
                        WriteLine(e.Message+"\r"+e.StackTrace);
                                            using(var fpW=new StreamWriter("ExceptionXXX_2.txt",true,Encoding.UTF8)){
                                                fpW.WriteLine($"---{DateTime.Now} {e.Message} \r{e.StackTrace}");
                                            }
                        goto LblRestart;
                    }
                    finally{
                        AnalyzerLap.Stop();
                        SDK_Ctrl.solLevelNN = Min(SDK_Ctrl.solLevelNN, AnMan.pBDL.Count(p => (p.FreeB!=0)) );
                    }
                }while(false);
            }
            catch( ThreadAbortException ex ){
                WriteLine( ex.Message+"\r"+ex.StackTrace );
            }
          succeedBreak:  //found
            SdkExecTime = AnalyzerLap.Elapsed;
            
        #endregion Solve
            return ret;  
        }




        public int  GetDifficultyLevel( out string prbMessage ){
            int DifL=0;
            prbMessage="";
            if(MethodLst_Run.Any(P=>(P.UsedCC>0))){
                DifL =MethodLst_Run.Where(P=>P.UsedCC>0).Max(P=>P.DifLevel);
                var R =MethodLst_Run.FindLast(Q=>(Q.UsedCC>0)&&(Q.DifLevel==DifL));
                prbMessage =(R!=null)? R.MethodName: "";
            }
            return DifL;
        }



        public string DGViewMethodCounterToString(){
            string solMessage="";
            foreach( var q in MethodLst_Run.Where(p=>p.UsedCC>0) ){
                solMessage += $" {q.MethodName}[{q.UsedCC}]";
            }
            return solMessage;
        }
    }
}