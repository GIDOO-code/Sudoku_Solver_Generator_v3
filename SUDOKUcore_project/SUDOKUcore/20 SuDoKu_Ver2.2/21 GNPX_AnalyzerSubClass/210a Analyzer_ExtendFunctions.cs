using System;
using System.Collections.Generic;
using System.Linq;
using static System.Diagnostics.Debug;

using GIDOO_space;

namespace GNPXcore{   

    //========== Extended Function ==========
    static public class StaticSA{ 
        static private char[] _sep=new Char[]{ ' ', ',', '\t' };
        static public bool rcbHitCheck(this int B, int C){
            if(B/9==C/9)  return true;
            if(B%9==C%9)  return true;
            if((B/27*3+(B%9)/3)==(C/27*3+(C%9)/3))  return true;
            return false;
        }


      #region ToString_SameHouseComp

#if false
        static public void testc() {
            string[] test = {"r3 c5", "r4 r6", "c2 c4", "r1c1 r1c2", " r1c5 r2c5 ", "r1c5 r2c5 r9c9 r1c1 r1c2 r2c8",  "r1c1 r1c2 r1c5 r2c5", };
                
            foreach( var p in test ){
                WriteLine("");
                WriteLine( $"p:{p} ToString_SameHouseComp :{p.ToString_SameHouseComp1()}" );
                WriteLine( $"p:{p} ToString_SameHouseComp2:{p.ToString_SameHouseComp()}" );
            }
        }
#endif

        static public string ToString_SameHouseComp( this string stOrg ){  // new version        
          // Preparation
            string stOrg2 = stOrg.Replace("c","s");     // Adjusting the order of r and c
            List<string> stLst=stOrg2.Split(_sep,StringSplitOptions.RemoveEmptyEntries).ToList();
            if( stLst.Count<=1 ) return stOrg.Trim();          
            stLst.Sort();
            var URCBCell = stLst.ConvertAll( p=>new _ClassNSS(p.Replace("s","c")) );

          // Convert to aggregate representation
            string st;
            bool cmpF=true;
            do{
                cmpF=false;
                foreach( var P in URCBCell ){
                    List<_ClassNSS> Qlst=URCBCell.FindAll(Q=>(Q.stP1==P.stP1 && Q.stP2[0]==P.stP2[0]));
                    if(Qlst.Count>=2){
                        st=Qlst[0].stP2[0].ToString();
                        Qlst.ForEach(Q=>st+=Q.stP2.Substring(1,Q.stP2.Length-1));
                        _ClassNSS R=URCBCell.Find(Q=>(Q.stP1==P.stP1));
                        R.stP2 = st;
                        foreach(var U in Qlst.Skip(1)) URCBCell.Remove(U);
                        cmpF=true;
                        break;
                    }

                    Qlst=URCBCell.FindAll(Q=>(Q.stP2==P.stP2 && Q.stP1[0]==P.stP1[0]));
                    if(Qlst.Count>=2){
                        st=Qlst[0].stP1[0].ToString();
                        Qlst.ForEach(Q=>st+=Q.stP1.Substring(1,Q.stP1.Length-1));
                        _ClassNSS R=URCBCell.Find(Q=>(Q.stP2==P.stP2));
                        R.stP1 = st;
                        foreach(var U in Qlst.Skip(1)) URCBCell.Remove(U);
                        cmpF=true;
                        break;
                    }
                }

            }while(cmpF);
            st="";
            URCBCell.ForEach(P=> st+=(P.stP1+P.stP2.Trim()+" ") );
            return st;
        }

        private class _ClassNSS{
            public int sz;
            public string stP1;
            public string stP2="  ";

            public _ClassNSS( int sz, string stP1, string stP2 ){
                this.sz=sz; this.stP1=stP1; this.stP2=stP2;
            }
            public _ClassNSS( string st ){
                try{
                    sz=1; 
                    if(st.Length>=2) stP1=st.Substring(0,2);
                    if(st.Length>=4) stP2=st.Substring(2,2);
                }
                catch(Exception){ }
            }
        }
     




