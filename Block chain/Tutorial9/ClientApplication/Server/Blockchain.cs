using DetailClasses;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClientApplication
{
    public class Blockchain
    {
        private List<Block> Chain;

        public Blockchain()
        {
            if (Chain == null)
            {
                CreateChain();
            }

        }

        public void CreateChain()
        {
            Chain = new List<Block>();
            addIntialBlock();
        }

        public void addIntialBlock()
        {
            Block firstBlock = new Block(0, "", 0, 0, 0);
            Chain.Add(firstBlock);
        }

        public void AddToChain(Block inBlock)
        {
            Chain.Add(inBlock);
        }

        public Block GetRecentBlock()
        {
            return Chain.Last();
        }

        public List<Block> getBlockchain()
        {
            return Chain;
        }

        public int Count()
        {
            return Chain.Count() - 1;
        }

        public void ReplaceChain(List<Block> inChain)
        {
            if(inChain.Count()> Chain.Count())
            {
                Chain.Clear();
                foreach (Block b in inChain)
                {
                    Chain.Add(b);
                }
            }
            
            
        } 

    }
}