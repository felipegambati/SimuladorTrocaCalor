using SimuladorTrocaCalor.Models;

namespace SimuladorTrocaCalor.Estruturas
{
    public class NoMaterial
    {
        private Material material;
        private NoMaterial? proximo;

        public Material Material { get { return material; } set { material = value; } }
        public NoMaterial? Proximo { get { return proximo; } set { proximo = value; } }

        public NoMaterial(Material material)
        {
            this.material = material;
            proximo = null;
        }
    }
}
