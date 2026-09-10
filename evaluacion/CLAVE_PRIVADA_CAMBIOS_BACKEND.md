# CLAVE PRIVADA — NO DEBE ENTREGARSE AL ESTUDIANTE

Esta clave contiene las causas, reparaciones y fragmentos exactos de la práctica. Debe separarse de toda copia destinada al estudiante.

## Control y arquitectura

| Campo | Valor |
| --- | --- |
| Rama de origen registrada | master |
| Commit de origen registrado | 9382c7f2888e9793b22a9a7d19f626e141eebc94 |
| Rama de práctica | practica-evaluacion-backend |
| Fecha | 2026-09-10, America/Guatemala |
| Alcance | Solo C# backend y evaluacion/ |

La solución Lumina_WEB/Lumina_WEB.slnx contiene un proyecto ASP.NET Core MVC net10.0 con EF Core/SQL Server. Program.cs registra MVC, sesión y ApplicationDbContext. Los controladores usan el contexto directamente; no hay repositorios ni servicios inyectados convencionales. DetectorAlertas es un helper estático usado desde el flujo de diario. La autenticación existente depende de sesión.

Graphify original: 437 nodos, 631 relaciones y ApplicationDbContext como nodo central. El código confirmó que los controladores dependen del contexto y que EntradasDiarioController llama a DetectorAlertas. Graphify no representa correctamente las sentencias de nivel superior de Program.cs; el código es la fuente de verdad.

## Línea base

| Comando | Resultado antes de la práctica |
| --- | --- |
| dotnet restore .\Lumina_WEB.slnx --verbosity minimal | Código 0. |
| dotnet build .\Lumina_WEB.slnx --no-restore --verbosity minimal | Código 0; 42 advertencias, 0 errores. |
| dotnet test .\Lumina_WEB.slnx --no-build --verbosity normal | Código 0; no se descubrieron pruebas. |

El host necesitó C:\Program Files\dotnet\dotnet.exe porque dotnet no estaba en PATH. Las 42 advertencias son preexistentes, principalmente nullable y una advertencia de migración.

## Tabla maestra

| ID | Categoría | Dificultad | Archivo | Símbolo | Error previsto |
| --- | --- | --- | --- | --- | --- |
| BACK-01 | Compilación / retorno faltante | Media | Controllers/UsuarioController.cs | GuardarFoto | CS0161 |
| BACK-02 | Compilación / contrato de tipo | Media | Controllers/DetectorAlertas.cs | ObtenerContador | CS0266 |
| BACK-03 | Compilación / relación EF | Alta | Datos/ApplicationDbContext.cs | OnModelCreating | CS1061 |
| BACK-04 | Compilación / referencia EF | Media | Program.cs | Registro de DbContext | CS1061 |
| BACK-05 | Compilación / contrato de sesión | Media | Controllers/AuthController.cs | Login POST | CS1503 |
| BACK-06 | Compilación / llamada de servicio | Media | Controllers/EntradaDiarioController.cs | Create POST | CS1503 |
| BACK-07 | Compilación / firma de API async | Media | Controllers/RecordatorioController.cs | Edit POST | CS1503 |
| BACK-08 | Compilación / colección tipada | Media | Controllers/TipoAlertaController.cs | Index | CS0266 |
| BACK-09 | Compilación / colección tipada | Media | Controllers/TipController.cs | Index | CS0029 |
| BACK-10 | Compilación / miembro de modelo | Media | Controllers/EstadoAnimoController.cs | Index | CS1061 |
| BACK-11 | Compilación / contrato de modelo | Media | Controllers/HomeController.cs | Index | CS0029 |

## Fichas de reparación

### BACK-01

| Campo | Contenido |
| --- | --- |
| ID | BACK-01 |
| Categoría | Compilación / retorno faltante |
| Dificultad | Media |
| Archivo | Lumina_WEB/Lumina_WEB/Controllers/UsuarioController.cs |
| Símbolo | UsuarioController.GuardarFoto(IFormFile) |
| Estado original | El método Task<string> devolvía la ruta virtual de la foto. |
| Alteración realizada | Se eliminó su sentencia return final. |
| Motivo educativo | Recuperar una salida requerida de una operación asíncrona. |
| Síntoma | CS0161: no todos los caminos devuelven un valor. |
| Solución esperada | Restaurar la sentencia return original. |
| Verificación | dotnet build deja de informar CS0161. |
| Impacto arquitectónico | Create y Perfil esperan el resultado de GuardarFoto. |

