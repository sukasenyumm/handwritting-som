using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tugas
{
    public class GameObject
    {
        Texture2D texture;
        public Vector2 Position;
        public Vector2 ValPosition
        {
            get
            {
                return Position;
            }
            set
            {
                Position = value;
            }
        }
        public Vector2 Velocity;
        public Color color;

        public Rectangle BoundingBox
        {
            get
            {
                return new Rectangle(
                    (int)Position.X,
                    (int)Position.Y,
                    texture.Width,
                    texture.Height);
            }
        }

        public GameObject(Texture2D texture, Vector2 position)
        {
            this.texture = texture;
            this.Position = position;
        }

        public GameObject(Texture2D texture, Vector2 position, Color col)
        {
            this.texture = texture;
            this.Position = position;
            this.color = col;
        }

        public void Draw(SpriteBatch spriteBatch, float scale)
        {
            spriteBatch.Draw(texture, Position, null, Color.White, 0, new Vector2(0, 0), scale, SpriteEffects.None, 0);
        }

        public Texture2D getTexture()
        {
            if (this.texture != null)
                return this.texture;
            else
                return null;
        }
    }
}
