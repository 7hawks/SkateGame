using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace StarterGame
{
    public class Popup
    {
        private Rectangle rect;
        public string message { get; set; }
        private bool isOpen;

        public Popup(int x, int y, string inputMessage)
        {
            isOpen = false;
            message = inputMessage;
            rect = new Rectangle(x, y, 200, 200);
        }

    }
}
