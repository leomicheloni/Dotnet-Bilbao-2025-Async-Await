# Charla: Desmitificando async/await y la programación asíncrona en .NET

## 1. Introducción
- Presentación personal y contexto.
- ¿Por qué es importante entender la asincronía en aplicaciones modernas?
- Problemas comunes: aplicaciones lentas, bloqueos, mal uso de recursos.

---

## 2. ¿Qué es la programación asíncrona?

Es poder ejecutar más de una tarea al mismo tiempo sin que una afecte a la otra.
Por ejemplo, en una aplicación web, mientras se espera la respuesta de una base de datos, el servidor puede atender otras solicitudes.

Para poder tener programación asincrónica necesitamos tener la habilidad de poder ejecutar más de una tarea al mismo tiempo sin que afecte a la otra.
Para esto existe el concepto de Thread

Un Thread es una unidad de ejecución dentro de un proceso. Los Threads permiten que un programa realice múltiples tareas al mismo tiempo, lo que es fundamental para la programación asincrónica.


- Las aplicaciones en .NET tienen la capacidad de ejecutar Threads o hilos, que son unidades de trabajo que pueden ejecutarse de forma independiente dentro de la misma aplicación.
- En general, la cantidad de hilos que se pueden ejecutar simultáneamente está limitada por los recursos del sistema.

- Definición sencilla: permite que el programa gestione múltiples tareas sin bloquear el hilo principal.
- Diferencia entre código síncrono y asíncrono.

- **Analogía:** Pedir comida y limpiar la casa mientras esperas el delivery.

> Entonces, en resumen, la unidad que permite aplicación asincrónicas en .NET es el Thread.
> Es una tecnología de 2012 C# 5

---

## 3. ¿Qué es un thread?

Para poder lograr ejecutar más de una tarea simultáneamente, necesitamos algún tipo de elemento que tome una tarea y la ejecute mientras podemos seguir haciendo otras cosas.
Este elemento es el **thread**.

> Un **thread** (hilo) es una unidad básica de ejecución dentro de un proceso. Permite que distintas partes de un programa se ejecuten en paralelo.  
> Todos los threads comparten la memoria y recursos del proceso.

### Creando un thread

Ejemplo de creación de un thread.

```csharp
var thread = new System.Threading.Thread(() =>
{
    Thread.Sleep(2000); // detiene el thread!
    Console.WriteLine("Hello from another thread!");
});

thread.Start();

Console.WriteLine("Hello, World!");

Console.ReadLine();
```

Analicemos lo que hace este código:
1. Crea un nuevo thread que ejecuta una función anónima.
2. Dentro del thread, se simula una tarea que tarda 2 segundos usando `Thread.Sleep(2000)`.
3. Luego imprime un mensaje en la consola.
4. El thread se inicia con `thread.Start()`.
5. Mientras tanto, el thread principal continúa y imprime "Hello, World!" inmediatamente.

Hasta aquí todo bien, pero qué pasa si queremos utilizar el valor que retorna el thread en otro thread?


```csharp
        var result = 0;

        var thread2 = new System.Threading.Thread(() =>
        {
            Thread.Sleep(2000);
            result = 42;
        });

        thread2.Start();

        Console.WriteLine("Hello, World!");

        thread2.Join(); // Espera a que thread2 termine

        Console.WriteLine($"Result from thread: {result}");

        Console.ReadLine();
```

Esto funciona, pero estamos bloqueando el thread principal hasta que el thread2 termine.


**Nota:**  
- Al crear un thread tendremos dos, el Thread principal de la aplicación que es creado por el runtime y el nuevo thread que hemos creado.
- El Thread principal de la aplicación, en casos como Winforms, WPF, Xamarin, es el **UI Thread**.
- El UI Thread es responsable de actualizar la interfaz de usuario y manejar la interacción del usuario.
- Tenemos que evitar bloquear el UI Thread con operaciones largas o intensivas en recursos.
- Crear muchos threads manualmente puede saturar el sistema y degradar el rendimiento.
- En la gran mayoría de los casos, no es necesario crear hilos manualmente para la mayoría de las operaciones asíncronas. El runtime y el ThreadPool gestionan los hilos por nosotros.
- Creando un thread manualmente, si hacemos un `thread.Join()` en el UI Thread, este se bloqueará hasta que el thread termine, lo que puede hacer que la aplicación deje de responder.
- De este modo es complejo manejar múltiples threads y coordinar su trabajo.
- También es difícil saber cuándo un thread ha terminado su trabajo y cómo manejar su resultado.
- Es difícil manejar errores y excepciones que ocurren en threads separados.

