using System;
using System.Collections.Generic;
using static Conect_4_Angel_Perez.Conecta4 .Tablero;

namespace Conect_4_Angel_Perez
{
    namespace Conecta4
    {
        public class Program
        {
            static void Main(string[] args)
            {
                int opcion;
                bool salir = false;
                List<string> registroGanadores = new List<string>();
                //--------------------------------------------------- Menu Principal ---------------------------------------------------
                while (!salir)
                {
                    Console.Clear();
                    Console.WriteLine("Conecta 4");
                    Console.WriteLine("1. Jugar 1 vs 1");
                    Console.WriteLine("2. Jugar 1 vs computadora");
                    Console.WriteLine("3. Registro de ganadores");
                    Console.WriteLine("4. Finalizar programa");
                    Console.Write("Ingrese la opción deseada: ");
                    opcion = int.Parse(Console.ReadLine());
                    //---------------------------------------- Preguntas antes de jugar -------------------------------------------------
                    switch (opcion)
                    {
                        case 1:
                            string jugador1 = SolicitarNombre("Ingrese el nombre del Jugador 1: ");
                            string jugador2 = SolicitarNombre("Ingrese el nombre del Jugador 2: ");
                            string ganador = Jugar(false, jugador1, jugador2);
                            if (ganador != null)
                            {
                                registroGanadores.Add(ganador);
                            }
                            break;
                        case 2:
                            string jugador = SolicitarNombre("Ingrese su nombre: ", "COMPUTADORA");
                            ganador = Jugar(true, jugador, "COMPUTADORA");
                            if (ganador != null)
                            {
                                registroGanadores.Add(ganador);
                            }
                            break;
                        case 3:
                            MostrarRegistroGanadores(registroGanadores);
                            break;
                        case 4:
                            salir = true;
                            break;
                        default:
                            Console.WriteLine("Opción no válida. Intente de nuevo.");
                            break;
                    }
                }
            }

            static string SolicitarNombre(string mensaje, string nombreInvalido = null)
            {
                string nombre;
                do
                {
                    Console.Write(mensaje);
                    nombre = Console.ReadLine();
                    if (nombre == nombreInvalido)
                    {
                        Console.WriteLine("Error: El nombre ingresado no está permitido. Intente de nuevo.");
                    }
                } while (nombre == nombreInvalido);

                return nombre;
            }

            static string Jugar(bool jugarContraComputadora, string jugador1, string jugador2)
            {
                Tablero tablero = new Tablero();
                string ganador = null;
                bool turnoJugador1 = true;

                while (ganador == null && !tablero.TableroLleno())
                {
                    Console.Clear();
                    tablero.MostrarTablero();

                    if (turnoJugador1 || !jugarContraComputadora)
                    {
                        string jugadorActual = turnoJugador1 ? jugador1 : jugador2;
                        Console.WriteLine($"Turno de {jugadorActual}");
                        int columna = SolicitarColumna();
                        bool piezaColocada = tablero.ColocarPieza(columna, turnoJugador1 ? Celda.Jugador1 : Celda.Jugador2);
                        if (piezaColocada)
                        {
                            if (tablero.Ganador(columna))
                            {
                                ganador = jugadorActual;
                            }
                            else
                            {
                                turnoJugador1 = !turnoJugador1;
                            }
                        }
                    }
                    //--------------------------------------------- Jugar Contra la Computadora -----------------------------------------------------------
                    else
                    {
                        int columna = tablero.MovimientoComputadora();
                        bool piezaColocada = tablero.ColocarPieza(columna, Celda.Jugador2);
                        if (piezaColocada)
                        {
                            if (tablero.Ganador(columna))
                            {
                                ganador = jugador2; // Computadora
                            }
                            else
                            {
                                turnoJugador1 = !turnoJugador1;
                            }
                        }
                    }
                }
                //-------------------------------------- Anuncion de Ganador ---------------------------------------------------------------------
                Console.Clear();
                tablero.MostrarTablero();
                if (ganador != null)
                {
                    Console.WriteLine($"¡Felicidades {ganador}! Has ganado.");
                }
                else
                {
                    Console.WriteLine("El tablero está lleno. Es un empate.");
                }

                Console.WriteLine("Presione cualquier tecla para continuar.");
                Console.ReadKey();

                return ganador;
            }
            //-------------------------------------------------------- Preguntas del Conecta 4 ---------------------------------------------------
            static int SolicitarColumna()
            {
                int columna;
                Console.Write("Ingrese el número de columna (1-7): ");
                while (!int.TryParse(Console.ReadLine(), out columna) || columna < 1 || columna > 7)
                {
                    Console.WriteLine("Entrada inválida. Ingrese el número de columna (1-7): ");
                }
                return columna - 1;
            }
            //-------------------------------------------- Registro de Ganadores ----------------------------------------------------------
            static void MostrarRegistroGanadores(List<string> registroGanadores)
            {
                Console.Clear();
                Console.WriteLine("Registro de ganadores:");
                if (registroGanadores.Count == 0)
                {
                    Console.WriteLine("No hay ganadores registrados.");
                }
                else
                {
                    foreach (string ganador in registroGanadores)
                    {
                        Console.WriteLine(ganador);
                    }
                }
                Console.WriteLine("\nPresione cualquier tecla para continuar.");
                Console.ReadKey();
            }
        }
        class Jugador
        {
            public string Nombre { get; set; }
            public int Puntos { get; set; }

