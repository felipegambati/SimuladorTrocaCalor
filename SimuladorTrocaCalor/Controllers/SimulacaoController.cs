using SimuladorTrocaCalor.Estruturas;
using SimuladorTrocaCalor.Models;

namespace SimuladorTrocaCalor.Controllers
{
    public class SimulacaoController
    {
        private const int limiteDePassos = 10000;
        private const double IntervaloDoPasso = 1.0; // s
        private const double ToleranciaDeEquilibrio = 0.01; // K
        private readonly ListaMateriais materiais;
        private Simulacao? simulacaoAtual;

        // Entrega uma cópia dos nós para que a lista fixa não seja alterada pela View
        public ListaMateriais Materiais
        {
            get
            {
                ListaMateriais copia = new ListaMateriais();
                NoMaterial? atual = materiais.Cabeca;
                while (atual != null)
                {
                    copia.Adicionar(atual.Material);
                    atual = atual.Proximo;
                }
                return copia;
            }
        }
        public Simulacao? SimulacaoAtual { get { return simulacaoAtual; } }
        public int LimiteDePassos { get { return limiteDePassos; } }

        public SimulacaoController()
        {
            // Valores aproximados de referência
            materiais = new ListaMateriais();
            materiais.Adicionar(new Material("Cobre", 8960, 385, 401));
            materiais.Adicionar(new Material("Alumínio", 2700, 900, 237));
            materiais.Adicionar(new Material("Ferro", 7870, 449, 80));
            materiais.Adicionar(new Material("Aço", 7850, 490, 50));
            materiais.Adicionar(new Material("Chumbo", 11340, 128, 35));
            materiais.Adicionar(new Material("Vidro", 2500, 840, 1.0));
            materiais.Adicionar(new Material("Concreto", 2400, 880, 1.7));
            materiais.Adicionar(new Material("Madeira", 700, 1700, 0.12));
        }

        // Cria simulação
        public void CriarSimulacao(int tamanho, double lado, Material material, double temperatura)
        {
            if (tamanho < 1 || lado <= 0 || temperatura < 0 || !materiais.Contem(material))
            {
                throw new ArgumentException("Informe valores válidos para criar a matriz.");
            }

            simulacaoAtual = new Simulacao(tamanho, lado, material, temperatura);
        }

        // Alterar material de um corpo específico na matriz
        public void AlterarMaterial(int linha, int coluna, Material material)
        {
            if (!materiais.Contem(material))
            {
                throw new ArgumentException("Material inválido.");
            }

            ObterCorpo(linha, coluna).AlterarMaterial(material);
        }

        // Alterar temperatura de um corpo específico na matriz
        public void AlterarTemperatura(int linha, int coluna, double temperatura)
        {
            if (temperatura < 0)
            {
                throw new ArgumentException("A temperatura não pode ser menor que 0 K.");
            }

            ObterCorpo(linha, coluna).AlterarTemperatura(temperatura);
        }

        // Obter corpo específico da matriz
        public Corpo ObterCorpo(int linha, int coluna)
        {
            if (SimulacaoAtual == null || linha < 0 || coluna < 0 || linha >= SimulacaoAtual.Tamanho || coluna >= SimulacaoAtual.Tamanho)
            {
                throw new ArgumentException("Selecione um corpo da matriz.");
            }

            return SimulacaoAtual.ObterCorpo(linha, coluna);
        }

        // Verifica se a simulação atingiu o equilíbrio térmico
        public bool EstaEmEquilibrio()
        {
            if (SimulacaoAtual == null)
            {
                return false;
            }

            double menor = double.MaxValue;
            double maior = double.MinValue;

            for (int linha = 0; linha < SimulacaoAtual.Tamanho; linha++)
            {
                for (int coluna = 0; coluna < SimulacaoAtual.Tamanho; coluna++)
                {
                    Corpo corpo = SimulacaoAtual.ObterCorpo(linha, coluna);
                    if (corpo.Temperatura < menor) menor = corpo.Temperatura;
                    if (corpo.Temperatura > maior) maior = corpo.Temperatura;
                }
            }

            // A diferença entre os extremos testa toda a matriz, não só um contato
            return maior - menor <= ToleranciaDeEquilibrio;
        }

