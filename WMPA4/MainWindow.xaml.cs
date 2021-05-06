//......................................................................................................................................
//.EEEEEEEEEEE..ttt..hhhh................................. HHH...HHHH......................eekk..................ttt....................
//.EEEEEEEEEEE.Ettt..hhhh................................. HHH...HHHH......................eekk.................sttt....................
//.EEEEEEEEEEE.Ettt..hhhh................................. HHH...HHHH......................eekk.................sttt....................
//.EEEE......EEEttttthhhhhhhh....aaaaaa...annnnnnn........ HHH...HHHH....oooooo....eeeeee..eekk..kkkk.kssssss.sssttttttrrrrrr.aaaaaa....
//.EEEE......EEEttttthhhhhhhhh..aaaaaaaa..annnnnnnn....... HHH...HHHH..Hoooooooo..eeeeeeee.eekk.kkkk.kkssssssssssttttttrrrrrrraaaaaaa...
//.EEEEEEEEEE..Ettt..hhhh.hhhhhhaaa.aaaaa.annn.nnnnn...... HHHHHHHHHH..Hooo.ooooooeee.eeee.eekkkkkk..kkss.ssss..sttt.ttrrr..rraa.aaaaa..
//.EEEEEEEEEE..Ettt..hhhh..hhhh....aaaaaa.annn..nnnn...... HHHHHHHHHH.HHoo...oooooeee..eeeeeekkkkk...kksss......sttt.ttrr.......aaaaaa..
//.EEEEEEEEEE..Ettt..hhhh..hhhh.aaaaaaaaa.annn..nnnn...... HHHHHHHHHH.HHoo...oooooeeeeeeeeeeekkkkkk...ksssss....sttt.ttrr....raaaaaaaa..
//.EEEE........Ettt..hhhh..hhhhhaaaaaaaaa.annn..nnnn...... HHH...HHHH.HHoo...oooooeeeeeeeeeeekkkkkk....sssssss..sttt.ttrr...rraaaaaaaa..
//.EEEE........Ettt..hhhh..hhhhhaaa.aaaaa.annn..nnnn...... HHH...HHHH.HHoo...oooooeee......eekkkkkkk.......ssss.sttt.ttrr...rraa.aaaaa..
//.EEEEEEEEEEE.Ettt..hhhh..hhhhhaaa.aaaaa.annn..nnnn...... HHH...HHHH..Hooo.ooooooeee..eeeeeekk.kkkk.kkss..ssss.sttt.ttrr...rraa.aaaaa..
//.EEEEEEEEEEE.Ettttthhhh..hhhhhaaaaaaaaa.annn..nnnn...... HHH...HHHH..Hoooooooo..eeeeeeee.eekk..kkkkkksssssss..sttttttrr...rraaaaaaaa..
//.EEEEEEEEEEE.Ettttthhhh..hhhh.aaaaaaaaa.annn..nnnn...... HHH...HHHH....oooooo....eeeeee..eekk..kkkk..ssssss...sttttttrr....raaaaaaaa..
//......................................................................................................................................

//PROGRAMMER        : See Above
//PROJECT           : WMPA4
//FILE              : MainWindow.xaml.cs
//LAST MODIFIED     : 2019-10-11
//DESCRIPTION       : This program draws and hovers trails around a canvas that can be created,
//                    suspended, resumed, and deleted, all by the user.  Speed and expiration
//                    time of the trails may also be controlled through sliders in in the ui.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

#pragma warning disable CS0618 // Type or member is obsolete

namespace WMPA4
{
    public partial class MainWindow : Window
    {
        private bool darkTheme = false;                         //bool to toggle theme

        public bool raveMode;                                   //bool for toggle of rave mode

        public int width = 673;                                 //width range for points
        public int height = 410;                                //height range for y points

        public int numberOfChildren;                            //track how many ui elements there are for target deletion

        Random rng = new Random();                              //handy dandy random number generator for colours of trails

        private List<Thread> threadList = new List<Thread>();   //list to track all of the threads running
        private List<Thread> subThreadList = new List<Thread>();//list to track all sub threads
        private List<Trail> trailList = new List<Trail>();      //list to track all of the trails in memory

        public MainWindow()
        {
            InitializeComponent();

            this.Title = "Trails";

            numberOfChildren = trailCanvas.Children.Count;     //get the number of ui elements to avoid deleting our buttons later

            height = (int)trailCanvas.Height;
            width = (int)trailCanvas.Width;

            this.Closed += new EventHandler(MainWindow_Closed);//add an event handler to terminate all threads when the program is closed
        }