**Antes**

~~~csharp
return "~" + WC.ImagenRuta.Replace("\\", "/") + fileName + extension;
~~~

**Después**

~~~csharp
// No existe una sentencia return al final del método.
~~~

### BACK-02

| Campo | Contenido |
| --- | --- |
| ID | BACK-02 |
| Categoría | Compilación / contrato de tipo |
| Dificultad | Media |
| Archivo | Lumina_WEB/Lumina_WEB/Controllers/DetectorAlertas.cs |
| Símbolo | DetectorAlertas.ObtenerContador |
| Estado original | El método devolvía int, compatible con DeterminarNivel(int). |
| Alteración realizada | Su retorno cambió a long. |
| Motivo educativo | Seguir un contrato de tipos entre métodos de negocio. |
| Síntoma | CS0266 al asignar el contador. |
| Solución esperada | Restaurar el retorno int, sin agregar casts. |
| Verificación | dotnet build deja de informar CS0266 en DetectorAlertas. |
| Impacto arquitectónico | El cálculo alimenta alertas del flujo de diario. |

**Antes**

~~~csharp
public static int ObtenerContador(ApplicationDbContext db, int usuarioId)
~~~

**Después**

~~~csharp
public static long ObtenerContador(ApplicationDbContext db, int usuarioId)
~~~

### BACK-03

| Campo | Contenido |
| --- | --- |
| ID | BACK-03 |
| Categoría | Compilación / relación EF |
| Dificultad | Alta |
| Archivo | Lumina_WEB/Lumina_WEB/Datos/ApplicationDbContext.cs |
| Símbolo | ApplicationDbContext.OnModelCreating |
| Estado original | La relación Usuario → Cuenta usaba la propiedad idCuenta existente. |
| Alteración realizada | Se cambió la propiedad de clave foránea a IdCuenta. |
| Motivo educativo | Validar que una configuración EF coincide exactamente con el modelo. |
| Síntoma | CS1061: Usuario no contiene IdCuenta. |
| Solución esperada | Restaurar idCuenta. |
| Verificación | dotnet build deja de informar CS1061 en ApplicationDbContext. |
| Impacto arquitectónico | La relación la consumen perfiles, cuentas y datos asociados a usuarios. |

**Antes**

~~~csharp
.HasForeignKey(u => u.idCuenta)
~~~

**Después**

~~~csharp
.HasForeignKey(u => u.IdCuenta)
~~~

### BACK-04

| Campo | Contenido |
| --- | --- |
| ID | BACK-04 |
| Categoría | Compilación / referencia EF |
| Dificultad | Media |
| Archivo | Lumina_WEB/Lumina_WEB/Program.cs |
| Símbolo | Registro de ApplicationDbContext |
| Estado original | La extensión UseSqlServer estaba disponible mediante su using. |
| Alteración realizada | Se eliminó using Microsoft.EntityFrameworkCore;. |
| Motivo educativo | Resolver una dependencia de extensión a partir del compilador. |
| Síntoma | CS1061: DbContextOptionsBuilder no contiene UseSqlServer. |
| Solución esperada | Restaurar using Microsoft.EntityFrameworkCore;. |
| Verificación | dotnet build reconoce UseSqlServer. |
| Impacto arquitectónico | Program configura la única conexión EF del backend. |

**Antes**

~~~csharp
using Microsoft.EntityFrameworkCore;
~~~

**Después**

~~~csharp
// La importación de EntityFrameworkCore no está presente.
~~~

### BACK-05

| Campo | Contenido |
| --- | --- |
| ID | BACK-05 |
| Categoría | Compilación / contrato de sesión |
| Dificultad | Media |
| Archivo | Lumina_WEB/Lumina_WEB/Controllers/AuthController.cs |
| Símbolo | AuthController.Login POST |
| Estado original | SetInt32 recibía el identificador entero de cuenta. |
| Alteración realizada | Se le pasó Email, que es string. |
| Motivo educativo | Comprobar la firma de una API de sesión. |
| Síntoma | CS1503: string no se puede convertir a int. |
| Solución esperada | Restaurar cuenta.Id como segundo valor de SetInt32. |
| Verificación | dotnet build deja de informar CS1503 en AuthController. |
| Impacto arquitectónico | CuentaId es una clave de sesión leída por los flujos de perfil y contenido personal. |

