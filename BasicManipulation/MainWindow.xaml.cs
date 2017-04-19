using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace BasicManipulation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        void Window_ManipulationStarting(object sender, ManipulationStartingEventArgs manipulation)
        {
            manipulation.ManipulationContainer = this;
            manipulation.Handled = true;
        }

        void Window_ManipulationDelta(object sender, ManipulationDeltaEventArgs manipulation)
        {
            //Get the Rectangle and its RenderTransform matrix.
            Rectangle rectToMove = manipulation.OriginalSource as Rectangle;
            Matrix rectsMatrix = ((MatrixTransform)rectToMove.RenderTransform).Matrix;

            //Rotate the Rectangle.
            rectsMatrix.RotateAt(manipulation.DeltaManipulation.Rotation,
                                 manipulation.ManipulationOrigin.X,
                                 manipulation.ManipulationOrigin.Y);

            //Resize the Rectangle. Keep it square
            //so use only the X value of Scale.
            rectsMatrix.ScaleAt(manipulation.DeltaManipulation.Scale.X,
                                manipulation.DeltaManipulation.Scale.X,
                                manipulation.ManipulationOrigin.X,
                                manipulation.ManipulationOrigin.Y);

            //Move the Rectangle
            rectsMatrix.Translate(manipulation.DeltaManipulation.Translation.X,
                                  manipulation.DeltaManipulation.Translation.Y);

            //Apply the changes to the Rectangle.
            rectToMove.RenderTransform = new MatrixTransform(rectsMatrix);

            Rect containRect =
                new Rect(((FrameworkElement)manipulation.ManipulationContainer).RenderSize);

            Rect shapeBounds =
                rectToMove.RenderTransform.TransformBounds(
                    new Rect(rectToMove.RenderSize));

            //Check if the rectangle is completely in the window.
            //If it is not and inertia is occuring, stop the manipulation
            if (manipulation.IsInertial && !containRect.Contains(shapeBounds))
            {
                manipulation.Complete();
            }

            manipulation.Handled = true;
        }

        //Occurs when the user raises all fingers from the screen.
        void Window_InertiaStarting(object sender, ManipulationInertiaStartingEventArgs manipulation)
        {
            //Derease the velocity of the Rectangle's movement by 10 inches
            //per secind every second.
            //(10 inches * 96 pixels per inch / 1000ms^2)
            manipulation.TranslationBehavior.DesiredDeceleration = 10.0 * 96.0 / (1000.0 * 1000.0);

            //Decrease the velocity of the Rectangle's resizing by
            //0.1 inches per second every second.
            //(0.1 inches * 96 pixels per inch / (1000ms^2)
            manipulation.ExpansionBehavior.DesiredDeceleration = 0.1 * 96 / (1000.0 * 1000.0);

            //Decrease the velocity of the Rectangle's rotation rate by
            //2 rotations per second every second.
            //(2 * 360 degrees / (1000ms^2)
            manipulation.RotationBehavior.DesiredDeceleration = 720 / (1000.0 * 1000.0);

            manipulation.Handled = true;
        }
    }
}
