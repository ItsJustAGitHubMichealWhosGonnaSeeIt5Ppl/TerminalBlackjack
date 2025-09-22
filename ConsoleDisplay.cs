// This will be in charge of updating the console for the came


/* Terminal Display handler
TODOs below
- DONE allow merging layers
- DONE allow partially clearing an area
- TODO allow centering somehow
- TODO Allow a set framerate to be specified and factor in the time it takes to draw each frame
*/

public class TerminalDisplay //
{
    public int size_x { get; }
    public int size_y { get; }
    int total_layers;
    char[,,] display;
    /// <summary>
    /// Create a display. By default, the display will only have a single layer. Layers will be drawn lowest to highest
    /// </summary>
    public TerminalDisplay(int x, int y, int layers = 1)
    {
        this.size_x = x;
        this.size_y = y;
        this.total_layers = layers;
        this.display = new char[x, y, layers];
        Console.Clear(); // Clear the terminal
        Clear();

    }

    public void Clear(int layer = 0) // Clear the entire display (set to blank) //TODO allow a base layer (background) to be set
    {
        FillSection(' ', 0, 0, size_x-1, size_y-1, layer);
    }

    public void FillSection(char fill, int x1, int y1, int x2, int y2, int layer = 0)
    {
        for (int ay = y1; ay <= y2; ay++)
        {
            for (int j = x1; j <= x2; j++)
            {
                try
                {
                    display[j, ay, layer] = fill;
                }
                catch (IndexOutOfRangeException e) // Avoid out of range
                {
                    Console.Write(e);
                    Console.ReadKey(true);
                }
            }
        }
    }


    public void Update(int x, int y, char[,] arrayToDisplay, int layer = 0) // try to draw image onto display. Whitespace WILL be included
    {
        for (int ay = 0; ay < arrayToDisplay.GetLength(1); ay++) // must use getlength instead of length
        {

            for (int ax = 0; ax < arrayToDisplay.GetLength(0); ax++)
            {
                if (y + ay >= size_y) // Prevent drawing out of bounds
                {
                    break;
                }
                else if (x + ax < size_x)
                {
                    display[x + ax, y + ay, layer] = arrayToDisplay[ax, ay];
                }
            }
        }
    }
    public void MergeLayer(int mergeFrom, int mergeTo) // Merge layers
    {
        for (int i = 0; i < size_y; i++)
        {
            for (int j = 0; j < size_x; j++)
            {
                if (!(display[j, i, mergeFrom] == ' ' || display[j, i, mergeFrom] == '\0')) // Will ignore whitespace unless its the first (or only) layer.
                {
                    display[j, i, mergeTo] = display[j, i, mergeFrom];
                }
            }
        }
    }
    public char[,] CopySection(int x1, int y1, int x2, int y2, int layer = 0) // Move a section of the display around
    {
        char[,] selectedChars = new char[x2 - x1, y2 - y1]; // This will store the selected section

        // Grab the section
        for (int i = x1; i <= x2; i++)
        {
            for (int j = y1; j <= y2; j++)
            {
                {
                    selectedChars[j - y1, i - x1] = display[j, i, layer];
                }
            }
        }
        return selectedChars;
    }
    public void MoveLayer(int layer, int x, int y) // Move an entire layer around
    {
        //TODO implement me!
    }

    public void Draw(int layer = -1) // TODO allow a specific layer to be "drawn" instead of doing all of them
    {
        string frame = "";
        char[,] frameChars = new char[size_x, size_y]; // This is the base frame.  Each layer will be overlayed onto this.

        for (int l = 0; l < total_layers; l++) // iterate through the layers
        {
            for (int i = 0; i < size_y; i++)
            {
                for (int j = 0; j < size_x; j++)
                {
                    if (l == 0 || !(display[j, i, l] == ' ' || display[j, i, l] == '\0')) // Will ignore whitespace unless its the first (or only) layer.
                    {
                        frameChars[j, i] = display[j, i, l];
                    }
                }
            }
        }

        for (int i = 0; i < size_y; i++)
        {
            for (int j = 0; j < size_x; j++)
            {
                frame += frameChars[j, i];
            }
            frame += '\n'; // Newline for Y axis   

        }
        Console.SetCursorPosition(0, 0); // This sets the cursor to the top left which means its not printing new ones each time (I like that more)
        Console.Write(frame); // Write the entire thing at once
    }

}