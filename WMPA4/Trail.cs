using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;

//..........................................................................................
//.TTTTTTTTTTTTTTTTTT.RRRRRRRRRRRRRRRR...........AAAAAAAA........IIIIII...LLLLL.............
//.TTTTTTTTTTTTTTTTTT.RRRRRRRRRRRRRRRRR..........AAAAAAAA........IIIIII...LLLLL.............
//.TTTTTTTTTTTTTTTTTT.RRRRRRRRRRRRRRRRRR........AAAAAAAAA........IIIIII...LLLLL.............
//.TTTTTTTTTTTTTTTTTT.RRRRRRRRRRRRRRRRRR........AAAAAAAAAA.......IIIIII...LLLLL.............
//.......TTTTTT.......RRRRR......RRRRRRRR.......AAAAAAAAAA.......IIIIII...LLLLL.............
//.......TTTTTT.......RRRRR.......RRRRRRR......AAAAAAAAAAA.......IIIIII...LLLLL.............
//.......TTTTTT.......RRRRR........RRRRRR......AAAAAAAAAAAA......IIIIII...LLLLL.............
//.......TTTTTT.......RRRRR.......RRRRRRR.....AAAAAA.AAAAAA......IIIIII...LLLLL.............
//.......TTTTTT.......RRRRR.....RRRRRRRRR.....AAAAAA.AAAAAA......IIIIII...LLLLL.............
//.......TTTTTT.......RRRRRRRRRRRRRRRRRR......AAAAAA..AAAAAA.....IIIIII...LLLLL.............
//.......TTTTTT.......RRRRRRRRRRRRRRRRRR.....AAAAAA...AAAAAA.....IIIIII...LLLLL.............
//.......TTTTTT.......RRRRRRRRRRRRRRRRR......AAAAAA...AAAAAAA....IIIIII...LLLLL.............
//.......TTTTTT.......RRRRRRRRRRRRRRR........AAAAAA....AAAAAA....IIIIII...LLLLL.............
//.......TTTTTT.......RRRRR.RRRRRRRRR.......AAAAAAAAAAAAAAAAA....IIIIII...LLLLL.............
//.......TTTTTT.......RRRRR...RRRRRRRR......AAAAAAAAAAAAAAAAAA...IIIIII...LLLLL.............
//.......TTTTTT.......RRRRR....RRRRRRRR.....AAAAAAAAAAAAAAAAAA...IIIIII...LLLLL.............
//.......TTTTTT.......RRRRR.....RRRRRRR....AAAAAAAAAAAAAAAAAAA...IIIIII...LLLLL.............
//.......TTTTTT.......RRRRR......RRRRRRR...AAAAAA.......AAAAAAA..IIIIII...LLLLL.............
//.......TTTTTT.......RRRRR......RRRRRRRR.RAAAAA.........AAAAAA..IIIIII...LLLLLLLLLLLLLLLL..
//.......TTTTTT.......RRRRR.......RRRRRRR.RAAAAA.........AAAAAA..IIIIII...LLLLLLLLLLLLLLLL..
//.......TTTTTT.......RRRRR........RRRRRRRRAAAAA.........AAAAAAA.IIIIII...LLLLLLLLLLLLLLLL..
//.......TTTTTT.......RRRRR........RRRRRRRRAAAA...........AAAAAA.IIIIII...LLLLLLLLLLLLLLLL..
//..........................................................................................


//CLASS         : Trail
//PROGRAMMER    : Ethan Hoekstra
//LAST MODIFIED : 2019-10-12
//DESCRIPTION   : This class contains methods for a line resizing and rotating across the
//                window.  All outputting to a canvas is done in main.
//
//                The class may be used with a wpf canvas to draw sticks to the screen in
//                a fun and exciting manner.  Just revel in the uncontrollable euphoria as
//                you watch various coloured lines draw themselves across the screen.
namespace WMPA4
{
    public class Trail : MainWindow
    {
        private int expiration;                             //how long the line persists
        private int speed;                                  //how fast the line moves

        private Brush brush;                                //picks a colour for the trail

        private List<Line> lineList = new List<Line>();     //list to keep all of the previous lines for deletion later

        Random rng = new Random();

        private bool raveMode = false;  //bool used to make all lines different colours

        public bool on;                  //bool used to start or stop the trail mid running.

        private int px1;                 //coordinates for the current location of the line for each trail
        private int py1;                 //coordinates for the current location of the line for each trail
        private int px2;                 //coordinates for the current location of the line for each trail
        private int py2;                 //coordinates for the current location of the line for each trail

