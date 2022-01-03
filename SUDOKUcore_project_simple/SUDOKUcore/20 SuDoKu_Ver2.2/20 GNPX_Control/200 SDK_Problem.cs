using System;
using System.Collections.Generic;
using static System.Math;

using GIDOO_space;

namespace GNPXcore{


    // UProbS : Class for multiple solution analysis.
    //   Elements are information about the analysis result.
    public class UProbS{
        public int        IDmp1{ get; set; }
        public int        DifLevel{ get; set; }
        public string     Sol_Result{ get; set; }
        public string     Sol_ResultLong{ get; set; }
        public UProbS( int IDmp1, int DifLevel, string Sol_Result, string Sol_ResultLong ){
            this.IDmp1 = IDmp1; this.DifLevel = DifLevel;
            this.Sol_Result = new string(Sol_Result);
            this.Sol_ResultLong = new string(Sol_ResultLong);
        }
        public UProbS( UPuzzle P ){
            this.IDmp1=P.IDmp1; this.DifLevel=P.DifLevel;
            this.Sol_Result=P.Sol_Result; this.Sol_ResultLong=P.Sol_ResultLong;
        }
    }


    // UPuzzleMan : Class for multiple solution analysis.
    //   Represent the solution in a tree structure.
    //   There are elements that represent the current state and the state of parents and children.
    public class UPuzzleMan{
        static public GNPZ_Engin pGNPX_Eng;     // Analysis engine
        public List<UPuzzle> MltUProbLst=null;  // children
        public int        stageNo;              // stage No.
        public UPuzzle    pGPsel=null;          // current state
        public UPuzzleMan GPMpre=null;          // parents.
        public UPuzzleMan GPMnxt=null;          // (next version)

        public UPuzzleMan( int stageNo ){
            this.stageNo=stageNo;
            MltUProbLst = new List<UPuzzle>();
        }

        public bool CreateNextStage(){
            if(pGPsel==null)  return true;
            UPuzzleMan Q=new UPuzzleMan(stageNo+1);
            Q.GPMpre=this; this.GPMnxt=Q;
            pGNPX_Eng.pGP=pGPsel.Copy(stageNo+1,0);
            SDK_Ctrl.UGPMan=Q;
            return false;
        }
    }



    // UPuzzle : Class of Sudoku problem
    public class UPuzzle{
        public int         IDm;
        public int         IDmp1{get{return (IDm+1);}}
        public int         ID;
        public List<UCell> BDL;                     // Board cells(81 cell)  Defined in List for use the Linq functions.
        public int[]       AnsNum;                  // used in PuzzleTrans
        public bool?       IsSelected;              // Working variables during analysis.

        public long        HTicks;                  // Time measurement during analysis.
        public string      Name;                    // Name
        public string      TimeStamp;               // TimeStamp

        public int         DifLevel{ get; set; }    // -1:InitialState　0:Manual
        public bool        Insoluble;               // No solution

        public int         stageNo;                 // Stage No.
        public UAlgMethod  pMethod=null;            // Analytical method
        public string      solMessage;              // Message of how to solve
        public string      Sol_Result{ get; set; }  // Solution description
        public string      Sol_ResultLong;          // Solution description(Long)
        public string      __SolRes;
        public string      GNPX_AnalyzerMessage;
		public string      extRes{ get; set; }
        public int         SolCode;
   
        public UPuzzle( ){
            ID=-1;
            BDL = new List<UCell>();
            for(int rc=0; rc<81; rc++ ) BDL.Add(new UCell(rc));
            this.DifLevel = 0;
            HTicks=DateTime.Now.Ticks;
        }
        public UPuzzle( string Name ): this(){ this.Name=Name; }

        public UPuzzle( List<UCell> BDL ){
            this.BDL      = BDL;
            this.DifLevel = 0;
            HTicks=DateTime.Now.Ticks;
        }

        public UPuzzle( int ID, List<UCell> BDL, string Name="", int DifLvl=0, string TimeStamp="" ){
            this.ID       = ID;
            this.BDL      = BDL;
            this.Name     = Name;
            this.DifLevel = DifLvl;
            this.TimeStamp = TimeStamp;
            HTicks=DateTime.Now.Ticks;
        }




        public UPuzzle Copy( int stageNo, int IDm ){
            UPuzzle P = (UPuzzle)this.MemberwiseClone();
            P.BDL = new List<UCell>();
            foreach( var q in BDL ) P.BDL.Add(q.Copy());
            P.HTicks=DateTime.Now.Ticks;;
            P.stageNo=this.stageNo+1;
            P.IDm=IDm;
            return P;
        }

        public string CopyToBuffer(){
            string st = BDL.ConvertAll(q=>Max(q.No,0)).Connect("").Replace("0",".");
            return st;
        }

        public string ToGridString( bool SolSet ){
            string st="";
            BDL.ForEach( P =>{
                st+=(SolSet? P.No: Max(P.No,0));
                if( P.c==8 ) st+="\r";
                else if( P.rc!=80 ) st+=",";
            } );
            return st;
        }
    }
}