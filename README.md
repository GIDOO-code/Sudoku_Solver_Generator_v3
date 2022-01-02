# Sudoku_Solver_Generator
![GNPX](/images/NPX_start.png)

Sudoku analysis and generation C# program.
The only algorithm used is non-backtracking.  
The algorithm used is  
>Single, LockedCandidate, (hidden)LockedSet(2D-7D),
  (Finned)(Franken/Mutant/Kraken)Fish(2D-7D),
  Skyscraper, EmptyRectangle, XY-Wing, W-Wing, RemotePair, XChain, XYChain,
  SueDeCoq, (Multi)Coloring,
  ALS-Wing, ALS-XZ, ALS-Chain,
 (ALS)DeathBlossom(Ext.), (Grouped)NiceLoop, ForceChain and GeneralLogic.<br>
There are also functions for transposed transformation of Sudoku problems, standardization and ordering of Sudoku problems.  

The algorithm is explained on the HTML page.  
https://gidoo-code.github.io/Sudoku_Solver_Generator/  
https://gidoo-code.github.io/Sudoku_Solver_Generator_jp/


# For simple version:

  You can change from the regular version to the simplified version.
  For consistency with the regular version, there are some unnecessary but unchanged parts.


## How to change the version ( Regular version -> Simple version )

    (1) Remove "RegularVersion" from the conditional compilation symbol field.

    (2) Exclude the source files in the folder with "nnEx" in the name from the project.
        - There are four folders to exclude. (00Ex,21Ex,23Ex,25Ex)
        - Exclude, not delete. So that it can be restored.
        - Exclude the files in the folder, not the folder.

    (3) It can also be undone( Simple version -> Regular version ).   Do try!
        - Set "RegularVersion" to the conditional compilation symbol field.
        - Add files to the "nnEx" folders.
        - Don't forget to add both .xaml and .cs to 00Ex. 
