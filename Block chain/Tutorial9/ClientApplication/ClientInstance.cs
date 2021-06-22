using DetailClasses;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;

namespace ClientApplication
{
    public static class ClientInstance
    {
        private static Blockchain the_chain;
        private static ClientDetails client;
        public static List<string> transactions;
        public static List<string> solved;
        public static List<Transaction> completedJobs;


        public static ClientDetails GetClient()
        {
            return client;
        }

        public static void SetClient(ClientDetails inClient)
        {
            client = inClient;
        }

        public static void SetBlockchain(Blockchain inChain)
        {
            if(the_chain == null)
            {
                the_chain = inChain;
            }
            else
            {
                the_chain.ReplaceChain(inChain.getBlockchain());
            }
        }
        public static void SetTransactions(List<string> inList)
        {
            transactions = inList;
        }

        public static void SetSolved(List<string> inList)
        {
            solved = inList;
        }

        public static void addTransaction(String inTrans) //add a transaction to the transaction list
        {
            bool found = false;
            foreach (string t in transactions)
            {
                if ((t.Equals(inTrans)))
                {
                    found = true;
                    solved.Add(t);
                }
            }
            if(!found)
            {
                transactions.Add(inTrans);
            } 
            
        }

        public static bool Solved(string inTrans)
        {
            bool found = false;
            if(the_chain.Count() == solved.Count())
            {
                    return true;
            }
            return found;
        }

        public static List<string> GetTransaction()
        {
            return transactions;
        }

        public static Blockchain GetBlockchain()
        {
            return the_chain;
        }

        public static Block GetRecentBlock()
        {
           return the_chain.GetRecentBlock();
        }

        public static void SetComplete(List<Transaction> inCompleted)
        {
            if(completedJobs == null)
            {
                completedJobs = inCompleted;
            }else
            {
                completedJobs.Clear();
                //Block firstBlock = new Block(0, 0, 0, 0, 0, 0, 0);
                foreach (Transaction t in inCompleted)
                {
                    completedJobs.Add(t);
                }
            }
        }
    }
}