        private int dx1;                 //which direction to move the first point of the line
        private int dy1;                 //which direction to move the first point of the line
        private int dx2;                 //which direction to move the first point of the line
        private int dy2;                 //which direction to move the first point of the line

        //NAME          : Trail
        //PARAMETERS    : None
        //RETURNS       : None
        //DESCRIPTION   : The constructor for Trail assigns it random starting points, random directions,
        //                a random colour, and sets its speed and expiration time.
        public Trail()
        {
            px1 = rng.Next(width);          //set the starting a point 1 on the line
            py1 = rng.Next(height);         //set the starting a point 1 on the line
            px2 = rng.Next(width);          //set the starting a point 2 on the line
            py2 = rng.Next(height);         //set the starting a point 2 on the line

            do
            {
                dx1 = rng.Next(4) - 2;      //generate a random directing for the point to start moving
                dy1 = rng.Next(4) - 2;      //generate a random directing for the point to start moving
                dx2 = rng.Next(4) - 2;      //generate a random directing for the point to start moving
                dy2 = rng.Next(4) - 2;      //generate a random directing for the point to start moving
            } 
            while (dx1 == 0 || dx2 == 0 || dy1 == 0 || dy2 == 0);

            //generate a random colour for the trail using three rbg values that isn't white
            brush = new SolidColorBrush(Color.FromRgb((byte)rng.Next(1, 255),
              (byte)rng.Next(1, 255), (byte)rng.Next(1, 220)));

            on = true;                            //set the trail to run upon instantiation

            this.expiration = (int)expirationSlider.Value;
            this.speed = (int)speedSlider.Value;  //set speed and expiration time according to sliders
        }


        //NAME          : ToLine
        //PARAMETERS    : None
        //RETURNS       : Line   
        //DESCRIPTION   : Converts the Trail into a line for use in main
        public Line ToLine()
        {

            Line line = new Line();         //make a tmp line

            line.X1 = px1;                  //assign it the two points coordinates
            line.Y1 = py1;
            line.X2 = px2;
            line.Y2 = py2;

            line.StrokeThickness = 1;       //set thickness

            if (this.raveMode == false)
            {
                line.Stroke = this.brush;   //set the colour as the trails colour
            }
            else {                          //if party mode is on, make it a random colour
                line.Stroke = new SolidColorBrush(Color.FromRgb((byte)rng.Next(1, 255),
              (byte)rng.Next(1, 255), (byte)rng.Next(1, 220)));
            }   

            return line;                    //return the tmp line
        }


        //NAME          : MoveLine
        //PARAMETERS    : None
        //RETURNS       : None
        //DESCRIPTION   : Moves a line according to the direction it is travelling.
        //                "Bounces" it off of the edge if needed.
        public void MoveLine() {

            this.px1 = this.px1 + this.dx1;              //move the two points according to the direction attributes
            this.py1 = this.py1 + this.dy1;              //move the two points according to the direction attributes
            this.px2 = this.px2 + this.dx2;              //move the two points according to the direction attributes
            this.py2 = this.py2 + this.dy2;              //move the two points according to the direction attributes


            if (this.px1 <= 0 || this.px1 >= width) {    //If a point moves out of bounds, negate it so it "bounces"
                this.dx1 = this.dx1 * -1;
            }
            if (this.py1 <= 0 || this.py1 >= height)     //If a point moves out of bounds, negate it so it "bounces"
            {
                this.dy1 = this.dy1 * -1;
            }
            if (this.px2 <= 0 || this.px2 >= width)      //If a point moves out of bounds, negate it so it "bounces"
            {
                this.dx2 = this.dx2 * -1;
            }
            if (this.py2 <= 0 || this.py2 >= height)     //If a point moves out of bounds, negate it so it "bounces"
            {
                this.dy2 = this.dy2 * -1;
            }
        }


        //I hope you don't need comments for these
        public int GetExpiration() {
            return this.expiration;
        }

        public void SetExpiration(int newExpiration)
        {
            this.expiration = newExpiration;
        }

        public int GetSpeed()
        {
            return this.speed;
        }

        public void SetSpeed(int newSpeed)
        {
            this.speed = newSpeed;
        }

        public void ToggleRaveMode()
        {
            if (raveMode == false)
            {
                raveMode = true;
            }
            else {
                raveMode = false;
            }
        }
    }
}

