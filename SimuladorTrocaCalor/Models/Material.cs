namespace SimuladorTrocaCalor.Models
{
    public class Material
    {
        private readonly string nome;
        private readonly double densidade;
        private readonly double calorEspecifico;
        private readonly double condutividadeTermica;

        public string Nome { get { return nome; } }
        public double Densidade { get { return densidade; } } // kg/m³
        public double CalorEspecifico { get { return calorEspecifico; } } // J/(kg·K)
        public double CondutividadeTermica { get { return condutividadeTermica; } }

        public Material(string nome, double densidade, double calorEspecifico, double condutividadeTermica)
        {
            this.nome = nome;
            this.densidade = densidade;
            this.calorEspecifico = calorEspecifico;
            this.condutividadeTermica = condutividadeTermica;
        }

        public override string ToString()
        {
            return Nome;
        }
    }
}
