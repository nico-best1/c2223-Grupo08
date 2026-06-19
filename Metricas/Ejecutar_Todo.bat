@echo off
echo Activando entorno virtual...

call venv\Scripts\activate

echo Ejecutando script Manual_Reset.py...

python src\Manual_Reset.py

echo.
echo Ejecucion finalizada.

echo Ejecutando script Player_Death.py...

python src\Player_Death.py

echo.
echo Ejecucion finalizada.

echo Ejecutando script Room_Time.py...

python src\Room_Time.py
echo.
echo Ejecucion finalizada.

echo Ejecutando script HeatMap.py...

python src\HeatMap.py

echo.
echo Ejecucion finalizada.
pause
