using System.Globalization;
using SimuladorTrocaCalor.Controllers;
using SimuladorTrocaCalor.Estruturas;
using SimuladorTrocaCalor.Models;

namespace SimuladorTrocaCalor
{
    // View: recebe os comandos do usuário e mostra o estado calculado pelo Controller
    public partial class FormPrincipal : Form
    {
        private readonly SimulacaoController controller = new SimulacaoController();
        private readonly System.Windows.Forms.Timer temporizador = new System.Windows.Forms.Timer();
        private readonly NumericUpDown entradaN = new NumericUpDown();
        private readonly NumericUpDown entradaLado = new NumericUpDown();
        private readonly NumericUpDown entradaTemperaturaInicial = new NumericUpDown();
        private readonly ComboBox materiaisIniciais = new ComboBox();
        private readonly ComboBox materiaisDoCorpo = new ComboBox();
        private readonly NumericUpDown temperaturaDoCorpo = new NumericUpDown();
        private readonly Button botaoAlterarMaterial = new Button();
        private readonly Button botaoAlterarTemperatura = new Button();
        private readonly Button botaoPasso = new Button();
        private readonly Button botaoIniciar = new Button();
        private readonly Button botaoParar = new Button();
        private readonly Panel painelMatriz = new Panel();
        private readonly Label detalhes = new Label();
        private readonly Label estado = new Label();
        private Button[,]? botoesDosCorpos;
        private int linhaSelecionada = -1;
        private int colunaSelecionada = -1;

        public FormPrincipal()
        {
            InitializeComponent();
            MontarInterface();
            temporizador.Interval = 50;
            temporizador.Tick += (sender, e) => AvancarPasso();
            AtualizarEstado();
        }

