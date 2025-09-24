// This will be in charge of updating the console for the came


/* Terminal Display handler
TODOs below
- TODO Allow a set framerate to be specified and factor in the time it takes to draw each frame
- DONE allow merging layers
- DONE allow partially clearing an area
- DONE allow centering somehow
*/

public class TerminalDisplay //
{
    public int size_x { get; } //TODO deprecate
    public int size_y { get; }
    public Coordinate Size { get; } = new Coordinate();
    public Coordinate Center { get; } = new Coordinate();

    int total_layers;
    char[,,] display;
    /// <summary>
    /// Create a display. By default, the display will only have a single layer. Layers will be drawn lowest to highest
    /// </summary>
    public TerminalDisplay(int x, int y, int layers = 1)
    {
        this.Size.x = x > 0 ? x : 100;
        this.size_x = Size.x;
        this.Size.y = y> 0 ? y: 100;
        this.size_y = Size.y;
        this.Center.x = Size.x / 2;
        this.Center.y = Size.y / 2;
        this.total_layers = layers;
        this.display = new char[Size.x , Size.y , layers]; // The 100 will allow it to run when debugging
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
                }
            }
        }
    }

    public void Update(int x, int y, char[,] arrayToDisplay, int layer = 0, Anchor anchor = Anchor.TopLeft) // try to draw image onto display. Whitespace WILL be included
    {
        switch (anchor)
        {
            case Anchor.TopLeft: // change nothing
                break;
            case Anchor.TopCenter: // offset X
                x -= (arrayToDisplay.GetLength(0) - 1) / 2;
                break;
            case Anchor.TopRight:
                x = arrayToDisplay.GetLength(0) - 1;
                break;
            case Anchor.Left:
                y -= (arrayToDisplay.GetLength(1) - 1) / 2;
                break;
            case Anchor.Center:
                x -= (arrayToDisplay.GetLength(0) - 1) / 2;
                y -= (arrayToDisplay.GetLength(1) - 1) / 2;
                break;
            case Anchor.Right:
                x = arrayToDisplay.GetLength(0) - 1;
                y -= (arrayToDisplay.GetLength(1) - 1) / 2;
                break;
            case Anchor.BottomLeft:
                y = arrayToDisplay.GetLength(1) - 1;
                break;
            case Anchor.BottomCenter:
                x -= (arrayToDisplay.GetLength(0) - 1) / 2;
                y = arrayToDisplay.GetLength(1) - 1;
                break;
            case Anchor.BottomRight:
                x = arrayToDisplay.GetLength(0) - 1;
                y = arrayToDisplay.GetLength(1) - 1;
                break;
        }

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
    public void MergeLayer(int mergeFrom, int mergeTo) // Merge layers and clear the one it merged from
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
        Clear(mergeFrom);
    }
    public char[,] CopySection(int x1, int y1, int x2, int y2, int layer = 0, bool clear=false) // Move a section of the display around
    {
        char[,] selectedChars = new char[x2 - x1, y2 - y1]; // This will store the selected section

        // Grab the section
        for (int i = y1; i < y2; i++)
        {
            for (int j = x1; j < x2; j++)
            {
                {
                    selectedChars[j - x1, i - y1] = display[j, i, layer];
                }
            }
        }
        if (clear)
        {
            FillSection(' ', x1, y1, x2, y2, layer);
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