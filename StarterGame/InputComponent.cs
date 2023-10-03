using Microsoft.Xna.Framework.Input;

namespace StarterGame
{
    public class InputComponent
    {
        public Direction direction; // the current direction based on player input
        public Direction prevDirection;
        public Keys prevKeyX;
        public Keys prevKeyY;

        public Direction xDirection;
        public Direction yDirection;


        public bool rightMouseButton = false;
        public bool rightMouseRelease = false;

        public InputComponent()
        {
            direction = Direction.None;
        }

        public void Update(Player player)
        {
            UpdateMouse();

            if (player.state == State.Jumping) // disable changing directions mid-air
            {
                return;                         
            }
            if (direction != Direction.None && direction != prevDirection)
            {
                prevDirection = direction;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                xDirection = Direction.Left;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                xDirection = Direction.Right;
            }
            else
                xDirection = Direction.None;

            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                yDirection = Direction.Up;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                yDirection = Direction.Down;
            }
            else 
                yDirection = Direction.None;


            switch (Keyboard.GetState().GetPressedKeys().Length)
            {
                case 1:
                    OneButtonPress();
                    break;
                case 2:
                    TwoButtonPress();
                    break;
                case 3:
                    ThreeButtonPress();
                    break;
                default:
/*                    if (player.direction != Direction.None)
                    {
                        prevDirection = player.direction;
                    }*/

                    direction = Direction.None;
                    break;
            }
        }

        public void UpdateMouse()
        {
            if(Mouse.GetState().RightButton == ButtonState.Pressed)
            {
                if (!rightMouseButton)
                {
                    rightMouseButton = true;
                }
            }
            else if (Mouse.GetState().RightButton != ButtonState.Pressed)
            {
                if(rightMouseButton)
                {
                    rightMouseButton = false;
                    rightMouseRelease = true;
                    return;
                }
            }
            rightMouseRelease = false;
        }


        private void OneButtonPress()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                prevKeyX = Keys.A;
                if (direction != Direction.None && direction != Direction.Left)
                {
                    prevDirection = direction;
                }
                direction = Direction.Left;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                prevKeyY = Keys.W;
                if (direction != Direction.None && direction != Direction.Up)
                {
                    prevDirection = direction;
                }
                direction = Direction.Up;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                prevKeyY = Keys.S;
                if (direction != Direction.None && direction != Direction.Down)
                {
                    prevDirection = direction;
                }
                direction = Direction.Down;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                prevKeyX = Keys.D;
                if (direction != Direction.None && direction != Direction.Right)
                {
                    prevDirection = direction;
                }
                direction = Direction.Right;
            }
        }

        private void TwoButtonPress()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.D) && Keyboard.GetState().IsKeyDown(Keys.A))
            {
                if (prevKeyX == Keys.D)
                {
                    direction = Direction.Right;
                    
                }
                else if (prevKeyX == Keys.A)
                {
                    direction = Direction.Left;
                }
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.W) && Keyboard.GetState().IsKeyDown(Keys.S))
            {
                if (prevKeyY == Keys.W)
                {
                    direction = Direction.Up;
                }
                else if (prevKeyY == Keys.S)
                {
                    direction = Direction.Down;
                }
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D) && Keyboard.GetState().IsKeyDown(Keys.S))
            {
                direction = Direction.DownRight;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.A) && Keyboard.GetState().IsKeyDown(Keys.S))
            {
                direction = Direction.DownLeft;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.A) && Keyboard.GetState().IsKeyDown(Keys.W))
            {
                direction = Direction.UpLeft;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D) && Keyboard.GetState().IsKeyDown(Keys.W))
            {
                direction = Direction.UpRight;
            }
        }

        private void ThreeButtonPress()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.A) && Keyboard.GetState().IsKeyDown(Keys.W) && Keyboard.GetState().IsKeyDown(Keys.D))
            {
                if (prevKeyX == Keys.A)
                {
                    direction = Direction.UpLeft;

                }
                else
                    direction = Direction.UpRight;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.W) && Keyboard.GetState().IsKeyDown(Keys.D) && Keyboard.GetState().IsKeyDown(Keys.S))
            {
                if (prevKeyY == Keys.W)
                {
                    direction = Direction.UpRight;
                }
                else
                    direction = Direction.DownRight;

            }
            else if (Keyboard.GetState().IsKeyDown(Keys.S) && Keyboard.GetState().IsKeyDown(Keys.A) && Keyboard.GetState().IsKeyDown(Keys.W))
            {
                if (prevKeyY == Keys.W)
                {
                    direction = Direction.UpLeft;
                }
                else
                    direction = Direction.DownLeft;

            }
            else if (Keyboard.GetState().IsKeyDown(Keys.A) && Keyboard.GetState().IsKeyDown(Keys.S) && Keyboard.GetState().IsKeyDown(Keys.D))
            {
                if (prevKeyX == Keys.A)
                {
                    direction = Direction.DownLeft;
                }
                else
                    direction = Direction.DownRight;
            }
        }
    }
}
