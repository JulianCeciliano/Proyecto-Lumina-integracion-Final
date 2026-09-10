# CLAVE PRIVADA — NO DEBE ENTREGARSE AL ESTUDIANTE

Este documento contiene las respuestas, ubicaciones exactas y reparaciones de la práctica. Sepárelo de la copia que recibirá el estudiante.

## Control de origen

| Campo | Valor |
| --- | --- |
| Rama de origen | master |
| Commit de origen | 9382c7f2888e9793b22a9a7d19f626e141eebc94 |
| Rama de la práctica | practica-evaluacion-backend |
| Fecha de generación | 2026-09-10, America/Guatemala |
| Alcance de cambios | Backend C# y documentos bajo evaluacion/ |

El repositorio llegó como una rama sin commits y con todos los archivos sin seguimiento. Con autorización explícita se creó el commit base anterior antes de abrir la rama de práctica. No se hizo push, no se modificó master después de ese commit y no se ejecutaron migraciones ni se tocaron datos.

## Arquitectura detectada

La solución Lumina_WEB/Lumina_WEB.slnx contiene un único proyecto ASP.NET Core MVC, Lumina_WEB/Lumina_WEB/Lumina_WEB.csproj, dirigido a net10.0 y con EF Core/SQL Server 9.0.16. No hay proyectos de pruebas.

- Program.cs es el punto de entrada de nivel superior: registra MVC, caché distribuida en memoria, sesión y ApplicationDbContext con SQL Server; configura el pipeline HTTP.
- Los controladores usan ApplicationDbContext directamente. No existen repositorios, interfaces de servicio ni contenedores de dominio separados.
- ApplicationDbContext expone los DbSet de cuentas, usuarios, diario, recordatorios, alertas, catálogos y relaciones EntradaDiario → Usuario, Recordatorio → Usuario y Usuario → Cuenta.
- La autenticación existente es personalizada y basada en las claves de sesión CuentaId, UsuarioId y ModoAdmin. No hay AddAuthentication, UseAuthentication, claims, policies ni una relación real de roles/permisos aplicada a usuarios.
- DetectorAlertas es un helper estático de la capa backend. EntradasDiarioController lo llama después de crear o editar una entrada.

### Evidencia Graphify

Se leyó graphify-out/GRAPH_REPORT.md y se consultó graphify-out/graph.json únicamente de forma dirigida. El informe original contiene 437 nodos, 631 relaciones y 69 comunidades; ApplicationDbContext es el nodo central con 44 relaciones.

El ejecutable Graphify no estaba en PATH, pero se localizó en C:\Users\Julian\AppData\Roaming\uv\tools\graphifyy\Scripts\graphify.exe (graphifyy 0.9.57). Se ejecutaron las cuatro consultas solicitadas, además de explain sobre ApplicationDbContext y EntradasDiarioController, y path entre controladores y ApplicationDbContext. Las consultas de controladores/datos y usuarios/roles devolvieron subgrafos; las consultas léxicas de arquitectura y DI no encontraron nodos. Los paths confirmaron dependencias directas de controladores hacia ApplicationDbContext y la ruta no dirigida EntradasDiarioController → Create() → EvaluarYGenerarAlerta() ← DetectorAlertas.

Hay discrepancias conocidas entre el grafo y el código:

- Graphify no modela las sentencias de nivel superior de Program.cs; solo detecta una importación de Lumina_WEB.Datos y omite el registro de DbContext y sesión.
- Algunos rótulos repetidos de entidades se resuelven de forma ambigua en el grafo.
- El código fuente fue la autoridad para todas las decisiones de esta práctica.

No se actualizó el grafo después de alterar el backend. Un sello de caché creado por consultas fue eliminado para mantener graphify-out sin cambios.

## Comprobación inicial

En esta máquina dotnet no estaba disponible mediante PATH, por lo que se utilizó C:\Program Files\dotnet\dotnet.exe desde la carpeta Lumina_WEB.

| Comando | Resultado inicial |
| --- | --- |
| dotnet restore .\Lumina_WEB.slnx --verbosity minimal | Éxito, código 0; todos los proyectos actualizados. |
| dotnet build .\Lumina_WEB.slnx --no-restore --verbosity minimal | Éxito, código 0; 42 advertencias, 0 errores. |
| dotnet test .\Lumina_WEB.slnx --no-build --verbosity normal | Éxito, código 0; no se descubrieron ni ejecutaron pruebas. |

Las 42 advertencias preexistentes fueron principalmente CS8618 por propiedades no anulables sin inicializar, cuatro CS8602 en una vista de cuentas, dos CS8601 y dos CS8981 de una migración. No impidieron distinguir las incidencias de la práctica.

