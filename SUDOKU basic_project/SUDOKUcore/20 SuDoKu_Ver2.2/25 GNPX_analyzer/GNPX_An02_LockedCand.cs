using System;
using System.Collections.Generic;
using System.Linq;
using GIDOO_space;

namespace GNPXcore{
    public class LockedCandidateGen: AnalyzerBaseV2{
        public LockedCandidateGen( GNPX_AnalyzerMan pAnMan ): base(pAnMan){ }

        // Assumption: Cannot be solved by the "single" algorithm.
        // https://gidoo-code.github.io/Sudoku_Solver_Generator/page32.html




        public bool LockedCandidate( ){     //***** new version *****
            int b1, b2, hs0;

            // Change the search order. Search type1 for all digits.

            //==== Type-1 =====
            for(int no=0; no<9; no++ ){  //#no
                int noB=(1<<no);
                int[] BRCs = new int[9];

                //aggregate rows and columns with #no for each block
                foreach(var P in pBDL.Where(Q=>(Q.FreeB&noB)>0)){ BRCs[P.b] |= (1<<P.r)|(1<<(P.c+9)); }

                for( int hs=0; hs<18; hs++ ){           // 0-8:row 9-17:column
                    int b0=hs%9, RCdir=(hs/9)*9;        // RCdir:[0,9]

                    int RCH = (BRCs[b0]>>RCdir)&0x1FF;
                    if( RCH.BitCount()!=1 ) continue;                       //only one row(column) has #no
                      
                    hs0 = RCH.BitToNum(9)+RCdir;                            //hs0:house number
                    if( pBDL.IEGetCellInHouse(hs0,noB).All(Q=>Q.b==b0) )  continue;

                    //in house hs0, blocks other than b0 have #no
                    { //----- found ----- 
                        SolCode = 2; //----- found -----
                        foreach( var P in pBDL.IEGetCellInHouse(hs0,noB) ){ 
                            if(P.b!=b0) P.CancelB=noB;
                            else        P.SetNoBBgColor(noB,AttCr3,SolBkCr);
                        }
                        string SolMsg= "Locked Candidate B"+(b0+1)+" #"+(no+1);
                        Result=SolMsg;
                        if(__SimpleAnalyzerB__) return true;
                        if(SolInfoB) ResultLong=SolMsg;
                        if(!pAnMan.SnapSaveGP())  return true;
                        return true;
                    }

                }
            }

            //==== Type-2 =====
            for(int no=0; no<9; no++ ){  //#no
                int noB=(1<<no);
                int[] BRCs2 = new int[18];

                // aggregate blocks with #no for each rows or columns and columns
                foreach(var P in pBDL.Where(Q=>(Q.FreeB&noB)>0)){ BRCs2[P.r] |= (1<<P.b); BRCs2[P.c+9] |= (1<<P.b); }

                for( int hs=0; hs<18; hs++ ){   //   0-8:row 9-17:column
                    int HRC = BRCs2[hs];
                    if( HRC.BitCount() != 1 )  continue;

                    int b0 = HRC.BitToNum();    //b0:target block
                    if( hs<9 ){ //row house
                        if( pBDL.IEGetCellInHouse(b0+18,noB).All(Q=>Q.r==hs) )  continue;
                        //Conditions with a solution: Block b0 has number(#no) other than row #hs.
                        b1=b0/3*3+(b0+1)%3; b2=b0/3*3+(b0+2)%3;    // b1,b2:block(row direction)
                    }
                    else{       //column house
                        if( pBDL.IEGetCellInHouse(b0+18,noB).All(Q=>Q.c==(hs-9)) )  continue;
                        //Conditions with a solution: Block b0 has number(#no) other than Colimn #(hs-9).
                        b1=(b0+3)%9;        b2=(b0+6)%9;           // b1,b2:block(collumn direction)  
                    }

                    { //----- found -----
                        SolCode=2; 
                        foreach( var P in pBDL.IEGetCellInHouse(18+b0,noB) ){
                            if(!HouseCells[hs].IsHit(P.rc))   P.CancelB=noB;
                            else                              P.SetNoBBgColor(noB,AttCr3,SolBkCr);
                        }
                        string SolMsg= "Locked Candidate B"+(b0+1)+" #"+(no+1);
                        Result=SolMsg; 
                        if(__SimpleAnalyzerB__)  return true;
                        foreach(var P in pBDL.IEGetCellInHouse(18+b1,noB)) P.SetNoBBgColor(noB,AttCr3,SolBkCr);
                        foreach(var P in pBDL.IEGetCellInHouse(18+b2,noB)) P.SetNoBBgColor(noB,AttCr3,SolBkCr);
                        if(SolInfoB) ResultLong=SolMsg;
                        if(!pAnMan.SnapSaveGP())  return true;
                    }
                }
            }
            return false;
        }

    }
}