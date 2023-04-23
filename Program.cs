using System;
using System.Reflection;
using System.Text.Json;
using System.Xml.Linq;
using static Program;


public class Program
{
    public class Game
    {
        private Caretaker stack;
        private State state;
        public Game() 
        {
            stack = new Caretaker();

            List<List<int>> board = new List<List<int>>();
            for (int i = 0; i < 3; i++)
            {
                board.Add(new List<int>() { 0, 0, 0 });
            }

            state = new State(1,board,2,2);
            stack.Add(new Memento(State.DeepCopy(state)));
        }
        public void MakeMove()
        {
            BEGIN:
            string message;
            PrintBoard();

            int nextMove = state.GetNextMove();
            if (nextMove == 1)
                message = "Teraz kolej ma: X";
            else
                message = "Teraz kolej ma: O";

            Console.WriteLine("Czy chcesz cofnąć ruch ? Wpisz 1 jeżeli tak, 0 jeżeli nie");
            int revertMove = Convert.ToInt32(Console.ReadLine());

            if (revertMove == 1)
            {
                ReverseMove();
                goto BEGIN;
            }

            Console.WriteLine("Następna kolej: " + message);
            Console.Write("Wpisz wiersz pola do strzalu:");
            int strzal = Convert.ToInt32(Console.ReadLine());
            Console.Write("Wpisz kolumne pola do strzalu:");
            int strzal2 = Convert.ToInt32(Console.ReadLine());

            State current_state = State.DeepCopy(state);

            bool correct_move = current_state.SetBoardState(strzal, strzal2);
            if(!correct_move ) 
            { 
                goto BEGIN; 
            }
            string win_message = string.Empty;
            bool isWin = false;
            (isWin,win_message) = CheckWin(current_state.GetBoardState());

            if ( isWin ) { Console.WriteLine(win_message); Console.ReadKey(); Environment.Exit(0); }
            current_state.SetNextMove();

            Memento current_state_memento = new Memento(current_state);
            stack.Add(current_state_memento);

            state = current_state;
        }
        private void ReverseMove()
        {
            stack.DeleteLast();
            Memento old_memento = (Memento)stack.GetLast();
            state = State.DeepCopy(old_memento.GetState());
        }
        private void PrintBoard()
        {
            foreach (var item in state.GetBoardState())
            {
                foreach (var internalItem in item)
                {
                    if (internalItem == 0)
                        Console.Write("   |");
                    if (internalItem == 1)
                        Console.Write(" X |");
                    if (internalItem == -1)
                        Console.Write(" O |");
                }
                Console.WriteLine("");
            }
        }
        private (bool,string) CheckWin(List<List<int>> board)
        {
            int diagonal1sum = 0;

            for (int i = 0; i < board.Count(); i++)
            {
                int vertical_sum = 0;
                int horizontal_sum = 0;
                for (int j = 0; j < board[i].Count(); j++)
                {
                    horizontal_sum += board[i][j];
                    vertical_sum += board[j][i];
                    if (i == j)
                        diagonal1sum += board[i][j];

                    if (horizontal_sum == 3 || vertical_sum == 3) return (true, "Wygrał X");
                    else if (horizontal_sum == -3 || vertical_sum == -3) return (true, "Wygrał Y");
                }
            }
            if (diagonal1sum == 3) return (true, "Wygrał X");
            else if (diagonal1sum == -3) return (true, "Wygrał Y");

            int diagonal2sum = board[2][0] + board[1][1] + board[0][2];

            if (diagonal2sum == 3) return (true, "Wygrał X");
            else if (diagonal2sum == -3) return (true, "Wygrał Y");

            return (false,"None");
        }
        internal class Memento
        {
            private State _State;
            public Memento(State state)
            {
                _State = state;
            }
            public Memento() { }

            internal State GetState()
            {
                return this._State;
            }
        }
        internal class State
        {
            public int _NextMove { get; set; }
            public List<List<int>> _Board{ get; set; }
            public int maxColumnNumber { get; set; }
            public int maxRowNumber { get; set; }
            public State() { }
            public State(int nextMove, List<List<int>> board, int maxColumnNumber, int maxRowNumber)
            {
                _NextMove = nextMove;
                _Board = board;
                this.maxColumnNumber = maxColumnNumber;
                this.maxRowNumber = maxRowNumber;
            }

            public bool SetBoardState(int index, int index2)
            {
                bool correct = true;
                if (this._NextMove == 1)
                {
                    if (index <= maxColumnNumber && index2 <= maxRowNumber && (index >= 0 && index2 >= 0))
                        this._Board[index][index2] = 1;
                    else
                    {
                        Console.WriteLine("Wyszedles po za zakres planszy");
                        correct = false;
                    }
                    
                }
                else
                {
                    if (index <= maxColumnNumber && index2 <= maxRowNumber && (index >= 0 && index2 >= 0))
                        this._Board[index][index2] = -1;
                    else
                    {
                        Console.WriteLine("Wyszedles po za zakres planszy");
                        correct = false;
                    }
                }
                return correct;
            }
            public List<List<int>> GetBoardState()
            {
                return this._Board;
            }
            public void SetNextMove() 
            {
                this._NextMove *= -1;
            }
            public int GetNextMove() { return this._NextMove; }
            public static T DeepCopy<T>(T input)
            {
                var jsonString = JsonSerializer.Serialize(input);
                return JsonSerializer.Deserialize<T>(jsonString);
            }
        }
    }
    public class Caretaker
    {
        private List<object> statesList;
        public Caretaker()
        {
            statesList = new List<object>();
        }
        public void Add(object memento) 
        {
            statesList.Add(memento);
        }
        public void DeleteLast() 
        {
            statesList.RemoveAt(statesList.Count - 1);
        }
        public object GetLast() 
        {
            return statesList[statesList.Count - 1];
        }
    }
    public static void Main()
    {
        Game game = new Game();
        while(true) 
        {
            game.MakeMove();
        }

    }
}
