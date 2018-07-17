using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormMono
{
    class OverworldUndo
    {
        public int index = 0;
        public ushort[,] tiles;
        public OverworldUndo(int index, ushort[,] tiles) //tiles
        {
            this.index = index;
            this.tiles = tiles;
        }
        public OverworldUndo(bool a)
        {

        }
        public OverworldUndo(bool a, bool b)
        {

        }

    }
}
