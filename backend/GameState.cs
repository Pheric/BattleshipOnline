namespace server {
    public enum GameState {
        Setup, // waiting for players to register and submit their board settings
        Player1, // waiting for player1 to fire
        Player2, // waiting for player2 to fire
        Player1Victor, // player 1 wins, game deletion pending
        Player2Victor // player 2 wins, game deletion pending
    }
}