**Antes**

~~~csharp
HttpContext.Session.SetInt32("CuentaId", cuenta.Id);
~~~

**Después**

~~~csharp
HttpContext.Session.SetInt32("CuentaId", cuenta.Email);
~~~

### BACK-06

| Campo | Contenido |
| --- | --- |
| ID | BACK-06 |
| Categoría | Compilación / llamada de servicio |
| Dificultad | Media |
| Archivo | Lumina_WEB/Lumina_WEB/Controllers/EntradaDiarioController.cs |
| Símbolo | EntradasDiarioController.Create POST |
| Estado original | El segundo argumento era ApplicationDbContext. |
| Alteración realizada | Se sustituyó por la propiedad Database del contexto. |
| Motivo educativo | Comparar tipos de argumentos con la firma real de un helper. |
| Síntoma | CS1503: DatabaseFacade no se puede convertir a ApplicationDbContext. |
| Solución esperada | Restaurar _context como segundo argumento. |
| Verificación | dotnet build deja de informar CS1503 en Create. |
| Impacto arquitectónico | Create usa DetectorAlertas para procesar una entrada guardada. |

**Antes**

~~~csharp
DetectorAlertas.EvaluarYGenerarAlerta(entrada, _context);
~~~

**Después**

~~~csharp
DetectorAlertas.EvaluarYGenerarAlerta(entrada, _context.Database);
~~~

### BACK-07

| Campo | Contenido |
| --- | --- |
| ID | BACK-07 |
| Categoría | Compilación / firma de API async |
| Dificultad | Media |
| Archivo | Lumina_WEB/Lumina_WEB/Controllers/RecordatorioController.cs |
| Símbolo | RecordatorioController.Edit POST |
| Estado original | SaveChangesAsync se invocaba sin argumentos. |
| Alteración realizada | Se agregó recordatorio como argumento. |
| Motivo educativo | Consultar una firma asíncrona y el tipo de su parámetro opcional. |
| Síntoma | CS1503: Recordatorio no se puede convertir a CancellationToken. |
| Solución esperada | Restaurar SaveChangesAsync() sin argumentos. |
| Verificación | dotnet build deja de informar CS1503 en Edit. |
| Impacto arquitectónico | Edit persiste cambios del recordatorio del usuario actual. |

**Antes**

~~~csharp
await _context.SaveChangesAsync();
~~~

**Después**

~~~csharp
await _context.SaveChangesAsync(recordatorio);
~~~

### BACK-08

| Campo | Contenido |
| --- | --- |
| ID | BACK-08 |
| Categoría | Compilación / colección tipada |
| Dificultad | Media |
| Archivo | Lumina_WEB/Lumina_WEB/Controllers/TipoAlertaController.cs |
| Símbolo | TipoAlertaController.Index |
| Estado original | La colección declarada de TipoAlerta procedía de TiposAlerta. |
| Alteración realizada | Se asignó la colección Tips. |
| Motivo educativo | Detectar una incompatibilidad entre DbSet y colección de destino. |
| Síntoma | CS0266 entre DbSet<Tip> e IEnumerable<TipoAlerta>. |
| Solución esperada | Restaurar _db.TiposAlerta. |
| Verificación | dotnet build deja de informar CS0266 en TipoAlertaController. |
| Impacto arquitectónico | El catálogo de tipos de alerta alimenta el sistema de alertas. |

**Antes**

~~~csharp
IEnumerable<TipoAlerta> lista = _db.TiposAlerta;
~~~

**Después**

~~~csharp
IEnumerable<TipoAlerta> lista = _db.Tips;
~~~

### BACK-09

