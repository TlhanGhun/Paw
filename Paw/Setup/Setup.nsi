!include "MUI2.nsh"
!include "checkDotNet3.nsh"

!define MIN_FRA_MAJOR "3"
!define MIN_FRA_MINOR "5"
!define MIN_FRA_BUILD "*"


; The name of the installer
Name "Paw"

; The file to write
OutFile "Setup-Paw.exe"





; The default installation directory
InstallDir "$PROGRAMFILES\Tlhan Ghun\Paw"

; Registry key to check for directory (so if you install again, it will 
; overwrite the old one automatically)
InstallDirRegKey HKLM "Software\Paw" "Install_Dir"

; Request application privileges for Windows Vista
RequestExecutionLevel admin


 


;--------------------------------

  !define MUI_ABORTWARNING



!define MUI_HEADERIMAGE
!define MUI_HEADERIMAGE_BITMAP "tlhanGhun.bmp"
!define MUI_WELCOMEFINISHPAGE_BITMAP "tlhanGhunWelcome.bmp"
!define MUI_WELCOMEPAGE_TITLE "Paw"
!define MUI_WELCOMEPAGE_TEXT "Paw is a bridge between GNTP (the protocol used in Growl for Windows and future versions of Growl for Mac) and the Windows notification system Snarl (http://www.fullphat.net/).$\r$\n$\r$\nPlease stop any instance of Paw prior to installing this version."
!define MUI_STARTMENUPAGE_DEFAULTFOLDER "Tlhan Ghun\Paw"
!define MUI_ICON "..\Paw.ico"
!define MUI_UNICON "uninstall.ico"


Var StartMenuFolder
; Pages

  !insertmacro MUI_PAGE_WELCOME
  !insertmacro MUI_PAGE_LICENSE "License.txt"
  !insertmacro MUI_PAGE_COMPONENTS
  !insertmacro MUI_PAGE_DIRECTORY

  !define MUI_STARTMENUPAGE_REGISTRY_ROOT "HKCU" 
  !define MUI_STARTMENUPAGE_REGISTRY_KEY "Software\Paw" 
  !define MUI_STARTMENUPAGE_REGISTRY_VALUENAME "Start Menu Folder"
  !insertmacro MUI_PAGE_STARTMENU Application $StartMenuFolder

  !insertmacro MUI_PAGE_INSTFILES
  !define MUI_FINISHPAGE_RUN "Paw.exe"
  !insertmacro MUI_PAGE_FINISH




  !insertmacro MUI_UNPAGE_WELCOME
  !insertmacro MUI_UNPAGE_CONFIRM
  !insertmacro MUI_UNPAGE_INSTFILES
  !insertmacro MUI_UNPAGE_FINISH





;--------------------------------




!insertmacro MUI_LANGUAGE "English"

; LoadLanguageFile "${NSISDIR}\Contrib\Language files\English.nlf"
;--------------------------------
;Version Information

  VIProductVersion "1.3.1.2"
  VIAddVersionKey /LANG=${LANG_ENGLISH} "ProductName" "Paw"
  VIAddVersionKey /LANG=${LANG_ENGLISH} "CompanyName" ""
  VIAddVersionKey /LANG=${LANG_ENGLISH} "LegalCopyright" "Tlhan Ghun (Sven Walther) © 2010"
  VIAddVersionKey /LANG=${LANG_ENGLISH} "FileDescription" "GNTP bridge to Snarl"
  VIAddVersionKey /LANG=${LANG_ENGLISH} "FileVersion" "2.0"







Function un.UninstallDirs
    Exch $R0 ;input string
    Exch
    Exch $R1 ;maximum number of dirs to check for
    Push $R2
    Push $R3
    Push $R4
    Push $R5
       IfFileExists "$R0\*.*" 0 +2
       RMDir "$R0"
     StrCpy $R5 0
    top:
     StrCpy $R2 0
     StrLen $R4 $R0
    loop:
     IntOp $R2 $R2 + 1
      StrCpy $R3 $R0 1 -$R2
     StrCmp $R2 $R4 exit
     StrCmp $R3 "\" 0 loop
      StrCpy $R0 $R0 -$R2
       IfFileExists "$R0\*.*" 0 +2
       RMDir "$R0"
     IntOp $R5 $R5 + 1
     StrCmp $R5 $R1 exit top
    exit:
    Pop $R5
    Pop $R4
    Pop $R3
    Pop $R2
    Pop $R1
    Pop $R0
FunctionEnd









; The stuff to install
Section "Paw"

  SectionIn RO
  
  SetOutPath "$INSTDIR"
  File "..\Paw.ico"
  File "..\Paw.exe"
  File "..\Paw.exe.config"
  File "..\Paw.pdb"
  
  !insertmacro AbortIfBadFramework

  ; Put file there
  File "Documentation.URL"
  File "..\Growl.Connector.dll"
  File "..\Growl.CoreLibrary.dll"
  File "..\Growl.Daemon.dll"
  File "..\Mono.Zeroconf.dll"
  File "..\Mono.Zeroconf.providers.Bonjour.dll"
  File "LICENSE.txt"
  File "Documentation.ico"
  File "..\Winkle.dll"
  File "..\Winkle.pdb"
  ; Write the installation path into the registry
  WriteRegStr HKLM SOFTWARE\Paw "Install_Dir" "$INSTDIR"
  
  ; Write the uninstall keys for Windows
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Paw" "DisplayName" "Paw"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Paw" "UninstallString" '"$INSTDIR\uninstall.exe"'
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Paw" "NoModify" 1
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Paw" "NoRepair" 1
  WriteUninstaller "uninstall.exe"



  
SectionEnd

; Optional section (can be disabled by the user)
Section "Start Menu Shortcuts"

!insertmacro MUI_STARTMENU_WRITE_BEGIN Application

  CreateDirectory "$SMPROGRAMS\$StartMenuFolder"
  CreateShortCut "$SMPROGRAMS\$StartMenuFolder\Paw.lnk" "$INSTDIR\Paw.exe" "" "$INSTDIR\Paw.ico" 0
 ; CreateShortCut "$SMPROGRAMS\$StartMenuFolder\Documentation.lnk" "$INSTDIR\Documentation.URL" "" $INSTDIR\Documentation.ico" 0
  CreateShortCut "$SMPROGRAMS\$StartMenuFolder\Uninstall.lnk" "$INSTDIR\uninstall.exe" "" "$INSTDIR\uninstall.exe" 0
  
!insertmacro MUI_STARTMENU_WRITE_END

  
SectionEnd


;--------------------------------

; Uninstaller

Section "Uninstall"

  
  ; Remove registry keys
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\Paw"
  DeleteRegKey HKLM "Software\Paw"
  ; Remove files and uninstaller
  Delete $INSTDIR\*.*

  ; Remove shortcuts, if any
  !insertmacro MUI_STARTMENU_GETFOLDER Application $StartMenuFolder
    
  Delete "$SMPROGRAMS\$StartMenuFolder\\*.*"
  


  DeleteRegKey HKCU "Software\Paw"


  ; Remove directories used
   ; RMDir "$SMPROGRAMS\$StartMenuFolder"
Push 10 #maximum amount of directories to remove
  Push "$SMPROGRAMS\$StartMenuFolder" #input string
    Call un.UninstallDirs

   
  ; RMDir "$INSTDIR"
  
  Push 10 #maximum amount of directories to remove
  Push $INSTDIR #input string
    Call un.UninstallDirs


SectionEnd
