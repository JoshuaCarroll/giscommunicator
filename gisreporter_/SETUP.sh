echo off
clear
echo
echo
echo "Enter the full path to the folder(s) that contain your Winlink messages."
echo "Multiple folders can be separated by a space. Be sure to wrap your paths"
echo "in quotes."
echo
echo "Default Winlink messages path:      C:\RMS Express\[[callsign]]\Messages"
echo "Default Pat messages path (Linux)   /home/[[user]]/.local/share/pat/mailbox/[[callsign]]/in"
echo
echo ::
read folders 
echo echo off > RUN.sh
echo clear >> RUN.sh
echo dotnet gisreporter_.dll $folders >> RUN.sh
echo echo >> RUN.sh
echo echo  ------------------------------------------------------------ >> RUN.sh
echo echo  -                                                          - >> RUN.sh
echo echo  -   If the program did not run, you need to install the    - >> RUN.sh
echo echo  -     .Net Core 3.1 runtime.  You can download it at:      - >> RUN.sh
echo echo  -                                                          - >> RUN.sh
echo echo  -            https://aka.ms/dotnet-download                - >> RUN.sh
echo echo  -                                                          - >> RUN.sh
echo echo  ------------------------------------------------------------ >> RUN.sh
echo read -n1 -r -p "Press any key to continue..." key >> RUN.sh
echo  >> RUN.sh
sudo chmod +x RUN.sh
echo You can now run the program by executing RUN.sh.
echo
echo "For instructions on how you can view the reports, visit https://aa5jc.com/map"
echo
read -n1 -r -p "Press any key to continue..." key
echo 
