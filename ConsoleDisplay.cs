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

/* ANIMATION TOOL
Basic animations like moving an object around (todos below)
- TODO allow object to move from one coordinate to another
- TODO allow object speed to be set
- TODO allow object speed to increase and decrease
*/
public static class TerminalAnimation
{
    /// <summary>
    /// Create a basic animation for a sprite
    /// </summary>
    /// <param name="display"></param>
    /// <param name="sprite">The image/sprite to be displayed</param>
    /// <param name="start_x"></param>
    /// <param name="start_y"></param>
    /// <param name="end_x"></param>
    /// <param name="end_y"></param>
    /// <param name="movement_amount">How many pixels to move by in each frame</param>
    /// <param name="speed">milliseconds to delay between frames (defaults to 50)</param>
    /// <param name="layer"></param>
    public static void BasicAnimation(TerminalDisplay display, char[,] sprite, int start_x, int start_y, int end_x, int end_y, int movement_amount, int speed = 50, int layer = 0)
    {
        var coordinates = LinearRegression(start_x, start_y, end_x, end_y);
        // We are going to end up with half positions, so we will just round to the nearest full number in that situation
        double slope; // How many pixels we will increase in a direction before increasing 1 in the other
        double intercept; // This is B
        slope = (double)(start_y - end_y) / (start_x - end_x); // To get our Y coordinate, we take this number * the x coordinate - (I DONT KNOW)
        intercept = ((double)(start_y + end_y) / 2) - slope * ((double)(start_x + end_x) / 2);  // This is what Y would be if X = 0


        //foreach ((int x, int y) coord in coordinates)
        {
            display.Clear(layer);
            //display.Update(coord.x, coord.y, sprite, layer);
            display.Draw();
            Thread.Sleep(speed);
        }

    }
    public static (double x_slope, double x_intercept, double y_slope, double y_intercept) LinearRegression(int start_x, int start_y, int end_x, int end_y)
    {
        /* Y = mX + b
        Y is what we get out 
        m is our calculated slope < I know how to get this
        X is our input X coordinate 
        B is what Y would equal if X equaled 0 < To get this we take the average of Y - slope * average of X
        */
        // We need both the X and Y versions
        double slope_y; // How many pixels we will increase in a direction before increasing 1 in the other
        double intercept_y; // This is B
        slope_y = (double)(start_y - end_y) / (start_x - end_x); // To get our Y coordinate, we take this number * the x coordinate
        intercept_y = ((double)(start_y + end_y) / 2) - slope_y * ((double)(start_x + end_x) / 2);  // This is what Y would be if X = 0

        double slope_x;
        double intercept_x; // TO USE THESE WITH A Y NUMBER, TAKE THE X COORDINATE AND ADD (Y/slope_y) <This gives you the correct intercept/offset
        slope_x = (double)(start_x - end_x) / (start_y - end_y);
        intercept_x = ((double)(start_x + end_x) / 2) - slope_x * ((double)(start_y + end_y) / 2);

        return (slope_x, intercept_x, slope_y, intercept_y);
    }

    public static (double x, double y) VectorPls(int direction)
    {
        /* Scaling and offset equations
        - To offset the origin (start point), use velocity_x + <offset>
        - To increase the speed/magnitude/scale, multiply velocity_x by your new scale
        - To use both, it would be <scale> * velocity_x + <offset>.  ORDER MATTERS!!
        
        
        Direction will be 0 - 360 (NO NEGATIVES!)
        - 0 will be straight up (Need to think about inverting this, so that when drawing on the grid, so that bottom left is 0,0 instead of top left)
        - 90 will be right (x+)
        Thinking of this in terms of vectors (x, y)
        Degrees | Vector
        0       | (0, 1)
        45      | (1, 1)
        90      | (1, 0)
        If magnitude doesn't matter, then just multiply the degrees by 0.02222222222 (just 45 / 1). Each 45 degrees, switch which number you're multiplying
        */
        double velocity_x = 0;
        double velocity_y = 0;

        // Scaling to 4 with decimal so it is easier to work with.  This is how we ensure that the magnitude remains 1
        // For each segment of 100, if the number is 
        double direction_scaled = ((double)direction / 360) * 4;
        // Get squared route for velocity coords so that their total magnitude is 1.
        switch (direction_scaled)
        {
            case <= 1: // 0 - 90 degrees
                velocity_x = Math.Sqrt(direction_scaled);
                velocity_y = Math.Sqrt(1 - direction_scaled);
                break;
            case <= 2: // 91 - 180 degrees // Y needs to be negative // Flip the way we input the coords
                velocity_x = Math.Sqrt(2 - direction_scaled);
                velocity_y = -Math.Sqrt(direction_scaled - 1);
                break;
            case <= 3: // 181 - 240 degrees // X and Y need to be negative
                velocity_x = -Math.Sqrt(direction_scaled - 2);
                velocity_y = -Math.Sqrt(3 - direction_scaled);
                break;
            case <= 4: // 241 - 360 degrees // X needs to be negative // Flip the way we input the coords
                velocity_x = -Math.Sqrt(4 - direction_scaled);
                velocity_y = Math.Sqrt(direction_scaled - 3);
                break;
        }

        return (velocity_x, velocity_y);

        // Magnitude is basically just the speed - Also known as a scaler
        // - The length of the object also specifies the magnitude
        // Direction is also needed
        // When you have Magnitude and direction, you have velocity
        // If you add a vector (x, y) to another vector, you will get back the position assuming that the end of your first position is the start of your second position.  
        // Once you have your vector (which is simply an X and Y coordinate), you multiply that by a scaler to move.  You keep the vector numbers and just add them to your coordinates
    }

