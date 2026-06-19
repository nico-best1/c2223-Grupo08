@echo off
echo Activando entorno virtual...

call venv\Scripts\activate

echo Ejecutando script Death_Cause.py...

python src\Death_Cause.py

echo.
echo Ejecucion finalizada.
pause
