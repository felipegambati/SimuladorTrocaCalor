# Simulador de Troca de Calor

## Contexto acad├¬mico

Este projeto ├⌐ um trabalho de faculdade desenvolvido em C# utilizando
Windows Forms e Visual Studio Community.

O projeto deve seguir um padr├úo MVC b├ísico.

O n├¡vel do projeto deve permanecer b├ísico e compat├¡vel com o conte├║do
ensinado em sala de aula. O c├│digo deve ser simples, leg├¡vel e f├ícil
de explicar durante uma apresenta├º├úo ou corre├º├úo.

N├úo devem ser utilizadas arquiteturas, bibliotecas ou conceitos
avan├ºados desnecess├írios.

Coment├írios significativos no c├│digo s├úo obrigat├│rios, pois ser├úo
avaliados pelo professor.

## Enunciado original

ΓùÅ	Fa├ºa coment├írios significativos em seu c├│digo, para mostrar que voc├¬ sabe o que est├í fazendo. Esses coment├írios ser├úo observados no momento da corre├º├úo.

Implementa├º├úo de um simulador de troca de calor
Implementar simulador troca de calor. Este simulador dever├í exibir uma matriz de N x N corpos de formato c├║bico (pode ser visualizado por cima). Cada corpo dever├í estar em contato f├¡sico com os corpos ao seu lado.
Ao criar os corpos dever├í ser informado o material do corpo e o tamanho do seu lado.
Dever├í ser poss├¡vel alterar o material do corpo ap├│s a sua cria├º├úo.
As Unidades de Medida dever├úo estar no SI.
Dever├í ter interface gr├ífica (pode utilizar Widows Forms);
Dever├úo ser considerados v├írios tipos de material diferente. Pelo menos 8 materiais.

A transfer├¬ncia de calor de um corpo para o corpo adjacente dever├í utilizar a Lei de Fourrier, com a seguinte equa├º├úo:

 				q = k.A.[(Delta T)/(Delta x)]
            
       Onde: 
ΓÇó	ΓÇ£qΓÇ¥ ├⌐ a taxa de transfer├¬ncia de calor (em W ou J/s); 
ΓÇó	ΓÇ£kΓÇ¥ ├⌐ a constante de condutividade t├⌐rmica do material utilizado (adimensional); 
ΓÇó	ΓÇ£AΓÇ¥ ├⌐ ├írea da se├º├úo transversal por onde o calor vai fluir entre os corpos; 
ΓÇó	ΓÇ£Delta TΓÇ¥ ├⌐ a diferen├ºa de temperatura entre as superf├¡cies que est├úo trocando calor; 
ΓÇó	ΓÇ£Delta xΓÇ¥ ├⌐ espessura ou comprimento do material usado. Para este trabalho, desconsidere o Delta x, pois vamos considerar que os corpos est├úo se tocando perfeitamente.
Com essa equa├º├úo, d├í pra saber quanto de calor vai fluir de um lado para o outro, considerando que o lado com maior calor sens├¡vel tem a tendencia a ceder calor pro lado com menor calor sens├¡vel, ent├úo o sentido do fluxo pode ser sempre definido assim para cada intera├º├úo, at├⌐ que se obtenha o equil├¡brio t├⌐rmico entre os corpos.
Para os casos em que os corpos s├úo de materiais diferentes, o k utilizado ser├í o menor, pois mesmo que um corpo queira transmitir uma quantidade maior de calor, o outro s├│ vai receber uma quantidade menor.

Calor sens├¡vel ├⌐ a quantidade de calor em Joules (J) que ├⌐ calculada conforme a f├│rmula:
   
Q = m.c.(Delta T)
   
Onde:
ΓÇó	ΓÇ£QΓÇ¥ ├⌐ a quantidade de calor de um corpo;
ΓÇó	ΓÇ£mΓÇ¥ ├⌐ a massa do corpo;
ΓÇó	ΓÇ£cΓÇ¥ ├⌐ o Calor espec├¡fico do corpo;
ΓÇó	ΓÇ£Delta TΓÇ¥ ├⌐ a diferen├ºa de temperatura. Considere que ├⌐ de 0 K at├⌐ a temperatura atual do corpo.
