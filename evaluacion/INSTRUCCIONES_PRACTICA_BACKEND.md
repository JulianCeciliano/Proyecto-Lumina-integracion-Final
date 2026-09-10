# Práctica de evaluación: recuperación de compilación en .NET

## Contexto y objetivo

Lumina es una aplicación ASP.NET Core MVC de bienestar que usa Entity Framework Core, SQL Server y sesiones HTTP. Esta práctica contiene cambios intencionales en el backend que impiden compilar la solución.

Tu objetivo es diagnosticar cada error del compilador, recuperar contratos correctos entre capas y dejar el backend compilando sin modificar la interfaz.

## Requisitos previos

- SDK de .NET compatible con el proyecto.
- Git para revisar tu diff.
- Editor con navegación a errores, consola y depurador.

No ejecutes migraciones destructivas ni modifiques datos reales durante la práctica.

## Compilar y probar

Desde la carpeta Lumina_WEB:

~~~powershell
dotnet restore .\Lumina_WEB.slnx
dotnet build .\Lumina_WEB.slnx
dotnet test .\Lumina_WEB.slnx
~~~

El primer paso es resolver todos los errores de compilación. Lee cada diagnóstico completo: tipo esperado, tipo recibido, miembro inexistente, firma de método o referencia no disponible.

## Reglas

- Modifica solamente backend C# y lo estrictamente necesario para restaurar la compilación.
- No modifiques Views, cshtml, Razor, HTML, CSS, JavaScript, TypeScript, wwwroot, imágenes, recursos ni diseño.
- No modifiques migraciones existentes, secretos, credenciales ni configuración de producción.
- No debilites validaciones, autenticación ni controles de acceso para eliminar un error.
- Conserva los contratos que consume la interfaz.
- No consultes ni entregues la clave privada de la evaluación.

## Resultado esperado

Al finalizar:

1. La solución debe compilar con código 0.
2. Los contratos entre el acceso a datos, controladores, modelo y configuración deben ser coherentes.
3. Las llamadas asíncronas y APIs del framework deben respetar sus firmas.
4. Las consultas y colecciones deben usar los tipos del modelo que corresponden.
5. No debe haber cambios en interfaz, recursos, pruebas existentes ni migraciones.

## Entregables

- Código backend reparado.
- Salida de dotnet build y dotnet test.
- Resumen técnico de los diagnósticos resueltos: síntoma, causa y verificación.
- Resultado de git diff.

## Rúbrica (100 puntos)

| Área | Puntos | Evidencia |
| --- | ---: | --- |
| Errores de compilación y contratos | 45 | Solución compila y cada reparación conserva el contrato correcto. |
| Acceso a datos y modelos | 20 | Tipos, relaciones y consultas EF Core compatibles con las entidades. |
| Configuración y framework | 15 | Referencias y APIs de ASP.NET Core/EF Core restauradas correctamente. |
| Calidad de la solución | 10 | Cambios mínimos, legibles y sin conversiones o parches arbitrarios. |
| Verificación y evidencia | 10 | Build, test y diff documentados. |
| **Total** | **100** | |

## Recomendación

Resuelve primero los errores que impiden al compilador reconocer una referencia o un miembro. Continúa con incompatibilidades de tipos y firmas. Tras cada corrección, vuelve a compilar: una reparación puede revelar el siguiente diagnóstico. Antes de entregar, comprueba que tu diff solo contiene cambios de backend permitidos.
