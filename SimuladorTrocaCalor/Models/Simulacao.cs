namespace SimuladorTrocaCalor.Models
{
    public class Simulacao
    {
        private readonly Corpo[,] corpos;
        private readonly int tamanho;
        private int numeroDePassos;

        public int Tamanho { get { return tamanho; } }
        public int NumeroDePassos { get { return numeroDePassos; } }

        public Simulacao(int tamanho, double lado, Material materialInicial, double temperaturaInicial)
        {
            this.tamanho = tamanho;
            corpos = new Corpo[tamanho, tamanho];

            for (int linha = 0; linha < tamanho; linha++)
            {
                for (int coluna = 0; coluna < tamanho; coluna++)
                {
                    corpos[linha, coluna] = new Corpo(materialInicial, lado, temperaturaInicial);
                }
            }
        }

        public Corpo ObterCorpo(int linha, int coluna)
        {
            return corpos[linha, coluna];
        }

        public void RegistrarPasso()
        {
            numeroDePassos++;
        }
    }
}
