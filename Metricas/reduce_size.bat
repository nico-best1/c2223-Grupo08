@echo off
echo Activando entorno virtual...

call venv\Scripts\activate

echo Ejecutando script...

python src\Reduce_Size.py

echo.
echo Ejecucion finalizada.
pause
