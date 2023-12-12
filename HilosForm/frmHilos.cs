using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HilosForm
{
    public partial class frmHilos : Form
    {
        private Random random = new Random();
        private CancellationTokenSource cancellationTokenSource;
        private List<Task> hilos = new List<Task>();

        public frmHilos()
        {
            InitializeComponent();
        }
        private void FrmHilos_Load(object sender, EventArgs e)
        {
            IniciarHilos();
        }

        private void IniciarHilos()
        {
            cancellationTokenSource = new CancellationTokenSource();
            CancellationToken cancellationToken = cancellationTokenSource.Token;

            prgHilo1.Value = 0;
            prgHilo2.Value = 0;
            prgHilo3.Value = 0;
            prgHilo4.Value = 0;
            prgHilo5.Value = 0;

            Task task1 = Task.Run(() => IniciarProceso(prgHilo1, lblHilo1), cancellationToken);
            Task task2 = Task.Run(() => IniciarProceso(prgHilo2, lblHilo2), cancellationToken);
            Task task3 = Task.Run(() => IniciarProceso(prgHilo3, lblHilo3), cancellationToken);
            Task task4 = Task.Run(() => IniciarProceso(prgHilo4, lblHilo4), cancellationToken);
            Task task5 = Task.Run(() => IniciarProceso(prgHilo5, lblHilo5), cancellationToken);

            hilos.Add(task1);
            hilos.Add(task2);
            hilos.Add(task3);
            hilos.Add(task4);
            hilos.Add(task5);
        }

        private void IniciarProceso(ProgressBar prg, Label lbl)
        {
            while (prg.Value < prg.Maximum && !cancellationTokenSource.IsCancellationRequested)
            {
                Thread.Sleep(random.Next(100, 1000));
                //prg.Increment(1);
                //lbl.Text = $"Hilo Nº{Task.CurrentId} - {prg.Value}%";
                IncrementarBarraProgreso(prg, lbl, Task.CurrentId.Value);
            }
            FinalizarProceso(prg, lbl, Task.CurrentId.Value);
        }

        private void IncrementarBarraProgreso(ProgressBar prg, Label lbl, int idHilo)
        {
            if (InvokeRequired)
            {
                Action<ProgressBar, Label, int> delegado = IncrementarBarraProgreso;
                object[] parametros = new object[] { prg, lbl, idHilo };
                Invoke(delegado, parametros);
            }
            else
            {
                prg.Increment(1);
                lbl.Text = $"Hilo Nº{idHilo} - {prg.Value}%";
            }
        }

        private void FinalizarProceso(ProgressBar prg, Label lbl, int idHilo)
        {
            if (InvokeRequired)
            {
                Action<ProgressBar, Label, int> finalizarProceso = FinalizarProceso;
                object[] parametros = new object[] { prg, lbl, idHilo };
                Invoke(finalizarProceso, parametros);
            }
            else
            {
                if (prg.Value == prg.Maximum)
                {
                    lbl.Text = $"Hilo Nº{idHilo} - FINALIZADO";
                }
                else
                {
                    lbl.Text = $"Hilo Nº{idHilo} - CANCELADO";
                }
            }
        }

        private void btnMostrar_Click(object sender, EventArgs e)
        {
            MessageBox.Show(txtMostrar.Text);
        }

        private async void btnReiniciar_Click(object sender, EventArgs e)
        {
            cancellationTokenSource.Cancel();

            while (!hilos.All(h => h.IsCompleted))
            {
                await Task.Delay(200);
            }

            IniciarHilos();
        }

        private void btnVerInfo_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();

            foreach (Task hilo in hilos)
            {
                sb.Append($"Hilo {hilo.Id} ");
                string mensaje = hilo.IsCanceled ? $"está completado." : $"en estado {hilo.Status}.";
                sb.Append(mensaje);
                sb.AppendLine();
            }

            MessageBox.Show(sb.ToString(), "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            cancellationTokenSource.Cancel();
        }
    }
}
