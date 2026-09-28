using SimuladorTrocaCalor.Models;

namespace SimuladorTrocaCalor.Estruturas
{
    public class ListaMateriais
    {
        private NoMaterial? cabeca;
        private int qtdElementos;

        public NoMaterial? Cabeca { get { return cabeca; } }
        public int QtdElementos { get { return qtdElementos; } }

        public ListaMateriais()
        {
            cabeca = null;
            qtdElementos = 0;
        }

        public void Adicionar(Material material)
        {
            NoMaterial novo = new NoMaterial(material);
            if (cabeca == null)
            {
                cabeca = novo;
            }
            else
            {
                // Percorre até o último nó para conservar a ordem de inclusão.
                NoMaterial atual = cabeca;
                while (atual.Proximo != null)
                {
                    atual = atual.Proximo;
                }
                atual.Proximo = novo;
            }

            qtdElementos++;
        }

        public bool Contem(Material material)
        {
            NoMaterial? atual = cabeca;
            while (atual != null)
            {
                if (atual.Material == material)
                {
                    return true;
                }
                atual = atual.Proximo;
            }
            return false;
        }
    }
}
