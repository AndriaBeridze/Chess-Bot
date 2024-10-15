namespace Chess.App;

class Program {
    static void Main(string[] args) {
        Game game = new Game(new Human(true), new Bot(false));
    }
}