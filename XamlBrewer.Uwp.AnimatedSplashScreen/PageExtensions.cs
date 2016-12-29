using System;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;

namespace XamlBrewer.Uwp.Controls
{
    /// <summary>
    /// Class PageExtensions.
    /// </summary>
    public static class PageExtensions
    {
        /// <summary>
        /// The current page
        /// </summary>
        private static Page currentPage;

        /// <summary>
        /// Opens from splash screen.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="imageBounds">The image bounds.</param>
        public static void OpenFromSplashScreen(this Page page, Rect imageBounds)
        {
            page.OpenFromSplashScreen(imageBounds, Colors.Transparent, new Uri("ms-appx:///Assets/SplashScreen.png"));
        }

        /// <summary>
        /// Opens from splash screen.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="imageBounds">The image bounds.</param>
        /// <param name="backgroundColor">Color of the background.</param>
        public static void OpenFromSplashScreen(this Page page, Rect imageBounds, Color backgroundColor)
        {
            page.OpenFromSplashScreen(imageBounds, backgroundColor, new Uri("ms-appx:///Assets/SplashScreen.png"));
        }

        /// <summary>
        /// Opens from splash screen.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="imageBounds">The image bounds.</param>
        /// <param name="imageUri">The URI of the image.</param>
        public static void OpenFromSplashScreen(this Page page, Rect imageBounds, Uri imageUri)
        {
            page.OpenFromSplashScreen(imageBounds, Colors.Transparent, imageUri);
        }

        /// <summary>
        /// Opens from splash screen.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="imageBounds">The image bounds.</param>
        /// <param name="backgroundColor">Color of the background.</param>
        /// <param name="imageUri">The URI of the image.</param>
        public static void OpenFromSplashScreen(this Page page, Rect imageBounds, Color backgroundColor, Uri imageUri)
        {
            page.Loaded += Page_Loaded;

            // Initialize the surface loader
            SurfaceLoader.Initialize(ElementCompositionPreview.GetElementVisual(page).Compositor);

            // Show the custome splash screen
            ShowImage(page, imageBounds, imageUri, backgroundColor);
        }

        /// <summary>
        /// Called when the page is loaded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Windows.UI.Xaml.RoutedEventArgs" /> instance containing the event data.</param>
        private static void Page_Loaded(object sender, RoutedEventArgs e)
        {
            (sender as Page).Loaded -= Page_Loaded;

            // Now that loading is complete, dismiss the custom splash screen
            ShowContent(sender as Page);
        }

        /// <summary>
        /// Shows the image.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="imageBounds">The image bounds.</param>
        /// <param name="imageUri">The image URI.</param>
        /// <param name="backgroundColor">Color of the background.</param>
        private static async void ShowImage(Page page, Rect imageBounds, Uri imageUri, Color backgroundColor)
        {
            var compositor = ElementCompositionPreview.GetElementVisual(page).Compositor;
            var windowSize = new Vector2((float)Window.Current.Bounds.Width, (float)Window.Current.Bounds.Height);

            //
            // Create a container visual to hold the color fill background and image visuals.
            // Configure this visual to scale from the center.
            //
            var container = compositor.CreateContainerVisual();
            container.Size = windowSize;
            container.CenterPoint = new Vector3(windowSize.X, windowSize.Y, 0) * .5f;
            ElementCompositionPreview.SetElementChildVisual(page, container);

            //
            // Create the colorfill sprite for the background, set the color to the same as app theme
            //
            var backgroundSprite = compositor.CreateSpriteVisual();
            backgroundSprite.Size = windowSize;
            backgroundSprite.Brush = compositor.CreateColorBrush(backgroundColor);
            container.Children.InsertAtBottom(backgroundSprite);

            //
            // Create the image sprite containing the splash screen image.  Size and position this to
            // exactly cover the Splash screen image so it will be a seamless transition between the two
            //
            var surface = await SurfaceLoader.LoadFromUri(imageUri);
            var imageSprite = compositor.CreateSpriteVisual();
            imageSprite.Brush = compositor.CreateSurfaceBrush(surface);
            imageSprite.Offset = new Vector3((float)imageBounds.X, (float)imageBounds.Y, 0f);
            imageSprite.Size = new Vector2((float)imageBounds.Width, (float)imageBounds.Height);
            container.Children.InsertAtTop(imageSprite);
        }

        /// <summary>
        /// Shows the content.
        /// </summary>
        /// <param name="page">The page.</param>
        private static void ShowContent(Page page)
        {
            var container = (ContainerVisual)ElementCompositionPreview.GetElementChildVisual(page);
            var compositor = container.Compositor;

            // Setup some constants for scaling and animating
            const float scaleFactor = 7.5f;
            var duration = TimeSpan.FromMilliseconds(2000);

            // Create the fade animation which will target the opacity of the outgoing splash screen
            var fadeOutAnimation = compositor.CreateScalarKeyFrameAnimation();
            fadeOutAnimation.InsertKeyFrame(1, 0);
            fadeOutAnimation.Duration = duration;

            // Optional: zoom in content.
            // Create the scale up animation for the grid
            //var scaleUpGridAnimation = compositor.CreateVector2KeyFrameAnimation();
            //scaleUpGridAnimation.InsertKeyFrame(0.1f, new Vector2(1 / scaleFactor, 1 / scaleFactor));
            //scaleUpGridAnimation.InsertKeyFrame(1, new Vector2(1, 1));
            //scaleUpGridAnimation.Duration = duration;

            // Create the scale up animation for the Splash screen visuals
            var scaleUpSplashAnimation = compositor.CreateVector2KeyFrameAnimation();
            scaleUpSplashAnimation.InsertKeyFrame(0, new Vector2(1, 1));
            scaleUpSplashAnimation.InsertKeyFrame(1, new Vector2(scaleFactor, scaleFactor));
            scaleUpSplashAnimation.Duration = duration;

            // Configure the visual to scale from the center
            var frameworkElement = page.Content as FrameworkElement;
            var visual = ElementCompositionPreview.GetElementVisual(frameworkElement);
            visual.Size = new Vector2((float)frameworkElement.ActualWidth, (float)frameworkElement.ActualHeight);
            visual.CenterPoint = new Vector3(visual.Size.X, visual.Size.Y, 0) * .5f;

            //
            // Create a scoped batch for the animations.  When the batch completes, we can dispose of the
            // splash screen visuals which will no longer be visible.
            //
            var batch = compositor.CreateScopedBatch(CompositionBatchTypes.Animation);

            container.StartAnimation("Opacity", fadeOutAnimation);
            container.StartAnimation("Scale.XY", scaleUpSplashAnimation);
            // visual.StartAnimation("Scale.XY", scaleUpGridAnimation);

            currentPage = page; // TODO: find a better way to pass the page to the event.
            batch.Completed += Batch_Completed;
            batch.End();
        }

        /// <summary>
        /// Called when all animations in the scoped batch are completed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="Windows.UI.Composition.CompositionBatchCompletedEventArgs" /> instance containing the event data.</param>
        private static void Batch_Completed(object sender, CompositionBatchCompletedEventArgs args)
        {
            // Now that the animations are complete, dispose of the custom Splash Screen visuals
            ElementCompositionPreview.SetElementChildVisual(currentPage, null);
        }
    }
}
