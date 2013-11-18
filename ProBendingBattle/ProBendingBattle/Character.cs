using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BradleyXboxUtils;

namespace ProBendingBattle
{
    class Character
    {
        public static int windowX=100;
        public static int windowY=100;
        public static int maxHealth;
        public static int playerNum;
        public static int maxFireDelay = 60;
        public static int maxBlockDelay = 100;
        public static int rightAI = 0;
        public static int leftAI = 0;
        public static int RANDOMAI = 0;
        public static int MOBAI = 2;
        public static int TYPEAI = 1;
        public static int CLOSEAI = 3;
        public static int INVSWARM = 4;
        public static int blockNodes = 5;
        public static float scale = .2f;
        private Vector2 location;
        private Vector2 previousJoystick;
        private Texture2D image = null;
        private Random random;
        private static Random randomSeed = new Random();
        private float direction;
        public static float speed;
        private int type;
        private int health;
        private AINode aiNode;
        private int fireDelay;
        private int blockDelay;
        private List<Character> leftCorners;
        private List<Character> rightCorners;

        public Character(Vector2 location)
        {
            this.location = location;
        }

        public Character(Vector2 location, float direction, int type)
        {
            this.location = location;
            this.direction = direction;
            this.type = type;
            this.health = maxHealth;
            this.previousJoystick = new Vector2(100, 100);
            this.aiNode = null;
            random = new Random(randomSeed.Next());
            fireDelay = maxFireDelay;
            blockDelay = maxBlockDelay;
            leftCorners = new List<Character>();
            rightCorners = new List<Character>();
            leftCorners.Add(new Character(new Vector2(0, 0)));
            leftCorners.Add(new Character(new Vector2(0, windowY)));
            rightCorners.Add(new Character(new Vector2(windowX, 0)));
            rightCorners.Add(new Character(new Vector2(windowX, windowY)));

            playerNum++;
        }
        public Character(Vector2 location, float direction, int type, AINode aiNode)
        {
            this.location = location;
            this.direction = direction;
            this.type = type;
            this.health = maxHealth;
            this.aiNode = aiNode;
            this.previousJoystick = new Vector2(100, 100);
            random = new Random(randomSeed.Next());
            fireDelay = maxFireDelay;
            blockDelay = maxBlockDelay;
            leftCorners = new List<Character>();
            rightCorners = new List<Character>();
            leftCorners.Add(new Character(new Vector2(0, 0)));
            leftCorners.Add(new Character(new Vector2(0, windowY)));
            rightCorners.Add(new Character(new Vector2(windowX, 0)));
            rightCorners.Add(new Character(new Vector2(windowX, windowY)));

            playerNum++;
        }