    public static List<(double x, double y)> Reflect(int x, int y, int border_x, int border_y, double velocity_x, double velocity_y, int max_reflects)
    {
        double xd = Convert.ToDouble(x);
        double yd = Convert.ToDouble(y);
        var coord = new List<(double x, double y)>();
        int relfects = 0;
        while (relfects < max_reflects)
        {
            xd += velocity_x;
            yd += velocity_y;
            if (Convert.ToInt32(xd) >= border_x || Convert.ToInt32(xd) <= 0)
            {
                velocity_x = -velocity_x; // invert X axis
                relfects++;
            }

            if (Convert.ToInt32(yd) >= border_y || Convert.ToInt32(yd) <= 0)
            {
                velocity_y = -velocity_y; // invert X axis
                relfects++;
            }
            coord.Add((xd, yd));
        }
        return coord;
    }
    
    // This doesn't work, im leaving it here to remind myself how bad it was
    public static List<(int x, int y)> Rebound2(int start_x, char direction, int border_x, int border_y, double slope_x, double intercept_x, double slope_y, double intercept_y, int max_rebounds)
    {
        double og_intercept_y = intercept_y;
        double og_intercept_x = intercept_x;
        // These are the coordinates at which a bounce action ocurred
        double bounce_x = intercept_x;
        double bounce_y = intercept_y;
        char calculate_from = 'x'; // This will be X or Y
        var list = new List<(int x, int y)>();

        int x = start_x;
        int y = Convert.ToInt16(Math.Round(slope_y * x + intercept_y));
        int rebounds = 0;
        while (rebounds < max_rebounds) // Simpler way to do this
        {
            if (x < 0 || x > border_x)
            {
                rebounds++; // Count towards maximum rebounds
                calculate_from = 'y';
                //direction = x < 0 ? '+' : '-'; // Flip the direction
                x = x < 0 ? 0 : border_x;
                y = list.Last().y;

                // This is where we collided (Im sure there is a better way)
                bounce_x = x;
                bounce_y = y;

                if (slope_x > (double)0)
                {
                    slope_x = -slope_x; // Invert X slope
                }
                else
                {
                    slope_x = (-1 * slope_x);
                }
            }
            else if (y < 0 || y > border_y)
            {
                rebounds++; // Count towards maximum rebounds
                if (calculate_from == 'y') // Previously calculating from Y, so need to flip our direction
                {
                    direction = direction == '+' ? '-' : '+';
                }
                calculate_from = 'x'; // Use X to calculate

                y = y < 0 ? 0 : border_y; // Make sure that Y coord doesn't end up out of bounds
                x = list.Last().x; // Set X back to the last successfully added one

                if (slope_y > (double)0)
                {
                    slope_y = -slope_y; // Inverting our Y slope lets us go the other way I think
                }
                else
                {
                    slope_y = (-1 * slope_y);
                }
                if (intercept_y > (double)0)
                {
                    intercept_y = -intercept_y;
                }
                else
                {
                    intercept_y = (-1 * og_intercept_y) + (y * 2); // IT WON'T GO BACK THE OTHER WAY WHY :(
                }
            }
            else
            {
                list.Add((x, y));
            }

            if (calculate_from == 'x')
            {

                x = direction == '+' ? x + 1 : x - 1;  // Increase or decrease depending on direction
                y = Convert.ToInt16(Math.Round(slope_y * x + intercept_y));
            }
            else // Calculate from Y then
            {
                y = direction == '+' ? y + 1 : y - 1;
                x = Convert.ToInt16(Math.Round(slope_x * y + bounce_x + (bounce_y / slope_y)));
                x = x;
            }

        }
        return list;
    }
}
