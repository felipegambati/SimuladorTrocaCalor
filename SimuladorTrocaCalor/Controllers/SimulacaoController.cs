using SimuladorTrocaCalor.Estruturas;
using SimuladorTrocaCalor.Models;

namespace SimuladorTrocaCalor.Controllers
{
    public class SimulacaoController
    {
        private const int limiteDePassos = 10000;
        private const double IntervaloDoPasso = 1.0; // s; o enunciado não define outro intervalo.
        private const double ToleranciaDeEquilibrio = 0.01; // K
        private readonly ListaMateriais materiais;
        private Simulacao? simulacaoAtual;

        // Entrega uma cópia dos nós para que a lista fixa não seja alterada pela View.
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
            // Valores aproximados de referência, em SI. O k é usado numericamente
            // na fórmula simplificada do professor, sem dividir por Delta x.
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

        public void CriarSimulacao(int tamanho, double lado, Material material, double temperatura)
        {
            if (tamanho < 1 || lado <= 0 || temperatura < 0 || !materiais.Contem(material))
            {
                throw new ArgumentException("Informe valores válidos para criar a matriz.");
            }

            simulacaoAtual = new Simulacao(tamanho, lado, material, temperatura);
        }

        public void AlterarMaterial(int linha, int coluna, Material material)
        {
            if (!materiais.Contem(material))
            {
                throw new ArgumentException("Material inválido.");
            }

            ObterCorpo(linha, coluna).AlterarMaterial(material);
        }

        public void AlterarTemperatura(int linha, int coluna, double temperatura)
        {
            if (temperatura < 0)
            {
                throw new ArgumentException("A temperatura não pode ser menor que 0 K.");
            }

            ObterCorpo(linha, coluna).AlterarTemperatura(temperatura);
        }

        public Corpo ObterCorpo(int linha, int coluna)
        {
            if (SimulacaoAtual == null || linha < 0 || coluna < 0 ||
                linha >= SimulacaoAtual.Tamanho || coluna >= SimulacaoAtual.Tamanho)
            {
                throw new ArgumentException("Selecione um corpo da matriz.");
            }

            return SimulacaoAtual.ObterCorpo(linha, coluna);
        }

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

            // A diferença entre os extremos testa toda a matriz, não só um contato.
            return maior - menor <= ToleranciaDeEquilibrio;
        }

        public bool PodeExecutarPasso()
        {
            return SimulacaoAtual != null && !EstaEmEquilibrio() &&
                   SimulacaoAtual.NumeroDePassos < limiteDePassos;
        }

