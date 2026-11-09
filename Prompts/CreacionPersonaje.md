# TAREA: Crear interfaz de creación y selección de personajes

## 1. Contexto del proyecto

Te encuentras dentro del directorio raíz de un proyecto de desarrollo de un videojuego inspirado en Stardew Valley.

El proyecto está siendo desarrollado por un estudiante de segundo curso del Ciclo Formativo de Grado Superior de Desarrollo de Aplicaciones Multiplataforma (DAM).

Se necesita implementar una interfaz para la creación y selección de personajes del videojuego.

La implementación debe tener una dificultad adecuada para un estudiante de segundo de DAM. Se busca un resultado funcional, sencillo, visualmente atractivo y cuyo código sea fácil de entender, modificar y ampliar.

## 2. Objetivo principal

Desarrollar una interfaz de creación de personajes que permita al jugador personalizar su personaje antes de comenzar la partida.

La interfaz debe seguir una estética visual inspirada en los videojuegos de simulación de granjas y vida rural, especialmente Stardew Valley.

No se busca replicar exactamente su interfaz, sino utilizarla como referencia estética.

## 3. Funcionalidades necesarias

### 3.1. Creación del personaje

La interfaz debe permitir:

- Introducir el nombre del personaje.
- Seleccionar el aspecto o apariencia del personaje.
- Elegir entre las opciones de personalización que permita el sistema actual.
- Visualizar una vista previa del personaje mientras se realizan los cambios.
- Confirmar la creación del personaje.
- Disponer de una opción para cancelar o volver atrás.

No es necesario implementar un sistema de personalización excesivamente complejo.

Prioriza las opciones que puedan implementarse fácilmente con la tecnología y los recursos que ya existen en el proyecto.

### 3.2. Personajes predeterminados

Es importante preparar la interfaz para que en el futuro se puedan incorporar cinco personajes predeterminados.

Por tanto:

- Debe existir un espacio reservado para cinco personajes.
- Cada espacio debe estar preparado para mostrar la imagen o sprite del personaje.
- Cada personaje deberá poder seleccionarse mediante un botón o elemento interactivo.
- Al seleccionar uno de estos personajes, deberá poder utilizarse como personaje inicial sin necesidad de crearlo desde cero.

IMPORTANTE: Actualmente no es necesario crear los cinco personajes definitivos ni diseñar sus sprites.

Deja preparada la estructura necesaria para incorporarlos posteriormente de manera sencilla.

Utiliza datos de prueba o espacios visuales provisionales si son necesarios para desarrollar la interfaz.

La implementación futura de estos cinco personajes no debería requerir rehacer toda la interfaz.

## 4. Diseño visual

Mira las otras interfaces para llevar a cabo el diseño que más parecido sea al diseño visual del proyecto.

La interfaz debe tener una estética coherente con un videojuego de granjas y simulación de vida.

Evita interfaces excesivamente modernas, recargadas o con demasiados efectos.

La prioridad es que resulte agradable, intuitiva y fácil de utilizar.

## 5. Requisitos técnicos

Antes de comenzar a programar:

- Analiza la estructura actual del proyecto.
- Identifica el lenguaje de programación, framework y tecnologías utilizadas.
- Comprueba cómo están organizadas las interfaces existentes.
- Revisa los estilos visuales y componentes que ya utiliza el videojuego.
- Identifica cómo se gestionan actualmente las escenas, pantallas o navegación.

A partir de ese análisis:

- Utiliza las tecnologías existentes.
- Respeta la arquitectura y organización actual.
- Reutiliza componentes cuando sea posible.
- No introduzcas dependencias innecesarias.
- Evita modificar funcionalidades que no estén relacionadas con esta tarea.
- Mantén el código sencillo y comentado cuando sea necesario.
- Separa la lógica de la interfaz cuando la estructura del proyecto lo permita.

No implementes sistemas complejos de persistencia, inventarios o gestión de personajes si no son necesarios para esta funcionalidad.

## 6. Organización de archivos y documentación

Es obligatorio crear y utilizar las siguientes carpetas en el directorio raíz del proyecto:

### Carpeta Prompts

Crear la carpeta `Prompts/`.

Dentro de ella, guardar este prompt en un archivo llamado `Prompts/CreacionPersonaje.md`.

### Carpeta Documentacion

Crear la carpeta `Documentacion/`.

Dentro de ella, crear una subcarpeta llamada `Documentacion/Creacion de Personaje/`.

En esta carpeta se debe generar la documentación de la funcionalidad implementada.

Como mínimo, debe contener un archivo README.md con los siguientes apartados:

- Introducción: Explicación de la funcionalidad de creación de personajes.
- Objetivo: Qué se pretende conseguir con esta interfaz.
- Funcionamiento: Explicación del proceso de creación y selección de personajes.
- Diseño de la interfaz: Descripción de los elementos visuales y su distribución.
- Estructura del código: Archivos creados o modificados y su responsabilidad.
- Personajes predeterminados: Explicación de cómo está preparada la estructura para incorporar los cinco personajes.
- Tecnologías utilizadas: Lenguajes, frameworks y herramientas empleados.
- Posibles mejoras futuras: Funcionalidades que podrían añadirse posteriormente.

La documentación debe reflejar la implementación real del proyecto. No inventes funcionalidades que no se hayan desarrollado.

Si es posible, incluye capturas de pantalla de la interfaz terminada.

## 7. Restricciones

- No rehagas el proyecto desde cero.
- No modifiques funcionalidades ajenas a la creación de personajes.
- No añadas complejidad innecesaria.
- No implementes todavía los cinco personajes definitivos.
- No utilices sistemas de personalización demasiado avanzados.
- No introduzcas dependencias sin una justificación clara.
- Respeta el estilo y la estructura del código existente.

## 8. Comprobaciones finales

Cuando termines:

- Comprueba que la interfaz se puede abrir correctamente.
- Verifica que los campos y botones funcionan.
- Comprueba que se puede crear un personaje.
- Comprueba que los cinco espacios para personajes predeterminados están preparados.
- Comprueba que la estructura permite incorporar los personajes definitivos posteriormente.
- Comprueba que no has roto funcionalidades existentes.
- Verifica que el prompt se encuentra en `Prompts/CreacionPersonaje.md`.
- Verifica que la documentación está en `Documentacion/Creacion de Personaje/`.

## 9. Resultado esperado

Al finalizar, el proyecto debe disponer de una interfaz funcional de creación y selección de personajes, con un diseño inspirado en Stardew Valley y una estructura preparada para incorporar cinco personajes predeterminados en el futuro.

Además, deben quedar creados el archivo del prompt y la documentación correspondiente en las carpetas indicadas.

Prioridad principal: conseguir una interfaz sencilla, funcional, visualmente coherente con el videojuego y adecuada al nivel de desarrollo de un estudiante de segundo de DAM.

## 10. Forma de trabajar

Antes de realizar cualquier modificación, analiza el proyecto y explica brevemente qué archivos vas a crear y cuáles necesitas modificar.

No comiences a programar hasta haber entendido la estructura existente.

Trabaja directamente sobre el proyecto actual, respetando sus tecnologías, convenciones y funcionalidades.

Si encuentras alguna incompatibilidad o necesitas tomar una decisión técnica importante, prioriza la solución más sencilla y compatible con el proyecto.

No sustituyas archivos completos si basta con modificar una parte de ellos.

Al finalizar, presenta un resumen de los cambios realizados, los archivos afectados y las pruebas efectuadas.

No afirmes que una funcionalidad está terminada si no has podido comprobar que funciona.
