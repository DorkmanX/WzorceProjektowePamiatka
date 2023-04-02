using System;
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
            state = new State();
        }
        public void MakeMove()
        {
            string message;
            PrintBoard();

            int nextMove = state.GetNextMove();
            if (nextMove == 1)
                message = "Teraz kolej ma: X";
            else
                message = "Teraz kolej ma: O";
            Console.WriteLine("Następna kolej: " + message);

            Console.Write("Wpisz wiersz pola do strzalu:");
            int strzal = Convert.ToInt32(Console.ReadLine());
            Console.Write("Wpisz kolumne pola do strzalu:");
            int strzal2 = Convert.ToInt32(Console.ReadLine());

            state.SetBoardState(strzal, strzal2);
            state.SetNextMove();

            stack.Add(state);
        }
        public void ReverseMove()
        {
            Console.WriteLine("Czy chcesz cofnąć ruch ? Wpisz 1 jeżeli tak, 0 jeżeli nie");
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
            private State _State;

            public Memento(State State)
            {
                _State = State;
            }
        }
        internal class State
        {
            private int _NextMove;
            // 1 -> X   -1 -> O //zaczyna X
            private List<List<int>> _Board;
            // 1 -> X 0 -> Puste pole  -1 -> O
            public State() 
            {
                _NextMove = 1;
                _Board = new List<List<int>>();
                for(int i = 0; i < 3; i++) 
                {
                    _Board.Add(new List<int>() { 0,0,0}); //empty field
                }
            }
            public void SetBoardState(int index, int index2)
            {
                if (_NextMove == 1)
                    _Board[index][index2] = 1;
                else
                    _Board[index][index2] = -1;    
            }
            public List<List<int>> GetBoardState()
            {
                return _Board;
            }
            public void SetNextMove() 
            {
                _NextMove *= -1;
            }
            public int GetNextMove() { return _NextMove; }
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
        public void Delete(object memento) 
        {
            statesList.Remove(memento);
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