## Tabla maestra de incidencias

| ID | Categoría | Dificultad | Archivo | Símbolo | Síntoma principal |
| --- | --- | --- | --- | --- | --- |
| BACK-01 | Compilación / código faltante | Media | Lumina_WEB/Lumina_WEB/Controllers/UsuarioController.cs | GuardarFoto | CS0161 por falta de retorno. |
| BACK-02 | Compilación / contrato | Media | Lumina_WEB/Lumina_WEB/Controllers/DetectorAlertas.cs | ObtenerContador | CS0266 al pasar long a DeterminarNivel. |
| BACK-03 | Inyección de dependencias | Alta | Lumina_WEB/Lumina_WEB/Datos/ApplicationDbContext.cs | Constructor de ApplicationDbContext | El contenedor no resuelve las opciones genéricas correctas. |
| BACK-04 | Configuración de middleware | Media | Lumina_WEB/Lumina_WEB/Program.cs | Pipeline de sesión | Las acciones que leen sesión fallan en ejecución. |
| BACK-05 | Lógica de negocio | Media | Lumina_WEB/Lumina_WEB/Controllers/DetectorAlertas.cs | DeterminarNivel | En el umbral exacto 9 se calcula Media, no Grande. |
| BACK-06 | Validación | Media | Lumina_WEB/Lumina_WEB/Controllers/UsuarioController.cs | POST Perfil | El correo propio se trata como duplicado. |
| BACK-07 | Validación | Media | Lumina_WEB/Lumina_WEB/Controllers/RecordatorioController.cs | POST Create | Entradas válidas se devuelven al formulario. |
| BACK-08 | EF Core / tracking | Alta | Lumina_WEB/Lumina_WEB/Controllers/RecordatorioController.cs | POST Edit | El redirect ocurre, pero los cambios no se persisten. |
| BACK-09 | EF Core / carga de relación | Media | Lumina_WEB/Lumina_WEB/Controllers/EntradaDiarioController.cs | Index | La relación EstadoAnimo queda sin cargar al mostrar el diario. |
| BACK-10 | Autorización basada en sesión | Media | Lumina_WEB/Lumina_WEB/Controllers/TipController.cs | Index | Un administrador legítimo recibe solo lectura. |
| BACK-11 | Asincronía | Media | Lumina_WEB/Lumina_WEB/Controllers/EstadoAnimoController.cs | Delete | CS4014 y posible borrado no completado antes del redirect. |

## Detalle y clave de reparación

### BACK-01

| Campo | Contenido |
| --- | --- |
| ID | BACK-01 |
| Categoría | Compilación / recuperación de código faltante |
| Dificultad | Media |
| Archivo | Lumina_WEB/Lumina_WEB/Controllers/UsuarioController.cs |
| Símbolo | UsuarioController.GuardarFoto(IFormFile) |
| Estado original | Tras cerrar el FileStream, devolvía la ruta virtual construida a partir de WC.ImagenRuta, fileName y extension. |
| Alteración realizada | Se eliminó la única sentencia return del método Task<string>. |
| Motivo educativo | Recuperar una salida necesaria de una operación asíncrona y seguir el valor usado por dos flujos de perfil. |
| Síntoma | CS0161: no todos los caminos devuelven un valor. |
| Solución esperada | Restaurar exactamente la devolución de la ruta virtual: return "~" + WC.ImagenRuta.Replace("\\", "/") + fileName + extension;. |
| Verificación | Ejecutar dotnet build; después, crear o editar un perfil con foto y comprobar que se guarda una ruta de foto válida. |
| Impacto arquitectónico | Create y POST Perfil esperan await GuardarFoto; el método usa IWebHostEnvironment y WC para escribir en el área de recursos. |

Antes:

~~~csharp
return "~" + WC.ImagenRuta.Replace("\\", "/") + fileName + extension;
~~~

Después: no queda sentencia return al final del método.

### BACK-02

| Campo | Contenido |
| --- | --- |
| ID | BACK-02 |
| Categoría | Compilación / contrato |
| Dificultad | Media |
| Archivo | Lumina_WEB/Lumina_WEB/Controllers/DetectorAlertas.cs |
| Símbolo | DetectorAlertas.ObtenerContador(ApplicationDbContext, int) |
| Estado original | El contrato devolvía int, consistente con DeterminarNivel(int contador). |
| Alteración realizada | Se cambió el tipo de retorno de int a long. |
| Motivo educativo | Rastrear un contrato entre métodos de lógica de negocio y corregir tipos sin introducir conversiones que oculten el origen. |
| Síntoma | CS0266 en la asignación a contador dentro de EvaluarYGenerarAlerta. |
| Solución esperada | Restaurar el tipo de retorno int; no agregar un cast en el consumidor. |
| Verificación | Ejecutar dotnet build y confirmar que desaparece CS0266. Probar la evaluación de alertas con entradas de riesgo. |
| Impacto arquitectónico | EntradasDiarioController.Create y Edit invocan EvaluarYGenerarAlerta; Graphify vincula este flujo con ApplicationDbContext y DetectorAlertas. |

