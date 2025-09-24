using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Permissions;
using System.Text.Json;
using System.Threading.Tasks;

// Ensamblado y atributos de depuración y seguridad generados automáticamente
[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.Default | DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints | DebuggableAttribute.DebuggingModes.EnableEditAndContinue | DebuggableAttribute.DebuggingModes.DisableOptimizations)]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
[assembly: AssemblyVersion("0.0.0.0")]
[module: UnverifiableCode]
[module: RefSafetyRules(11)]

// Clases modelo con campos privados generados automáticamente para las propiedades
public class Library
{
    // Campos privados automáticos para las propiedades
    private string <Name>k__BackingField;
    private string <Version>k__BackingField;

    public string Name
    {
        get { return <Name>k__BackingField; }
        set { <Name>k__BackingField = value; }
    }

    public string Version
    {
        get { return <Version>k__BackingField; }
        set { <Version>k__BackingField = value; }
    }
}

// Servicio que implementa la petición HTTP de manera asíncrona
public class LibraryService
{
    // Esta clase es la máquina de estados generada por el compilador para el método async
    private sealed class <GetDataAsync>d__2 : IAsyncStateMachine
    {
        // Estado interno de la máquina de estados. -1 significa "inicio", 0, 1, 2 son estados intermedios en los await.
        public int <>1__state;

        // Builder para manejar el Task resultado del método async.
        public AsyncTaskMethodBuilder<IEnumerable<Library>> <>t__builder;

        // Referencia al objeto original de LibraryService
        public LibraryService <>4__this;

        // Variables locales que sobreviven entre awaits
        private HttpResponseMessage <response>5__1;
        private Stream <content>5__2;
        private List<Library> <libraries>5__3;

        // Variables temporales usadas durante los awaits
        private HttpResponseMessage <>s__4;
        private Stream <>s__5;
        private List<Library> <>s__6;
        private TaskAwaiter<HttpResponseMessage> <>u__1;
        private TaskAwaiter<Stream> <>u__2;
        private ValueTaskAwaiter<List<Library>> <>u__3;

        // Este método es el "cerebro" de la máquina de estados. Ejecuta el código hasta el siguiente await.
        private void MoveNext()
        {
            int num = <>1__state;
            IEnumerable<Library> result;
            try
            {
                TaskAwaiter<HttpResponseMessage> awaiter3;
                TaskAwaiter<Stream> awaiter2;
                ValueTaskAwaiter<List<Library>> awaiter;
                switch (num)
                {
                    default:
                        // Primer paso: llamada a GetAsync (await)
                        awaiter3 = <>4__this._client.GetAsync("https://example.com").GetAwaiter();
                        if (!awaiter3.IsCompleted)
                        {
                            // Si la tarea no terminó, guardo el estado y salgo
                            num = (<>1__state = 0);
                            <>u__1 = awaiter3;
                            <GetDataAsync>d__2 stateMachine = this;
                            <>t__builder.AwaitUnsafeOnCompleted(ref awaiter3, ref stateMachine);
                            return;
                        }
                        goto IL_008f;
                    case 0:
                        // Retomo después del await GetAsync
                        awaiter3 = <>u__1;
                        <>u__1 = default(TaskAwaiter<HttpResponseMessage>);
                        num = (<>1__state = -1);
                        goto IL_008f;
                    case 1:
                        // Retomo después del await ReadAsStreamAsync
                        awaiter2 = <>u__2;
                        <>u__2 = default(TaskAwaiter<Stream>);
                        num = (<>1__state = -1);
                        goto IL_0120;
                    case 2:
                        // Retomo después del await DeserializeAsync
                        awaiter = <>u__3;
                        <>u__3 = default(ValueTaskAwaiter<List<Library>>);
                        num = (<>1__state = -1);
                        break;
                        // --------- Lógica secuencial original (pero desdoblada en casos según el await) ---------
                        IL_0120:
                        <>s__5 = awaiter2.GetResult();
                        <content>5__2 = <>s__5;
                        <>s__5 = null;
                        awaiter = JsonSerializer.DeserializeAsync<List<Library>>(<content>5__2).GetAwaiter();
                        if (!awaiter.IsCompleted)
                        {
                            num = (<>1__state = 2);
                            <>u__3 = awaiter;
                            <GetDataAsync>d__2 stateMachine = this;
                            <>t__builder.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
                            return;
                        }
                        break;
                        IL_008f:
                        <>s__4 = awaiter3.GetResult();
                        <response>5__1 = <>s__4;
                        <>s__4 = null;
                        <response>5__1.EnsureSuccessStatusCode();
                        awaiter2 = <response>5__1.Content.ReadAsStreamAsync().GetAwaiter();
                        if (!awaiter2.IsCompleted)
                        {
                            num = (<>1__state = 1);
                            <>u__2 = awaiter2;
                            <GetDataAsync>d__2 stateMachine = this;
                            <>t__builder.AwaitUnsafeOnCompleted(ref awaiter2, ref stateMachine);
                            return;
                        }
                        goto IL_0120;
                }
                // Al final, obtengo el resultado y lo retorno
                <>s__6 = awaiter.GetResult();
                <libraries>5__3 = <>s__6;
                <>s__6 = null;
                result = <libraries>5__3;
            }
            catch (Exception exception)
            {
                // Si hubo excepción, la seteo en el Task
                <>1__state = -2;
                <response>5__1 = null;
                <content>5__2 = null;
                <libraries>5__3 = null;
                <>t__builder.SetException(exception);
                return;
            }
            // Termino el método async, libero recursos y coloco el resultado en el Task
            <>1__state = -2;
            <response>5__1 = null;
            <content>5__2 = null;
            <libraries>5__3 = null;
            <>t__builder.SetResult(result);
        }

        // Métodos requeridos por la interfaz IAsyncStateMachine (usados por el runtime)
        void IAsyncStateMachine.MoveNext() => this.MoveNext();
        private void SetStateMachine(IAsyncStateMachine stateMachine) { }
        void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine) => this.SetStateMachine(stateMachine);
    }

    private readonly HttpClient _client;

    public LibraryService(HttpClient client)
    {
        _client = client;
    }

    // Este método es lo que ves en tu código, pero el compilador lo transforma así
    // Crea la máquina de estados, la inicializa y la ejecuta.
    public Task<IEnumerable<Library>> GetDataAsync()
    {
        <GetDataAsync>d__2 stateMachine = new <GetDataAsync>d__2();
        stateMachine.<>t__builder = AsyncTaskMethodBuilder<IEnumerable<Library>>.Create();
        stateMachine.<>4__this = this;
        stateMachine.<>1__state = -1;
        stateMachine.<>t__builder.Start(ref stateMachine);
        return stateMachine.<>t__builder.Task;
    }
}