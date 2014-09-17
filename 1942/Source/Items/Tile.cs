using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _1942
{
    public class Tile
    {

        public Vector2 position = Vector2.Zero;
        protected Texture2D texture = null;

        public Tile()
        {
        }

        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        public void Draw(SpriteBatch SB)
        {
            SB.Draw(texture, position, Color.White);
        }
    }
}