Antes:

~~~csharp
public static int ObtenerContador(ApplicationDbContext db, int usuarioId)
~~~

Después:

~~~csharp
public static long ObtenerContador(ApplicationDbContext db, int usuarioId)
~~~

### BACK-03

| Campo | Contenido |
| --- | --- |
| ID | BACK-03 |
| Categoría | Inyección de dependencias |
| Dificultad | Alta |
| Archivo | Lumina_WEB/Lumina_WEB/Datos/ApplicationDbContext.cs |
| Símbolo | Constructor de ApplicationDbContext |
| Estado original | Recibía DbContextOptions<ApplicationDbContext>, el contrato registrado por AddDbContext<ApplicationDbContext>. |
| Alteración realizada | El parámetro cambió a DbContextOptions<DbContext>. |
| Motivo educativo | Diagnosticar una resolución DI que compila pero solicita una abstracción genérica distinta de la registrada. |
| Síntoma | Al activar un controlador que depende del contexto, el contenedor no puede resolver DbContextOptions<DbContext>. |
| Solución esperada | Restaurar DbContextOptions<ApplicationDbContext> en el constructor. |
| Verificación | Una vez resueltos los errores de compilación, iniciar la aplicación y cargar una acción que usa datos; no debe haber InvalidOperationException de resolución de servicios. |
| Impacto arquitectónico | Program.cs registra ApplicationDbContext y los controladores de cuentas, diario, recordatorios, usuarios y catálogos lo reciben por constructor. |

Antes:

~~~csharp
public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :
~~~

Después:

~~~csharp
public ApplicationDbContext(DbContextOptions<DbContext> options) :
~~~

### BACK-04

| Campo | Contenido |
| --- | --- |
| ID | BACK-04 |
| Categoría | Configuración / middleware |
| Dificultad | Media |
| Archivo | Lumina_WEB/Lumina_WEB/Program.cs |
| Símbolo | Pipeline de solicitudes |
| Estado original | app.UseSession() se ejecutaba después de app.UseRouting() y antes de app.UseAuthorization(). |
| Alteración realizada | Se eliminó app.UseSession() sin eliminar AddSession(). |
| Motivo educativo | Distinguir el registro de servicios de la activación del middleware correspondiente. |
| Síntoma | Login y otras acciones que acceden a HttpContext.Session lanzan que la sesión no fue configurada para la aplicación o solicitud. |
| Solución esperada | Restaurar app.UseSession() entre UseRouting() y UseAuthorization(). |
| Verificación | Iniciar la aplicación, completar login y abrir una acción de diario o recordatorios; el acceso a sesión debe funcionar. |
| Impacto arquitectónico | AuthController establece CuentaId, UsuarioId y ModoAdmin; varios controladores leen esas claves. Graphify omite este detalle de Program.cs, por lo que se confirmó en código. |

### BACK-05

| Campo | Contenido |
| --- | --- |
| ID | BACK-05 |
| Categoría | Lógica de negocio |
| Dificultad | Media |
| Archivo | Lumina_WEB/Lumina_WEB/Controllers/DetectorAlertas.cs |
| Símbolo | DetectorAlertas.DeterminarNivel |
| Estado original | El nivel Grande aplicaba cuando contador era mayor o igual al umbral de 9. |
| Alteración realizada | Se sustituyó >= UmbralGrande por > UmbralGrande. |
| Motivo educativo | Comprobar bordes de reglas escalonadas y evitar errores de comparación de una sola unidad. |
| Síntoma | Con exactamente nueve entradas de riesgo, el flujo cae en Media; con diez vuelve a Grande. |
| Solución esperada | Restaurar la comparación contador >= UmbralGrande. |
| Verificación | Crear o evaluar suficientes entradas de riesgo para alcanzar exactamente 9 y confirmar el nivel Grande. |
| Impacto arquitectónico | El resultado alimenta TempData en EntradasDiarioController y el historial de alertas asociado a ApplicationDbContext. |

### BACK-06

