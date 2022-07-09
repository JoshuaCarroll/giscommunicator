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
sudo chmod +x RUN.sh

if (dotnet --list-runtimes | grep -q "Microsoft.NETCore.App 3.1")
then
    echo "DotNet Core v3.1 is installed.  Good to go."
    echo " "
    echo "To start GIS Reporter, execute RUN.sh in this folder."
else
    echo 
    echo 
    echo "STOP..  It looks like you do not have DotNet Core v3.1 installed."
    echo " "
    echo " Go to https://github.com/dotnet/core/issues/4360#issuecomment-627792352"
    echo " for instructions on installing DotNet Core 3.1."
    echo " "
    echo " If that doesn't work for your operating system, go to the official"
    echo " site at https://aka.ms/dotnet-download"
    echo " "
    echo " Once you get that installed, you can run GIS Reporter by "
    echo " running RUN.sh"
fi

echo " "
read -n1 -r -p "Press any key to continue..." key
echo 
