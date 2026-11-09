# Menú de configuración

Se ha ampliado el menú de configuración de Moonveil con controles para:

- Ajustar el volumen general del juego.
- Elegir una resolución de 1280 x 720, 1600 x 900 o 1920 x 1080.
- Limitar los fotogramas a 30, 60 o 120 FPS, o quitar el límite.
- Activar pantalla completa o usar una ventana.

Los cambios se aplican al seleccionarlos y se guardan con `PlayerPrefs`, por lo que se mantienen al volver a abrir el juego. El límite de FPS desactiva VSync para que el valor elegido pueda aplicarse.

La lógica está en `Assets/Scripts/Interface/FuncionalidadBotonesInterface.cs`; los controles y su aspecto están en `Assets/UIElements/MenuConfigurer.uxml` y `MenuConfigurer.uss`.
