using System;
using System.Linq;
using static System.Diagnostics.Debug;
using GIDOO_space;

namespace GNPXcore{
    public partial class FishGen: AnalyzerBaseV2{
        public FishGen( GNPX_AnalyzerMan pAnMan ): base(pAnMan){ }

        //=======================================================================
        //Fish:
        // Understand this algorithm, you need to know BaseSet and CoverSet.
        // https://gidoo-code.github.io/Sudoku_Solver_Generator/page34.html
        public bool XWing()     => Fish_Basic(2);
        public bool SwordFish() => Fish_Basic(3);
        public bool JellyFish() => Fish_Basic(4);
        public bool Squirmbag() => Fish_Basic(5);    //complementary to 4D 
        public bool Whale()     => Fish_Basic(6);    //complementary to 3D 
        public bool Leviathan() => Fish_Basic(7);    //complementary to 2D 

        //FinnedFish
        public bool FinnedXWing()     => Fish_Basic(2,fin:true);
        public bool FinnedSwordFish() => Fish_Basic(3,fin:true);
        public bool FinnedJellyFish() => Fish_Basic(4,fin:true);
        public bool FinnedSquirmbag() => Fish_Basic(5,fin:true);
        public bool FinnedWhale()     => Fish_Basic(6,fin:true);
        public bool FinnedLeviathan() => Fish_Basic(7,fin:true);

      //-----------------------------------------------------------------------
        // Basic Fish
        public bool Fish_Basic( int sz, bool fin=false ){
            const int rowSel=0x1FF, colSel=0x1FF<< 9;
            for (int no=0; no<9; no++ ){
                if( ExtFishSub(sz,no,18,rowSel,colSel,FinnedF:fin) ) return true;
                if( ExtFishSub(sz,no,18,colSel,rowSel,FinnedF:fin,_Fdef:false) ) return true;
            }
            return false;
        }

      //-----------------------------------------------------------------------
        private FishMan FMan=null;
        public bool ExtFishSub( int sz, int no, int FMSize, int BaseSel, int CoverSel, bool FinnedF, bool _Fdef=true ){       
            int noB=(1<<no);
            bool extFlag = (sz>=3 && ((BaseSel|CoverSel).BitCount()>18));
            if(_Fdef) FMan=new FishMan(this,FMSize,no,sz,extFlag);

            foreach( var Bas in FMan.IEGet_BaseSet(BaseSel,FinnedF:FinnedF )){    //select BaseSet
                if( pAnMan.CheckTimeOut() ) return false;

                foreach( var Cov in FMan.IEGet_CoverSet(Bas,CoverSel,FinnedF) ){  //select CoverSet
                    Bit81 FinB81 = Cov.FinB81;

                    Bit81 ELM=null;
                    var FinZeroB = FinB81.IsZero();
                    if( !FinnedF && FinZeroB ){         //===== no Fin =====
                        if( !FinnedF && (ELM=Cov.CoverB81-Bas.BaseB81).Count>0 ){                      
                            foreach( var P in ELM.IEGetUCeNoB(pBDL,noB) ){ P.CancelB=noB; SolCode=2; }
                            if(SolCode>0){              //solved!
                                if(SolInfoB){
                                    _Fish_FishResult(no,sz,Bas,Cov,(FMSize==27)); //FMSize 18:Standard 27:Franken/Mutant
                                }
                                if(__SimpleAnalyzerB__)  return true;
                                if(!pAnMan.SnapSaveGP(true)) return true; 
                            }
                        }
                    }
                    else if( FinnedF && !FinZeroB ){    //===== Finned ===== 
                        Bit81 Ecand=Cov.CoverB81-Bas.BaseB81;
                        ELM=new Bit81();
                        foreach( var P in Ecand.IEGetUCeNoB(pBDL,noB) ){
                            if( (FinB81-ConnectedCells[P.rc]).Count==0 ) ELM.BPSet(P.rc);
                        }
                        if(ELM.Count>0){    //there are cells/digits can be excluded                        
                            foreach( var P in ELM.IEGet_rc().Select(p=>pBDL[p]) ){ P.CancelB=noB; SolCode=2; }   
                            if(SolCode>0){  //solved!
                                if(SolInfoB){
                                    _Fish_FishResult(no,sz,Bas,Cov,(FMSize==27)); //FMSize 18:Standard 27:Franken/Mutant
                                }
                                if(__SimpleAnalyzerB__)  return true;
                                if(!pAnMan.SnapSaveGP(true)) return true;
                            }
                        }
                    }
                    continue;
                }
            }
            return false;
        }


      //-----------------------------------------------------------------------
        private void _Fish_FishResult( int no, int sz, UFish Bas, UFish Cov, bool FraMut ){
            int   HB=Bas.HouseB, HC=Cov.HouseC;
            Bit81 PB=Bas.BaseB81, PFin=Cov.FinB81; 
            Bit81 EndoFin=Bas.EndoFin, CnaaFin=Cov.CannFin;
            string[] FishNames={ "Xwing","SwordFish","JellyFish","Squirmbag","Whale", "Leviathan" };
    
            PFin-=EndoFin;
            try{
                int noB=(1<<no);                 
                foreach( var P in PB.IEGet_rc().Select(p=>pBDL[p]) )      P.SetNoBBgColor(noB,AttCr,SolBkCr);
                foreach( var P in PFin.IEGet_rc().Select(p=>pBDL[p]) )    P.SetNoBBgColor(noB,AttCr,SolBkCr2);
                foreach( var P in EndoFin.IEGet_rc().Select(p=>pBDL[p]) ) P.SetNoBBgColor(noB,AttCr,SolBkCr3);
                foreach( var P in CnaaFin.IEGet_rc().Select(p=>pBDL[p]) ) P.SetNoBBgColor(noB,AttCr,SolBkCr3);

                string msg = "\r     Digit: " + (no+1);                 
                msg += "\r   BaseSet: " + HB.HouseToString();  //+"#"+(no+1);
                msg += "\r  CoverSet: " + HC.HouseToString();  //+"#"+(no+1);
                string msg2=" #"+(no+1)+" "+HB.HouseToString().Replace(" ","")+"/"+HC.HouseToString().Replace(" ","");
 
                string FinmsgH="", FinmsgT="";
                if( PFin.Count>0 ){
                    FinmsgH = "Finned ";
                    string st="";
                    foreach( var rc in PFin.IEGet_rc() ) st += " "+rc.ToRCString();
                    msg += "\r    FinSet: "+st.ToString_SameHouseComp();                
                }

                string Fsh = FishNames[sz-2];
                if(FraMut) Fsh = "Franken/Mutant "+Fsh;
                Fsh = FinmsgH+Fsh+FinmsgT;
                ResultLong = Fsh+msg;  
                Result=Fsh.Replace("Franken/Mutant","F/M")+msg2;
            }
            catch( Exception ex ){
                WriteLine(ex.Message+"\r"+ex.StackTrace);
            }
        }
    }  
}