            public Jugador(string nombre)
            {
                Nombre = nombre;
                Puntos = 0;
            }
        }
        class Partida
        {
            public Jugador Jugador1 { get; private set; }
            public Jugador Jugador2 { get; private set; }
            public TimeSpan Duracion { get; set; }
            public List<Tuple<string, TimeSpan>> TiemposJugadas { get; private set; }
            public int PuntosJugador1 { get; set; }
            public int PuntosJugador2 { get; set; }

            public Partida(Jugador jugador1, Jugador jugador2)
            {
                Jugador1 = jugador1;
                Jugador2 = jugador2;
                TiemposJugadas = new List<Tuple<string, TimeSpan>>();
            }
        }
        //-------------------------------- Diseño del Tablero ---------------------------------------------------
        class Tablero
        {
            //Usamos enum para definir un conjunto de constantes con nombres y lo llamamos celda.
            public enum Celda
            {
                Vacio = '-',
                Jugador1 = 'O',
                Jugador2 = 'X',
                Jugador1Ganador = '⦿',
                Jugador2Ganador = '⨯'
            }

            private Celda[,] celdas;
            private int[] alturas;

            public Tablero()
            {
                celdas = new Celda[6, 7];
                alturas = new int[7];

                for (int i = 0; i < 6; i++)
                {
                    for (int j = 0; j < 7; j++)
                    {
                        celdas[i, j] = Celda.Vacio;
                    }
                }

                for (int i = 0; i < 7; i++)
                {
                    alturas[i] = 0;
                }
            }

            private bool PosicionValida(int fila, int columna)
            {
                return fila >= 0 && fila < 6 && columna >= 0 && columna < 7;
            }

            public void MostrarTablero()
            {
                for (int i = 0; i < 6; i++)
                {
                    for (int j = 0; j < 7; j++)
                    {
                        Console.Write((char)celdas[i, j] + " ");
                    }
                    Console.WriteLine();
                }
                Console.WriteLine("1 2 3 4 5 6 7");
            }

            public bool ColocarPieza(int columna, Celda pieza)
            {
                if (alturas[columna] >= 6)
                {
                    Console.WriteLine("La columna está llena. Elija otra columna.");
                    return false;
                }

                celdas[5 - alturas[columna], columna] = pieza;
                alturas[columna]++;
                return true;
            }

