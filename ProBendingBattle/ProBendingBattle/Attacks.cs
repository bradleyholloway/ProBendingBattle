using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using BradleyXboxUtils;

namespace ProBendingBattle
{
    class Attacks
    {
        private int size;
        private List<Attack> objects;
        public Attacks()
        {
            objects = new List<Attack>();
            size = 0;
        }
        public void add(Attack a)
        {
            if (a != null)
            {
                objects.Add(a);
                size++;
            }
        }
        public void run()
        {
            for (int a = 0; a < size; a++)
            {
                if (objects.ElementAt<Attack>(a).run())
                {
                    objects.RemoveAt(a);
                    size--;
                }
            }
        }
        public int contains(Vector2 point)
        {
            for (int a = 0; a < size; a++)
            {
                if (objects.ElementAt<Attack>(a).contains(point))
                {
                    int type = objects.ElementAt<Attack>(a).getType();
                    objects.RemoveAt(a);
                    size--;
                    return type;
                }
            }
            return -1;
        }
        public void collide()
        {
            for (int a = 0; a < size; a++)
            {
                for (int b = a + 1; b < size; b++)
                {
                    if (UTIL.distance(objects.ElementAt<Attack>(a).getLocation(),objects.ElementAt<Attack>(b).getLocation())<Attack.radius*2)
                    {
                        if (objects.ElementAt<Attack>(b).getType() == Attack.AIR || objects.ElementAt<Attack>(a).getType() != Attack.AIR && Character.typeAdvantage(objects.ElementAt<Attack>(a).getType(), objects.ElementAt<Attack>(b).getType()) == 3)
                        {
                            objects.RemoveAt(a);
                            size--;
                        }
                        else if (objects.ElementAt<Attack>(a).getType() == Attack.AIR || Character.typeAdvantage(objects.ElementAt<Attack>(b).getType(), objects.ElementAt<Attack>(a).getType()) == 3)
                        {
                            objects.RemoveAt(b);
                            size--;
                        }
                        else
                        {
                            objects.RemoveAt(b);
                            objects.RemoveAt(a);
                            size--; size--;
                        }
                    }
                }
            }
        }
        public Vector2 nearest(Vector2 point)
        {
            Vector2 nearest = new Vector2(10000,10000);

            foreach (Attack a in objects)
            {
                if (UTIL.distance(a.getLocation(), point) < UTIL.distance(nearest, point))
                {
                    nearest = a.getLocation();
                }
            }
            if(UTIL.distance(nearest, point) < 100)
                return nearest;
            return Vector2.Zero;
        }
        public Vector2 nearest(Vector2 point, double maxDistance)
        {
            Vector2 nearest = new Vector2(10000, 10000);

            foreach (Attack a in objects)
            {
                if (UTIL.distance(a.getLocation(), point) < UTIL.distance(nearest, point))
                {
                    nearest = a.getLocation();
                }
            }
            if (UTIL.distance(nearest, point) < maxDistance)
                return nearest;
            return Vector2.Zero;
        }
        public int nearestType(Vector2 point)
        {
            Attack nearest = new Attack(new Vector2(10000,10000), Vector2.Zero, Vector2.Zero, 0);

            foreach (Attack a in objects)
            {
                if (UTIL.distance(a.getLocation(), point) < UTIL.distance(nearest.getLocation(), point))
                {
                    nearest = a;
                }
            }
            if (UTIL.distance(nearest.getLocation(), point) < 100)
                return nearest.getType();
            return 0;
        }
        public int nearestType(Vector2 point, double maxDistance)
        {
            Attack nearest = new Attack(new Vector2(10000, 10000), Vector2.Zero, Vector2.Zero, 0);

            foreach (Attack a in objects)
            {
                if (UTIL.distance(a.getLocation(), point) < UTIL.distance(nearest.getLocation(), point))
                {
                    nearest = a;
                }
            }
            if (UTIL.distance(nearest.getLocation(), point) < maxDistance)
                return nearest.getType();
            return 0;
        }

        public List<Attack> getAttacks()
        {
            return objects;
        }
        public void Clear()
        {
            size = 0;
            objects.Clear();
        }
    }
}
