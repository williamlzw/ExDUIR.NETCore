using ExDuiR.NET.Frameworks.Controls;
using ExDuiR.NET.Frameworks.Utility;
using ExDuiR.NET.Frameworks.Graphics;
using ExDuiR.NET.Frameworks;
using ExDuiR.NET.Native;
using static ExDuiR.NET.Native.ExConst;
using System;

namespace ExDuiRTest
{
    static class CarouselWindow
    {
        static private ExSkin skin;
        static private ExCarousel carousel;

        static public void CreateCarouselWindow(ExSkin pOwner)
        {
            skin = new ExSkin(pOwner, null, "测试轮播", 0, 0, 800, 600,
            EWS_NOINHERITBKG | EWS_BUTTON_CLOSE | EWS_BUTTON_MIN | EWS_MOVEABLE |
            EWS_CENTERWINDOW | EWS_TITLE | EWS_HASICON | EWS_NOSHADOW);
            if (skin.Validate)
            {
                skin.BackgroundColor = Util.ExRGBA(150, 150, 150, 255);
                carousel = new ExCarousel(skin, "", 20, 40, 760, 550);
                carousel.SetSize(500, 500);
                var carousel1 = File.ReadAllBytes("Resources/carousel1.jpeg");
                var carousel2 = File.ReadAllBytes("Resources/carousel2.jpeg");
                var carousel3 = File.ReadAllBytes("Resources/carousel3.jpeg");
                carousel.AddImage(new ExImage(carousel1, carousel1.Length));
                carousel.AddImage(new ExImage(carousel2, carousel2.Length));
                carousel.AddImage(new ExImage(carousel3, carousel3.Length));

                carousel.Timer = 3000;
                skin.Visible = true;
            }
        }
    }
}
