# Overview

This program was created to simulate a computer opponent for the game Gobblet by Blue Orange. It allows the user to specify the depth of the computer's search algorithm, as well as whether or not the computer goes first. The program also contains functionality for human-versus-human games (mostly for testing purposes) as well as computer-versus-computer games. There is even an option to run a tournament for all depth values between two numbers, with every depth playing as both black and white in every matchup, to see how the search depths as well as turn order can impact which side has the advantage.

### Gobblet Rules

The objective of Gobblet is to be the first player to align four of your Gobblets horizontally, vertically, or diagonally on the four-by-four playing grid (similar to the game Tic Tac Toe). There are 4 sizes of Gobblet piece, and larger pieces are able to cover smaller pieces. At the start of the game, each player has three nested stacks of four Gobblets each, with only the largest Gobblets showing. On a player's turn, they may choose to either play a Gobblet from their hand or move one of their already placed pieces. Gobblets can be moved to any empty space, or used to cover smaller Gobblets of either color. If a Gobblet covering a piece is moved, the revealed piece does not move along with the piece being moved. When playing from the hand, only the top-most Gobblet of a stack may be played. Even if a player's hand is depleted completely, play continues using the pieces on the board. White always moves first.

# Application Usage

The application runs in the command line, and users input commands to start games and specify parameters. Type 'help' to see a list of commands, and 'info' followed by a command name to see the syntax of that command. Commands with arguments should be entered with spaces inbetween each argument, i.e. 'hvc 3 w' for a human-versus-computer game where the computer has a depth of 3 and is playing as white.

## Board Display

When running a game with at least one human player, information about the board will be printed out onto the screen after each turn. The board consists of sixteen two-character spaces, with the first digit representing the piece's color and the second representing its size. For example, a white piece of the largest size would be represented as 'W4' on the board readout. A space represented by '00' indicates an empty space. Black's hand is printed above the board, and White's is printed below. Underneath the board readout is a display of how many points the current board state is worth as evaluated by the scoring algorithm (see the section on the algorithm below for more information). Lastly, if the move was played by the computer, the time it took is listed in the format m:ss:SSS Below is a sample board readout after one turn has been played by a computer playing as White:

```
B4 B4 B4

00 00 00 00
00 00 00 00
00 00 00 00
00 00 00 W4

W4 W4 W3

Points: 3
Time taken: 0:00.034
```
In this example, White played one of their large pieces to the bottom right board space, resulting in a board score of 3. The turn took 34 milliseconds to compute and execute.

## Taking Your Turn

To play a turn, you will first be prompted to enter the starting position of the move. The board squares are numbered from left to right, top to bottom, starting at 0 and ending with 15. A starting position of 16 indicates a move from the pieces in your hand - you will then be prompted to specify the size of piece to play. Lastly, select an ending position for the piece. Remember you can cover smaller pieces!

#The Algorithm
## Scoring a Position
Board positions are given a score based on simple parameters. For every row, column, and diagonal that has a white piece with no black pieces, the score is increased by 1 for each white piece in that line. Additionally, for each row, column, and diagonal that has a black price but no white pieces, the score is decreased by one for each black piece in that line. What this means is that a high score indicates a good position for White, while a low score is good for Black.

## Searching for Moves
The application uses an alpha-beta pruning search algorithm to compute the best move on each computer player turn. This works by looking at all possible moves for both the computer and its opponent for a certain number of turns in the future, or the search's depth. After filling out the 'tree' of all possible moves, each endpoint has its score evaluated, and that score is used to determine which move should be played, with White playing towards the state with the highest possible score and Black playing towards the state with the lowest. The pruning comes into play when the algorithm is able to determine that some branches are not worth evaluating because they will never be chosen if each side is playing optimally. This drastically reduces the computation time and allows higher depths to be more feasible to play against in real time. 

# Findings

Note that for these purposes, I determined a game to be a draw if after several thousand moves neither side was able to win.

From my findings, computers with an even depth play more defensively and are more likely to draw the game. Conversely, computers with an odd depth play more offensively and are better at winning games. This is likely because with an even depth, the last move that the computer checks is the opponent’s move, meaning it is trying to create the worst position for the opponent. On the other hand, computers with an odd depth check their moves last, meaning that they optimize the best situation for themselves, leading to offensive play. When both players have equal depths, even depths lead to a draw while odd depths lead to a win for white. This has led me to conclude that the player going first in Gobblet had an advantage, but perfect defensive play can force a draw.
