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

Graphify registró 437 nodos y 631 relaciones, con ApplicationDbContext como nodo central. Para elegir un controlador se priorizaron destinos semánticos externos únicos sobre referencias repetidas: UsuarioController tiene 15, frente a 12 de EntradasDiarioController. Además, su nodo de clase tiene el mayor grado total entre controladores, 9 relaciones (8 salientes y 1 entrante), frente a 8 de RecordatorioController. El código confirmó que los controladores dependen del contexto y que EntradasDiarioController llama a DetectorAlertas. Graphify no representa correctamente las sentencias de nivel superior de Program.cs; el código es la fuente de verdad.

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
| BACK-03 | Compilación / relación EF con dependiente invertido | Alta | Datos/ApplicationDbContext.cs | OnModelCreating | CS1061 |
| BACK-04 | Compilación / proveedor EF y configuración tipada | Alta | Program.cs | Registro de DbContext | CS1503 |
| BACK-05 | Compilación / contrato de sesión | Media | Controllers/AuthController.cs | Login POST | CS1503 |
| BACK-06 | Compilación / llamada de servicio | Media | Controllers/EntradaDiarioController.cs | Create POST | CS1503 |
| BACK-07 | Compilación / firma de API async | Media | Controllers/RecordatorioController.cs | Edit POST | CS1503 |
| BACK-08 | Compilación / colección tipada | Media | Controllers/TipoAlertaController.cs | Index | CS0266 |
| BACK-09 | Compilación / colección tipada | Media | Controllers/TipController.cs | Index | CS0029 |
| BACK-10 | Compilación / miembro de modelo | Media | Controllers/EstadoAnimoController.cs | Index | CS1061 |
| BACK-11 | Compilación / contrato de modelo | Media | Controllers/HomeController.cs | Index | CS0029 |
| BACK-12 | Compilación / contrato síncrono-asíncrono del contexto | Alta | Datos/ApplicationDbContext.cs | SaveChanges | CS0029 |
| BACK-13 | Compilación / ciclo de vida DI y factory asíncrona | Alta | Program.cs | Registro adicional de ApplicationDbContext | CS0029 + CS1662 |
| BACK-14 | Compilación / frontera de dependencia del controlador | Muy alta | Controllers/UsuarioController.cs | Campo y constructor _context | 7 × CS1061 |

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
| Categoría | Compilación / relación EF con dependiente invertido |
| Dificultad | Alta |
| Archivo | Lumina_WEB/Lumina_WEB/Datos/ApplicationDbContext.cs |
| Símbolo | ApplicationDbContext.OnModelCreating |
| Estado original | La relación Usuario → Cuenta tenía a Usuario como dependiente, cardinalidad WithMany y la FK Usuario.idCuenta. |
| Alteración realizada | Se invirtieron simultáneamente cardinalidad y tipo dependiente: WithOne y HasForeignKey<Cuenta>; el selector ahora busca Cuenta.idCuenta. |
| Motivo educativo | Diferenciar principal, dependiente, cardinalidad y propiedad FK antes de seguir solo el primer miembro inexistente. |
| Síntoma | CS1061: Cuenta no contiene idCuenta. Es el síntoma visible; cambiarlo a Cuenta.Id haría compilar una relación semánticamente incorrecta. |
| Solución esperada | Restaurar la cadena completa: WithMany y HasForeignKey(u => u.idCuenta), sin el argumento genérico Cuenta. |
| Verificación | dotnet build deja de informar CS1061 y la configuración vuelve a representar Usuario → Cuenta. |
| Impacto arquitectónico | La relación la consumen perfiles, cuentas, migraciones y datos asociados a usuarios. |

**Antes**

~~~csharp
modelBuilder.Entity<Usuario>()
    .HasOne(u => u.Cuenta)
    .WithMany()
    .HasForeignKey(u => u.idCuenta)
    .OnDelete(DeleteBehavior.Cascade);
~~~

**Después**

~~~csharp
modelBuilder.Entity<Usuario>()
    .HasOne(u => u.Cuenta)
    .WithOne()
    .HasForeignKey<Cuenta>(cuenta => cuenta.idCuenta)
    .OnDelete(DeleteBehavior.Cascade);
~~~

### BACK-04