**Mito:**  
“Para hacer código asíncrono necesito crear muchos threads.”  
**Realidad:**  
No es necesario crear hilos manualmente para la mayoría de las operaciones asíncronas. El runtime y el ThreadPool gestionan los hilos por nosotros.

Ejemplo que bloquea el UI Thread:

``` csharp
var thread = new System.Threading.Thread(() =>
{
    Thread.Sleep(2000);
    System.Console.WriteLine("Hello from another thread!");
});

thread.Start();
//thread.Join(); // Esto bloquearía el UI Thread

Console.WriteLine("Original Thread!");

Console.ReadLine();
```

---

### Background thread

Un **background thread** es un hilo que se ejecuta en segundo plano, permitiendo que la aplicación continúe respondiendo a la interacción del usuario. Estos hilos son útiles para tareas que no requieren la atención inmediata del usuario, como la carga de datos o el procesamiento de información.

``` csharp
var thread = new System.Threading.Thread(() =>
{
    Console.WriteLine("Hello from another thread!");
    System.Threading.Thread.Sleep(9000);
});
//thread.IsBackground = true;
thread.Start();

Console.WriteLine("Hello, World!");
```

> Ahora que aprendimos a hacer un Thread tenemos que dejar de hacerlo.

### Desventajas de crear un thread manualmente

- Complejidad: Manejar múltiples hilos puede complicar el código y hacerlo más propenso a errores.
- Recursos: Cada hilo consume recursos del sistema. Crear muchos hilos puede llevar a una sobrecarga y degradar el rendimiento.
- Sincronización: Coordinar el acceso a recursos compartidos entre hilos puede ser complicado y propenso a errores.
- Si queremos leer el resultado de otro thread, tenemos que usar mecanismos de sincronización como `Join` que bloquean el hilo actual.

Y cómo lo hacemos entonces?

> Con el **ThreadPool**

## 4. ¿Qué es el ThreadPool?

> El **ThreadPool** es un conjunto gestionado de hilos reutilizables en .NET. Permite ejecutar tareas en paralelo de forma eficiente, sin crear y destruir hilos manualmente.

Ejemplo de creación de un thread usando el ThreadPool

```csharp
using System;
using System.Threading;

class Program
{
    static void Main()
    {
        ThreadPool.QueueUserWorkItem(DoWork);
    }

    static void DoWork(object state)
    {
        Console.WriteLine("Haciendo trabajo en un thread del pool.");
    }
}
```

Siempre tenemos que usar el ThreadPool en lugar de crear hilos manualmente.

Sin embargo, el ThreadPool tiene sus propias limitaciones y consideraciones:

- No es adecuado para tareas que requieren un control preciso sobre el ciclo de vida del hilo.
- Tampoco podemos controlar excepciones que ocurren en los hilos del ThreadPool.

## Usando Task.Run

```csharp
var t = Task.Run(() =>
{
    Console.WriteLine("Hello from another thread!");
    System.Threading.Thread.Sleep(10000);
    Console.WriteLine("Goodbye from another thread!");
});

Console.WriteLine("Hello, World!");
Console.ReadLine();
```

**Nota:**  
La mayoría de las tareas lanzadas con `Task.Run` o APIs asíncronas se ejecutan en el ThreadPool, que decide cómo y cuándo asignar los hilos disponibles. La cantidad de hilos en el ThreadPool se ajusta dinámicamente según la carga de trabajo y los recursos del sistema, no se comportará igual en una CPU de 4 núcleos que en una de 16 núcleos.

**Mito:**  
“Cada Task crea un nuevo hilo.”  

**Realidad:**  
No necesariamente. El ThreadPool reutiliza hilos existentes la mayoría de las veces.

**Mito:**  
Si uso el Threadpool mis problemas con el rendimiento y los bloqueos se solucionan automáticamente.

**Realidad:**  
No, el ThreadPool puede ayudar a mejorar el rendimiento al reutilizar hilos, pero no resuelve todos los problemas de bloqueo. Es importante diseñar el código de manera asíncrona y evitar operaciones bloqueantes.

