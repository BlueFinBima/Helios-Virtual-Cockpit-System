@echo off
REM output location (you will need to change this for your local folder structure)
set SHARE_FOLDER=d:\google\derammo.github\Helios

REM arguments and validation
set VERSION=%1
set REFERENCE=%2
git rev-parse %VERSION%
if %errorlevel% neq 0 (
	echo Failed to check version git tag.  First parameter should be tag of build to publish
	exit /b %errorlevel%
)
git rev-parse %REFERENCE%
if %errorlevel% neq 0 (
	echo Failed to check reference git tag.  Second parameter should be tag of build relative to which we collect logs
	exit /b %errorlevel%
)

REM publish tag
git push origin %VERSION%

REM publish installer file
mkdir %SHARE_FOLDER%\%VERSION%
copy "Helios Installer\Release\*.msi" %SHARE_FOLDER%\%VERSION%\ 
copy "Helios Installer\Release32\*.msi" %SHARE_FOLDER%\%VERSION%\ 

REM collect and format log
FOR /F %%i IN ('git rev-parse %VERSION%') DO @set COMMIT=%%i
git log --date=short --tags --decorate-refs="1.*" --format="##### [%%h](https://github.com/HeliosVirtualCockpit/Helios/commit/%%H) by %%an on %%ad %%d%%n%%w(0,4,4)%%B  %%n" %REFERENCE%..%VERSION% > %SHARE_FOLDER%\%VERSION%\changes_%COMMIT%.md
 