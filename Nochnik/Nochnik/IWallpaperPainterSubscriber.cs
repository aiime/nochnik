using System;
using System.Collections.Generic;
using System.Drawing;

namespace Nochnik
{
    interface IWallpaperPainterSubscriber
    {
        List<WallpaperPart> GetWallpaperParts();
    }

    struct WallpaperPart
    {
        public Image image;
        public int x;
        public int y;
        public int width;
        public int height;

        public WallpaperPart(Image part, int x, int y, int width, int height)
        {
            this.image = part;
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        } 
    }
}
