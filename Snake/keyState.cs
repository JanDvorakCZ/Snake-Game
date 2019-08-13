using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Snake
{
    class keyState
    {
        private static Hashtable keyHT = new Hashtable();

        public bool keyPressed(Keys key)
        {
            if (keyHT[key] == null)
            {
                return false;
            }
            return (bool) keyHT[key];
        }

        public void changeState(Keys key, bool value)
        {
            keyHT[key] = value;
        }
    }
}
