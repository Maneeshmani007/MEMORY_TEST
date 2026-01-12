# MEMORY_TEST
Game Flow Overview :

Loading Screen The game starts with a Loading screen managed by LoadingDotSCript  which: -> Displays "Loading" with animated dots -> Automatically transitions to main menu after a delay

Main Menu MainmenuScree Script  handles the main menu interactions: -> Play button: Opens gridselection panel -> Exit button: Quits application

Grid Selection GridSelection provides three game modes: -> 2x2 (4 cards) -> 2x3 (6 cards) -> 5x6 (30 cards) Each selection initializes the game with different grid configurations.

Game Manager -> The core game logic is handled by GameManager which: -> Creates and positions cards based on selected grid size -> Manages card interactions and matching logic -> Handles scoring system -> Controls game state (save/load) -> Manages audio feedback

Card System Individual cards are managed by the Card class which: -> Handles card flipping animations -> Manages card states (matched, flipped) -> Processes player interactions -> Controls visual elements (front/back faces)

Save/Load System The game implements persistence through: -> Save functionality to store current game state -> Load functionality to restore previous game -> Automatically save button state management
