using System;
using System.Collections.Generic;

namespace Ivory.TesteEstagio.CampoMinado
{
    class Solver
    {
        readonly CampoMinado Campo;
        private int IndiceInteracao;
        private HashSet<Tuple<int, int>> PosicoesMinas; // { (x, y) }

        public Solver(CampoMinado campo)
        {
            this.Campo = campo;
            this.IndiceInteracao = 0;
            this.PosicoesMinas = new HashSet<Tuple<int, int>>();
        }

        List<Tuple<int, int>> AnalisarMinas(string[] matrix, int i, int j, HashSet<Tuple<int, int>> posMinas = null)
        {
            var centerNr = matrix[i][j] - '0';
            if (centerNr == 0)
            {
                return null;
            }

            var freeSpots = new List<Tuple<int, int>>();
            Action<int, int, int> populateFreeSpot = (int column, int rowBegin, int rowEnd) =>
            {
                for (; rowBegin <= rowEnd; ++rowBegin)
                {
                    if (matrix[column][rowBegin] == '-')
                    {
                        if (posMinas.Contains(new Tuple<int, int>(column, rowBegin)))
                        {
                            // há uma bomba já encontrada nas posições disponíveis, 
                            // compensar diminuindo o número de bombas ao redor
                            centerNr--;
                        }
                        else
                        {
                            freeSpots.Add(new Tuple<int, int>(column, rowBegin));
                        }
                    }
                }
            };

            // posições na linha acima
            if (i != 0)
            {

                populateFreeSpot(Math.Max(i - 1, 0), Math.Max(j - 1, 0), Math.Min(j + 1, 8));
            }

            // posições possíveis na mesma linha
            {
                populateFreeSpot(Math.Max(i, 0), Math.Max(j - 1, 0), Math.Min(j + 1, 8));
            }

            // posições na linha abaixo
            if (i != 8)
            {
                populateFreeSpot(Math.Max(i + 1, 0), Math.Max(j - 1, 0), Math.Min(j + 1, 8));
            }

            // se há a mesma quantidade de posições livres do que o número indicado pela posição analisada
            // então todas as posições livres possuem bombas
            // ex: a posição indica que tem 3 bombas ao redor e há 3 posições possíveis de abrir
            if (freeSpots.Count == centerNr)
            {
                return freeSpots;
            }

            return null;
        }

        public bool Terminado()
        {
            return this.Campo.JogoStatus != 0; ;
        }

        public int NivelIteracao()
        {
            return this.IndiceInteracao;
        }

        public void Step()
        {
            ++this.IndiceInteracao;

            var matrix = this.Campo.Tabuleiro.Split("\r\n");
            for (int x = 0; x < matrix.Length; ++x)
            {
                for (int y = 0; y < matrix[x].Length; ++y)
                {
                    var c = matrix[x][y];

                    // uma posição aberta: ver se há uma mina na posição encontrada antes de abrir
                    if (c == '-')
                    {
                        if (((this.IndiceInteracao % 2) == 0) && !this.PosicoesMinas.Contains(new Tuple<int, int>(x, y)))
                        {
                            this.Campo.Abrir(x + 1, y + 1);

                            // pular a linha após abrir a posição
                            break;
                        }
                    }

                    // um dígito: ver se é possível identificar alguma mina através dele
                    else if (char.IsDigit(c))
                    {
                        var p = this.AnalisarMinas(matrix, x, y, this.PosicoesMinas);
                        if (p != null)
                        {
                            foreach (var l in p)
                            {
                                this.PosicoesMinas.Add(l);
                            }
                        }
                    }
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var campoMinado = new CampoMinado();
            Console.WriteLine("Início do jogo\n=========");
            Console.WriteLine(campoMinado.Tabuleiro);

            // Realize sua codificação a partir deste ponto, boa sorte!
            var solver = new Solver(campoMinado);
            while (!solver.Terminado())
            {
                solver.Step();

                Console.WriteLine($"\nPasso: { solver.NivelIteracao() }\n{ campoMinado.Tabuleiro }");
            }

            Console.WriteLine($"\nEstado final do jogo: { campoMinado.JogoStatus }");
        }
    }
}
