# Simulador de Troca de Calor

Projeto desenvolvido em C# utilizando Windows Forms e uma estrutura MVC básica.

O programa simula a transferência de calor entre corpos cúbicos organizados em uma matriz N x N. Cada corpo possui um material, temperatura e propriedades térmicas próprias, podendo trocar calor com os corpos adjacentes.

## Funcionalidades

- Criação de uma matriz N x N
- 8 materiais diferentes
- Alteração do material e da temperatura dos corpos
- Simulação passo a passo ou automática
- Transferência de calor entre corpos adjacentes
- Detecção de equilíbrio térmico
- Interface gráfica em Windows Forms

## Cálculos

A transferência de calor utiliza a Lei de Fourier simplificada:

`q = k * A * ΔT`

O calor sensível é calculado por:

`Q = m * c * ΔT`

A massa de cada corpo é calculada a partir da densidade do material e do volume do cubo.

## Tecnologias

- C#
- .NET
- Windows Forms
- Visual Studio Community

## Como executar

Clone o repositório e abra `SimuladorTrocaCalor.slnx` no Visual Studio. Depois, compile e execute o projeto.
