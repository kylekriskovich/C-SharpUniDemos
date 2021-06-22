using DetailClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using RestSharp;
using System.IO;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace ClientApplication.Mining
{
    public class Miner
    {
        /*
        * 	Purpose: Creates a valid block for the blockchain for the 
        *	Input: an array of base 64 strings representing the python code and result
        *	Output: a valid block
        */
        public static Block MineBlock(string[] inTrans)
        {
            Blockchain blockchain = ClientInstance.GetBlockchain();
            Block block = new Block(); 
            try
            {
                block.transactions = JsonConvert.SerializeObject(inTrans);

                Block currentLast = blockchain.GetRecentBlock();
                block.previousHash = currentLast.hash;
                block.blockID = currentLast.blockID + 1;

                bool foundHash = false;
                while (foundHash == false)
                {
                    block.offset = block.offset + 1;
                    block.hash = ComputeHash(block);
                    if (block.hash.ToString().Length > 5)
                    {
                        if (block.hash.ToString().Substring(0, 5).Equals("12345")) //testing for valid hash
                        {
                            foundHash = true;

                        }
                    }
                }

            }
            catch(InvalidDataException e)
            {
                Console.WriteLine("Miner.MineBlock: " + e.Message);
            }
            catch(NullReferenceException e)
            {
                Console.WriteLine("Miner.MineBlock: " + e.Message);
            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.WriteLine("Miner.MineBlock: " + e.Message);
            }

            return block;

        }

        /*
        * 	Purpose: Creates a valid hash
        *	Input: the block that the hash will be stored in
        *	Output: interger representation of the hash
        */
        private static int ComputeHash(Block block)
        {
            int integerRep = 0;
            try
            {
                SHA256 sha256 = SHA256.Create();

                String data = block.blockID.ToString() + block.transactions + block.offset.ToString() + block.previousHash.ToString();

                byte[] inputBytes = Encoding.ASCII.GetBytes(data);

                byte[] outputBytes = sha256.ComputeHash(inputBytes);

                integerRep = BitConverter.ToInt32(outputBytes, 0);

                return integerRep;
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("Miner.ComputeHash: " + e.Message);
            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.WriteLine("Miner.ComputeHash: " + e.Message);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine("Miner.ComputeHash: " + e.Message);
            }
            catch (System.Reflection.TargetInvocationException e)
            {
                Console.WriteLine("Miner.ComputeHash: " + e.Message);
            }
            return integerRep;
        }

        /*
        * 	Purpose: Finds the most frequent interger in a list
        *	Input: list of ints
        *	Output: the most frequent int 
        */
        public static int mostFrequent(List<int> list)
        {
            try
            {
                list.Sort();
                int n = list.Count();

                // find the max frequency using  
                // linear traversal 
                int max_count = 1, result = list.ElementAt(0);
                int curr_count = 1;

                for (int i = 1; i < list.Count(); i++)
                {
                    if (list.ElementAt(i) == list.ElementAt(i - 1))
                        curr_count++;
                    else
                    {
                        if (curr_count > max_count)
                        {
                            max_count = curr_count;
                            result = list.ElementAt(i - 1);
                        }
                        curr_count = 1;
                    }
                }

                // If last element is most frequent 
                if (curr_count > max_count)
                {
                    max_count = curr_count;
                    result = list.ElementAt(n - 1);
                }

                return result;
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("Miner.MostFrequent: " + e.Message);
                return 0;
            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.WriteLine("Miner.MostFrequent: " + e.Message);
                return 0;
            }
            catch (OverflowException e)
            {
                Console.WriteLine("Miner.MostFrequent: " + e.Message);
                return 0;
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine("Miner.MostFrequent: " + e.Message);
                return 0;
            }
        }
    }
}