| Campo | Contenido |
| --- | --- |
| ID | BACK-06 |
| Categoría | Validación de regla de negocio |
| Dificultad | Media |
| Archivo | Lumina_WEB/Lumina_WEB/Controllers/UsuarioController.cs |
| Símbolo | POST UsuarioController.Perfil |
| Estado original | La comprobación de email descartaba la cuenta actual mediante c.Id != usuario.idCuenta. |
| Alteración realizada | Se cambió el predicado a c.Id == usuario.idCuenta. |
| Motivo educativo | Revisar una validación de unicidad que debe excluir el registro que se está editando. |
| Síntoma | Guardar cambios de perfil con el mismo correo propio informa erróneamente que ya está registrado. |
| Solución esperada | Restaurar c.Id != usuario.idCuenta. |
| Verificación | Con una sesión de usuario válida, editar un campo del perfil sin cambiar su correo y confirmar la persistencia. |
| Impacto arquitectónico | El flujo carga Usuario con su Cuenta y consulta Cuentas para preservar la unicidad del correo. |

### BACK-07

| Campo | Contenido |
| --- | --- |
| ID | BACK-07 |
| Categoría | Validación |
| Dificultad | Media |
| Archivo | Lumina_WEB/Lumina_WEB/Controllers/RecordatorioController.cs |
| Símbolo | POST RecordatorioController.Create |
| Estado original | La acción devolvía la vista cuando ModelState no era válido y guardaba solo datos válidos. |
| Alteración realizada | Se invirtió la condición a if (ModelState.IsValid). |
| Motivo educativo | Diagnosticar inversión de control en la frontera HTTP/modelo antes de una escritura. |
| Síntoma | Un formulario válido vuelve a mostrarse; datos inválidos pueden avanzar a la persistencia. |
| Solución esperada | Restaurar if (!ModelState.IsValid). |
| Verificación | Enviar un recordatorio válido y comprobar redirección/listado; enviar datos inválidos y confirmar que la vista conserva el modelo. |
| Impacto arquitectónico | La acción asigna el propietario desde UsuarioId de sesión antes de validar y persiste en ApplicationDbContext.Recordatorios. |

### BACK-08

| Campo | Contenido |
| --- | --- |
| ID | BACK-08 |
| Categoría | EF Core / tracking |
| Dificultad | Alta |
| Archivo | Lumina_WEB/Lumina_WEB/Controllers/RecordatorioController.cs |
| Símbolo | POST RecordatorioController.Edit |
| Estado original | La consulta de recordatorioExistente estaba trackeada por el DbContext; las asignaciones posteriores se detectaban al guardar. |
| Alteración realizada | Se agregó AsNoTracking() a esa consulta. |
| Motivo educativo | Distinguir lectura de solo consulta de una entidad que será modificada en la misma unidad de trabajo. |
| Síntoma | La acción redirige sin error, pero título, descripción, fecha y estado no quedan actualizados. |
| Solución esperada | Quitar AsNoTracking() de esta consulta; no reemplazarlo por Update de un modelo ligado sin validar propiedad y propietario. |
| Verificación | Editar un recordatorio propio, recargar el listado o reiniciar la solicitud y confirmar que los cambios persisten. |
| Impacto arquitectónico | El filtro por IdUsuario conserva la propiedad del registro y SaveChangesAsync depende del tracking de recordatorioExistente. |

### BACK-09

| Campo | Contenido |
| --- | --- |
| ID | BACK-09 |
| Categoría | EF Core / carga de relación |
| Dificultad | Media |
| Archivo | Lumina_WEB/Lumina_WEB/Controllers/EntradaDiarioController.cs |
| Símbolo | GET EntradasDiarioController.Index |
| Estado original | La consulta incluía EstadoAnimo antes de ordenar y materializar las entradas. |
| Alteración realizada | Se eliminó Include(e => e.EstadoAnimo). |
| Motivo educativo | Identificar una carga explícita necesaria cuando no hay lazy loading configurado. |
| Síntoma | Al mostrar entradas del diario, la información relacionada de estado de ánimo queda nula o no está disponible. |
| Solución esperada | Restaurar Include(e => e.EstadoAnimo) en la consulta, manteniendo el filtro por el usuario en sesión. |
| Verificación | Con entradas que tengan estado de ánimo, abrir el listado y confirmar que se visualiza el dato relacionado. |
| Impacto arquitectónico | La relación EntradaDiario → EstadoAnimo procede del modelo y el listado ya mantiene el filtro de propiedad por UsuarioId. |

### BACK-10

