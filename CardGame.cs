/* GOALS
Game of blackjack. 1 player vs dealer
multiple decks shuffled together
Aces 1 or 11
Dealer will hit on 16 or less
Dealer will NOT hit on soft 17


SOUNDS?
Card drawn
Button selected
Someone wins

OTHER
- Use this font https://strlen.com/square/

*/

namespace BlackjackGame
{
    class Game
    {
        static void Main(string[] args)
        {
            // Get console size
            int terminal_width = Console.WindowWidth;
            int terminal_height = Console.WindowHeight;
            

            TerminalDisplay gameDisplay = new TerminalDisplay(terminal_width, terminal_height, 3);
            bool test = false;
            if (test)
            {
            // TEST
                (double x, double y) velocity2 = TerminalMovement.UnitVector(89); // set the start angle here
            var coordlist = TerminalMovement.Reflect(1, 1, gameDisplay.size_x-4, gameDisplay.size_y-4, velocity2.x, velocity2.y, 20); // Remove 4 from the border so the ball stays on screen
            char[,] x_char = new char[,] { { 'O','O' }, { 'O','O' } };
            char[,] ball = new char[,] { { ' ', 'O','O', ' ' }, { 'O','O','O','O' }, { 'O','O','O','O' }, { ' ', 'O','O', ' ' } };
            int trail = 0; // flip back and forth between two layers so that there is a trail
            foreach (var coord in coordlist)
            {
                gameDisplay.Clear(trail == 0 ? 1 : 0); // Clear the inactive layer
                gameDisplay.Update(Convert.ToInt32(coord.x), Convert.ToInt32(coord.y), ball, trail);
                gameDisplay.Draw();
                Thread.Sleep(20);
                trail = trail == 0 ? 1 : 0; // flip the layer

            }

            // Trying to make a circle
            double scale = 35;// This is the radius of the circle
            int origin = 50; // This is the center of the circle  // We'ere just gonna put it dead centre
            for (int i = 0; i <= -1; i++) // Debug the circle
            {
                (double x, double y) velocity = TerminalMovement.UnitVector(i);
                Console.WriteLine($"{i}: {velocity.x}, {velocity.y}");
                Console.WriteLine($"{i}: X={Convert.ToInt32(scale * velocity.x + origin)}, Y={Convert.ToInt32(scale * velocity.y + origin)}");
                Thread.Sleep(10);
            }

            for (int i = 0; i <= 360; i++) // Make a circle
            {
                (double x, double y) velocity = TerminalMovement.UnitVector(i); // I probably don't need to declare the datatype here but whatever
                char[,] coords = AsciiArt.StringToArray($"{Convert.ToInt32(scale * velocity.x + (gameDisplay.size_x / 2))}, {Convert.ToInt32(scale * velocity.y + (gameDisplay.size_y / 2))}");
                gameDisplay.Update((gameDisplay.size_x / 2)-2, gameDisplay.size_y / 2, coords, 1); // Disply the coords of the circle 

                gameDisplay.Update(Convert.ToInt16(scale * velocity.x + (gameDisplay.size_x / 2)), Convert.ToInt16(scale * velocity.y + (gameDisplay.size_y / 2)), x_char, 2);

                gameDisplay.Draw();
                Thread.Sleep(10);

            }

            Console.ReadKey(true);
            // END TEST
            }

            gameDisplay.Clear();
            gameDisplay.Update((gameDisplay.size_x / 2) - (AsciiArt.BlackJackLogo.GetLength(0) / 2), 0, AsciiArt.BlackJackLogo, 0); // Draw this as the background in the center ish
            gameDisplay.Update((gameDisplay.size_x / 2) - (AsciiArt.PlayButton.GetLength(0) / 2), 10, AsciiArt.PlayButton, 1);
            gameDisplay.Update((gameDisplay.size_x / 2) - (AsciiArt.PlayButtonSelected.GetLength(0) / 2), 10, AsciiArt.PlayButtonSelected, 2); // Set layer 2 play button to selected
            gameDisplay.Update((gameDisplay.size_x / 2) - (AsciiArt.QuitButton.GetLength(0) / 2), 22, AsciiArt.QuitButton, 1);
            gameDisplay.Draw();

            int button = 1; // Track whether the play button is being selected or not
            while (true)
            {
                int prevButton = button; // store the previous button value
                bool select = false;
                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.UpArrow: // Play
                        button = 1;
                        break;

                    case ConsoleKey.DownArrow: // Quit
                        button = 0;
                        break;

                    case ConsoleKey.Enter:
                        select = true;
                        break;

                    default:
                        select = false;
                        break;
                }
                if (prevButton == button && select == false) // Only draw updates if a change has happened
                {
                    continue;
                }
                gameDisplay.Clear(2); // Clear the display in preperation
                if (button == 1) // I've decided this means play 
                {
                    if (select)
                    {
                        gameDisplay.Update(1, 10, AsciiArt.PlayButtonPressed, 2);
                        break; // Button has been pushed break out of the loop (Not a great solution long term)
                    }
                    else
                    {
                        gameDisplay.Update((gameDisplay.size_x / 2) - (AsciiArt.PlayButtonSelected.GetLength(0) / 2), 10, AsciiArt.PlayButtonSelected, 2);
                    }
                    gameDisplay.Draw();

                }
                else if (button == 0) // This means quit
                {
                    gameDisplay.Update((gameDisplay.size_x / 2) - (AsciiArt.QuitButton.GetLength(0) / 2), 22, AsciiArt.QuitButtonSelected, 2);
                    gameDisplay.Draw();
                }
            }
            // Clear the menu off
            gameDisplay.Clear(1);
            gameDisplay.Clear(2);
            Deck myDeck = new Deck();

