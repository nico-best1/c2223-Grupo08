@echo off
echo Activando entorno virtual...

call venv\Scripts\activate

echo Ejecutando script...

python src\Manual_Reset.py

echo.
echo Ejecucion finalizada.
pause
