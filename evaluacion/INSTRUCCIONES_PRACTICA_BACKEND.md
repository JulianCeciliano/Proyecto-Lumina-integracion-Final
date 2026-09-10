# Práctica de evaluación: depuración y reparación de backend .NET

## Contexto

Lumina es una aplicación web ASP.NET Core MVC para bienestar personal. El backend gestiona cuentas y perfiles, diario emocional, recordatorios, estados de ánimo, tips, ejercicios y alertas de riesgo. Usa Entity Framework Core con SQL Server y conserva el estado de inicio de sesión mediante sesión HTTP.

La copia de trabajo contiene incidencias intencionales de nivel medio-alto. Tu objetivo es investigar y reparar el backend para devolverlo a un estado coherente, seguro y verificable.

## Objetivo

Debes recuperar el funcionamiento completo del backend sin modificar la interfaz. La evaluación incluye problemas de compilación, configuración de servicios, lógica de negocio, validación, persistencia con Entity Framework Core, comportamiento de administrador y asincronía.

No se calificará una solución que solo haga desaparecer errores del compilador si introduce conversiones arbitrarias, bypasses de seguridad o cambios incompatibles con el comportamiento esperado.

## Requisitos previos

- SDK de .NET compatible con el proyecto.
- Acceso a la instancia local de SQL Server o LocalDB configurada para esta copia.
- Git para revisar tu propio diff.
- Editor con depurador y consola de salida.

No ejecutes migraciones destructivas ni cambies datos reales. Trabaja siempre sobre una copia local de la práctica.

## Ejecutar, compilar y probar

Desde la carpeta Lumina_WEB:

~~~powershell
dotnet restore .\Lumina_WEB.slnx
dotnet build .\Lumina_WEB.slnx
dotnet test .\Lumina_WEB.slnx
dotnet run --project .\Lumina_WEB\Lumina_WEB.csproj
~~~

Empieza por los diagnósticos de compilación. Después de conseguir una compilación correcta, usa el navegador, los logs, el depurador y escenarios manuales para investigar los problemas de ejecución.

La solución puede no contener pruebas automatizadas detectables. En ese caso, documenta con precisión los escenarios manuales que ejecutaste y sus resultados.

## Reglas y restricciones

- Modifica únicamente backend C# y la configuración estrictamente necesaria para el backend.
- No modifiques Views, archivos cshtml, Razor, HTML, CSS, JavaScript, TypeScript, wwwroot, imágenes, recursos ni diseño.
- No cambies migraciones existentes, datos, secretos ni credenciales.
- No desactives validaciones ni abras acceso a usuarios no autorizados para hacer que un escenario funcione.
- Conserva las rutas, modelos y contratos que necesita la interfaz.
- No ocultes excepciones, no ignores tareas asíncronas y no uses cambios de base de datos como sustituto de una reparación.
- No uses ni entregues el documento de clave privada.

## Comportamientos que deben funcionar al terminar

Usa estos comportamientos como guía de aceptación, no como una lista de ubicaciones a editar:

1. La solución compila sin errores y el backend inicia correctamente.
2. Un usuario puede iniciar sesión y navegar por funciones que dependen de su sesión.
3. Un usuario puede crear un recordatorio válido; datos inválidos deben permanecer en el formulario con sus errores.
4. Al editar un recordatorio propio, los cambios deben persistir al volver a consultar la información.
5. Las entradas del diario deben mostrar el estado de ánimo relacionado y mantenerse asociadas al usuario actual.
6. Las alertas de riesgo deben progresar correctamente en sus umbrales de negocio, incluido el umbral máximo.
7. Un usuario puede actualizar su perfil conservando su propio correo y puede asociar una foto de perfil correctamente.
8. Un administrador legítimo debe ver las capacidades administrativas previstas para los contenidos de bienestar; una sesión normal debe seguir siendo de solo lectura.
9. Las operaciones de eliminación deben completar su persistencia antes de devolver al usuario al listado.

## Entregables esperados

Entrega:

- Los cambios de backend necesarios para reparar la práctica.
- Un resumen técnico de cada problema encontrado: síntoma, causa raíz, reparación y evidencia.
- Salidas relevantes de build y test.
- Escenarios manuales ejecutados, incluyendo datos de prueba no sensibles.
- El resultado de git diff para facilitar la revisión.

No incluyas cambios de interfaz, migraciones nuevas ni la clave privada de la evaluación.

## Criterios de evaluación

| Área | Puntos | Evidencia esperada |
| --- | ---: | --- |
| Compilación y contratos | 20 | Compilación limpia y reparaciones que preservan los contratos del backend. |
| Comportamiento funcional y validación | 25 | Flujos de perfil, diario y recordatorios con resultados coherentes. |
| Acceso a datos y persistencia | 20 | Consultas, relaciones y actualizaciones EF Core correctas y verificadas. |
| Seguridad y autorización | 15 | Las funciones administrativas se comportan correctamente sin elevar privilegios de sesiones normales. |
| Calidad de la solución | 10 | Cambios acotados, legibles, sin código muerto ni parches que oculten la causa. |
| Verificación y evidencia | 10 | Build, test y escenarios manuales documentados de forma reproducible. |
| **Total** | **100** | |

## Recomendación de trabajo

Avanza de lo más observable a lo más contextual:

1. Lee cada mensaje del compilador completo.
2. Sigue las dependencias entre controladores, modelo de datos, configuración y sesión.
3. Usa puntos de interrupción en las solicitudes afectadas y observa el estado de las entidades antes y después de guardar.
4. Comprueba la persistencia con una segunda consulta, no solo con un redirect exitoso.
5. Antes de entregar, revisa que tu diff no incluya archivos de interfaz ni recursos.
