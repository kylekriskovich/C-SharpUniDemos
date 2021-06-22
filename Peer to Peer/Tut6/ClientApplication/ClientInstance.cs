using DetailClasses;
using Microsoft.Scripting.Interpreter;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApplication
{
    public class ClientInstance
    {
        private String job = null;
        private ClientDetails client;
        private int completedJobs = 0;
        private String solution = "";

        public  ClientInstance(ClientDetails inClient)
        {
            client = inClient;
        }

        public void setJob(String inJob)
        {
            job = inJob;
        }

        public String getJob()
        {
            String outJob = job;
            job = null;
            return outJob;
        }

        public ClientDetails getInstance()
        {
            return client;
        }

        public bool checkForJob()
        {
            if(String.IsNullOrEmpty(job))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public void completed(String inSolution)
        {
            completedJobs++;
            solution = inSolution;
        }

        public int getComplete()
        {
            return completedJobs;
        }

        public String getSolution()
        {
            if (String.IsNullOrEmpty(solution))
            {
                return "waiting for solution";
            }
            else
            {
                String outSol = solution;
                solution = "";
                return outSol;
            }
        }
    }
}