            bool play = true;
            Participant player = new Participant(gameDisplay.size_x / 2 - 20, gameDisplay.size_y - 5);
            Participant dealer = new Participant(gameDisplay.size_x / 2 - 20, 10);
            while (play)
            {
                myDeck.Shuffle(); // Shuffle the deck
                int cardCount = 51;
                bool roundActive = true; // each round will consist of as many hands as the deck allows

                while (roundActive)
                {
                    if (cardCount < 4)
                    {
                        roundActive = false; // End of round, cards need to be reshuffled
                    }
                    bool handActive = true; // start of a new round
                    player.addCard(myDeck.deck[cardCount--]);
                    dealer.addCard(myDeck.deck[cardCount--]);
                    player.addCard(myDeck.deck[cardCount--]);
                    dealer.addCard(myDeck.deck[cardCount--]);
                    Animations.StartHand(gameDisplay, player.hand, dealer.hand);
                    // We'll use this loop to display a play again option or something
                    bool stand = false;
                    while (handActive)
                    {
                        player.checkHand();
                        dealer.checkHand();
                        // display will be here
                        // Checking for wins
                        if (player.blackjack && dealer.blackjack)
                        {
                            gameDisplay.Update((gameDisplay.size_x / 2) - (AsciiArt.DrawText.GetLength(0) / 2), gameDisplay.size_y / 2, AsciiArt.DrawText, 2);
                            Console.ReadKey();
                            break;
                            // push
                        }
                        else if (player.blackjack || dealer.bust)
                        {
                            Animations.Win(gameDisplay);
                            break;
                            // Player wins
                        }

                        else if (dealer.blackjack || player.bust)
                        {
                            gameDisplay.Update((gameDisplay.size_x / 2) - (AsciiArt.LoseText.GetLength(0) / 2), gameDisplay.size_y / 2, AsciiArt.LoseText, 2);
                            Console.ReadKey();
                            break;
                            // dealer wins
                        }
                        else if (stand && player.handValue == dealer.handValue && dealer.handValue >= 17)
                        {
                            gameDisplay.Update((gameDisplay.size_x / 2) - (AsciiArt.DrawText.GetLength(0) / 2), gameDisplay.size_y / 2, AsciiArt.DrawText, 2);
                            Console.ReadKey();
                            break;
                            // push
                        }

                        else if (dealer.handValue >= 17 && player.handValue > dealer.handValue)
                        {
                            Animations.Win(gameDisplay);
                            break;
                            // player wins
                        }
                        else if (dealer.handValue >= 17 && dealer.handValue > player.handValue)
                        {
                            gameDisplay.Update((gameDisplay.size_x / 2) - (AsciiArt.LoseText.GetLength(0) / 2), gameDisplay.size_y / 2, AsciiArt.LoseText, 2);
                            Console.ReadKey();
                            break;
                            // dealer wins
                        }
                        else if (!stand) // nobody has one, keep moving
                        {
                            switch (Console.ReadKey(true).Key)
                            {
                                case ConsoleKey.H: // Hit
                                    player.addCard(myDeck.deck[cardCount--]);
                                    Animations.AddCard(gameDisplay, player.hand);
                                    break;

                                case ConsoleKey.S: // Stand
                                    stand = true;
                                    break;

                                case ConsoleKey.Q: // Quit
                                    handActive = false;
                                    roundActive = false;
                                    play = false;
                                    break;

                                default:
                                    break;
                            }
                        }
                        else // dealer needs to draw
                        {
                            dealer.addCard(myDeck.deck[cardCount--]);
                            Animations.AddCard(gameDisplay, dealer.hand);

                        }
                    }
                }
                // TODO display that this round has ended
            }
        }
    }


    public class Participant // Will store things like whether the player is a dealer, what cards are in their hand, etc
    {
        public List<Card> hand { get; private set; }
        public int handValue { get; private set; }
        public bool bust { get; private set; }
        public bool blackjack { get; private set; }
        public (int x, int y) handPos;
        public Participant(int x, int y)
        {
            this.handPos = (x, y);
            this.hand = new List<Card>();
        }
        public void checkHand()
        {
            this.handValue = 0;
            this.bust = false;
            this.blackjack = false;
            bool ace = false;

            foreach (Card card in hand) // check all the cards
            {
                if (card.Rank >= 10) // Everything above 10 is just worth 10 (duh)
                {
                    handValue += 10;
                }
                else if (card.Rank == 1)
                {
                    ace = true;
                    handValue += 11;
                }
                else
                {
                    handValue += card.Rank;
                }
            }
            if (ace && handValue > 21)
            {
                handValue -= 10;
            }
            if (handValue > 21) // Check whether hand is bust
            {
                bust = true;
            }
            else if (handValue == 21 && ace && hand.Count() == 2)
            {
                blackjack = true;
            }
        }
        public void addCard(Card card) // Add a card to hand
        {
            hand.Add(card);
        }
        public void resetHand() // Reset hand for a new round
        {
            handValue = 0;
            hand.Clear();
        }
    }
    public class Deck
    {
        public Card[] deck = new Card[52]; // Create a standard deck
        char[] suits = { 'd', 'c', 'h', 's' };
        public Deck()
        {
            int deckPos = 0; // Used to iterate through the cards
            foreach (char suit in suits)
            {
                for (int i = 1; i < 14; i++)
                {
                    deck[deckPos++] = new Card(suit, i);
                }
            }
        }
        public void Shuffle() // shuffle deck my god this was hard to figure out
        {
            Random.Shared.Shuffle(deck);
        }
    }

    public class Card
    {
        public char Suit { get; } // This is an "Auto Implemented property"
        public int Rank { get; } // Maybe there is a better name for this? // Ace is 1

        public Card(char suit, int rank)
        {
            this.Suit = suit;
            this.Rank = rank;
        }
        public override string ToString() // This will return a string showing the card.  Not useful long term, just for debugging really
        {
            return $"{Rank} of {Suit}";
        }
    }

    public static class Animations // Pre-configured animations
    {
        public static void StartHand(TerminalDisplay gameDisplay, List<Card> playerHand, List<Card> dealerHand)
        {
            // For now this will be on layer 1 I guess
            gameDisplay.Clear(1);
            gameDisplay.Clear(2);
            (int x, int y) deckPos = (gameDisplay.size_x - 15, 15); // This is where the deck is

            int center_x = gameDisplay.size_x / 2;
            int delay = 20;
            // Player card
            TerminalMovement.BasicAnimation(gameDisplay, AsciiArt.Card(playerHand[0]), deckPos.x, deckPos.y, gameDisplay.size_x / 2,  gameDisplay.size_y - 6, 1, delay, 1);
            TerminalMovement.BasicAnimation(gameDisplay, AsciiArt.CardFaceDown, deckPos.x, deckPos.y, gameDisplay.size_x / 2, 10, 1, delay, 2);
            int j = gameDisplay.size_x;
            gameDisplay.MergeLayer(2, 1); // Merge layer 2 to layer 1
            TerminalMovement.BasicAnimation(gameDisplay, AsciiArt.Card(playerHand[1]), deckPos.x, deckPos.y, gameDisplay.size_x / 2 + 10,  gameDisplay.size_y - 6, 1, delay, 2);
            gameDisplay.MergeLayer(2, 1); // Merge layer 2 to layer 1
            TerminalMovement.BasicAnimation(gameDisplay, AsciiArt.Card(dealerHand[1]), deckPos.x, deckPos.y, gameDisplay.size_x / 2 + 10, 10 , 1, delay, 2);
            gameDisplay.MergeLayer(2, 1); // Merge layer 2 to layer 1
        }
        public static void AddCard(TerminalDisplay gameDisplay, List<Card> hand)
        {
            int center_x = gameDisplay.size_x / 2;
            int j = gameDisplay.size_x;
            for (int i = 0; i < center_x / 2 - (5 * (hand.Count - 1)); i++)
            {
                gameDisplay.Clear(2);
                gameDisplay.Update(j -= 2, gameDisplay.size_y - 10, AsciiArt.Card(hand.Last()), 2);
                gameDisplay.Draw();
                Thread.Sleep(30); // Sleep .5 seconds
            }
            gameDisplay.MergeLayer(2, 1); // Merge layer 2 to layer 1
        }

        public static void Win(TerminalDisplay gameDisplay)
        {

            int i = 0;
            int loop = 20;
            while (loop-- > 0)
            {
                gameDisplay.Clear(2);
                switch (i++)
                {
                    case 0:
                        gameDisplay.Update((gameDisplay.size_x / 2) - (AsciiArt.WinTextNE.GetLength(0) / 2), gameDisplay.size_y / 2, AsciiArt.WinTextNE, 2);
                        break;
                    case 1:
                        gameDisplay.Update((gameDisplay.size_x / 2) - (AsciiArt.WinTextNE.GetLength(0) / 2), gameDisplay.size_y / 2, AsciiArt.WinTextSE, 2);
                        break;
                    case 2:
                        gameDisplay.Update((gameDisplay.size_x / 2) - (AsciiArt.WinTextNE.GetLength(0) / 2), gameDisplay.size_y / 2, AsciiArt.WinTextSW, 2);
                        break;
                    case 3:
                        gameDisplay.Update((gameDisplay.size_x / 2) - (AsciiArt.WinTextNE.GetLength(0) / 2), gameDisplay.size_y / 2, AsciiArt.WinTextNW, 2);
                        i = 0;
                        break;
                }
                gameDisplay.Draw();
                Thread.Sleep(100); // Sleep .06 seconds  
            }
        }
    }
    public static class AsciiArt // Store the ascii art here
    {
        public static char[,] StringToArray(string text) // Convert strings into arrays
        {
            char[] charsFromInput = text.ToCharArray();
            int size_x = 1;
            int size_y = 1;
            int line_x = 1; // This will store the line length, then the longest line will be used
            foreach (char singleChar in charsFromInput)
            {
                if (singleChar == '\n') // Move down a row and reset X axis
                {
                    size_y++;
                    line_x = 1;
                }
                else
                {
                    line_x++;
                }
                size_x = line_x > size_x ? line_x : size_x; // only use it if its longer
            }
            char[,] array = new char[size_x, size_y];
            int x = 0;
            int y = 0;

            foreach (char singleChar in charsFromInput)
            {
                if (singleChar == '\n') // Move down a row and reset X axis
                {
                    y++;
                    x = 0;
                }
                else
                {
                    array[x, y] = singleChar;
                    x++;
                }
            }
            return array;
        
    }
        public static readonly char[,] PlayButtonPressed = StringToArray(@"
 .--------------------------------------------------------. 
|█.------------------------------------------------------.█|
|█|   ______      _____            __        ____  ____  |█|
|█|  |_   __ \   |_   _|          /  \      |████||████| |█|
|█|    | |__) |    | |           / /\ \       \█\  /█/   |█|
|█|    |  ___/     | |   _      / ____ \       \█\/█/    |█|
|█|   _| |_       _| |__/ |   _/ /    \ \_     _|██|_    |█|
|█|  |_____|     |________|  |____|  |____|   |██████|   |█| 
|█|                                                      |█|
|█'------------------------------------------------------'█|
 '--------------------------------------------------------' ");
        public static readonly char[,] PlayButtonSelected = StringToArray(@"
 .--------------------------------------------------------. 
| .------------------------------------------------------. |
| |   ______      _____            __        ____  ____  | |
| |  |_   __ \   |_   _|          /  \      |_  _||_  _| | |
| |    | |__) |    | |           / /\ \       \ \  / /   | |
| |    |  ___/     | |   _      / ____ \       \ \/ /    | |
| |   _| |_       _| |__/ |   _/ /    \ \_     _|  |_    | |
| |  |_____|     |________|  |____|  |____|   |______|   | |
| |                                                      | |
| '------------------------------------------------------' |
 '--------------------------------------------------------' ");

        public static readonly char[,] PlayButton = StringToArray(@"


      ______      _____            __        ____  ____  
     |_   __ \   |_   _|          /  \      |_  _||_  _| 
       | |__) |    | |           / /\ \       \ \  / /   
       |  ___/     | |   _      / ____ \       \ \/ /    
      _| |_       _| |__/ |   _/ /    \ \_     _|  |_    
     |_____|     |________|  |____|  |____|   |______|   
                                                         

                                                            ");
        public static readonly char[,] QuitButtonSelected = StringToArray(@"
 .------------------------------------------------------. 
| .----------------------------------------------------. |
| |    ___        _____  _____    _____    _________   | |
| |  .'   '.     |_   _||_   _|  |_   _|  |  _   _  |  | |
| | /  .-.  \      | |    | |      | |    |_/ | | \_|  | |
| | | |   | |      | '    ' |      | |        | |      | |
| | \  `-'  \_      \ `--' /      _| |_      _| |_     | |
| |  `.___.\__|      `.__.'      |_____|    |_____|    | |
| |                                                    | |
| '----------------------------------------------------' |
 '------------------------------------------------------'");
        public static readonly char[,] QuitButton = StringToArray(@"


       ___        _____  _____    _____    _________   
     .'   '.     |_   _||_   _|  |_   _|  |  _   _  |  
    /  .-.  \      | |    | |      | |    |_/ | | \_|  
    | |   | |      | '    ' |      | |        | |      
    \  `-'  \_      \ `--' /      _| |_      _| |_     
     `.___.\__|      `.__.'      |_____|    |_____|    
                                                       

                                                         ");
        public static readonly char[,] BlackJackLogo = StringToArray(@"
 /$$$$$$$  /$$                     /$$          /$$$$$                     /$$      
| $$__  $$| $$                    | $$         |__  $$                    | $$      
| $$  \ $$| $$  /$$$$$$   /$$$$$$$| $$   /$$      | $$  /$$$$$$   /$$$$$$$| $$   /$$
| $$$$$$$ | $$ |____  $$ /$$_____/| $$  /$$/      | $$ |____  $$ /$$_____/| $$  /$$/
| $$__  $$| $$  /$$$$$$$| $$      | $$$$$$/  /$$  | $$  /$$$$$$$| $$      | $$$$$$/ 
| $$  \ $$| $$ /$$__  $$| $$      | $$_  $$ | $$  | $$ /$$__  $$| $$      | $$_  $$ 
| $$$$$$$/| $$|  $$$$$$$|  $$$$$$$| $$ \  $$|  $$$$$$/|  $$$$$$$|  $$$$$$$| $$ \  $$
|_______/ |__/ \_______/ \_______/|__/  \__/ \______/  \_______/ \_______/|__/  \__/");

        // Cards (Maybe group these so you can do card.facedown, card.spade, etc.)
         public static readonly char[,] LoseText = StringToArray(@"
$$\     $$\  $$$$$$\  $$\   $$\       $$\       $$$$$$\   $$$$$$\  $$$$$$$$\ 
\$$\   $$  |$$  __$$\ $$ |  $$ |      $$ |     $$  __$$\ $$  __$$\ $$  _____|
 \$$\ $$  / $$ /  $$ |$$ |  $$ |      $$ |     $$ /  $$ |$$ /  \__|$$ |      
  \$$$$  /  $$ |  $$ |$$ |  $$ |      $$ |     $$ |  $$ |\$$$$$$\  $$$$$\    
   \$$  /   $$ |  $$ |$$ |  $$ |      $$ |     $$ |  $$ | \____$$\ $$  __|   
    $$ |    $$ |  $$ |$$ |  $$ |      $$ |     $$ |  $$ |$$\   $$ |$$ |      
    $$ |     $$$$$$  |\$$$$$$  |      $$$$$$$$\ $$$$$$  |\$$$$$$  |$$$$$$$$\ 
    \__|     \______/  \______/       \________|\______/  \______/ \________|");
         public static readonly char[,] WinTextNE = StringToArray(@"
 /$$     /$$ /$$$$$$  /$$   /$$       /$$      /$$ /$$$$$$ /$$   /$$
|  $$   /$$//$$__  $$| $$  | $$      | $$  /$ | $$|_  $$_/| $$$ | $$
 \  $$ /$$/| $$  \ $$| $$  | $$      | $$ /$$$| $$  | $$  | $$$$| $$
  \  $$$$/ | $$  | $$| $$  | $$      | $$/$$ $$ $$  | $$  | $$ $$ $$
   \  $$/  | $$  | $$| $$  | $$      | $$$$_  $$$$  | $$  | $$  $$$$
    | $$   | $$  | $$| $$  | $$      | $$$/ \  $$$  | $$  | $$\  $$$
    | $$   |  $$$$$$/|  $$$$$$/      | $$/   \  $$ /$$$$$$| $$ \  $$
    |__/    \______/  \______/       |__/     \__/|______/|__/  \__/");
         public static readonly char[,] WinTextNW = StringToArray(@"
$$\     $$\  $$$$$$\  $$\   $$\       $$\      $$\ $$$$$$\ $$\   $$\ 
\$$\   $$  |$$  __$$\ $$ |  $$ |      $$ | $\  $$ |\_$$  _|$$$\  $$ |
 \$$\ $$  / $$ /  $$ |$$ |  $$ |      $$ |$$$\ $$ |  $$ |  $$$$\ $$ |
  \$$$$  /  $$ |  $$ |$$ |  $$ |      $$ $$ $$\$$ |  $$ |  $$ $$\$$ |
   \$$  /   $$ |  $$ |$$ |  $$ |      $$$$  _$$$$ |  $$ |  $$ \$$$$ |
    $$ |    $$ |  $$ |$$ |  $$ |      $$$  / \$$$ |  $$ |  $$ |\$$$ |
    $$ |     $$$$$$  |\$$$$$$  |      $$  /   \$$ |$$$$$$\ $$ | \$$ |
    \__|     \______/  \______/       \__/     \__|\______|\__|  \__|");
         public static readonly char[,] WinTextSE = StringToArray(@"
|  \    /  \ /      \ |  \  |  \      |  \  _  |  \|      \|  \  |  \
 \$$\  /  $$|  $$$$$$\| $$  | $$      | $$ / \ | $$ \$$$$$$| $$\ | $$
  \$$\/  $$ | $$  | $$| $$  | $$      | $$/  $\| $$  | $$  | $$$\| $$
   \$$  $$  | $$  | $$| $$  | $$      | $$  $$$\ $$  | $$  | $$$$\ $$
    \$$$$   | $$  | $$| $$  | $$      | $$ $$\$$\$$  | $$  | $$\$$ $$
    | $$    | $$__/ $$| $$__/ $$      | $$$$  \$$$$ _| $$_ | $$ \$$$$
    | $$     \$$    $$ \$$    $$      | $$$    \$$$|   $$ \| $$  \$$$
     \$$      \$$$$$$   \$$$$$$        \$$      \$$ \$$$$$$ \$$   \$$");
         public static readonly char[,] WinTextSW = StringToArray(@"
/  \    /  |/      \ /  |  /  |      /  |  _  /  |/      |/  \  /  |
$$  \  /$$//$$$$$$  |$$ |  $$ |      $$ | / \ $$ |$$$$$$/ $$  \ $$ |
 $$  \/$$/ $$ |  $$ |$$ |  $$ |      $$ |/$  \$$ |  $$ |  $$$  \$$ |
  $$  $$/  $$ |  $$ |$$ |  $$ |      $$ /$$$  $$ |  $$ |  $$$$  $$ |
   $$$$/   $$ |  $$ |$$ |  $$ |      $$ $$/$$ $$ |  $$ |  $$ $$ $$ |
    $$ |   $$ \__$$ |$$ \__$$ |      $$$$/  $$$$ | _$$ |_ $$ |$$$$ |
    $$ |   $$    $$/ $$    $$/       $$$/    $$$ |/ $$   |$$ | $$$ |
    $$/     $$$$$$/   $$$$$$/        $$/      $$/ $$$$$$/ $$/   $$/ ");
         public static readonly char[,] DrawText = StringToArray(@"
$$$$$$$\  $$$$$$$\   $$$$$$\  $$\      $$\ 
$$  __$$\ $$  __$$\ $$  __$$\ $$ | $\  $$ |
$$ |  $$ |$$ |  $$ |$$ /  $$ |$$ |$$$\ $$ |
$$ |  $$ |$$$$$$$  |$$$$$$$$ |$$ $$ $$\$$ |
$$ |  $$ |$$  __$$< $$  __$$ |$$$$  _$$$$ |
$$ |  $$ |$$ |  $$ |$$ |  $$ |$$$  / \$$$ |
$$$$$$$  |$$ |  $$ |$$ |  $$ |$$  /   \$$ |
\_______/ \__|  \__|\__|  \__|\__/     \__|");
        public static readonly char[,] CardFaceDown = StringToArray(@"
.------.
|*.  .*|
| *  * |
| *  * |
|*'  '*|
'------'");

        /// <summary>
        /// Create a playing card
        /// </summary>
        /// <param name="card"></param>
        /// <returns>formatted card</returns>
        public static char[,] Card(Card card)
        {
            char suit = ' ';
            string rank;
            switch (card.Suit)
            {
                case 'd':
                    suit = '♦';
                    break;

                case 's':
                    suit = '♠';
                    break;

                case 'c':
                    suit = '♣';
                    break;

                case 'h':
                    suit = '♥';
                    break;
            }
            switch (card.Rank)
            {
                case 1:
                    rank = "A";
                    break;
                case 11:
                    rank = "J";
                    break;
                case 12:
                    rank = "Q";
                    break;
                case 13:
                    rank = "K";
                    break;
                default:
                    rank = Convert.ToString(card.Rank);
                    break;
            }
            return StringToArray(@$"
.------.
|{rank,2}--{suit} |
| :  : |
| :  : |
| {suit}--{(rank.Length == 1 ? rank + " " : rank)}|
'------'");
        }
    }
}