| Campo | Contenido |
| --- | --- |
| ID | BACK-04 |
| Categoría | Compilación / proveedor EF y configuración tipada |
| Dificultad | Alta |
| Archivo | Lumina_WEB/Lumina_WEB/Program.cs |
| Símbolo | Registro de ApplicationDbContext |
| Estado original | UseSqlServer recibía únicamente la cadena de conexión configurada. |
| Alteración realizada | Se agregó la sobrecarga del proveedor y se pasó la cadena de conexión a CommandTimeout. |
| Motivo educativo | Seguir la cadena Configuration → proveedor EF → opción tipada y distinguir una cadena de un tiempo de espera numérico. |
| Síntoma | CS1503: el argumento 1 no puede convertirse de string a int?. |
| Solución esperada | Restaurar la llamada de un solo argumento; no resolverlo con casts ni reutilizar la cadena de conexión como timeout. |
| Verificación | dotnet build deja de informar CS1503 en la configuración de SQL Server. |
| Impacto arquitectónico | Program configura la única conexión EF del backend. |

**Antes**

~~~csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);
~~~

**Después**

~~~csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptions => sqlServerOptions.CommandTimeout(
            builder.Configuration.GetConnectionString("DefaultConnection")
        )
    )
);
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

### BACK-12

| Campo | Contenido |
| --- | --- |
| ID | BACK-12 |
| Categoría | Compilación / contrato síncrono-asíncrono del contexto |
| Dificultad | Alta |
| Archivo | Lumina_WEB/Lumina_WEB/Datos/ApplicationDbContext.cs |
| Símbolo | ApplicationDbContext.SaveChanges(bool) |
| Estado original | ApplicationDbContext no redefinía SaveChanges; se usaba directamente la implementación síncrona de DbContext. |
| Alteración realizada | Se añadió una sobreescritura síncrona que intenta devolver SaveChangesAsync. |
| Motivo educativo | Distinguir contrato de retorno, ciclo de vida del contexto y API síncrona/asíncrona sin "arreglar" con bloqueos de tareas. |
| Síntoma | CS0029: Task<int> no se puede convertir implícitamente a int. |
| Solución esperada | Eliminar la sobreescritura para restaurar el estado original, o delegar en base.SaveChanges si se conserva por una razón real. |
| Verificación | dotnet build deja de informar CS0029 en ApplicationDbContext. |
| Impacto arquitectónico | Afectaría todos los controladores y helpers que persisten entidades con el contexto compartido. |

**Antes**

~~~csharp
// ApplicationDbContext no sobreescribe SaveChanges.
~~~

**Después**

~~~csharp
public override int SaveChanges(bool acceptAllChangesOnSuccess)
{
    return base.SaveChangesAsync(acceptAllChangesOnSuccess);
}
~~~

### BACK-13

| Campo | Contenido |
| --- | --- |
| ID | BACK-13 |
| Categoría | Compilación / ciclo de vida DI y factory asíncrona |
| Dificultad | Alta |
| Archivo | Lumina_WEB/Lumina_WEB/Program.cs |
| Símbolo | Registro adicional de ApplicationDbContext |
| Estado original | AddDbContext ya registra ApplicationDbContext con el ciclo de vida apropiado para las solicitudes web. |
| Alteración realizada | Se añadió un AddScoped redundante cuya factory pide IDbContextFactory<ApplicationDbContext> y devuelve CreateDbContextAsync sin esperar su Task. |
| Motivo educativo | Relacionar el contrato del delegado de DI, la fábrica EF, la asincronía y el ciclo de vida del contexto. |
| Síntoma | En la misma línea aparecen CS0029 (Task<ApplicationDbContext> a ApplicationDbContext) y CS1662 (lambda incompatible). Son dos diagnósticos de una sola alteración. |
| Solución esperada | Eliminar el AddScoped adicional. No usar casts, .Result ni GetAwaiter().GetResult(); AddDbContext es el registro que debe permanecer. |
| Verificación | dotnet build deja de informar ambos diagnósticos de Program.cs y no queda un registro duplicado del contexto. |
| Impacto arquitectónico | El contexto es dependencia directa de todos los controladores; un ciclo de vida incorrecto rompería peticiones concurrentes y el acceso a datos. |

**Antes**

~~~csharp
// AddDbContext<ApplicationDbContext>(...) es el único registro del contexto.
~~~

**Después**

~~~csharp
builder.Services.AddScoped<ApplicationDbContext>(serviceProvider =>
    serviceProvider
        .GetRequiredService<IDbContextFactory<ApplicationDbContext>>()
        .CreateDbContextAsync()
);
~~~

