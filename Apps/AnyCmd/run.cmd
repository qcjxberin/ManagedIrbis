@echo off

cd bin\Debug

rem AnyCmd.exe "host=127.0.0.1;port=6666;user=1;password=1;db=IBIS;arm=A;" "2" "IBIS" "v200^a,10,100,1" "" "0" "0" "" ""
rem AnyCmd.exe "host=127.0.0.1;port=6666;user=1;password=1;db=IBIS;arm=A;" "7" "IBIS" "@tabf1w" ""  "" "T=A$" "0" "0" "" ""
rem AnyCmd.exe "host=127.0.0.1;port=6666;user=1;password=1;db=IBIS;arm=A;" "M" "2.IBIS.IBIS.mst" "C:\ibis.bak" 
rem AnyCmd.exe "host=127.0.0.1;port=6666;user=1;password=1;db=IBIS;arm=A;" "+1"
rem AnyCmd.exe "host=127.0.0.1;port=6666;user=1;password=1;db=IBIS;arm=A;" "V" "IBIS" "2" "1" "2"
AnyCmd.exe "host=127.0.0.1;port=6666;user=1;password=1;db=IBIS;arm=A;" "Z" "IBIS"

cd ..\..