| Campo | Contenido |
| --- | --- |
| ID | BACK-10 |
| Categoría | Autorización basada en sesión |
| Dificultad | Media |
| Archivo | Lumina_WEB/Lumina_WEB/Controllers/TipController.cs |
| Símbolo | TipController.Index |
| Estado original | EsAdmin se obtenía con HttpContext.Session.GetInt32("ModoAdmin") == 1. |
| Alteración realizada | La comparación ahora exige == 2. |
| Motivo educativo | Seguir una clave/valor de autorización existente desde su emisión hasta su consumo sin conceder acceso adicional. |
| Síntoma | Una sesión que AuthController marcó como administrador recibe la vista de solo lectura. Ningún usuario normal obtiene privilegios. |
| Solución esperada | Restaurar la comparación == 1. |
| Verificación | Iniciar sesión en el modo administrador ya existente y confirmar que el flujo de administración de tips vuelve a estar disponible; una sesión normal debe seguir en solo lectura. |
| Impacto arquitectónico | AuthController establece ModoAdmin; TipController consume la clave para seleccionar la experiencia. La práctica no avala este esquema propio como autenticación de producción. |

### BACK-11

| Campo | Contenido |
| --- | --- |
| ID | BACK-11 |
| Categoría | Asincronía |
| Dificultad | Media |
| Archivo | Lumina_WEB/Lumina_WEB/Controllers/EstadoAnimoController.cs |
| Símbolo | POST EstadoAnimoController.Delete |
| Estado original | Después de Remove, esperaba await _db.SaveChangesAsync() antes del redirect. |
| Alteración realizada | Se eliminó await y se dejó la tarea sin observar. |
| Motivo educativo | Detectar una operación persistente iniciada sin esperar su finalización antes de terminar la solicitud. |
| Síntoma | Advertencia CS4014; el borrado puede no completarse de forma fiable o fallar tarde cuando el contexto ya no esté disponible. |
| Solución esperada | Restaurar await _db.SaveChangesAsync(). |
| Verificación | Borrar un estado de ánimo, volver a cargar el listado y confirmar que el registro no reaparece; ejecutar dotnet build sin la advertencia CS4014. |
| Impacto arquitectónico | El controlador modifica ApplicationDbContext.EstadosAnimo y depende del ciclo de vida de solicitud del DbContext. |

## Resultado tras introducir las incidencias

| Comando | Resultado |
| --- | --- |
| dotnet build .\Lumina_WEB.slnx --no-restore --verbosity minimal | Código 1; 43 advertencias y 2 errores. Los errores son CS0266 de BACK-02 y CS0161 de BACK-01. La advertencia adicional CS4014 corresponde a BACK-11. |
| dotnet test .\Lumina_WEB.slnx --verbosity minimal | Código 0; solo informó restauración actualizada. No hay proyectos/pruebas detectables, por lo que no compiló ni ejecutó pruebas de aplicación. |

Las 42 advertencias de base siguen presentes. No se repararon las incidencias intencionales tras validar.

## Procedimiento de verificación después de reparar

1. Ejecutar restore, build y test de la solución.
2. Iniciar la aplicación con dotnet run desde el proyecto.
3. Validar sesión con login y un flujo que lea CuentaId o UsuarioId.
4. Crear, editar y consultar recordatorios propios, distinguiendo datos válidos e inválidos.
5. Crear entradas de diario con estado de ánimo, validar que sus relaciones se muestran y probar los umbrales 3, 6 y 9 de alerta.
6. Editar un perfil conservando el email actual y adjuntar una foto.
7. Comprobar el acceso administrativo existente para la gestión de tips sin elevar una sesión normal.
8. Eliminar un estado de ánimo y recargar para confirmar persistencia.

## Comparación y recuperación con Git

Inspeccionar la práctica sin alterarla:

~~~powershell
git status --short
git diff --name-only 9382c7f2888e9793b22a9a7d19f626e141eebc94
git diff 9382c7f2888e9793b22a9a7d19f626e141eebc94 -- Lumina_WEB/Lumina_WEB
~~~

Consultar una versión correcta concreta:

~~~powershell
git show 9382c7f2888e9793b22a9a7d19f626e141eebc94:Lumina_WEB/Lumina_WEB/Controllers/DetectorAlertas.cs
~~~

Para volver a la copia base sin borrar la rama de práctica, cambiar a master:

~~~powershell
git switch master
~~~

Para restaurar deliberadamente un solo archivo de la rama de práctica al estado de origen, primero inspeccionar el diff y luego usar una ruta explícita:

~~~powershell
git restore --source=9382c7f2888e9793b22a9a7d19f626e141eebc94 -- Lumina_WEB/Lumina_WEB/Controllers/DetectorAlertas.cs
~~~

No usar un restore masivo si se desea conservar la práctica; recuperar cada incidencia como parte del ejercicio o crear una rama nueva desde el commit de origen.
