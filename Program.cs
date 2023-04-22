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
            current_state.SetBoardState(strzal, strzal2);
            current_state.SetNextMove();

            Memento current_state_memento = new Memento(current_state);
            stack.Add(current_state_memento);

            state = current_state;
        }
        public void ReverseMove()
        {
            stack.DeleteLast();
            Memento old_memento = (Memento)stack.GetLast();
            state = State.DeepCopy(old_memento._State);
        }
        public void PrintBoard()
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
        internal class Memento
        {
            public State _State { get; private set; }
            public Memento(State state)
            {
                _State = state;
            }
            public Memento() { }
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

            public void SetBoardState(int index, int index2)
            {
                if (this._NextMove == 1)
                {
                    if (index <= maxColumnNumber && index2 <= maxRowNumber && (index >= 0 && index2 >= 0))
                        this._Board[index][index2] = 1;
                    
                }
                else
                {
                    if (index <= maxColumnNumber && index2 <= maxRowNumber && (index >= 0 && index2 >= 0))
                        this._Board[index][index2] = -1;
                }
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
