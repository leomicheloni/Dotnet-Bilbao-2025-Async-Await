using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Security.Permissions;

// Ensamblado y atributos generados automáticamente por el compilador
[assembly: CompilationRelaxations(8)]
[assembly: RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly: Debuggable(DebuggableAttribute.DebuggingModes.Default | DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints | DebuggableAttribute.DebuggingModes.EnableEditAndContinue | DebuggableAttribute.DebuggingModes.DisableOptimizations)]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
[assembly: AssemblyVersion("0.0.0.0")]
[module: UnverifiableCode]
[module: RefSafetyRules(11)]

public class Test
{
    // Esta clase interna es la máquina de estados generada por el compilador para manejar el flujo asincrónico.
    [CompilerGenerated]
    private sealed class <MyVoidMethodAsync>d__0 : IAsyncStateMachine
    {
        public int <>1__state;
        // Este builder es específico para métodos async void. NO devuelve Task al llamador.
        public AsyncVoidMethodBuilder <>t__builder;
        public Test <>4__this;

        // El método MoveNext es el "motor" de la máquina de estados
        private void MoveNext()
        {
            int num = <>1__state;
            try
            {
                // Aquí se lanza una excepción (podría ser después de un await)
                throw new Exception("error");
            }
            catch (Exception exception)
            {
                // La excepción se pasa al builder
                <>1__state = -2;
                // En AsyncVoidMethodBuilder, SetException NO propaga la excepción al llamador,
                // sino que la manda al SynchronizationContext o al ThreadPool (no hay Task)
                <>t__builder.SetException(exception);
                // Por eso, el llamador no puede capturar la excepción con try/catch.
            }
        }

        void IAsyncStateMachine.MoveNext()
        {
            this.MoveNext();
        }

        [DebuggerHidden]
        private void SetStateMachine([Nullable(1)] IAsyncStateMachine stateMachine)
        {
        }

        void IAsyncStateMachine.SetStateMachine([Nullable(1)] IAsyncStateMachine stateMachine)
        {
            this.SetStateMachine(stateMachine);
        }
    }

    // Método público expuesto, sin retorno (void)
    [AsyncStateMachine(typeof(<MyVoidMethodAsync>d__0))]
    [DebuggerStepThrough]
    public void MyVoidMethodAsync()
    {
        // Se crea y arranca la máquina de estados (es asincrónico)
        <MyVoidMethodAsync>d__0 stateMachine = new <MyVoidMethodAsync>d__0();
        stateMachine.<>t__builder = AsyncVoidMethodBuilder.Create();
        stateMachine.<>4__this = this;
        stateMachine.<>1__state = -1;
        stateMachine.<>t__builder.Start(ref stateMachine);
        // El método retorna inmediatamente, sin Task, y el flujo asincrónico sigue aparte.
    }

    public void TestA()
    {
        try
        {
            // Llama al método async void, pero este retorna inmediatamente.
            MyVoidMethodAsync();
        }
        catch (Exception)
        {
            // Este bloque NUNCA captura la excepción lanzada dentro del método async void,
            // porque la excepción ocurre fuera del flujo del llamador y no hay Task para observarla.
        }
    }
}