        public List<Attack> runAI(List<Character> characters, bool attackRight, List<Character> targets, Attacks attacks)
        {

            List<Attack> aiAttacks = new List<Attack>();

            bool genetic = !(aiNode == null);
            if (previousJoystick.Equals(new Vector2(100, 100)))
            {
                if (!genetic)
                    previousJoystick = new Vector2((attackRight) ? 1 : -1, 0);
                else
                    previousJoystick = new Vector2((float)((attackRight) ? aiNode.getInitVelocity() : -aiNode.getInitVelocity()),0);
                if (genetic)
                {
                    if (!attackRight)
                    {
                        rightAI = aiNode.getStrategy();
                    }
                    else
                    {
                        leftAI = aiNode.getStrategy();
                    }
                }
            }

            float deltaX, deltaY, yAccel, xAccel;
            
            int target = 0;

            if (!attackRight)
            {
                target = aiMethod(rightAI, targets);
            }
            else
            {
                target = aiMethod(leftAI, targets);
            }
            
            deltaX = targets.ElementAt<Character>(target).getLocation().X - location.X;
            deltaY = targets.ElementAt<Character>(target).getLocation().Y - location.Y;

            Vector2 initialVelocity = new Vector2((attackRight) ? 10 : -10, 0);


            if (Math.Abs(deltaX)>=Math.Abs(deltaY))
            {
                xAccel = 0;
                yAccel = (2 * deltaY) / ((deltaX / initialVelocity.X) * (deltaX / initialVelocity.X));
            }
            else
            {
                initialVelocity = new Vector2(0, (deltaY > 0) ? 10 : -10);
                xAccel = (2 * deltaX) / ((deltaY / initialVelocity.Y) * (deltaY / initialVelocity.Y));
                yAccel = 0;
            }

            Vector2 acceleration = new Vector2(xAccel, yAccel);
            Vector2 joystick = new Vector2(random.Next(-10, 11), random.Next(-10, 11));//inclusive, exclusive
            joystick.Normalize();
            joystick = joystick * (float)((genetic) ? aiNode.getRandomAcceleration() : 1);
            if (!attacks.nearest(location, (genetic) ? aiNode.getReactionDistance() : 150).Equals(Vector2.Zero))
            {
                joystick = UTIL.magD(((double)250 * ((genetic) ? aiNode.getDodgeAcceleration() : 1) * (Character.typeAdvantage(type, attacks.nearestType(location))+1)) / UTIL.distance(attacks.nearest(location), location), UTIL.getDirectionTward(attacks.nearest(location), location));
                previousJoystick = previousJoystick * (float)((genetic) ? aiNode.getDodgeVelocity() : 1);
            }
            else
            {
                if (getNearest(characters, aiNode.getAvoidDistance()).Equals(-Vector2.One))
                {
                    if (getNearest(((attackRight) ? leftCorners : rightCorners), aiNode.getCornerDistance()).Equals(-Vector2.One))
                    {
                        previousJoystick = previousJoystick * (float)((genetic) ? aiNode.getRandomVelocity() : 1);
                    }
                    else
                    {
                        joystick = UTIL.magD(((double)250 * ((genetic) ? aiNode.getCornerAcceleration() : 1)) / UTIL.distance(getNearest(((attackRight) ? leftCorners : rightCorners), aiNode.getCornerDistance()), location), UTIL.getDirectionTward(getNearest(((attackRight) ? leftCorners : rightCorners), aiNode.getCornerDistance()), location));
                        previousJoystick = previousJoystick * (float)((genetic) ? aiNode.getCornerVelocity() : 1);
                    }
                }
                else
                {
                    joystick = UTIL.magD(((double)250 * ((genetic) ? aiNode.getAvoidAcceleration() : 1)) / UTIL.distance(getNearest(characters, aiNode.getAvoidDistance()), location), UTIL.getDirectionTward(getNearest(characters, aiNode.getAvoidDistance()), location));
                    previousJoystick = previousJoystick * (float)((genetic) ? aiNode.getAvoidVelocity() : 1);
                }
            }

            joystick = joystick * .1f + previousJoystick;
            
            move(joystick, characters, attackRight);
            if (fireDelay == 0 && random.Next(0, 40) == 0)
            {
                aiAttacks.Add(attack(initialVelocity, acceleration));
            }
            else if (blockDelay == 0 && random.Next(0, aiNode.getBlockFrequency()) == 0 && aiNode.getBlock())
            {
                aiAttacks = block();
            }
            List<Attack> empty = new List<Attack>();
            return aiAttacks;

        }
        public List<Attack> block()
        {
            blockDelay = maxBlockDelay;
            List<Attack> aiAttacks = new List<Attack>();
            for (double a = 0; a < 10*Math.PI; a += 10*Math.PI/aiNode.getBlockNodes())
            {
                aiAttacks.Add(new Attack(location, aiNode.getBlockRadius(), (int)a, type));
            }
            return aiAttacks;
        }

        private Vector2 getNearest(List<Character> characters,double range)
        {
            Vector2 nearest = -Vector2.One;
            foreach (Character c in characters)
            {
                if (!c.Equals(this))
                {
                    if (UTIL.distance(this.location, c.getLocation()) < range)
                    {
                        if (UTIL.distance(this.location, c.getLocation()) < UTIL.distance(this.location, nearest))
                        {
                            nearest = c.getLocation();
                        }
                    }
                }
            }
            return nearest;
        }

