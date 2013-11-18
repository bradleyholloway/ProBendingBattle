using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProBendingBattle
{
    class Attack
    {
        private int type;
        private Vector2 initialLocation;
        private Vector2 initialVelocities;
        private Vector2 acceleration;
        private int time;


        public static int AIR = 0;
        public static int GRASS = 1;
        public static int WATER = 2;
        public static int FIRE = 3;
        public static int EARTH = 4;
        public static int ICE = 5;
        public static int LIGHTNING = 6;

        public static int radius = 5;
        public static int life = 75;
        public static float scale = 1f;
        public static float sphereScale = 1f;
        public float rotation = 0f;
        public double Cradius;
        private bool circle;
        public static Texture2D sphere = null;
        public static Texture2D arrow = null;
        public static Vector2 origin = Vector2.Zero;
        public static Vector2 UP_INITIAL = new Vector2(7, 9);
        public static Vector2 DOWN_INTIAL = new Vector2(7, -9);
        public static Vector2 STRAIGHT_INITIAL = new Vector2(0, (float)Math.Sqrt(50));
        public static Vector2 UP_ACCEL = new Vector2(0f, -.2f);
        public static Vector2 DOWN_ACCEL = new Vector2(0f, .2f);
        public static Vector2 STRAIGHT_ACCEL = new Vector2(0, .1f);
        
        public Attack(Vector2 initialPosition, Vector2 initialVelocity, Vector2 accel, int type)
        {
            this.type = type;
            this.initialLocation = initialPosition + initialVelocity;
            this.initialVelocities = initialVelocity;
            this.acceleration = accel;
            this.time = 5;
            this.circle = false;
        }
        public Attack(Vector2 origin, double radius, int time, int type)
        {
            this.initialLocation = origin;
            this.time = time;
            this.circle = true;
            this.Cradius = radius;
            this.type = type;
        }

        public bool run()
        {
            time += 1;
            rotation += .01f;
            return (time > life);
        }

        public Vector2 getLocation()
        {
            float x, y;
            if (!circle)
            {
                x = (float).5 * acceleration.X * time * time + initialVelocities.X * time + initialLocation.X;
                y = (float).5 * acceleration.Y * time * time + initialVelocities.Y * time + initialLocation.Y;
            }
            else
            {
                x = (float)(initialLocation.X + Cradius * Math.Cos(time * .2));
                y = (float)(initialLocation.Y + Cradius * Math.Sin(time * .2));
            }
            return new Vector2(x, y);
        }
        public float getRotation()
        {
            if (!circle)
                return (float)Math.Atan2(acceleration.Y * time + initialVelocities.Y, acceleration.X * time + initialVelocities.X);
            else
                return (float)(time * .2 + Math.PI / 2);
        }

        public int getRadius()
        {
            return radius;
        }
        public static Color getColor(int t)
        {
            if (t == Attack.AIR)
                return Color.White;
            if (t == Attack.GRASS)
                return Color.DarkGreen;
            if (t == Attack.FIRE)
                return Color.Red;
            if (t == Attack.WATER)
                return Color.Blue;
            return Color.Black;
        }

        public Color getColor()
        {
            if (type == Attack.AIR)
                return Color.White;
            if (type == Attack.GRASS)
                return new Color(0,250,0);
            if (type == Attack.FIRE)
                return Color.Orange;
            if (type == Attack.WATER)
                return Color.Turquoise;
            if (type == Attack.EARTH)
                return Color.DarkGreen;
            if (type == Attack.LIGHTNING)
                return Color.Yellow;
            if (type == Attack.ICE)
                return Color.LightBlue;
            return Color.Black;
        }
        public bool contains(Vector2 point)
        {
            if (Vector2.Distance(point, getLocation()) <= radius + Character.scale * sphere.Width)
            {
                return true;
            }
            return false;
        }

        public static void updateSphere(Texture2D image, float s)
        {
            sphereScale = s;
            sphere = image;
            radius = (int)(image.Width * s/2);
            origin = new Vector2(radius/s, radius/s);
        }
        public static void updateArrow(Texture2D image, float s)
        {
            scale = s;
            arrow = image;
            radius = (int)(image.Width * s / 2);
            origin = new Vector2(radius / s, radius / s);
        }

        public int getType()
        {
            return type;
        }
    }
}
