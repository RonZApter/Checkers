using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers
{
    public abstract class Player
    {
        protected string name;
        protected bool isHuman;

        public Player(string name)
        {
            this.name = name;
            this.isHuman = false;
        }

        public Player() { name = "Human"; }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public bool IsHuman
        {
            get { return isHuman; }
            set { isHuman = value; }
        }
        
    }

}
