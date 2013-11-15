using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProBendingBattle
{
    class AIGenetics
    {
        private List<List<AINode>> generations;
        private int sample;
        private int mutation;
        private int iterations;
        private int iteration;
        private int cycle;

        public AIGenetics()
        {
            generations = new List<List<AINode>>();
        }
        public void init(int sample, int iterations, int mutation, AINode seed)
        {
            cycle = 0;
            this.sample = sample;
            this.iterations = iterations;
            this.mutation = mutation;
            generations.Clear();
            generations.Add(new List<AINode>());

            for(int a = 0; a < sample; a++)
                generations.ElementAt<List<AINode>>(0).Add(new AINode(seed, seed, mutation));
        }
        public void init(int sample, int iterations, int mutation)
        {
            cycle = 0;
            this.sample = sample;
            this.iterations = iterations;
            this.mutation = mutation;
            generations.Clear();
            generations.Add(new List<AINode>());

            for (int a = 0; a < sample; a++)
                generations.ElementAt<List<AINode>>(0).Add(new AINode());
        }

        public void newGen()
        {
            iteration = 0;
            cycle = 0;
            AINode p1 = null;
            AINode p2 = null;
            foreach (AINode a in generations.ElementAt<List<AINode>>(generations.Count - 1))
            {
                if (p1 == null)
                {
                    p1 = a;
                }
                else
                {
                    if (a.percent() > p1.percent())
                    {
                        p2 = p1;
                        p1 = a;
                    }
                    else if (p2 == null || a.percent() > p2.percent())
                    {
                        p2 = a;
                    }
                }
            }

            generations.Add(new List<AINode>());

            if ((generations.Count-2) % 50 == 0)
            {
                generations.ElementAt<List<AINode>>(0);
            }


            for(int a = 0; a < sample; a ++)
                generations.ElementAt<List<AINode>>(generations.Count - 1).Add(new AINode(p1, p2, mutation));

        }

        public List<AINode> getCurrentGen()
        {
            return generations.ElementAt<List<AINode>>(generations.Count - 1);
        }

        public List<AINode> getNextMatch()
        {
            if (iteration > iterations)
            {
                newGen();
            }
            int firstPlayer = 0;
            int secondPlayer = 1;
            int tempCycle = cycle;

            firstPlayer = tempCycle / (sample - 1);
            secondPlayer = tempCycle - (firstPlayer * (sample - 1));
            if (secondPlayer >= firstPlayer)
            {
                secondPlayer++;
            }

            cycle++;
            if (cycle >= sample * (sample - 1))
            {
                cycle = 0;
                iteration++;
            }
            List<AINode> returns = new List<AINode>();
            returns.Add(getCurrentGen().ElementAt<AINode>(firstPlayer));
            returns.Add(getCurrentGen().ElementAt<AINode>(secondPlayer));
            return returns;
        }
    }
}