### BACK-14

| Campo | Contenido |
| --- | --- |
| ID | BACK-14 |
| Categoría | Compilación / frontera de dependencia del controlador |
| Dificultad | Muy alta |
| Archivo | Lumina_WEB/Lumina_WEB/Controllers/UsuarioController.cs |
| Símbolo | Campo `_context` y constructor de UsuarioController |
| Evidencia de selección | Graphify: UsuarioController tiene 15 destinos semánticos externos únicos, el máximo entre controladores, y el mayor grado de clase (9). |
| Estado original | El campo y el parámetro de constructor eran ApplicationDbContext, servicio registrado por AddDbContext. |
| Alteración realizada | Ambos tipos se estrecharon a DbSet<Usuario>, que representa solo una colección y no la unidad de trabajo ni un servicio registrable. |
| Motivo educativo | Encontrar una causa raíz de frontera de dependencia detrás de varios errores de miembros aparentemente independientes. |
| Síntoma | Siete CS1061 en las líneas 32, 68, 69, 85, 107, 117 y 142: DbSet<Usuario> no expone Usuarios, Cuentas ni SaveChanges. |
| Solución esperada | Restaurar ApplicationDbContext tanto en el campo como en el constructor; no reescribir las acciones ni registrar DbSet<Usuario> en DI. |
| Verificación | Los siete CS1061 desaparecen con dotnet build y el controlador vuelve a poder resolverse desde el contenedor de servicios. |
| Impacto arquitectónico | Index, Create y las dos variantes de Perfil dependen de consultas, persistencia, sesión, archivos y cuenta; una reparación local dejaría una DI inválida en tiempo de ejecución. |

**Antes**

~~~csharp
private readonly ApplicationDbContext _context;
private readonly IWebHostEnvironment _webHostEnvironment;

public UsuarioController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
{
    _context = context;
    _webHostEnvironment = webHostEnvironment;
}
~~~

**Después**

~~~csharp
private readonly DbSet<Usuario> _context;
private readonly IWebHostEnvironment _webHostEnvironment;

public UsuarioController(DbSet<Usuario> context, IWebHostEnvironment webHostEnvironment)
{
    _context = context;
    _webHostEnvironment = webHostEnvironment;
}
~~~

## Dependencias y orden de diagnóstico

- BACK-03 es una trampa semántica: corregir solo el miembro que marca el compilador puede dejar la relación EF invertida. La reparación debe recuperar toda la cadena original.
- BACK-04 sigue una cadena de configuración hasta una API específica del proveedor SQL Server; el tipo que devuelve la configuración no es un valor de timeout.
- BACK-12 afecta cada llamada de persistencia, aunque el diagnóstico se emite dentro del contexto.
- BACK-13 genera dos diagnósticos en una misma expresión. Se cuenta como una alteración, pero ambos deben desaparecer al restaurar el registro correcto.
- BACK-14 concentra siete CS1061 en el controlador de mayor grado. Son una cascada de una sola frontera mal tipada; reparar llamadas individuales oculta la causa y deja la inyección rota.

## Resultado de la práctica alterada

| Comando | Resultado |
| --- | --- |
| dotnet build .\Lumina_WEB.slnx --no-restore --verbosity minimal | Código 1; 42 advertencias preexistentes y 21 diagnósticos de compilación intencionales generados por 14 alteraciones. BACK-13 emite CS0029 y CS1662; BACK-14 emite siete CS1061. |
| dotnet test .\Lumina_WEB.slnx --verbosity minimal | Código 0; restauración actualizada. No hay proyectos de prueba detectables ni pruebas ejecutadas. |

## Verificación final de reparación

1. Restaurar cada fragmento de la sección Antes correspondiente.
2. Ejecutar restore, build y test.
3. Confirmar que build termina con código 0 y no contiene los 21 diagnósticos documentados.
4. Revisar git diff y comprobar que no se modificaron vistas, recursos, migraciones ni frontend.

## Comparación y recuperación

~~~powershell
git status --short
git diff --name-only
git diff -- Lumina_WEB/Lumina_WEB
git show 9382c7f2888e9793b22a9a7d19f626e141eebc94:Lumina_WEB/Lumina_WEB/Controllers/DetectorAlertas.cs
~~~

Para recuperar un archivo concreto, inspeccionar primero su diff y después usar una ruta explícita con git restore --source=<commit> -- <ruta>.
