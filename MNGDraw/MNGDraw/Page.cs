using System;
using System.Collections.Generic;
using System.Text;

namespace MNGDraw
{
    public class Page
    {
        private const int DefaultLayerGenerationNumber = 3;

        public Color BackgourndColor { get; set; } = PaintColors.ArtBoardBackground;

        public List<Layer> LayerList { get; set; } = new List<Layer>();


        public Page()
        {
            //最初は、3つの新規レイヤーを追加
            for (int i = 0; i < DefaultLayerGenerationNumber; i++)
            {
                this.LayerList.Add(new Layer());
            }

        }
    }
}
