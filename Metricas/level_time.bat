@echo off
echo Activando entorno virtual...

call venv\Scripts\activate

echo Ejecutando script...

python src\Level_Time.py

echo.
echo Ejecucion finalizada.
pause