---

## 5. async/await: ¿Qué son y cómo funcionan?

Cómo hacemos entonces para poder invocar una llamada que es asincrónica desde una llamada síncrona, sin bloquear el thread actual?


Al marcar un método como async las cosas cambian internamente


- Introducción a los modificadores `async` y `await`.
- Facilitan la escritura de código no bloqueante.
- Ejemplo básico:

``` csharp
static async void AsyncCall() 
{ 
    Console.WriteLine("Async Method starts... ");
    Task.Delay(2000);
    Console.WriteLine("Async Method finishing ");
}
```

``` csharp
using System;
using System.Threading.Tasks;

var stopWatch = System.Diagnostics.Stopwatch.StartNew();
stopWatch.Start();
Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("Calling AsyncCall...");

AsyncCall();

Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("Called, took {0}", stopWatch.Elapsed);

Console.ReadLine();

static async void AsyncCall()
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Async Method starts... ");

    await Task.Delay(2000);

    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Async Method finishing ");
}
```

En este caso retorno void y no pasa mucho más, pero qué pasa si quiero leer el resultado?

``` csharp
static async int AsyncCall()
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Async Method starts... ");

    await Task.Delay(2000);

    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Async Method finishing ");
    return 42;
}
```

Me indica que el tipo de retorno no es correcto, sin embargo estoy devolviendo un int


``` csharp
using System;
using System.Threading.Tasks;

var stopWatch = System.Diagnostics.Stopwatch.StartNew();
stopWatch.Start();
Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("Calling AsyncCall...");

AsyncCall();

Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("Called, took {0}", stopWatch.Elapsed);

Console.ReadLine();

static async Task<int> AsyncCall()
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Async Method starts... ");

    await Task.Delay(2000);

    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Async Method finishing ");
    return 42;
}
```

Si hacemos esto tenemos un warning que el método no va a esperar

En realidad no espera, sino que sigue pero se queda pendiente de Task

versión corregida

``` csharp
using System;
using System.Threading.Tasks;

var stopWatch = System.Diagnostics.Stopwatch.StartNew();
stopWatch.Start();
Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("Calling AsyncCall...");

await AsyncCall();

Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("Called, took {0}", stopWatch.Elapsed);

Console.ReadLine();

static async Task<int> AsyncCall()
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Async Method starts... ");

    await Task.Delay(2000);

    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Async Method finishing ");
    return 42;
}
```

Si ejecutamos veo que el Thread principal no se bloquea mientras espera el resultado de AsyncCall.


```csharp
using System;
using System.Threading.Tasks;

var httpClient = new HttpClient();
var response = await httpClient.GetAsync("https://example.com"); // Result?
var content = await response.Content.ReadAsStringAsync();

Console.WriteLine(content);
Console.WriteLine("Hola, mundo!");
Console.ReadLine();
```

esto nos marca un warning.


#### Creando nuestro propio método async

```csharp
using System;
using System.Threading.Tasks;

var stopWatch = System.Diagnostics.Stopwatch.StartNew();
stopWatch.Start();
Console.WriteLine("Calling AsyncCall...");

AsyncCall();

Console.WriteLine("Called, took {0}", stopWatch.Elapsed);
Console.ReadLine();

static void AsyncCall() 
{ 
    Console.Write("Starting remote call... ");

    Thread.Sleep(2000);

    Console.WriteLine("Finishing ");
}
```

Este código bloquea el hilo principal porque hace un `Thread.Sleep`.

Intentamos agregar await

``` csharp
static async void AsyncCall() 
{ 
    Console.Write("Starting remote call... ");

    await Thread.Sleep(2000);

    Console.WriteLine("Finishing ");
}
```
No nos deja porque el método retorna void.

Usamos Task.Delay.

``` csharp
static async void AsyncCall() 
{ 
    Console.Write("Starting remote call... ");

    await Task.Delay(2000);

    Console.WriteLine("Finishing ");
}
```

Cosas interesantes sobre async/await:

- Solo podemos hacer await en métodos async.
- Solo podemos hacer await en métodos que retornan Task.
- Los métodos async retornan Task, pero nunca estamos retornando un Task explícitamente.
- Si intentamos devolver un Task da un error
- await libera el hilo actual mientras espera.