        static public int  sameHouseCheck(this int B, int C){
            // 0:Completely Different
            // 1:Same Row　2:Same Column　4:Same Block --> Bit Representation
            int ret = 0;
            if(B/9==C/9) ret = 1;
            if(B%9==C%9) ret |= 2;
            if((B/27*3+(B%9)/3)==(C/27*3+(C%9)/3)) ret |= 4;
            return ret;
        }

#if false
        static public string ToString_SameHouseComp_old(this string st){
            if(st.Length<=5){               
                if(st.Length>0) st=st.Replace(" ","");
                return st;
            }
            string retSt = "";

            string[] eLst;
            eLst=st.Split(_sep,StringSplitOptions.RemoveEmptyEntries);

            int[,] rcX = new int[2,9];
            Array.ForEach(eLst, s =>{
                int r = s.Substring(1,1).ToInt()-1;
                int c = s.Substring(3,1).ToInt()-1;
                rcX[0,r] |= (1<<c);
                rcX[1,c] |= (1<<r);
            });

            bool hitSW = false;
            for(int r=0; r<9; r++){
                if(rcX[0,r].BitCount()>1){ hitSW=true; break; }
            }

            if(hitSW){
                for(int r=0; r<9; r++){
                    if(rcX[0,r]==0)  continue;
                    retSt += $" r{(r+1)}c";
                    for(int c=0; c<9; c++){
                        if((rcX[0,r]&(1<<c))==0)  continue;
                        retSt += (c+1).ToString();
                        rcX[1,c] ^= (1<<r);
                    }
                }
            }

            for(int c=0; c<9; c++){
                if(rcX[1,c]==0)  continue;
                retSt += " r";
                for(int r=0; r<9; r++){
                    if((rcX[1,c]&(1<<r))>0)  retSt += (r+1).ToString();
                }
                retSt += $"c{(c+1)}";
            }

            return retSt.Trim();
        }
#endif
      #endregion





        static public string ToBitString(this int num, int ncc){
            int numW = num;
            string st="";
            for(int k=0; k<ncc; k++){
                st += ((numW&1)!=0)? (k+1).ToString(): ".";
                numW >>= 1;
            }
            return st;
        }
        static public string ToBitString27(this int num){
            string st = (num&0x1FF).ToBitString(9)
                      + " "+((num>>9)&0x1FF).ToBitString(9)
                      + " "+(num>>18).ToBitString(9) +" /";
            return st;
        }

        static public string ToBitStringN(this int num, int ncc){
            int numW = num;
            string st="";
            for(int k=0; k<ncc; k++){
                if((numW&1)!=0) st += (k+1).ToString();
                numW >>= 1;
            }
            if(st=="")  st = "-";

            return st;
        }
        static public string ToBitStringNor(this int num, int ncc){
            int numW = num;
            string st="";
            for(int k=0; k<ncc; k++){
                if((numW&1)!=0){
                    if(st=="") st = (k+1).ToString();
                    else st += " or "+(k+1).ToString();
                }
                numW >>= 1;
            }
            if(st=="")  st = "*";

            return st;
        }
        static public string ToBitStringNZ(this int num, int ncc){
            int numW = num;
            string st="";
            for(int k=0; k<ncc; k++){
                if((numW&1)!=0) st += (k+1).ToString();
                numW >>= 1;
            }
            return st;
        } 
        static public string ToBitString(this long LNum, int n9=4){
            string st = "";
            long LNumW = LNum;
            for(int k=0; k<n9; k++){
                int LNumInt = (int)(LNumW&0x1FF);
                st += LNumInt.ToBitString(9)+" /";
                LNumW >>= 9;
            }
            return st;
        }
        static public string ToBitStringN(this long LNum, int n9=4){
            string st = "";
            long LNumW = LNum;
            for(int k=0; k<n9; k++){
                int LNumInt = (int)(LNumW&0x1FF);
                st += LNumInt.ToBitStringN(9)+" /";
                LNumW >>= 9;
            }
            return st;
        }
        static public string ToRCString(this int rc){
            string po = $"r{rc/9+1}c{(rc%9)+1}";
            return po;
        }

        static public string ToRCNCLString(this int rc){
            return  rc.ToRCString().ToString_SameHouseComp();
        }



        static public string HouseToString(this int HH){
            string st="";
            if((HH&0x1FF)>0) st += "r"+(HH&0x1FF).ToBitStringN(9)+" ";
            if(((HH>>=9)&0x1FF)>0) st += "c"+(HH&0x1FF).ToBitStringN(9)+" ";
            if(((HH>>=9)&0x1FF)>0) st += "b"+(HH&0x1FF).ToBitStringN(9)+" ";
            return st.Trim();
        }
        static public string tfxToString(this int tfx, string noSt=" "){
            string st="";
            if(tfx<0)            st += "---";
            if(tfx>=0 &&tfx<9)   st += "r"+(tfx+1)+noSt;
            if(tfx>=9 && tfx<18) st += "c"+(tfx-9+1)+noSt;
            if(tfx>=18)          st += "b"+(tfx-18+1)+noSt;
            return st.Trim();
        }



        static public int ToRCBitPat(this int rc){
            int r=rc/9, c=rc%9, b=r/3*3+c/3;
            int rcbBP = (1<<(b+18)) | (1<<(c+9)) | (1<<r);
            return rcbBP;
        }          
        static public string Row3Col3ToString(this int rcX3){
            string st="";
            for(int k=0; k<27; k++){
                if((rcX3&(1<<k))>0)  st += (k%3+1).ToString();
                else st+=".";
                if((k%9)==8)  st += "<>";
                else if((k%3)==2)  st += " ";
            }
            return st;
        }
        //========== bit expression→Number  ==========   
        static public int BitToNum(this int FreeB, int sz=9){    
            if(FreeB.BitCount()!=1) return -1;
            for(int k=0; k<sz; k++){
                if(FreeB==(1<<k)) return k;
            }
            return -1;
        }
        //========== bit expression→2 Numbers ==========   
        static public bool BitTo2Nums(this int noB, ref int na, ref int nb){
            na=nb=-1;
            if(noB.BitCount()!=2) return false;
             for(int k=0; k<9; k++){
                if((noB&1)>0) nb=k;
                if(na<0) na=nb;
                noB >>= 1;
            }
            return true;
        }
        
