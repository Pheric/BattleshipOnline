namespace server {
    public enum GameState {
        SETUP, // waiting for players to register and submit their board settings
        PLAYER1, // waiting for player1 to fire
        PLAYER2, // waiting for player2 to fire
        TIMEOUT, // waiting for opponent to reconnect
        PLAYER1VICTOR, // player 1 wins, game deletion pending
        PLAYER2VICTOR // player 2 wins, game deletion pending
    }
}