        // Verifica se é possível executar mais um passo na simulação
        public bool PodeExecutarPasso()
        {
            return SimulacaoAtual != null && !EstaEmEquilibrio() &&
                   SimulacaoAtual.NumeroDePassos < limiteDePassos;
        }

        // Executa um passo da simulação, calculando as transferências de energia entre os corpos
        public void ExecutarPasso()
        {
            if (!PodeExecutarPasso() || SimulacaoAtual == null) { return; }

            int tamanho = SimulacaoAtual.Tamanho;
            double[,] variacoesDeEnergia = new double[tamanho, tamanho];

            for (int linha = 0; linha < tamanho; linha++)
            {
                for (int coluna = 0; coluna < tamanho; coluna++)
                {
                    // Só direita e baixo: cada face compartilhada aparece uma vez, sem incluir vizinhos diagonais
                    if (coluna + 1 < tamanho)
                    {
                        CalcularContato(linha, coluna, linha, coluna + 1, variacoesDeEnergia);
                    }

                    if (linha + 1 < tamanho)
                    {
                        CalcularContato(linha, coluna, linha + 1, coluna, variacoesDeEnergia);
                    }
                }
            }

            // Todos os contatos usam as temperaturas do início do passo. Só após
            // acumular as trocas atualizamos os corpos, evitando efeito da ordem do loop
            for (int linha = 0; linha < tamanho; linha++)
            {
                for (int coluna = 0; coluna < tamanho; coluna++)
                {
                    SimulacaoAtual.ObterCorpo(linha, coluna).AplicarVariacaoDeEnergia(variacoesDeEnergia[linha, coluna]);
                }
            }

            SimulacaoAtual.RegistrarPasso();
        }

        // Calcula a troca entre dois vizinhos e guarda as variações para aplicar ao fim do passo
        private void CalcularContato(int linhaA, int colunaA, int linhaB, int colunaB, double[,] variacoesDeEnergia)
        {
            Corpo primeiro = SimulacaoAtual!.ObterCorpo(linhaA, colunaA);
            Corpo segundo = SimulacaoAtual.ObterCorpo(linhaB, colunaB);
            if (primeiro.CalorSensivel == segundo.CalorSensivel) return;

            double diferencaDeTemperatura = Math.Abs(primeiro.Temperatura - segundo.Temperatura);
            if (diferencaDeTemperatura == 0) return;

            // q = k * A * Delta T
            // A face do cubo tem área lado², materiais diferentes usam o menor k
            double menorK = Math.Min(primeiro.Material.CondutividadeTermica, segundo.Material.CondutividadeTermica);
            double taxaDeCalor = menorK * primeiro.AreaDaFace * diferencaDeTemperatura;
            double energia = taxaDeCalor * IntervaloDoPasso;

            // O sentido depende apenas do calor sensível Q, conforme o enunciado
            bool primeiroCede = primeiro.CalorSensivel > segundo.CalorSensivel;
            int linhaCedente = primeiroCede ? linhaA : linhaB;
            int colunaCedente = primeiroCede ? colunaA : colunaB;
            Corpo cedente = primeiroCede ? primeiro : segundo;

            // Considera o saldo já acumulado para que nenhum corpo ceda mais energia do que possui
            double energiaDisponivel = Math.Max(0, cedente.CalorSensivel + variacoesDeEnergia[linhaCedente, colunaCedente]);
            energia = Math.Min(energia, energiaDisponivel);

            if (primeiroCede)
            {
                variacoesDeEnergia[linhaA, colunaA] -= energia;
                variacoesDeEnergia[linhaB, colunaB] += energia;
            }
            else
            {
                variacoesDeEnergia[linhaB, colunaB] -= energia;
                variacoesDeEnergia[linhaA, colunaA] += energia;
            }
        }
    }
}