        static public (int,int)? BitTo2Nums(this int noB ){
            int na=-1,nb=-1;
            if(noB.BitCount()!=2) return null;
            for(int k=0; k<9; k++){
                if((noB&1)>0) nb=k;
                if(na<0) na=nb;
                noB >>= 1;
            }
            return (na,nb);
        }

        static public List<int> GetRowList(this Bit81 X81, int r){
        List<int> RowLst=new List<int>();
            int _BPX=X81._BP[r/3];
            for(int c=0; c<9; c++){
                int p=(_BPX>>((r%3)*9+c))&1;
                RowLst.Add(p);
            }
            return RowLst;
        }
        static public int Get_Bto1rc(this Bit81 X81){
            if(X81.Count!=1)  return -1;
            int bp;
            for(int n=0; n<3; n++){
                if((bp=X81._BP[n])==0) continue;
                for(int k=0; k<27; k++){
                    if(((bp>>k)&1)!=0) return (n*27+k);
                }
            }
            return -1;
        }
        static public int Get_tfx_rc(this int tfx, int nx){
            int r=0, c=0;
            switch(tfx/9){
                case 0: r=tfx; c=nx; break; //row   
                case 1: r=nx; c=(tfx-9); break; //Column
                case 2: int b=(tfx-18); r=(b/3)*3+nx/3; c=(b%3)*3+nx%3; break; //block
            }
            return (r*9+c);
        }
        static public int ToBlock(this int rcx){ return (rcx/27*3+(rcx%9)/3); }
        static public long BPReset(this long A, long B){
            A &= (B^0x7FFFFFFFFFFFFFFF);
            return A;
        }

    #region Enumerators
        static public IEnumerable<UCell> IEGetCellInHouse(this List<UCell> pBDL, int tfx, int FreeB=0x1FF){
            int r=0, c=0, tp=tfx/9, fx=tfx%9;
            for(int nx=0; nx<9; nx++){
                switch(tp){
                    case 0: r=fx; c=nx; break; //row
                    case 1: r=nx; c=fx; break; //column
                    case 2: r=(fx/3)*3+nx/3; c=(fx%3)*3+nx%3; break; //block
                }
                UCell P=pBDL[r*9+c];
                P.nx=nx;
                if((P.FreeB&FreeB)>0) yield return P;
            }
        }
        static public IEnumerable<UCell> IEGetUCeNoB(this Bit81 BX, List<UCell> pBDL, int noBX){ //nx=0...8        
            for(int n=0; n<3; n++){
                int bp = BX._BP[n];
                for(int k=0; k<27; k++){
                    if(((bp>>k)&1)==0) continue;
                    UCell P=pBDL[n*27+k];
                    if((P.FreeB&noBX)>0)  yield return P;
                }
            }
        }
        static public IEnumerable<UCell> IEGetFixed_Pivot27(this List<UCell> pBDL, int rc0){
            int r0=rc0/9, c0=rc0%9, r=0, c=0;
            for(int tfx=0; tfx<27; tfx++){
                int fx=tfx%9;
                switch(tfx/9){
                    case 0: r=r0; c=fx; break; //row   
                    case 1: r=fx; c=c0; break; //Column
                    case 2: int b0=r0/3*3+c0/3; r=(b0/3)*3+fx/3; c=(b0%3)*3+fx%3; break;//block
                }
                if(r==r0 && c==c0) continue; //Exclude axis Cell
                int rc=r*9+c;
                if(pBDL[rc].No==0) continue; //Exclude unfixed Cell
                yield return pBDL[rc];
            }
        }
        static public IEnumerable<int> IEGet_BtoNo(this int noBin, int sz=9){
            for(int no=0; no<sz; no++){
                if((noBin&(1<<no))>0) yield return no;
            }
            yield break;
        }
        static public IEnumerable<int> IEGet_rc(this Bit81 X81){
            for(int nx=0; nx<3; nx++){
                int _BPX=X81._BP[nx];
                for(int m=0; m<27; m++){
                    if((_BPX&(1<<m))>0) yield return (nx*27+m);
                }
            }
            yield break;
        }
        static public IEnumerable<int> IEGet_tfb(this int tfbContainer){
            int P=tfbContainer;
            for(int m=0; m<27; m++){
                if((P&(1<<m))>0) yield return m;
            }
            yield break;
        }
        static public string Connect<T>(this IEnumerable<T> list, string separator){
    	    return string.Join(separator,list);
        }       
    #endregion


    }
}