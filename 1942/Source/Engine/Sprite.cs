using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _1942
{
    class Sprite
    {
        public Texture2D texture;
        public Vector2 position;

        public Sprite()
        {
            texture = null;
            position = new Vector2 (0, 0);
        }

        public Sprite(Texture2D tex, Vector2 pos)
        {
            texture = tex;
            position = pos;
        }

        public void Draw(SpriteBatch SB)
        {
            SB.Begin();
            SB.Draw(texture, position, Color.White);
            SB.End();
        }
    }
}