        //NAME          : RunTrail
        //PARAMETERS    : Trail trail
        //RETURNS       : void
        //DESCRIPTION   : This method runs on a continuous loop that creates a Subthread that in turn
        //                creates a line, moves the line in the current direction, sleeps, repeats.
        //                It will run until the thread is paused or the trail is turned off.
        public void RunTrail(Trail trail)
        {
            while (trail.on)
            {
                Thread SubThread = new Thread(() => DrawLine(trail));       //create a delegate for a new subthread o temporarily place a line
                subThreadList.Add(SubThread);                               //add the subthread to track it later
                SubThread.Start();                                          //run said thread
                trail.MoveLine();                                           //move the line according to its path
                Thread.Sleep(trail.GetSpeed());                             //wait, then do it again
            }
        }

        //NAME          : CreateLine
        //PARAMETERS    : Trail trail
        //RETURNS       : void
        //DESCRIPTION   : This method drops a line in place according to the passed trail 
        //                that will later be deleted by itself.The thread will then expire.
        public void DrawLine(Trail trail)
        {
            this.Dispatcher.Invoke(() =>                            //use dispatcher to avoid UI conflict
            {
                trailCanvas.Children.Add(trail.ToLine());           //add the trail to the line
            });
            Thread.Sleep(trail.GetExpiration());                    //wait the specified time
            this.Dispatcher.Invoke(() =>                            //use dispatcher to avoid conflict
            {
                trailCanvas.Children.RemoveAt(numberOfChildren);    //remove the oldest line from the canvas
            });
            subThreadList.Remove(Thread.CurrentThread);
        }

        //NAME          : NewTrailBtn_Click
        //PARAMETERS    : object sender, RoutedEventArgs e
        //RETURNS       : void
        //DESCRIPTION   : This button is used to spawn a new trail that moves accross the screen.
        //                It instantiates a trail, create a delegate to run it, fires the delegate,
        //                and saves both the trail and thread into a list to manage later.
        private void NewTrailBtn_Click(object sender, RoutedEventArgs e)
        {
            trailList.Add(new Trail());                                             //instantiate a new trail
            Thread TrailThread = new Thread(() => RunTrail(trailList[trailList.Count - 1]));     //create a delegate
            threadList.Add(TrailThread);                                            //save it to the list of threads for tracking
            threadList[threadList.Count - 1].Start();                               //run the thread and hand user control back
            numberOfTrailsLbl.Content = "Trails: " + trailList.Count.ToString();    //update the label
        }

        //NAME          : DeleteTrailBtn_Click
        //PARAMETERS    : object sender, RoutedEventArgs e
        //RETURNS       : void
        //DESCRIPTION   : This button removes the oldest trail currently running by
        //                disabling it with the "on" bool.
        private void DeleteTrailBtn_Click(object sender, RoutedEventArgs e)
        {
            if (trailList.Count > 0)            //make sure the list isn't blank before deleteing one
            {
                trailList[0].on = false;        //stop the thread from running
                trailList.RemoveAt(0);          //remove the trail from the list
                threadList.RemoveAt(0);         //remove the thread from the list
                numberOfTrailsLbl.Content = "Trails: " + trailList.Count.ToString();
            }
        }