        public int aiMethod(int aiMode, List<Character> targets)
        {
            int index = 0;
            int target = 0;

            if (aiMode == 0)
            {
                target = random.Next(0, targets.Count);
            }
            else if (aiMode == 1)
            {
                int max = 0;
                foreach (Character t in targets)
                {
                    if (typeAdvantage(t.getType(), type) > max)
                    {
                        target = index;
                        max = typeAdvantage(t.getType(), type);
                    }
                    index++;
                }
            }
            else if (aiMode == 2)
            {
                target = 0;
            }
            else if (aiMode == 4)
            {
                target = targets.Count - 1;
            }
            else if (aiMode == 3)
            {
                int min = (int)UTIL.distance(location, targets.ElementAt<Character>(0).getLocation());
                foreach (Character t in targets)
                {
                    if (UTIL.distance(location, t.getLocation()) < min)
                    {
                        min = (int)(UTIL.distance(location, t.getLocation()));
                        target = index;
                    }
                    index++;
                }
            }
            return target;
        }

        public Attack attack(Vector2 initialVelocity, Vector2 acceleration)
        {
            if (fireDelay == 0)
            {
                fireDelay = maxFireDelay;
                return new Attack(location, initialVelocity, acceleration, type + ((random.Next(0,101)>aiNode.getSecondTypePercent()) ? 3 : 0));
            }
            return null;
        }

        public Attack attackCurveUp(bool right)
        {
            return new Attack(location, (right) ? Attack.UP_INITIAL : Attack.UP_INITIAL * new Vector2(-1, 1), Attack.UP_ACCEL, type);
        }
        public Attack attackCurveDown(bool right)
        {
            return new Attack(location, (right) ? Attack.DOWN_INTIAL : Attack.DOWN_INTIAL * new Vector2(-1, 1), Attack.DOWN_ACCEL, type);
        }

        public void move(Vector2 joystick, List<Character> characters, bool attackRight)
        {
            if(fireDelay!=0)
                fireDelay--;
            if (blockDelay != 0)
                blockDelay--;
            if ((location + (joystick * speed)).X - 15 >= 0 && (location + (joystick * speed)).X <= windowX - 15)
            {
                if (attackRight && (location + (joystick * speed)).X + 25 <= windowX / 2)
                {

                    location.X += (joystick.X * speed);
                    previousJoystick.X = joystick.X;
                }
                else if (!attackRight && (location + (joystick * speed)).X - 25 >= windowX / 2)
                {
                    location.X += (joystick.X * speed);
                    previousJoystick.X = joystick.X;
                }
                else
                {
                    previousJoystick = new Vector2(0, 0);
                }
            }
            else
                previousJoystick.X = 0;
            if ((location + (joystick * speed)).Y >= 15 && (location + (joystick * speed)).Y <= windowY - 15)
            {
                location.Y += (joystick.Y * speed);
                previousJoystick.Y = joystick.Y;
            }
            else
                previousJoystick.Y = 0;
        }

        public Vector2 getLocation()
        {
            return location;
        }

        public Texture2D getImage()
        {
            return image;
        }

        public int getType()
        {
            return type;
        }

        public void hit(int typeHit)
        {
            health -= typeAdvantage(this.type, typeHit);
        }

        public bool isDead()
        {
            return health <= 0;
        }

        public static int typeAdvantage(int typeHost, int type)
        {
            if (typeHost == Attack.AIR)
                return 3;
            if (type == Attack.AIR)
                return 1;

            if (typeHost == Attack.GRASS)
            {
                if (type == Attack.GRASS || type == Attack.EARTH)
                    return 2;
                if (type == Attack.FIRE || type == Attack.ICE)
                    return 3;
                if (type == Attack.WATER || type == Attack.LIGHTNING)
                    return 1;
            }
            if (typeHost == Attack.FIRE)
            {
                if (type == Attack.GRASS || type == Attack.ICE)
                    return 1;
                if (type == Attack.FIRE || type == Attack.LIGHTNING)
                    return 2;
                if (type == Attack.WATER || type == Attack.EARTH)
                    return 3;
            }
            if (typeHost == Attack.WATER)
            {
                if (type == Attack.GRASS || type == Attack.LIGHTNING)
                    return 3;
                if (type == Attack.FIRE || type == Attack.EARTH)
                    return 1;
                if (type == Attack.WATER || type == Attack.ICE)
                    return 2;
            }
            return 0;
        }
    }
}
