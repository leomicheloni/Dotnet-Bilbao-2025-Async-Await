# Charla: Desmitificando async/await y la programación asíncrona en .NET

## Menú (Tabla de contenidos)

- [1. Introducción](#1-introducción)
- [2. ¿Qué es la programación asíncrona?](#2-¿qué-es-la-programación-asíncrona)
- [3. ¿Qué es un thread?](#3-¿qué-es-un-thread?)
- [4. ¿Qué es el ThreadPool?](#4-¿qué-es-el-threadpool?)
- [5. Qué es un Task?](#5-qué-es-un-task?)
- [6. async/await: ¿Qué son y cómo funcionan?](#6-asyncawait-¿qué-son-y-cómo-funcionan?)
- [7. El rol de la clase Task](#7-el-rol-de-la-clase-task?)
- [8. Cancellation token](#8-cancellation-token?)
- [9. ConfigureAwait](#9-configureawait)
- [10. Buenas prácticas y errores comunes](#10-buenas-prácticas-y-errores-comunes)
- [11. Cuándo y cómo usar async/await](#11-cuándo-y-cómo-usar-asyncawait)
- [12. Clases útiles](#12-clases-útiles)
- [Referencias](#referencias)


---

## 1. Introducción
- Presentación personal y contexto.
- ¿Por qué es importante entender la asincronía en aplicaciones modernas?
- Problemas comunes: aplicaciones lentas, bloqueos, mal uso de recursos.

---

## 2. ¿Qué es la programación asíncrona?

Es poder esperar a que una tarea termine mientras podemos seguir haciendo otras cosas.

- **Analogía:** Pedir comida y limpiar la casa mientras esperas el delivery.

- **Ejemplo:** En una aplicación web, mientras se espera la respuesta de una base de datos, el servidor puede atender otras solicitudes.

Para poder tener programación asincrónica necesitamos tener la habilidad de poder ejecutar más de una tarea al mismo tiempo sin que afecte a la otra.
Para esto existe el concepto de Thread

Un Thread es una unidad de ejecución dentro de un proceso. Los Threads permiten que un programa realice múltiples tareas al mismo tiempo, lo que es fundamental para la programación asincrónica.


- Las aplicaciones en .NET tienen la capacidad de ejecutar Threads o hilos, que son unidades de trabajo que pueden ejecutarse de forma independiente dentro de la misma aplicación.
- En general, la cantidad de hilos que se pueden ejecutar simultáneamente está limitada por los recursos del sistema.

- Definición sencilla: permite que el programa gestione múltiples tareas sin bloquear el hilo principal.
- Diferencia entre código síncrono y asíncrono.


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
    public static void Main(string[] args)
    {
        var thread = new System.Threading.Thread(() =>
        {
            Thread.Sleep(2000); // detiene el thread!
            Console.WriteLine("Hello from another thread!");
        });

        thread.Start();

        Console.WriteLine("Hello, World!");

        Console.ReadLine();

    }
```

Analicemos lo que hace este código:
1. Crea un nuevo thread que ejecuta una función anónima.
2. Dentro del thread, se simula una tarea que tarda 2 segundos usando `Thread.Sleep(2000)`.
3. Luego imprime un mensaje en la consola.
4. El thread se inicia con `thread.Start()`.
5. Mientras tanto, el thread principal continúa y imprime "Hello, World!" inmediatamente.

### Problemas con threads

Hasta aquí todo bien, pero qué pasa si queremos utilizar el valor que retorna el thread en otro thread?
> La clase Thread no tiene una forma directa de devolver un valor.

```csharp
    public static void Main(string[] args)
    {
        var result = 0;

        var thread2 = new System.Threading.Thread(() =>
        {
            Thread.Sleep(2000);
            result = 42;
        });

        thread2.Start();

        Console.WriteLine("Hello, World!");

        Console.WriteLine($"Result from thread: {result}");

        Console.ReadLine();
    }
```

```csharp
    public static void Main(string[] args)
    {
        var result = 0;

        var thread2 = new System.Threading.Thread(() =>
        {
            Thread.Sleep(2000);
            result = 42;
        });

        thread2.Start();

        Console.WriteLine("Hello, World!");

        //thread2.Join(); // Espera a que thread2 termine

        Console.WriteLine($"Result from thread: {result}");

        Console.ReadLine();
    }
```

Esto funciona, pero estamos bloqueando el thread principal hasta que el thread2 termine.


**Nota:**  
- Al crear un thread tendremos dos, el Thread principal de la aplicación que es creado por el runtime y el nuevo thread que hemos creado.
- El Thread principal de la aplicación, en casos como Winforms, WPF, Xamarin, es el **UI Thread**.
- El UI Thread es responsable de actualizar la interfaz de usuario y manejar la interacción del usuario.
- Tenemos que evitar bloquear el UI Thread con operaciones largas o intensivas en recursos.
- En una aplicación web si bloqueamos el thread principal, no podremos atender otras solicitudes con ese thread.
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
    static void Main()
    {
        Console.WriteLine("Iniciando");
        ThreadPool.QueueUserWorkItem(DoWork);
        Console.WriteLine("Trabajo en el thread principal.");

        Console.ReadLine();
    }

    static void DoWork(object state)
    {
        Console.WriteLine("Iniciando trabajo en un thread del pool.");
        Thread.Sleep(2000);
        Console.WriteLine("Trabajo en el thread del pool completado.");
    }
```

Siempre tenemos que usar el ThreadPool en lugar de crear hilos manualmente.

Sin embargo, el ThreadPool tiene sus propias limitaciones y consideraciones:

- No es adecuado para tareas que requieren un control preciso sobre el ciclo de vida del hilo.
- Tampoco podemos controlar excepciones que ocurren en los hilos del ThreadPool.

> En resumen debemos evitar crear hilos manualmente ya sea con o sin el ThreadPool.

## Usando Task.Run

La forma más recomendada de crear tareas asíncronas en .NET es usando `Task.Run`.

```csharp
    static void Main()
    {
        var t = Task.Run(() =>
        {
            Console.WriteLine("Hello from another thread!");
            System.Threading.Thread.Sleep(5000);
            Console.WriteLine("Goodbye from another thread!");
        });

        Console.WriteLine("Hello, World!");
        Console.ReadLine();
    }
```

**Mito:**  
“Cada Task crea un nuevo hilo.”  

**Realidad:**  
No necesariamente. El ThreadPool reutiliza hilos existentes la mayoría de las veces.

**Mito:**  
Si uso el Threadpool mis problemas con el rendimiento y los bloqueos se solucionan automáticamente.

**Realidad:**  
No, el ThreadPool puede ayudar a mejorar el rendimiento al reutilizar hilos, pero no resuelve todos los problemas de bloqueo. Es importante diseñar el código de manera asíncrona y evitar operaciones bloqueantes.

---

## 5. Qué es un Task?

Task no es más que una estructura que representa una operación que se va a ejecutar en el futuro.
Un Task en C# puede estar en varios estados:

- **Created:** El Task ha sido creado pero aún no ha comenzado a ejecutarse.
- **Running:** El Task está en proceso de ejecución.    
- **Completed:** El Task ha terminado su ejecución, ya sea de forma exitosa o con error.
- **Faulted:** El Task ha terminado con una excepción.
- **Canceled:** El Task ha sido cancelado antes de completar su ejecución.

Un Task puede devolver un valor o no. Si devuelve un valor, se usa `Task<T>`, donde `T` es el tipo del valor devuelto.

> Task representa una operación asincrónica con la que podemos interactuar.

> Task tiene relación directa con async/await.


## 6. async/await: ¿Qué son y cómo funcionan?

Al marcar un método como async las cosas cambian internamente

Creamo un método que demora un tiempo y lo marcamos como async para no tener que esperarlo.


``` csharp
    static void Main()
    {

        var stopWatch = System.Diagnostics.Stopwatch.StartNew();
        stopWatch.Start();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("Calling AsyncCall...");

        Task.Run(AsyncCall);

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("Called, took {0}", stopWatch.Elapsed);

        Console.ReadLine();
    }
    static async void AsyncCall()
    {
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine("Starting long running task... ");
        // marca un warning, si no hay await no espera por el delay
        Task.Delay(2000);

        //Thread.Sleep(2000);

        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine("Finished long running task... ");
    }
```

Me marca un warning que no estoy haciendo await de Task.Delay.
Pero no necesito leer ningún resultado, solo quiero que espere 2 segundos, estoy haciendo **fire-and-forget**. Así que todo bien...

Vemos que el método se ejecuta pero no espera dos segundos.

Porque AsyncCall no espera a que Task.Delay termine. No hace await.

Agregamos await

``` csharp
    static void Main()
    {

        var stopWatch = System.Diagnostics.Stopwatch.StartNew();
        stopWatch.Start();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("Calling AsyncCall...");

        Task.Run(AsyncCall);

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("Called, took {0}", stopWatch.Elapsed);

        Console.ReadLine();
    }
    static async void AsyncCall()
    {
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine("Starting long running task... ");

        await Task.Delay(2000);

        //Thread.Sleep(2000);

        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine("Finished long running task... ");
    }
```
Ahora si el método AsyncCall espera a que Task.Delay termine (la tarea asíncrona).
Entonces, sí estamos sincronizando el código.

**Pero el Thread de AsyncCall se libera mientras espera a que Task.Delay termine.**

**Await no es lo mismo que Wait**

Sin embargo tampoco puedo controlar los errores que puedan ocurrir en AsyncCall.


``` csharp
    static void Main()
    {

        var stopWatch = System.Diagnostics.Stopwatch.StartNew();
        stopWatch.Start();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("Calling AsyncCall...");

        try
        {
            AsyncCall();
        }
        catch (Exception)
        {

            Console.WriteLine("Error hay que reintentar");
        }

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("Called, took {0}", stopWatch.Elapsed);

        Console.ReadLine();
    }
    static async void AsyncCall()
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Async Method starts... ");

        // no uso await, por lo que no puedo capturar la excepción
        Task.Run(() =>
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            throw new Exception("Error in Task");
        });

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Async Method finishing ");
    }
```

En este caso no puedo capturar el error

Para ello uso await.

``` csharp
    static void Main()
    {
        var stopWatch = System.Diagnostics.Stopwatch.StartNew();
        stopWatch.Start();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("Calling AsyncCall...");

        try
        {
            AsyncCall();
        }
        catch (Exception)
        {
            Console.WriteLine("Error hay que reintentar");
        }

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("Called, took {0}", stopWatch.Elapsed);

        Console.ReadLine();
    }

    // devuelve void, no puedo hacer await
    static async void AsyncCall()
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Async Method starts... ");

        // si uso await, por lo que la excepción se propaga
        await Task.Run(() =>
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            throw new Exception("Error in Task");
        });

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Async Method finishing ");
    }
```

En este caso el error se propaga, pero no puedo capturarlo en el Main porque AsyncCall retorna void.

> **Es una mala práctica usar async void.**

Lo correcto es siempre devolver Task o Task<T>.

``` csharp
    static async Task Main()
    {
        var stopWatch = System.Diagnostics.Stopwatch.StartNew();
        stopWatch.Start();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("Calling AsyncCall...");

        try
        {
            var result = await AsyncCall(); // si no uso await me devuelve un Task<int>
            Console.WriteLine("Result: {0}", result);
        }
        catch (Exception)
        {
            Console.WriteLine("Error hay que reintentar");
        }

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("Called, took {0}", stopWatch.Elapsed);

        Console.ReadLine();
    }
    static async Task<int> AsyncCall()
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Async Method starts... ");

        var result = await Task.Run(() =>
        {
            return 44;            
        });

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Async Method finishing ");
        // retorno int pero el método está marcado como async Task<int>
        return result;
    }
```

> Me indica que el tipo de retorno no es correcto, sin embargo estoy devolviendo un int



Sincronización de Tasks

``` csharp
    static async Task Main()
    {
        var stopWatch = System.Diagnostics.Stopwatch.StartNew();
        stopWatch.Start();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("Calling AsyncCall...");

        try
        {
            var result = await AsyncCall();
            Console.WriteLine("Result: {0}", result);
        }
        catch (Exception)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Error hay que reintentar");
        }

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("Called, took {0}", stopWatch.Elapsed);

        Console.ReadLine();
    }
    static async Task<int> AsyncCall()
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Async Method starts... ");

        var result = await Task.Run(async () =>
        {
            await Task.Delay(2000);
            return 44;
        });

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Async Method finishing ");

        return 44;
    }
```

Ahora sí espera 2 segundos.

En resumen async/await nos permite escribir código asíncrono de manera más sencilla y legible.

Si el método es async, el compilador espera que haya al menos un await dentro del método.

Al haber un await el compilador retorna Task o Task<T> automáticamente.

### Ventajas de async/await

- Legibilidad: el código se lee como si fuera síncrono, lo que facilita entender el flujo de las operaciones.
- Mantenibilidad: agregar o quitar pasos en la secuencia requiere cambios mínimos en el código.
- Manejo de errores: el manejo de excepciones con bloques try/catch funciona de forma natural.
- Depuración: la pila de llamadas y la experiencia con el depurador son mucho mejores con async/await.
- Rendimiento: las optimizaciones del compilador para async/await son más sofisticadas que las cadenas manuales con ContinueWith.


### La clase Task

``` csharp
public class Task
{
    public bool IsCompleted { get; }
    public Task Run(Action action);
    public Task<TResult> Run<TResult>(Func<TResult> function);
    public ConfiguredTaskAwaitable ConfigureAwait(ConfigureAwaitOptions options);
    public Task<T> ContinueWith(Action<Task> continuationAction);
    public static YieldAwaitable Yield();
    public Task Delay(int millisecondsDelay);
    public Task WhenAll(params Task[] tasks);
    public Task WhenAny(params Task[] tasks);
}
```


### Qué ocurre internamente cuando usamos async/await?

```csharp
using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using System.Collections.Generic;

class LibraryService(HttpClient client)
{
    public async Task<IEnumerable<Library>> GetDataAsync()
    {
        var response = await client.GetAsync("https://example.com");
        response.EnsureSuccessStatusCode();
        
        var content = await response.Content.ReadAsStreamAsync();
        
        var libraries = await JsonSerializer.DeserializeAsync<List<Library>>(content);

        return libraries;
    }
}

class Library
{
    public string Name { get; set; }
    public string Version { get; set; }
}

> Ejemplo del código descompilado

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

> async crear una máquina de estados para que se pueda hacer await de un método y saltar de otro thread mientras termina

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

``` csharp
    public static void Main(string[] args)
    {
        // ejemplo de cencellation token
        CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken token = cts.Token;
        Task.Run(() =>
        {
            for (int i = 0; i < 10; i++)
            {
                if (token.IsCancellationRequested)
                {
                    Console.WriteLine("Tarea cancelada");
                    return;
                }
                Console.WriteLine($"Tarea en ejecución {i + 1}");
                Thread.Sleep(500);
            }
            Console.WriteLine("Tarea completada");
        }, token);

        Console.WriteLine("Presiona Enter para cancelar la tarea...");
        Console.ReadLine();
        cts.Cancel();
        Console.WriteLine("Programa finalizado");
        Console.ReadLine();
    }
```

---

## 7. ConfigureAwait

- Por defecto, await captura el contexto de sincronización (SynchronizationContext) para continuar en el mismo contexto después de la espera.
- En aplicaciones UI (WinForms, WPF), esto asegura que el código después de await se ejecute en el hilo de la interfaz de usuario.
- En aplicaciones de consola o servicios, no hay un contexto de sincronización, por lo que no es necesario capturarlo.
- Usar `ConfigureAwait(false)` indica que no es necesario capturar el contexto de sincronización, lo que puede mejorar el rendimiento en ciertos escenarios.

## 8. Buenas prácticas y errores comunes
- Evitar `async void` salvo en handlers de eventos.
- No usar `.Result` o `.Wait()` en código asíncrono (puede bloquear la aplicación).
    - Al usar .Result o .Wait() en un contexto con SynchronizationContext (como UI o ASP.NET), se puede producir un deadlock. Esto ocurre porque el hilo que espera la tarea bloqueada está reteniendo el contexto de sincronización, impidiendo que la tarea asíncrona complete su ejecución y libere el contexto.
    - También las excepciones puden ser envueltas en AggregateException, lo que complica su manejo.
    - En ASP.NET, usar .Result o .Wait() puede bloquear el hilo del servidor, reduciendo la capacidad de manejar otras solicitudes y afectando el rendimiento de la aplicación.
- Liberar recursos correctamente.
- Usa await en cada tarea asíncrona para manejar excepciones y resultados.
- No descartar con **_**
- Usa ConfigureAwait(false) en librerías para evitar capturar el contexto innecesariamente.

---

## 9. Cuándo y cómo usar async/await
- Operaciones de I/O (HTTP, base de datos, archivos).
- Interfaces gráficas (UI responsiveness).
- APIs y servicios web.
- Toda operación que requiere esperar un recurso externo

---

## 10. Clases útiles
- Parallel.ForEach

``` csharp
        Parallel.ForEach(new[] { 1, 2, 3, 4, 5 }, (i) =>
        {
            Console.WriteLine($"Processing {i}");
            Thread.Sleep(1000 * new Random().Next(1, 5));
            Console.WriteLine($"Processed {i}");
        });
```

- Task.WhenAll

Permite esperar a que múltiples tareas se completen.

``` csharp
    static async Task Main()
    {
        var tasks = new List<Task>
        {
            Task.Run(() => { Thread.Sleep(2000); Console.WriteLine("Task 1 completed"); }),
            Task.Run(() => { Thread.Sleep(1000); Console.WriteLine("Task 2 completed"); }),
            Task.Run(() => { Thread.Sleep(3000); Console.WriteLine("Task 3 completed"); })
        };

        await Task.WhenAll(tasks);
        Console.WriteLine("All tasks completed");
    }
```



- Task.Yield
Permite a otras tareas ejecutarse antes de continuar.

``` csharp
    static async Task Main()
    {
        Console.WriteLine("Before Yield");
        await Task.Yield();
        Console.WriteLine("After Yield");
    }
```

- SemaphoreSlim
``` csharp
    static SemaphoreSlim semaphore = new SemaphoreSlim(2); // Permitir 2 tareas simultáneas

    static async Task AccessResource(int id)
    {
        await semaphore.WaitAsync();
        try
        {
            Console.WriteLine($"Task {id} accessing resource");
            await Task.Delay(2000); // Simular trabajo
            Console.WriteLine($"Task {id} releasing resource");
        }
        finally
        {
            semaphore.Release();
        }
    }

    static async Task Main()
    {
        var tasks = new List<Task>();
        for (int i = 1; i <= 5; i++)
        {
            int taskId = i; // Capturar el valor de i
            tasks.Add(AccessResource(taskId));
        }
        await Task.WhenAll(tasks);
    }
```
---

# Referencias

- [Documentación oficial .NET Async](https://learn.microsoft.com/dotnet/csharp/programming-guide/concepts/async/)
- https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/
- https://sharplab.io
- https://www.youtube.com/watch?v=6frfLI3HqKI
- https://youtu.be/R-z2Hv-7nxk
- https://www.youtube.com/watch?v=oHKyzgGjKHQ

