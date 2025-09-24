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
            int terminal_width = Console.WindowWidth-1;
            int terminal_height = Console.WindowHeight-1;

            TerminalDisplay display = new TerminalDisplay(terminal_width, terminal_height, 3);
            var test2 = AsciiArt.Number(21);

            bool test = false;
            bool noInput = true; // For testing when input isn't allowed
            if (test)
            {
                // TEST
                (double x, double y) velocity2 = TerminalMovement.UnitVector(89); // set the start angle here
                var coordlist = TerminalMovement.Reflect(1, 1, display.size_x - 4, display.size_y - 4, velocity2.x, velocity2.y, 20); // Remove 4 from the border so the ball stays on screen
                char[,] x_char = new char[,] { { 'O', 'O' }, { 'O', 'O' } };
                char[,] ball = new char[,] { { ' ', 'O', 'O', ' ' }, { 'O', 'O', 'O', 'O' }, { 'O', 'O', 'O', 'O' }, { ' ', 'O', 'O', ' ' } };
                int trail = 0; // flip back and forth between two layers so that there is a trail
                foreach (var coord in coordlist)
                {
                    display.Clear(trail == 0 ? 1 : 0); // Clear the inactive layer
                    display.Update(Convert.ToInt32(coord.x), Convert.ToInt32(coord.y), ball, trail);
                    display.Draw();
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
                    char[,] coords = AsciiArt.StringToArray($"{Convert.ToInt32(scale * velocity.x + (display.size_x / 2))}, {Convert.ToInt32(scale * velocity.y + (display.size_y / 2))}");
                    display.Update((display.size_x / 2) - 2, display.size_y / 2, coords, 1); // Disply the coords of the circle 

                    display.Update(Convert.ToInt16(scale * velocity.x + (display.size_x / 2)), Convert.ToInt16(scale * velocity.y + (display.size_y / 2)), x_char, 2);

                    display.Draw();
                    Thread.Sleep(10);

                }

                Console.ReadKey(true);
                // END TEST
            }

            display.Clear();
            display.Update(display.size_x / 2, 3, AsciiArt.BlackJackLogo, 0, Anchor.TopCenter); // Draw this as the background in the center ish
            display.Update(display.size_x / 2, 13, AsciiArt.PlayButton, 1, Anchor.TopCenter);
            display.Update(display.size_x / 2, 13, AsciiArt.PlayButtonSelected, 2, Anchor.TopCenter); // Set layer 2 play button to selected
            display.Update(display.size_x / 2, 25, AsciiArt.QuitButton, 1, Anchor.TopCenter);
            display.Draw();

            int button = 1; // Track whether the play button is being selected or not
            while (true && !noInput)
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
                display.Clear(2); // Clear the display in preperation
                if (button == 1) // I've decided this means play 
                {
                    if (select)
                    {
                        display.Update(1, 10, AsciiArt.PlayButtonPressed, 2);
                        break; // Button has been pushed break out of the loop (Not a great solution long term)
                    }
                    else
                    {
                        display.Update(display.size_x / 2, 13, AsciiArt.PlayButtonSelected, 2, Anchor.TopCenter);
                    }
                    display.Draw();

                }
                else if (button == 0) // This means quit
                {
                    display.Update(display.size_x / 2 +1, 25, AsciiArt.QuitButtonSelected, 2, Anchor.TopCenter);
                    display.Draw();
                }
            }
            // Clear the menu off
            display.Clear(1);
            display.Clear(2);
            Deck myDeck = new Deck();

            bool play = true;
            Participant player = new Participant(display.size_x / 2 - 24, display.size_y - 10);
            Participant dealer = new Participant(display.size_x / 2 - 24, 15);
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
                    // Reset hands
                    display.Update(display.size_x - 15, 15, AsciiArt.CardFaceDown, 0); // This is the deck 
                    player.resetHand();
                    dealer.resetHand();
                    

                    player.addCard(myDeck.deck[cardCount--]);
                    dealer.addCard(myDeck.deck[cardCount--]);
                    player.addCard(myDeck.deck[cardCount--]);
                    dealer.addCard(myDeck.deck[cardCount--]);
                    Animations.StartHand(display, player, dealer);
                    // We'll use this loop to display a play again option or something
                    bool stand = false;
                    bool dealerHandShown = false;
                    while (handActive)
                    {
                        //Update counts and display player current hand value
                        player.checkHand();
                        display.FillSection(' ', player.handPos.x2 / 2, player.handPos.y - 10, player.handPos.x + 20, player.handPos.y, 1);
                        display.Update(display.size_x / 2, player.handPos.y - 10, AsciiArt.Number(player.handValue), 1, Anchor.Center);
                        dealer.checkHand();
                        if (dealerHandShown)
                        {
                            display.FillSection(' ', dealer.handPos.x2 / 2, dealer.handPos.y + 15, dealer.handPos.x + 20, dealer.handPos.y, 1);
                            display.Update(display.size_x / 2, dealer.handPos.y + 15, AsciiArt.Number(dealer.handValue), 1, Anchor.Center);
                        }
                        display.Draw();

                        // display will be here
                        // Checking for wins
                        if (player.blackjack && dealer.blackjack)
                        {
                            display.Update((display.size_x / 2) - (AsciiArt.DrawText.GetLength(0) / 2), display.size_y / 2, AsciiArt.DrawText, 2);
                            break;
                            // push
                        }
                        else if (player.blackjack || dealer.bust)
                        {
                            Animations.Win(display);
                            break;
                            // Player wins
                        }

                        else if (dealer.blackjack || player.bust)
                        {
                            display.Update(display.size_x / 2, display.size_y / 2, AsciiArt.LoseText, 2, Anchor.Center);
                            break;
                            // dealer wins
                        }
                        else if (stand && player.handValue == dealer.handValue && dealer.handValue >= 17)
                        {
                            display.Update((display.size_x / 2) - (AsciiArt.DrawText.GetLength(0) / 2), display.size_y / 2, AsciiArt.DrawText, 2);
                            break;
                            // push
                        }

                        else if (stand && dealer.handValue >= 17 && player.handValue > dealer.handValue)
                        {
                            Animations.Win(display); //TODO display dealer hand before the animation plays
                            break;
                            // player wins
                        }
                        else if (stand && dealer.handValue >= 17 && dealer.handValue > player.handValue)
                        {
                            display.Update((display.size_x / 2) - (AsciiArt.LoseText.GetLength(0) / 2), display.size_y / 2, AsciiArt.LoseText, 2);
                            break;
                            // dealer wins
                        }
                        else if (!stand) // nobody has one, keep moving
                        {
                            switch (Console.ReadKey(true).Key)
                            {
                                case ConsoleKey.H: // Hit
                                    player.addCard(myDeck.deck[cardCount--]);
                                    Animations.AddCard(display, player);
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
                            if (!dealerHandShown) // Avoid drawing multiple times
                            {
                                display.FillSection(' ', display.Center.x - 7, dealer.handPos.y, display.Center.x - 1, dealer.handPos.y + 6, 1); // Clear the face down card
                                display.Update(display.Center.x - 7, dealer.handPos.y, AsciiArt.Card(dealer.hand[0]), 2); // Display face down card
                                display.MergeLayer(2, 1); // Merge layer because some of the cards have a bit extra on the side
                                display.Draw();
                                dealerHandShown = true;
                            }
                            dealer.addCard(myDeck.deck[cardCount--]);
                            Animations.AddCard(display, dealer);
                        }
                    }
                    if (!dealerHandShown) // Avoid drawing multiple times
                    {
                        display.FillSection(' ', display.Center.x - 7, dealer.handPos.y, display.Center.x - 1, dealer.handPos.y + 6, 1); // Clear the face down card
                        display.Update(display.Center.x - 7, dealer.handPos.y, AsciiArt.Card(dealer.hand[0]), 2); // Display face down card
                        display.MergeLayer(2, 1); // Merge layer because some of the cards have a bit extra on the side
                        dealerHandShown = true;
                    }
                    display.Draw();
                    Console.ReadKey(); // Wait for player to hit a key to start the next round
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
        public (int x, int y, int x2, int y2) handPos; // Top left corner, Hand will hold 6 cards max for now. Each card is 8x6 for now //TODO Remove this
        public Participant(int x, int y)
        {
            this.handPos = (x, y, x+48, y+6);// should create hand space
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
        static int delay = 10;
        static int move_amount = 2; // How much magnitude we will move by
        public static void StartHand(TerminalDisplay display, Participant player, Participant dealer)
        {
            // For now this will be on layer 1 I guess
            display.Clear(1);
            display.Clear(2);
            (int x, int y) deckPos = (display.size_x - 15, 15); // This is where the deck is

            // Player card
            TerminalMovement.BasicAnimation(display, AsciiArt.Card(player.hand[0]), deckPos.x, deckPos.y, display.Center.x, player.handPos.y, move_amount, delay, 1);
            TerminalMovement.BasicAnimation(display, AsciiArt.CardFaceDown, deckPos.x, deckPos.y, display.Center.x, dealer.handPos.y, move_amount, delay, 2);
            int j = display.size_x;
            display.MergeLayer(2, 1); // Merge layer 2 to layer 1
            AddCard(display, player);
            AddCard(display, dealer);
        }
        public static void AddCard(TerminalDisplay display, Participant participant)
        {
            (int x, int y) deckPos = (display.size_x - 15, 15);
            TerminalMovement.BasicAnimation(display, AsciiArt.Card(participant.hand.Last()), deckPos.x, deckPos.y, display.Center.x + (4*participant.hand.Count) + (participant.hand.Count == 2 ? 0 : 1), participant.handPos.y, move_amount, delay, 2); // Should set the position correctly.  Need to add 1 after first card, idk 1
            display.MergeLayer(2, 1); // Merge layer 2 to layer 1
            char[,] cards = AsciiArt.TrimArray(display.CopySection(0, participant.handPos.y, display.Size.x-1, participant.handPos.y+7, 1, clear:true));// Fill the section we just copied with air
            display.Update(display.Center.x, participant.handPos.y, cards, 1, Anchor.TopCenter);
            display.Draw();
 
        }

        public static void Win(TerminalDisplay display)
        {

            int i = 0;
            int loop = 20;
            while (loop-- > 0)
            {
                display.Clear(2);
                switch (i++)
                {
                    case 0:
                        display.Update((display.size_x / 2) - (AsciiArt.WinTextNE.GetLength(0) / 2), display.size_y / 2, AsciiArt.WinTextNE, 2);
                        break;
                    case 1:
                        display.Update((display.size_x / 2) - (AsciiArt.WinTextNE.GetLength(0) / 2), display.size_y / 2, AsciiArt.WinTextSE, 2);
                        break;
                    case 2:
                        display.Update((display.size_x / 2) - (AsciiArt.WinTextNE.GetLength(0) / 2), display.size_y / 2, AsciiArt.WinTextSW, 2);
                        break;
                    case 3:
                        display.Update((display.size_x / 2) - (AsciiArt.WinTextNE.GetLength(0) / 2), display.size_y / 2, AsciiArt.WinTextNW, 2);
                        i = 0;
                        break;
                }
                display.Draw();
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
        public static char[,] TrimArray(char[,] array, int buffer = 1) // Trim whitespace from array

        {
            ((int x, int y) start, (int x, int y) end) trimmed = ((array.GetLength(0), array.GetLength(1)), (0, 0));
            bool newRow;
            char[,] trimmedArray;

            for (int i = 0; i < array.GetLength(1); i++)
            {
                newRow = true;
                for (int j = 0; j < array.GetLength(0); j++)
                {
                    if (array[j, i] != ' ' && array[j, i] != '\0') // Blank
                    {
                        if (newRow) // Checking for whitespace on the left
                        {
                            trimmed.start.x = trimmed.start.x >= j ? j - buffer : trimmed.start.x;
                            trimmed.start.y = trimmed.start.y >= i ? i - buffer : trimmed.start.y;
                            newRow = false;
                        }
                        else // Checking whitespace on the right
                        {
                            trimmed.end.x = trimmed.end.x <= j ? +j + buffer : trimmed.end.x;
                            trimmed.end.y = trimmed.end.y <= i ? +i + buffer : trimmed.end.y;
                        }
                    }
                }
            }
            // Reset trimming back to 0 if it is in the negatives
            trimmed.start.x = trimmed.start.x < 0 ? 0 : trimmed.start.x;
            trimmed.start.y = trimmed.start.y < 0 ? 0 : trimmed.start.y;

            trimmedArray = new char[trimmed.end.x - trimmed.start.x, trimmed.end.y - trimmed.start.y];

            for (int i = trimmed.start.y; i < trimmed.end.y; i++)
            {
                for (int j = trimmed.start.x; j < trimmed.end.x; j++)
                {
                    {
                        trimmedArray[j - trimmed.start.x, i - trimmed.start.y] = array[j, i]; // In thoery this will add the number
                    }
                }
            }
            return trimmedArray;
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
        public static char[,] Number(int number, int digitsToDisplay = 0) // TODO this is a pretty awful solution
        {
            int x_start = 0; // This is where we should start the next number
            char[] numbers = number.ToString().ToCharArray(); // set of numbers
            char[,] result = new char[digitsToDisplay == 0 ? numbers.GetLength(0) * 10 : digitsToDisplay * 10, 9]; // Either create 10 slots for each character, or limit it to digits to display

            void AddNumberToArray(char[,] charNumber)
            {
                //Console.WriteLine($"X: {charNumber.GetLength(0)}, Y: {charNumber.GetLength(1)}");
                //Console.WriteLine($"Starting Position: {x_start}.");
                for (int i = 0; i < charNumber.GetLength(1); i++)
                {
                    for (int j = x_start; j < charNumber.GetLength(0) + x_start; j++)
                    {
                        {
                            result[j, i] = charNumber[j - x_start, i]; // In thoery this will add the number
                        }
                    }
                }
                x_start += charNumber.GetLength(0); // Set the new starting position
            }

            for (int i = 0; i < numbers.GetLength(0); i++) // Could use foreach, but this is a bit cleaner
            {
                if (digitsToDisplay != 0 && i > digitsToDisplay) // Break if we're limiting characters
                {
                    break;
                }
                switch (numbers[i])
                {
                    case '0':
                        AddNumberToArray(zero);
                        break;
                    case '1':
                        AddNumberToArray(one);
                        break;
                    case '2':
                        AddNumberToArray(two);
                        break;
                    case '3':
                        AddNumberToArray(three);
                        break;
                    case '4':
                        AddNumberToArray(four);
                        break;
                    case '5':
                        AddNumberToArray(five);
                        break;
                    case '6':
                        AddNumberToArray(six);
                        break;
                    case '7':
                        AddNumberToArray(seven);
                        break;
                    case '8':
                        AddNumberToArray(eight);
                        break;
                    case '9':
                        AddNumberToArray(nine);
                        break;
                }
            }
            //return result;
            return TrimArray(result);
        }
            static readonly char[,] zero = StringToArray(@"
  ___  
 / _ \ 
| | | |
| | | |
| |_| |
 \___/ 
        
");
            static readonly char[,] one = StringToArray(@"
 __ 
/_ |
 | |
 | |
 | |
 |_|
    
");
            static readonly char[,] two = StringToArray(@"
 ___  
|__ \ 
   ) |
  / / 
 / /_ 
|____|
      
");
            static readonly char[,] three = StringToArray(@"
 ____  
|___ \ 
  __) |
 |__ < 
 ___) |
|____/ 
       
");
            static readonly char[,] four = StringToArray(@"
 _  _   
| || |  
| || |_ 
|__   _|
   | |  
   |_|  
        
");
            static readonly char[,] five = StringToArray(@"
 _____ 
| ____|
| |__  
|___ \ 
 ___) |
|____/ 
       
");
            static readonly char[,] six = StringToArray(@"
   __  
  / /  
 / /_  
| '_ \ 
| (_) |
 \___/ 
       
");
            static readonly char[,] seven = StringToArray(@"
 ______ 
|____  |
    / / 
   / /  
  / /   
 /_/    
        
");
            static readonly char[,] eight = StringToArray(@"
  ___  
 / _ \ 
| (_) |
 > _ < 
| (_) |
 \___/ 
       
");
            static readonly char[,] nine = StringToArray(@"
  ___  
 / _ \ 
| (_) |
 \__, |
   / / 
  /_/  
       
");
    }
}


