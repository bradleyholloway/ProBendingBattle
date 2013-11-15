using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProBendingBattle
{
    class AINode
    {
        private static double max = 250;
        private double total;
        private static Random randomSeed = new Random();

        private int generation;

        private int wins;
        private int losses;

        private bool block;
        private double initVelocity;
        private double dodgeAcceleration;
        private double dodgeVelocity;
        private double randomAcceleration;
        private double randomVelocity;
        private double reactionDistance;
        private double avoidAcceleration;
        private double avoidDistance;
        private double avoidVelocity;
        private double cornerDistance;
        private double cornerAcceleration;
        private double cornerVelocity;
        private double blockRadius;
        private int blockNodes;
        private int blockFrequency;
        private int firstType;
        private int secondType;
        private int thirdType;
        private int strategy;

        public AINode()
        {
            Random r = new Random(randomSeed.Next(0,100000));
            initVelocity = r.NextDouble();
            dodgeAcceleration = r.NextDouble();
            dodgeVelocity = Math.Min(1.5 - dodgeAcceleration, 1);
            randomAcceleration = r.NextDouble();
            randomVelocity = Math.Min(1.5 - randomAcceleration, 1);
            avoidAcceleration = r.NextDouble();
            avoidVelocity = Math.Min(1.5 - avoidAcceleration, 1);
            cornerAcceleration = r.NextDouble();
            cornerVelocity = Math.Min(1.5 - cornerAcceleration, 1);
            reactionDistance = r.NextDouble() * 100;
            avoidDistance = r.NextDouble() * 100;
            cornerDistance = r.NextDouble() * 100;
            firstType = r.Next(4);
            secondType = r.Next(4);
            thirdType = r.Next(4);
            generation = 0;
            strategy = r.Next(0,5);
            blockFrequency = r.Next(0, 100);
            block = r.Next(0, 2) == 1;
            blockRadius = r.Next(60, 200);
            blockNodes = r.Next(0, 5);
        }
        public AINode(double initV, double dodgeAccel, double randomAccel, double reactionD, double avoidD, double avoidA, double cornerD, double cornerA, int strategy, int fType, int sType, int tType, bool block, int blockFrequency, double blockRadius, int blockNodes)
        {
            initVelocity = initV;
            dodgeAcceleration = dodgeAccel;
            randomAcceleration = randomAccel;
            dodgeVelocity = Math.Min(1.5 - dodgeAcceleration,1);
            randomVelocity = Math.Min(1.5 - randomAcceleration,1);
            reactionDistance = reactionD;
            avoidDistance = avoidD;
            avoidAcceleration = avoidA;
            avoidVelocity = Math.Min(1.5 - avoidAcceleration, 1);
            cornerDistance = cornerD;
            cornerAcceleration = cornerA;
            cornerVelocity = Math.Min(1.5 - cornerAcceleration, 1);
            this.strategy = strategy;
            firstType = fType;
            secondType = sType;
            thirdType = tType;
            this.block = block;
            this.blockFrequency = blockFrequency;
            this.blockRadius = blockRadius;
            this.blockNodes = blockNodes;
        }
        public AINode(AINode p1, AINode p2, int mutation)
        {
            this.generation = p1.generation + 1;
            Random r = new Random(randomSeed.Next(100000));
            this.initVelocity = (r.Next(2) == 0) ? p1.initVelocity : p2.initVelocity;
            this.dodgeAcceleration = (r.Next(2) == 0) ? p1.dodgeAcceleration : p2.dodgeAcceleration;
            this.randomAcceleration = (r.Next(2) == 0) ? p1.randomAcceleration : p2.randomAcceleration;
            this.reactionDistance = (r.Next(2) == 0) ? p1.reactionDistance : p2.reactionDistance;
            this.strategy = (r.Next(2) == 0) ? p1.strategy : p2.strategy;
            this.avoidDistance = (r.Next(2) == 0) ? p1.avoidDistance : p2.avoidDistance;
            this.avoidAcceleration = (r.Next(2) == 0) ? p1.avoidAcceleration : p2.avoidAcceleration;
            this.cornerDistance = (r.Next(2) == 0) ? p1.cornerDistance : p2.cornerDistance;
            this.cornerAcceleration = (r.Next(2) == 0) ? p1.cornerAcceleration : p2.cornerAcceleration;
            this.firstType = (r.Next(2) == 0) ? p1.firstType : p2.firstType;
            this.secondType = (r.Next(2) == 0) ? p1.secondType : p2.secondType;
            this.thirdType = (r.Next(2) == 0) ? p1.thirdType : p2.thirdType;
            this.block = (r.Next(2) == 0) ? p1.block : p2.block;
            this.blockFrequency = (r.Next(2) == 0) ? p1.blockFrequency : p2.blockFrequency;
            this.blockRadius = (r.Next(2) == 0) ? p1.blockRadius : p2.blockRadius;
            this.blockNodes = (r.Next(2) == 0) ? p1.blockNodes : p2.blockNodes;
            //if (r.Next(mutation) == 0)
            //{
                int attribute = r.Next(0, 16);
                if (attribute == 0)
                {
                    initVelocity = Math.Abs(initVelocity + r.NextDouble() * 2 - 1);
                }
                if (attribute == 1)
                {
                    dodgeAcceleration = Math.Abs(dodgeAcceleration + r.NextDouble() * 2 - 1);
                }
                if (attribute == 2)
                {
                    randomAcceleration = Math.Abs(randomAcceleration + r.NextDouble() * 2 - 1);
                }
                if (attribute == 3)
                {
                    reactionDistance = Math.Abs(reactionDistance + (r.NextDouble() * 50 - 25));
                }
                if (attribute == 4)
                {
                    strategy = Math.Abs(strategy + r.Next(-4, 5));
                }
                if (attribute == 5)
                {
                    avoidDistance = Math.Abs(avoidDistance + (r.NextDouble() * 50 - 25));
                }
                if (attribute == 6)
                {
                    avoidAcceleration = Math.Abs(avoidAcceleration + r.NextDouble() * 2 - 1);
                }
                if (attribute == 7)
                {
                    cornerDistance = Math.Abs(cornerDistance + (r.NextDouble() * 50 - 25));
                }
                if (attribute == 8)
                {
                    cornerAcceleration = Math.Abs(cornerAcceleration + r.NextDouble() * 2 - 1);
                }
                if (attribute == 9)
                {
                    firstType = Math.Abs(firstType + r.Next(4) - 2);
                }
                if (attribute == 10)
                {
                    secondType = Math.Abs(secondType + r.Next(4) - 2);
                }
                if (attribute == 11)
                {
                    thirdType = Math.Abs(thirdType + r.Next(4) - 2);
                }
                if (attribute == 12)
                {
                    block = !block;
                }
                if (attribute == 13)
                {
                    blockFrequency = Math.Abs(blockFrequency + r.Next(-10, 10));
                }
                if (attribute == 14)
                {
                    blockRadius = 50 + Math.Abs(blockRadius - 50 + r.Next(-10, 10));
                }
                if (attribute == 15)
                {
                    blockNodes = 1 + Math.Abs(blockNodes - 1 + r.Next(-3,3));
                }
            //}

            if (dodgeAcceleration > 1)
                dodgeAcceleration = 1;
            if (randomAcceleration > 1)
                randomAcceleration = 1;
            if (avoidAcceleration > 1)
                avoidAcceleration = 1;
            if (cornerAcceleration > 1)
                cornerAcceleration = 1;
            if (blockFrequency < 10)
                blockFrequency = 10;
            if (blockNodes > 10)
                blockNodes = 10;

            dodgeVelocity = Math.Min(1.5 - dodgeAcceleration, 1);
            randomVelocity = Math.Min(1.5 - randomAcceleration, 1);
            avoidVelocity = Math.Min(1.5 - avoidAcceleration, 1);
            cornerVelocity = Math.Min(1.5 - cornerAcceleration, 1);

            if (initVelocity > 1)
                initVelocity = 1;

            if (strategy < 0)
                strategy = 0;
            if (strategy > 4)
                strategy = 4;
            if (firstType > 3)
                firstType = 3;
            if (secondType > 3)
                secondType = 3;
            if (thirdType > 3)
                thirdType = 3;

            total = avoidAcceleration + avoidDistance + avoidVelocity + cornerAcceleration + cornerDistance + cornerVelocity + dodgeAcceleration + dodgeVelocity + reactionDistance + initVelocity + randomAcceleration + randomVelocity;
            double diff = max - total;
            if (diff < 0)
            {
                double percent = -diff / total;
                percent = 1 - percent;

                avoidAcceleration *= percent;
                avoidDistance *= percent;
                avoidVelocity *= percent;
                cornerAcceleration *= percent;
                cornerDistance *= percent;
                cornerVelocity *= percent;
                dodgeAcceleration *= percent;
                dodgeVelocity *= percent;
                reactionDistance *= percent;
                initVelocity *= percent;
                randomAcceleration *= percent;
                randomVelocity *= percent;
            }
           
        }
        public void update(bool win)
        {
            if (win)
                wins += 1;
            else
                losses += 1;
        }
        public double percent()
        {
            return ((double)wins) / (wins + losses);
        }
        public int getStrategy()
        {
            return strategy;
        }
        public double getDodgeVelocity()
        {
            return dodgeVelocity;
        }
        public double getDodgeAcceleration()
        {
            return dodgeAcceleration;
        }
        public double getRandomVelocity()
        {
            return randomVelocity;
        }
        public double getRandomAcceleration()
        {
            return randomAcceleration;
        }
        public double getReactionDistance()
        {
            return reactionDistance;
        }
        public double getInitVelocity()
        {
            return initVelocity;
        }
        public double getAvoidAcceleration()
        {
            return avoidAcceleration;
        }
        public double getAvoidDistance()
        {
            return avoidDistance;
        }
        public double getAvoidVelocity()
        {
            return avoidVelocity;
        }
        public double getCornerAcceleration()
        {
            return cornerAcceleration;
        }
        public double getCornerVelocity()
        {
            return cornerVelocity;
        }
        public double getCornerDistance()
        {
            return cornerDistance;
        }
        public int getFirstType()
        {
            return firstType;
        }
        public int getSecondType()
        {
            return secondType;
        }
        public int getThirdType()
        {
            return thirdType;
        }
        public bool getBlock()
        {
            return block;
        }
        public int getBlockFrequency()
        {
            return blockFrequency;
        }
        public double getBlockRadius()
        {
            return blockRadius;
        }
        public int getBlockNodes()
        {
            return blockNodes;
        }
    }
}
