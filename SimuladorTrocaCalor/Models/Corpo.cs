namespace SimuladorTrocaCalor.Models
{
    public class Corpo
    {
        private Material material;
        private readonly double lado;
        private double temperatura;

        public Material Material { get { return material; } }
        public double Temperatura { get { return temperatura; } } // K

        private double Volume => lado * lado * lado; // m³
        public double AreaDaFace => lado * lado; // m²

        // A massa depende da densidade do material e do volume do cubo
        public double Massa => Material.Densidade * Volume; // kg
        public double CalorSensivel => Massa * Material.CalorEspecifico * Temperatura; // J

        private double CapacidadeTermica => Massa * Material.CalorEspecifico; // J/K

        public Corpo(Material material, double lado, double temperatura)
        {
            this.material = material;
            this.lado = lado;
            this.temperatura = temperatura;
        }

        public void AlterarMaterial(Material novoMaterial)
        {
            // A temperatura permanece igual; massa e calor sensível passam a refletir o novo material
            material = novoMaterial;
        }

        public void AlterarTemperatura(double novaTemperatura)
        {
            temperatura = novaTemperatura;
        }

        public void AplicarVariacaoDeEnergia(double variacaoEmJoules)
        {
            double novoCalorSensivel = CalorSensivel + variacaoEmJoules;
            // Isolando T em Q = m*c*T, conseguimos a nova temperatura após a troca
            temperatura = novoCalorSensivel / CapacidadeTermica;
        }
    }
}