            public bool Ganador(int columna)
            {
                int fila = 5 - alturas[columna] + 1;
                Celda pieza = celdas[fila, columna];

                // Comprobar horizontal
                int inicio = Math.Max(0, columna - 3);
                int fin = Math.Min(3, columna);
                for (int i = inicio; i <= fin; i++)
                {
                    if (celdas[fila, i] == pieza &&
                        celdas[fila, i + 1] == pieza &&
                        celdas[fila, i + 2] == pieza &&
                        celdas[fila, i + 3] == pieza)
                    {
                        MarcarGanador(fila, i, fila, i + 3);
                        return true;
                    }
                }
                // Comprobar vertical
                if (alturas[columna] >= 4)
                {
                    if (celdas[fila + 1, columna] == pieza &&
                        celdas[fila + 2, columna] == pieza &&
                        celdas[fila + 3, columna] == pieza)
                    {
                        return true;
                    }
                }

                // Comprobar diagonal ascendente
                int diagonalInicioX = Math.Max(0, columna - 3);
                int diagonalInicioY = fila + (columna - diagonalInicioX);
                int diagonalFinX = Math.Min(3, columna);
                int diagonalFinY = fila - (diagonalFinX - columna);

                for (int x = diagonalInicioX, y = diagonalInicioY; x <= diagonalFinX && y >= diagonalFinY; x++, y--)
                {
                    if (PosicionValida(y, x) && PosicionValida(y - 1, x + 1) &&
                        PosicionValida(y - 2, x + 2) && PosicionValida(y - 3, x + 3) &&
                        celdas[y, x] == pieza &&
                        celdas[y - 1, x + 1] == pieza &&
                        celdas[y - 2, x + 2] == pieza &&
                        celdas[y - 3, x + 3] == pieza)
                    {
                        return true;
                    }
                }


                // Comprobar diagonal descendente
                diagonalInicioY = fila - (columna - diagonalInicioX);
                diagonalFinY = fila + (diagonalFinX - columna);

                for (int x = diagonalInicioX, y = diagonalInicioY; x <= diagonalFinX && y <= diagonalFinY; x++, y++)
                {
                    if (PosicionValida(y, x) && PosicionValida(y + 1, x + 1) &&
                        PosicionValida(y + 2, x + 2) && PosicionValida(y + 3, x + 3) &&
                        celdas[y, x] == pieza &&
                        celdas[y + 1, x + 1] == pieza &&
                        celdas[y + 2, x + 2] == pieza &&
                        celdas[y + 3, x + 3] == pieza)
                    {
                        return true;
                    }
                }

                return false;
            }
            public void MarcarGanador(int inicioY, int inicioX, int finY, int finX)
            {
                int deltaY = finY > inicioY ? 1 : (finY < inicioY ? -1 : 0);
                int deltaX = finX > inicioX ? 1 : (finX < inicioX ? -1 : 0);

                int y = inicioY;
                int x = inicioX;

                for (int i = 0; i < 4; i++)
                {
                    Celda celda = celdas[y, x];
                    celdas[y, x] = celda == Celda.Jugador1 ? Celda.Jugador1Ganador : Celda.Jugador2Ganador;
                    y += deltaY;
                    x += deltaX;
                }
            }
            //----------------------------------------------------- Aviso del tablero lleno ----------------------------------------------
            public bool TableroLleno()
            {
                for (int i = 0; i < 7; i++)
                {
                    if (alturas[i] < 6)
                    {
                        return false;
                    }
                }
                return true;
            }

            public int MovimientoComputadora()
            {
                Random random = new Random();
                int columna;
                do
                {
                    columna = random.Next(7);
                } while (alturas[columna] >= 6);
                return columna;
            }
            public static void MostrarRegistroGanadores(List<Partida> partidas)
            {
                Console.WriteLine("\nRegistro de ganadores:");
                for (int i = 0; i < partidas.Count; i++)
                {
                    Console.WriteLine($"Partida {i + 1}:");
                    Console.WriteLine($"Duración: {partidas[i].Duracion}");
                    Console.WriteLine($"{partidas[i].Jugador1.Nombre}: {partidas[i].PuntosJugador1} puntos");
                    Console.WriteLine($"{partidas[i].Jugador2.Nombre}: {partidas[i].PuntosJugador2} puntos");
                    Console.WriteLine();
                }
            }
        }
    }
}