        //NAME          : SuspendBtn_Click
        //PARAMETERS    : object sender, RoutedEventArgs e
        //RETURNS       : void
        //DESCRIPTION   : This method suspends all threads currently running through a loop.
        private void SuspendBtn_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < trailList.Count; i++)
            {
                threadList[i].Suspend();
            }
        }

        //NAME          : ContinueBtn_Click
        //PARAMETERS    : object sender, RoutedEventArgs e
        //RETURNS       : void
        //DESCRIPTION   : This method resumes all threads if currently paused by running through a loop.
        private void ContinueBtn_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < threadList.Count; i++)
            {
                if (threadList[i].ThreadState == ThreadState.Suspended)
                {
                    threadList[i].Resume();
                }
            }
        }

        //NAME          : ResetBtn_Click
        //PARAMETERS    : object sender, RoutedEventArgs e
        //RETURNS       : void
        //DESCRIPTION   : Lets all threads drop out of loops by disabling the trail then clears both lists.
        private void ResetBtn_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < threadList.Count; i++)
            {
                trailList[i].on = false;
            }
            trailList.Clear();
            threadList.Clear();
            numberOfTrailsLbl.Content = "Trails: " + trailList.Count.ToString();
        }

        //NAME          : PartyBtn_Click
        //PARAMETERS    : None
        //RETURNS       : void
        //DESCRIPTION   : Toggles party mode
        private void RaveBtn_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < threadList.Count; i++)
            {
                trailList[i].ToggleRaveMode();                              //toggle party mode on all trails
            }
        }

        //NAME          : ExpirationSlider_ValueChanged
        //PARAMETERS    : object sender, RoutedPropertyChangedEventArgs<double> e
        //RETURNS       : void
        //DESCRIPTION   : Updates all the trails to have a new expiration value
        private void ExpirationSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.Dispatcher.Invoke(() =>                                     //use dispatcher to avoid conflict
            {
                for (int i = 0; i < threadList.Count; i++)
                {
                    trailList[i].SetExpiration((int)expirationSlider.Value); //Set all trails to have the new expiration value
                }
            });
        }

        //NAME          : SpeedSlider_ValueChanged
        //PARAMETERS    : object sender, RoutedPropertyChangedEventArgs<double> e
        //RETURNS       : None
        //DESCRIPTION   : Updates all the trails to have a new speed value
        private void SpeedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.Dispatcher.Invoke(() =>                                     //use dispatcher to avoid conflict
            {
                for (int i = 0; i < threadList.Count; i++)
                {
                    trailList[i].SetSpeed((int)speedSlider.Value);           //Set all trails to have the new speed value
                }
            });
        }

        //NAME          : ThemeBtn_Click
        //PARAMETERS    : object sender, RoutedEventArgs e
        //RETURNS       : void
        //DESCRIPTION   : Toggles the theme of the canvas and window between light and dark
        private void ThemeBtn_Click(object sender, RoutedEventArgs e)
        {
            themeBtn.IsEnabled = false;                                 //lock the button to prevent shenanigans
            themeBtn.Content = "Wait...";

            if (darkTheme == false)
            {
                Thread fadeThread = new Thread(() => FadeToDark());     //set a delegate to fade the background
                fadeThread.Start();
                main.Background = Brushes.DarkCyan;
                darkTheme = true;                                       //set the bool
            }
            else
            {
                Thread fadeThread = new Thread(() => FadeToLight());    //set a delegate to fade the background
                fadeThread.Start();
                main.Background = Brushes.PowderBlue;
                darkTheme = false;                                      //set the bool
            }
        }

        //NAME          : MainWindow_Closed 
        //PARAMETERS    : object sender, EventArgs e
        //RETURNS       : None
        //DESCRIPTION   : Forces all threads to join to ensure the application is fully killed
        //                if it is closed abruptly.
        void MainWindow_Closed(object sender, EventArgs e)
        {
            for (int i = 0; i < subThreadList.Count; i++)
            {
                subThreadList[i].Join();                    //join all subThreads back to main before closing
            }
            for (int i = 0; i < threadList.Count; i++)
            {
                threadList[i].Join();                       //join all threads back to main before closing
            }
            Environment.Exit(0);
        }

        //NAME          : FadeToBlack
        //PARAMETERS    : None
        //RETURNS       : void
        public void FadeToDark()
        {
            int r = 176;             //starting colour values
            int g = 224;
            int b = 230;

            while (b > 0)
            {                       //slowly fade to specified colours
                if (r <= 0)
                    r++;
                if (g <= 0)
                    g++;
                if (b <= 0)
                    b++;
                this.Dispatcher.Invoke(() =>                            //use dispatcher to avoid conflict
                {
                    trailCanvas.Background = new SolidColorBrush(Color.FromRgb((byte)r, (byte)g, (byte)b));
                });
                r--;
                g--;
                b--;
                Thread.Sleep(10);
            }
            this.Dispatcher.Invoke(() =>                            //use dispatcher to avoid conflict
            {
                themeBtn.IsEnabled = true;                                  //release button
                themeBtn.Content = "Toggle Theme";
            });
        }


        //NAME          : FadeToLight
        //PARAMETERS    : None
        //RETURNS       : void
        public void FadeToLight()
        {
            int r = 0;                  //starting colour values
            int g = 0;
            int b = 0;

            while (b < 255)
            {                           //slowly fade to specified colours
                if (r >= 240)
                    r--;
                if (g >= 248)
                    g--;
                if (b >= 255)
                    b--;
                this.Dispatcher.Invoke(() =>                            //use dispatcher to avoid conflict
                {
                    trailCanvas.Background = new SolidColorBrush(Color.FromRgb((byte)r, (byte)g, (byte)b));
                });
                r++;
                g++;
                b++;
                Thread.Sleep(10);
            }
            this.Dispatcher.Invoke(() =>                            //use dispatcher to avoid conflict
            {
                themeBtn.IsEnabled = true;                                  //release button
                themeBtn.Content = "Toggle Theme";
            });
        }
    }
}
