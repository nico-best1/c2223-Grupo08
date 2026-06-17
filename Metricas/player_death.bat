@echo off
echo Activando entorno virtual...

call venv\Scripts\activate

echo Ejecutando script...

python src\Player_Death.py

echo.
echo Ejecucion finalizada.
pause
