using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace RyCookieText.Views
{
    public partial class SplashScreen : Window
    {
        public SplashScreen()
        {
            InitializeComponent();
            Loaded += SplashScreen_Loaded;
        }

        private async void SplashScreen_Loaded(object sender, RoutedEventArgs e)
        {
            // Start animations
            var fadeIn = (Storyboard)FindResource("FadeInAnimation");
            var rotate = (Storyboard)FindResource("RotateAnimation");
            var pulse = (Storyboard)FindResource("PulseAnimation");
            
            fadeIn.Begin();
            await Task.Delay(500); // Wait for fade in
            
            rotate.Begin();
            pulse.Begin();
            
            // Auto-close after 2.5 seconds
            await Task.Delay(2500);
            
            // Fade out
            var fadeOut = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(300)
            };
            
            fadeOut.Completed += (s, args) => Close();
            MainBorder.BeginAnimation(OpacityProperty, fadeOut);
        }
    }
}
