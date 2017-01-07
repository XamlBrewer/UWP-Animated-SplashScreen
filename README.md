# UWP-Animated-SplashScreen

##Sample
![Splash Screen Animation] (https://github.com/XamlBrewer/UWP-Animated-SplashScreen/blob/master/AnimatedSplashScreen.gif)

##NuGet
To install SplashScreen animation for UWP, run the following command in the Package Manager Console

PM> `Install-Package XamlBrewer.Uwp.AnimatedSplashScreen`

##Usage
Add the following statement on top of your _App.xaml.cs_ file:
```cs
using XamlBrewer.Uwp.Controls;
```

Add the call to _OpenFromSplashScreen()_ in the _OnLaunched_ event handler:

```cs
if (rootFrame.Content == null)
{
  rootFrame.Navigate(typeof(Shell), e.Arguments);
  (rootFrame.Content as Page).OpenFromSplashScreen(e.SplashScreen.ImageLocation);  
}                
```

Use one of the overloads to provide a specific image, and/or background color.

##Credits
The original code was written by the Composition Team as part of the [Windows UI Dev Labs](https://github.com/Microsoft/WindowsUIDevLabs). I just simplified it, made it a bit more generic and reusable, and packaged it.