        public void ExecutarPasso()
        {
            if (!PodeExecutarPasso() || SimulacaoAtual == null)
            {
                return;
            }

            int tamanho = SimulacaoAtual.Tamanho;
            double[,] variacoesDeEnergia = new double[tamanho, tamanho];
            double[,] energiaCedida = new double[tamanho, tamanho];
            double[,] transferenciasDireita = new double[tamanho, tamanho];
            double[,] transferenciasBaixo = new double[tamanho, tamanho];

            for (int linha = 0; linha < tamanho; linha++)
            {
                for (int coluna = 0; coluna < tamanho; coluna++)
                {
                    // Só direita e baixo: cada face compartilhada aparece uma vez,
                    // sem incluir vizinhos diagonais.
                    if (coluna + 1 < tamanho)
                    {
                        double energia = CalcularContato(linha, coluna, linha, coluna + 1);
                        transferenciasDireita[linha, coluna] = energia;
                        if (energia > 0) energiaCedida[linha, coluna] += energia;
                        else energiaCedida[linha, coluna + 1] -= energia;
                    }

                    if (linha + 1 < tamanho)
                    {
                        double energia = CalcularContato(linha, coluna, linha + 1, coluna);
                        transferenciasBaixo[linha, coluna] = energia;
                        if (energia > 0) energiaCedida[linha, coluna] += energia;
                        else energiaCedida[linha + 1, coluna] -= energia;
                    }
                }
            }

            // Se um corpo cederia mais que seu Q inicial para vários vizinhos,
            // reduzimos todas as suas saídas na mesma proporção. Cada receptor
            // recebe exatamente a energia retirada do cedente.
            for (int linha = 0; linha < tamanho; linha++)
            {
                for (int coluna = 0; coluna < tamanho; coluna++)
                {
                    if (coluna + 1 < tamanho)
                    {
                        AcumularTransferencia(linha, coluna, linha, coluna + 1,
                            transferenciasDireita[linha, coluna], energiaCedida, variacoesDeEnergia);
                    }

                    if (linha + 1 < tamanho)
                    {
                        AcumularTransferencia(linha, coluna, linha + 1, coluna,
                            transferenciasBaixo[linha, coluna], energiaCedida, variacoesDeEnergia);
                    }
                }
            }

            // Todos os contatos usam as temperaturas do início do passo. Só após
            // acumular as trocas atualizamos os corpos, evitando efeito da ordem do loop.
            for (int linha = 0; linha < tamanho; linha++)
            {
                for (int coluna = 0; coluna < tamanho; coluna++)
                {
                    SimulacaoAtual.ObterCorpo(linha, coluna)
                        .AplicarVariacaoDeEnergia(variacoesDeEnergia[linha, coluna]);
                }
            }

            SimulacaoAtual.RegistrarPasso();
        }

        private double CalcularContato(int linhaA, int colunaA, int linhaB, int colunaB)
        {
            Corpo primeiro = SimulacaoAtual!.ObterCorpo(linhaA, colunaA);
            Corpo segundo = SimulacaoAtual.ObterCorpo(linhaB, colunaB);
            double diferencaDeTemperatura = Math.Abs(primeiro.Temperatura - segundo.Temperatura);
            if (diferencaDeTemperatura == 0) return 0;

            // Conforme o enunciado: q = k * A * Delta T, sem Delta x.
            // A face do cubo tem área lado²; materiais distintos usam o menor k.
            double menorK = Math.Min(primeiro.Material.CondutividadeTermica,
                                     segundo.Material.CondutividadeTermica);
            double taxaDeCalor = menorK * primeiro.AreaDaFace * diferencaDeTemperatura;
            double energia = taxaDeCalor * IntervaloDoPasso;

            // O maior Q cede calor quando isso também aproxima as temperaturas.
            // Se Q e T indicarem sentidos opostos, usamos o corpo mais quente para
            // cumprir o objetivo expresso de chegar ao equilíbrio térmico.
            bool primeiroMaisQuente = primeiro.Temperatura > segundo.Temperatura;
            bool primeiroCede = primeiro.CalorSensivel > segundo.CalorSensivel;
            if (primeiro.CalorSensivel == segundo.CalorSensivel ||
                primeiroCede != primeiroMaisQuente)
            {
                primeiroCede = primeiroMaisQuente;
            }
            return primeiroCede ? energia : -energia;
        }

        private void AcumularTransferencia(int linhaA, int colunaA, int linhaB, int colunaB,
                                            double energia, double[,] energiaCedida,
                                            double[,] variacoesDeEnergia)
        {
            if (energia == 0) return;

            int linhaCedente = energia > 0 ? linhaA : linhaB;
            int colunaCedente = energia > 0 ? colunaA : colunaB;
            Corpo cedente = SimulacaoAtual!.ObterCorpo(linhaCedente, colunaCedente);
            double proporcao = Math.Min(1.0, cedente.CalorSensivel /
                                             energiaCedida[linhaCedente, colunaCedente]);
            double energiaReal = energia * proporcao;
            variacoesDeEnergia[linhaA, colunaA] -= energiaReal;
            variacoesDeEnergia[linhaB, colunaB] += energiaReal;
        }
    }
}