**Mito:**  
“async/await es magia.”  
**Realidad:**  
Es azúcar sintáctico sobre continuaciones y máquinas de estado. El compilador transforma el código para que sea no bloqueante.

**Nota:**  
¡No! Usar `async` y `await` **no** crea nuevos hilos automáticamente. El hilo actual se libera mientras espera operaciones de I/O, pero no se generan nuevos hilos en el sistema.

**Mito:**  
await sincroniza los threads.”  
**Realidad:**  
await no "sincroniza" los threads, simplemente libera el hilo actual hasta que la tarea asíncrona complete su ejecución.

**Mito:**  
“await detiene el thread actual hasta que el otro termine.”  
**Realidad:**  
await no detiene el thread actual, sino que permite que otros trabajos se realicen en ese hilo mientras espera la finalización de la tarea asíncrona.

> ejemplo de código descompilado

>async crear una máquina de estados para que se pueda hacer await de un método y saltar de otro thread mientras termina

``` csharp
using System;
using System.Threading.Tasks;

var httpClient = new HttpClient();
var response = await httpClient.GetAsync("https://example.com"); // Result?
var content = await response.Content.ReadAsStringAsync();

Console.WriteLine(content);
Console.WriteLine("Hola, mundo!");
Console.ReadLine();
```

- Se crea una máquina de estados
- Se agrega un Try catch
- Sino no hay await se pierde la excepción.

## 6. El rol de la clase Task

Task es una representación de una operación asíncrona.

async Task / async Task<T>:
- Permite esperar el método con await.
- Puedes manejar excepciones usando try/catch en el llamador.
- El llamador puede saber cuándo termina la tarea.
- Es la forma recomendada para métodos asíncronos generales.

async void:

- No se puede esperar ni capturar el resultado.
- Las excepciones no se propagan al llamador, solo al manejador global.
- Se usa solo para controladores de eventos (por ejemplo, eventos de UI).
- No recomendado para lógica normal, porque dificulta el manejo de errores y el control de flujo.

En resumen: usa async Task o async Task<T> para métodos asíncronos, y async void solo para eventos.

un método asíncrono que retorna void no se puede esperar ni capturar su resultado. Tampoco manejar sus excepciones desde fuera, solo a nivel global.


## 6. Cancellation token

Qué es un cancellation token?

Un cancellation token es un mecanismo que permite cancelar operaciones asíncronas en .NET. Se utiliza para notificar a una tarea que debe detenerse, lo que es especialmente útil en escenarios de larga duración o en aplicaciones que requieren una respuesta rápida a la interacción del usuario.

---

## 7. Buenas prácticas y errores comunes
- Evitar `async void` salvo en handlers de eventos.
- No usar `.Result` o `.Wait()` en código asíncrono (puede bloquear la aplicación).
- Liberar recursos correctamente.

**Ejemplo de error:**
```csharp
var datos = GetDataAsync().Result; // ¡Malo!
```
**Solución:**
```csharp
var datos = await GetDataAsync(); // ¡Bien!
```

podemos hacer fire and forget

falso, si no hacemos await de una tarea asincrónica y falla se pierde la excepción.

---

## 9. Cuándo y cómo usar async/await
- Operaciones de I/O (HTTP, base de datos, archivos).
- Interfaces gráficas (UI responsiveness).
- APIs y servicios web.


---

## Middlewares y await next

## 10. Clases útiles
- Parallel.ForEach
- Task.WhenAll
- Task.Delay
- Task.Yield
- ConfigureAwait
- SemaphoreSlim
- yield
---

### Buenas prácticas

Await every task
- Usa await en cada tarea asíncrona para manejar excepciones y resultados.
- no descartar con **_**

## Cómo funciona un lock

task delay dentro de un foreach > por qué no usar thread sleep

# SafeFireAndForget

configureawait > false en aspnet core?
IAsyncEnumerable<T>
[EnumerationCancellationToken]



---

## 12. Recursos para seguir aprendiendo
- [Documentación oficial .NET Async](https://learn.microsoft.com/dotnet/csharp/programming-guide/concepts/async/)
- Libros, blogs, canales de YouTube.

---

# Referencias

https://sharplab.io
https://www.youtube.com/watch?v=6frfLI3HqKI
https://youtu.be/R-z2Hv-7nxk