| Campo | Contenido |
| --- | --- |
| ID | BACK-09 |
| Categoría | Compilación / colección tipada |
| Dificultad | Media |
| Archivo | Lumina_WEB/Lumina_WEB/Controllers/TipController.cs |
| Símbolo | TipController.Index |
| Estado original | La lista se infería desde _context.Tips. |
| Alteración realizada | Se la declaró como List<TipoAlerta>. |
| Motivo educativo | Rastrear el tipo materializado por ToListAsync. |
| Síntoma | CS0029 entre List<Tip> y List<TipoAlerta>. |
| Solución esperada | Restaurar var tips o declarar List<Tip>. |
| Verificación | dotnet build deja de informar CS0029 en TipController. |
| Impacto arquitectónico | Index prepara el catálogo de tips para la presentación. |

**Antes**

~~~csharp
var tips = await _context.Tips.ToListAsync();
~~~

**Después**

~~~csharp
List<TipoAlerta> tips = await _context.Tips.ToListAsync();
~~~

### BACK-10

| Campo | Contenido |
| --- | --- |
| ID | BACK-10 |
| Categoría | Compilación / miembro de modelo |
| Dificultad | Media |
| Archivo | Lumina_WEB/Lumina_WEB/Controllers/EstadoAnimoController.cs |
| Símbolo | EstadoAnimoController.Index |
| Estado original | El ordenamiento usaba la propiedad Nombre del modelo. |
| Alteración realizada | Se cambió a NombreCompleto, que el modelo no define. |
| Motivo educativo | Validar acceso a miembros de entidades antes de ejecutar la consulta. |
| Síntoma | CS1061: EstadoAnimo no contiene NombreCompleto. |
| Solución esperada | Restaurar a.Nombre. |
| Verificación | dotnet build deja de informar CS1061 en EstadoAnimoController. |
| Impacto arquitectónico | El índice consulta ApplicationDbContext.EstadosAnimo. |

**Antes**

~~~csharp
.OrderBy(a => a.Nombre)
~~~

**Después**

~~~csharp
.OrderBy(a => a.NombreCompleto)
~~~

### BACK-11

| Campo | Contenido |
| --- | --- |
| ID | BACK-11 |
| Categoría | Compilación / contrato de modelo |
| Dificultad | Media |
| Archivo | Lumina_WEB/Lumina_WEB/Controllers/HomeController.cs |
| Símbolo | HomeController.Index |
| Estado original | TipDelDia recibía un Tip desde _context.Tips. |
| Alteración realizada | Ahora recibe un Ejercicio desde _context.Ejercicios. |
| Motivo educativo | Comparar el tipo de una consulta EF con el tipo de destino del view model. |
| Síntoma | CS0029: Ejercicio no se puede convertir a Tip. |
| Solución esperada | Restaurar _context.Tips.FirstOrDefaultAsync(). |
| Verificación | dotnet build deja de informar CS0029 en HomeController. |
| Impacto arquitectónico | HomeUsuarioViewModel separa tip del día y ejercicios sugeridos. |

**Antes**

~~~csharp
TipDelDia = await _context.Tips.FirstOrDefaultAsync()
~~~

**Después**

~~~csharp
TipDelDia = await _context.Ejercicios.FirstOrDefaultAsync()
~~~

## Resultado de la práctica alterada

| Comando | Resultado |
| --- | --- |
| dotnet build .\Lumina_WEB.slnx --no-restore --verbosity minimal | Código 1; 42 advertencias preexistentes y 11 errores de compilación intencionales. |
| dotnet test .\Lumina_WEB.slnx --verbosity minimal | Código 0; restauración actualizada. No hay proyectos de prueba detectables ni pruebas ejecutadas. |

## Verificación final de reparación

1. Restaurar cada fragmento de la sección Antes correspondiente.
2. Ejecutar restore, build y test.
3. Confirmar que build termina con código 0 y no contiene los 11 errores documentados.
4. Revisar git diff y comprobar que no se modificaron vistas, recursos, migraciones ni frontend.

## Comparación y recuperación

~~~powershell
git status --short
git diff --name-only
git diff -- Lumina_WEB/Lumina_WEB
git show 9382c7f2888e9793b22a9a7d19f626e141eebc94:Lumina_WEB/Lumina_WEB/Controllers/DetectorAlertas.cs
~~~

Para recuperar un archivo concreto, inspeccionar primero su diff y después usar una ruta explícita con git restore --source=<commit> -- <ruta>.
