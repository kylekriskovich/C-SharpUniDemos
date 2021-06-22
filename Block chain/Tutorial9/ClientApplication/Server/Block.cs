using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace ClientApplication
{
    public class Block
    {
        public uint blockID { get; set; }
        public uint offset { get; set; }
        public int previousHash { get; set; }
        public  string transactions { get; set; }
        public int hash { get; set; }


        public Block()
        {
            blockID = 0;
            transactions = "";
            offset = 0;
            previousHash = 0;
            hash = 0;
        }

        public Block(uint inBlockID, string inTrans, int inPreviousHash, uint inOffset, int inHash)
        {
            blockID = inBlockID;
            transactions = inTrans;
            offset = inOffset;
            previousHash = inPreviousHash;
            hash = inHash;
        }

        public bool Equals(Block inBlock) 
        {
            if((blockID == inBlock.blockID) && 
              (offset == inBlock.offset) && (previousHash == inBlock.previousHash) && (hash == inBlock.hash))
            {
                return true;
            }
            return false;
        }
    }
}