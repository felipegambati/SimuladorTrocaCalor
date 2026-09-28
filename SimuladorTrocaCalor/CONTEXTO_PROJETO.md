# Simulador de Troca de Calor

## Contexto acadêmico

Este projeto é um trabalho de faculdade desenvolvido em C# utilizando
Windows Forms e Visual Studio Community.

O projeto deve seguir um padrão MVC básico.

O nível do projeto deve permanecer básico e compatível com o conteúdo
ensinado em sala de aula. O código deve ser simples, legível e fácil
de explicar durante uma apresentação ou correção.

Não devem ser utilizadas arquiteturas, bibliotecas ou conceitos
avançados desnecessários.

Comentários significativos no código são obrigatórios, pois serão
avaliados pelo professor.

## Enunciado original

●	Faça comentários significativos em seu código, para mostrar que você sabe o que está fazendo. Esses comentários serão observados no momento da correção.

Implementação de um simulador de troca de calor
Implementar simulador troca de calor. Este simulador deverá exibir uma matriz de N x N corpos de formato cúbico (pode ser visualizado por cima). Cada corpo deverá estar em contato físico com os corpos ao seu lado.
Ao criar os corpos deverá ser informado o material do corpo e o tamanho do seu lado.
Deverá ser possível alterar o material do corpo após a sua criação.
As Unidades de Medida deverão estar no SI.
Deverá ter interface gráfica (pode utilizar Widows Forms);
Deverão ser considerados vários tipos de material diferente. Pelo menos 8 materiais.

A transferência de calor de um corpo para o corpo adjacente deverá utilizar a Lei de Fourrier, com a seguinte equação:

 				q = k.A.[(Delta T)/(Delta x)]
            
       Onde: 
•	“q” é a taxa de transferência de calor (em W ou J/s); 
•	“k” é a constante de condutividade térmica do material utilizado (adimensional); 
•	“A” é área da seção transversal por onde o calor vai fluir entre os corpos; 
•	“Delta T” é a diferença de temperatura entre as superfícies que estão trocando calor; 
•	“Delta x” é espessura ou comprimento do material usado. Para este trabalho, desconsidere o Delta x, pois vamos considerar que os corpos estão se tocando perfeitamente.
Com essa equação, dá pra saber quanto de calor vai fluir de um lado para o outro, considerando que o lado com maior calor sensível tem a tendencia a ceder calor pro lado com menor calor sensível, então o sentido do fluxo pode ser sempre definido assim para cada interação, até que se obtenha o equilíbrio térmico entre os corpos.
Para os casos em que os corpos são de materiais diferentes, o k utilizado será o menor, pois mesmo que um corpo queira transmitir uma quantidade maior de calor, o outro só vai receber uma quantidade menor.

Calor sensível é a quantidade de calor em Joules (J) que é calculada conforme a fórmula:
   
Q = m.c.(Delta T)
   
Onde:
•	“Q” é a quantidade de calor de um corpo;
•	“m” é a massa do corpo;
•	“c” é o Calor específico do corpo;
•	“Delta T” é a diferença de temperatura. Considere que é de 0 K até a temperatura atual do corpo.