        private void MontarInterface()
        {
            Text = "Simulador de Troca de Calor";
            MinimumSize = new Size(800, 550);
            ClientSize = new Size(1050, 700);

            TableLayoutPanel layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                Padding = new Padding(10)
            };
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 92));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 100));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 55));
            Controls.Add(layout);

            GroupBox criacao = CriarGrupo("Criar matriz", 88);
            FlowLayoutPanel camposCriacao = CriarFaixa();
            criacao.Controls.Add(camposCriacao);
            PrepararNumero(entradaN, 1, 20, 3, 0, 60);
            PrepararNumero(entradaLado, 0.001m, 100, 0.1m, 3, 95);
            PrepararNumero(entradaTemperaturaInicial, 0, 10000, 293.15m, 2, 100);
            PrepararMateriais(materiaisIniciais);
            camposCriacao.Controls.Add(CriarCampo("N", entradaN));
            camposCriacao.Controls.Add(CriarCampo("Lado (m)", entradaLado));
            camposCriacao.Controls.Add(CriarCampo("Material inicial", materiaisIniciais));
            camposCriacao.Controls.Add(CriarCampo("Temperatura inicial (K)", entradaTemperaturaInicial));
            Button botaoCriar = CriarBotao("Criar matriz", 105);
            botaoCriar.Click += (sender, e) => CriarMatriz();
            camposCriacao.Controls.Add(botaoCriar);
            layout.Controls.Add(criacao, 0, 0);

            GroupBox edicao = CriarGrupo("Corpo selecionado e simulação", 96);
            FlowLayoutPanel camposEdicao = CriarFaixa();
            edicao.Controls.Add(camposEdicao);
            PrepararMateriais(materiaisDoCorpo);
            PrepararNumero(temperaturaDoCorpo, 0, 10000, 293.15m, 2, 100);
            camposEdicao.Controls.Add(CriarCampo("Novo material", materiaisDoCorpo));
            botaoAlterarMaterial.Text = "Alterar material";
            botaoAlterarMaterial.Width = 110;
            botaoAlterarMaterial.Height = 30;
            botaoAlterarMaterial.Margin = new Padding(6, 27, 3, 3);
            botaoAlterarMaterial.Click += (sender, e) => AlterarMaterial();
            camposEdicao.Controls.Add(botaoAlterarMaterial);
            camposEdicao.Controls.Add(CriarCampo("Temperatura (K)", temperaturaDoCorpo));
            botaoAlterarTemperatura.Text = "Alterar temperatura";
            botaoAlterarTemperatura.Width = 130;
            botaoAlterarTemperatura.Height = 30;
            botaoAlterarTemperatura.Margin = new Padding(6, 27, 3, 3);
            botaoAlterarTemperatura.Click += (sender, e) => AlterarTemperatura();
            camposEdicao.Controls.Add(botaoAlterarTemperatura);
            botaoPasso.Text = "1 passo";
            botaoPasso.Width = 75;
            botaoPasso.Height = 30;
            botaoPasso.Margin = new Padding(6, 27, 3, 3);
            botaoPasso.Click += (sender, e) => AvancarPasso();
            camposEdicao.Controls.Add(botaoPasso);
            botaoIniciar.Text = "Iniciar";
            botaoIniciar.Width = 75;
            botaoIniciar.Height = 30;
            botaoIniciar.Margin = new Padding(6, 27, 3, 3);
            botaoIniciar.Click += (sender, e) => Iniciar();
            camposEdicao.Controls.Add(botaoIniciar);
            botaoParar.Text = "Parar";
            botaoParar.Width = 75;
            botaoParar.Height = 30;
            botaoParar.Margin = new Padding(6, 27, 3, 3);
            botaoParar.Click += (sender, e) => Parar();
            camposEdicao.Controls.Add(botaoParar);
            layout.Controls.Add(edicao, 0, 1);

            painelMatriz.Dock = DockStyle.Fill;
            painelMatriz.AutoScroll = true;
            painelMatriz.BorderStyle = BorderStyle.FixedSingle;
            layout.Controls.Add(painelMatriz, 0, 2);

            TableLayoutPanel rodape = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2, ColumnCount = 1 };
            rodape.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            rodape.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            detalhes.Dock = DockStyle.Fill;
            estado.Dock = DockStyle.Fill;
            rodape.Controls.Add(detalhes, 0, 0);
            rodape.Controls.Add(estado, 0, 1);
            layout.Controls.Add(rodape, 0, 3);
        }

        private static GroupBox CriarGrupo(string titulo, int altura)
        {
            return new GroupBox { Text = titulo, Dock = DockStyle.Fill, Height = altura };
        }

        private static FlowLayoutPanel CriarFaixa()
        {
            return new FlowLayoutPanel { Dock = DockStyle.Fill, AutoScroll = true, WrapContents = false, Padding = new Padding(3) };
        }

        private static Panel CriarCampo(string titulo, Control entrada)
        {
            Panel campo = new Panel
            {
                Width = Math.Max(entrada.Width + 10, titulo.Length * 7 + 10),
                Height = 58,
                Margin = new Padding(3)
            };
            Label rotulo = new Label { Text = titulo, AutoSize = false, Dock = DockStyle.Top, Height = 22 };
            entrada.Location = new Point(0, 25);
            campo.Controls.Add(rotulo);
            campo.Controls.Add(entrada);
            return campo;
        }

        private static Button CriarBotao(string texto, int largura)
        {
            return new Button { Text = texto, Width = largura, Height = 30, Margin = new Padding(6, 27, 3, 3) };
        }

        private static void PrepararNumero(NumericUpDown entrada, decimal minimo, decimal maximo,
                                           decimal valor, int decimais, int largura)
        {
            entrada.Minimum = minimo;
            entrada.Maximum = maximo;
            entrada.DecimalPlaces = decimais;
            entrada.Increment = decimais == 0 ? 1 : 0.01m;
            entrada.Value = valor;
            entrada.Width = largura;
        }

        private void PrepararMateriais(ComboBox lista)
        {
            lista.DropDownStyle = ComboBoxStyle.DropDownList;
            lista.Width = 130;
            NoMaterial? atual = controller.Materiais.Cabeca;
            while (atual != null)
            {
                lista.Items.Add(atual.Material);
                atual = atual.Proximo;
            }
            lista.SelectedIndex = 0;
        }

        private void CriarMatriz()
        {
            temporizador.Stop();
            controller.CriarSimulacao((int)entradaN.Value, (double)entradaLado.Value,
                                      (Material)materiaisIniciais.SelectedItem!,
                                      (double)entradaTemperaturaInicial.Value);
            linhaSelecionada = -1;
            colunaSelecionada = -1;
            MontarMatriz();
            AtualizarTela();
        }

        private void MontarMatriz()
        {
            int tamanho = controller.SimulacaoAtual!.Tamanho;
            painelMatriz.Controls.Clear();
            botoesDosCorpos = new Button[tamanho, tamanho];
            TableLayoutPanel grade = new TableLayoutPanel
            {
                RowCount = tamanho,
                ColumnCount = tamanho,
                Location = new Point(0, 0),
                Size = new Size(tamanho * 105, tamanho * 70),
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };
            for (int indice = 0; indice < tamanho; indice++)
            {
                grade.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 105));
                grade.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));
            }

            for (int linha = 0; linha < tamanho; linha++)
            {
                for (int coluna = 0; coluna < tamanho; coluna++)
                {
                    Button celula = new Button
                    {
                        Dock = DockStyle.Fill,
                        Margin = Padding.Empty,
                        Tag = new Point(coluna, linha),
                        TextAlign = ContentAlignment.MiddleCenter
                    };
                    celula.Click += SelecionarCorpo;
                    botoesDosCorpos[linha, coluna] = celula;
                    grade.Controls.Add(celula, coluna, linha);
                }
            }

            painelMatriz.Controls.Add(grade);
        }

        private void SelecionarCorpo(object? sender, EventArgs e)
        {
            Point posicao = (Point)((Button)sender!).Tag!;
            colunaSelecionada = posicao.X;
            linhaSelecionada = posicao.Y;
            Corpo corpo = controller.ObterCorpo(linhaSelecionada, colunaSelecionada);
            materiaisDoCorpo.SelectedItem = corpo.Material;
            temperaturaDoCorpo.Value = (decimal)Math.Min((double)temperaturaDoCorpo.Maximum,
                                                           corpo.Temperatura);
            AtualizarTela();
        }

        private void AlterarMaterial()
        {
            if (linhaSelecionada < 0) return;
            temporizador.Stop();
            controller.AlterarMaterial(linhaSelecionada, colunaSelecionada,
                                      (Material)materiaisDoCorpo.SelectedItem!);
            AtualizarTela();
        }

        private void AlterarTemperatura()
        {
            if (linhaSelecionada < 0) return;
            temporizador.Stop();
            controller.AlterarTemperatura(linhaSelecionada, colunaSelecionada,
                                         (double)temperaturaDoCorpo.Value);
            AtualizarTela();
        }

        private void AvancarPasso()
        {
            if (!controller.PodeExecutarPasso())
            {
                temporizador.Stop();
                AtualizarEstado();
                return;
            }

            controller.ExecutarPasso();
            if (!controller.PodeExecutarPasso()) temporizador.Stop();
            AtualizarTela();
        }

        private void Iniciar()
        {
            if (controller.PodeExecutarPasso())
            {
                temporizador.Start();
                AtualizarEstado();
            }
        }

        private void Parar()
        {
            temporizador.Stop();
            AtualizarEstado();
        }

        private void AtualizarTela()
        {
            Simulacao? simulacao = controller.SimulacaoAtual;
            if (simulacao == null || botoesDosCorpos == null) return;

            for (int linha = 0; linha < simulacao.Tamanho; linha++)
            {
                for (int coluna = 0; coluna < simulacao.Tamanho; coluna++)
                {
                    Corpo corpo = simulacao.ObterCorpo(linha, coluna);
                    Button celula = botoesDosCorpos[linha, coluna];
                    celula.Text = corpo.Material.Nome + Environment.NewLine +
                                  corpo.Temperatura.ToString("F2", CultureInfo.CurrentCulture) + " K";
                    celula.BackColor = linha == linhaSelecionada && coluna == colunaSelecionada
                        ? Color.LightSkyBlue : SystemColors.Control;
                }
            }

            if (linhaSelecionada >= 0)
            {
                Corpo corpo = controller.ObterCorpo(linhaSelecionada, colunaSelecionada);
                materiaisDoCorpo.SelectedItem = corpo.Material;
                temperaturaDoCorpo.Value = (decimal)Math.Min((double)temperaturaDoCorpo.Maximum,
                                                               Math.Round(corpo.Temperatura, 2));
                detalhes.Text = $"Corpo ({linhaSelecionada + 1}, {colunaSelecionada + 1}) | " +
                                $"Material: {corpo.Material.Nome} | Massa: {corpo.Massa:F3} kg | " +
                                $"Calor sensível: {corpo.CalorSensivel:F2} J";
            }
            else
            {
                detalhes.Text = "Selecione um corpo na matriz para alterar material ou temperatura.";
            }

            AtualizarEstado();
        }

        private void AtualizarEstado()
        {
            Simulacao? simulacao = controller.SimulacaoAtual;
            bool existeMatriz = simulacao != null;
            bool existeSelecao = existeMatriz && linhaSelecionada >= 0 && !temporizador.Enabled;
            botaoAlterarMaterial.Enabled = existeSelecao;
            botaoAlterarTemperatura.Enabled = existeSelecao;
            materiaisDoCorpo.Enabled = existeSelecao;
            temperaturaDoCorpo.Enabled = existeSelecao;
            botaoPasso.Enabled = controller.PodeExecutarPasso() && !temporizador.Enabled;
            botaoIniciar.Enabled = controller.PodeExecutarPasso() && !temporizador.Enabled;
            botaoParar.Enabled = temporizador.Enabled;

            if (simulacao == null)
            {
                detalhes.Text = "Informe os dados e crie a matriz.";
                estado.Text = "Passos: 0";
            }
            else if (controller.EstaEmEquilibrio())
            {
                estado.Text = $"Passos: {simulacao.NumeroDePassos} | Equilíbrio térmico atingido (diferença ≤ 0,01 K).";
            }
            else if (simulacao.NumeroDePassos >= controller.LimiteDePassos)
            {
                estado.Text = $"Passos: {simulacao.NumeroDePassos} | Limite de segurança atingido sem equilíbrio.";
            }
            else
            {
                string situacao = temporizador.Enabled ? "Simulação em andamento" : "Simulação parada";
                estado.Text = $"Passos: {simulacao.NumeroDePassos} | {situacao}";
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            temporizador.Dispose();
            base.OnFormClosed(e);
        }
    }
}
