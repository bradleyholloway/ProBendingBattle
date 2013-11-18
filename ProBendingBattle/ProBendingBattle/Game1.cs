using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using BradleyXboxUtils;

namespace ProBendingBattle
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont timesNewRoman;
        MenuInput menuInput;
        int menuChoice;
        int menuState;
        int controllers;
        bool keyboard;
        bool keyboard2;
        bool leftTeam;
        bool genetic;
        bool direct;
         
        List<Input> inputs;
        List<MenuItem> menuItems;
        List<MenuItem> aisTypes;
        List<Character> characters;
        List<ControlButton> attack;
        List<ControlButton> superAttacks;
        List<Character> redTeam;
        List<Character> blueTeam;
        List<int> redplayers;
        List<int> blueplayers;
        List<int> ais;
        int redScore;
        int blueScore;
        int numCycles;

        Attacks attacks;

        AIGenetics genetics;
        AINode leftAI;
        AINode rightAI;

        //AINode seed = new AINode(1, 1, .457953, 69.69, 54.1416, .543631, 91.56, .70194, 1, Attack.FIRE, Attack.WATER, Attack.EARTH, true,100,70,5);
        //AINode seed = new AINode(.25107, .921565, .354274, 58.6856, 93.66255, .999922, 91.413, .146168, 1,Attack.FIRE, Attack.WATER, Attack.EARTH, false,1,1,1);
        //AINode seed2 = new AINode(1, 0, 0, 50, 50, 0, 50, 0, 0);
        //AINode seed2 = new AINode(.5820699774,1,.4579,77.6,154.14,.44755,54.1416,.7077037,4,Attack.AIR,Attack.AIR,Attack.AIR, true,100,70);
        //AINode seed = new AINode(.262824,1,.365328,117.7985,3.751075,.550444,111.567,.382055,3,1,1,1,true,111,102,1);
        //AINode seed = new AINode(.846, 1, .0357, 89.9, 20.16, .06933, 104.6, .395, 0, 1, 1, 1, false, 100, 70, 5);
        AINode seed = new AINode();//.846, 1, .0357, 89.9, 20.16, .06933, 104.6, .395, 0, Attack.GRASS, Attack.FIRE, Attack.WATER, false, 100, 70,5,50);
        AINode seed2;

        public Game1(bool keyboard)
        {
            this.IsFixedTimeStep = true;
            this.TargetElapsedTime = new TimeSpan(0, 0, 0, 0, 20);

            this.keyboard = keyboard = true;
            keyboard2 = false;
            
            controllers += ((GamePad.GetState(PlayerIndex.One).IsConnected) ? 1 : 0) + ((GamePad.GetState(PlayerIndex.Two).IsConnected) ? 1 : 0)
                + ((GamePad.GetState(PlayerIndex.Three).IsConnected) ? 1 : 0) + ((GamePad.GetState(PlayerIndex.Four).IsConnected) ? 1 : 0);
            graphics = new GraphicsDeviceManager(this);
            this.graphics.IsFullScreen = true;
           
            Content.RootDirectory = "Content";
            Attack.life = 100;
            Character.maxFireDelay = 20;
            Character.speed = 5f;
            Character.maxHealth = 3;
            Character.scale = .102f;
            Character.leftAI = Character.RANDOMAI;
            Character.rightAI = Character.RANDOMAI;
            menuState = 2;

            attacks = new Attacks();
            characters = new List<Character>();
            redTeam = new List<Character>();
            blueTeam = new List<Character>();
            inputs = new List<Input>();
            redplayers = new List<int>();
            blueplayers = new List<int>();
            ais = new List<int>();
            menuItems = new List<MenuItem>();
            aisTypes = new List<MenuItem>();
            
            inputs.Add((controllers >= 1) ? (Input) new ControllerInput(PlayerIndex.One) : (keyboard) ? new KeyboardInput(PlayerIndex.One) : null);
            inputs.Add((controllers >= 2) ? (Input) new ControllerInput(PlayerIndex.Two) : (keyboard) ? (controllers==1) ? new KeyboardInput(PlayerIndex.One) : ((keyboard2) ? new KeyboardInput(PlayerIndex.Two) : null) : null);
            inputs.Add((controllers >= 3) ? (Input) new ControllerInput(PlayerIndex.Three) : (controllers >= 1 && keyboard) ? (controllers==2) ? new KeyboardInput(PlayerIndex.One) : (keyboard2) ? new KeyboardInput(PlayerIndex.Two) : null : null);
            inputs.Add((controllers == 4) ? (Input) new ControllerInput(PlayerIndex.Four) : (controllers >= 2 && keyboard) ? (controllers == 3) ? new KeyboardInput(PlayerIndex.One) : (keyboard2) ? new KeyboardInput(PlayerIndex.Two) : null : null);
            inputs.Add((controllers >= 3) ? (controllers == 4) ? new KeyboardInput(PlayerIndex.One) : (keyboard2) ? new KeyboardInput(PlayerIndex.Two) : null : null);
            inputs.Add((controllers == 4) ? (keyboard2) ? new KeyboardInput(PlayerIndex.Two) : null : null);
            
            menuInput = new MenuInput((controllers>=1) ? (Input) new ControllerInput(PlayerIndex.One) : new KeyboardInput(PlayerIndex.One));
            
            attack = new List<ControlButton>();
            superAttacks = new List<ControlButton>();
            for (int a = 0; a < 6; a++)
            {
                attack.Add(new ControlButton());
                superAttacks.Add(new ControlButton(Character.maxFireDelay + 50));
            }
            menuItems.Add(new MenuItem("Play Game (No Keyboard)", new Vector2(150, 150), Color.White, 6));
            menuItems.Add(new MenuItem("Play Game (1 Keyboard)", new Vector2(200, 200), Color.White, 4));
            menuItems.Add(new MenuItem("Play Game (2 Keyboard)", new Vector2(250, 250), Color.White, 5));
            menuItems.Add(new MenuItem("Quit", new Vector2(300, 300), Color.White, -1));
            menuItems.Add(new MenuItem("Set Left AI", new Vector2(350, 350), Color.Red, 7));
            menuItems.Add(new MenuItem("Set Right AI", new Vector2(400, 400), Color.Blue, 8));
            menuItems.Add(new MenuItem("Genetic", new Vector2(450, 450), Color.Firebrick, 50));
            menuItems.Add(new MenuItem("Direct", new Vector2(500, 500), Color.Firebrick, 77));

            aisTypes.Add(new MenuItem("Random Targeting", new Vector2(100, 100), Color.White, 100));
            aisTypes.Add(new MenuItem("Distance Targeting", new Vector2(100, 150), Color.Red, 101));
            aisTypes.Add(new MenuItem("Swarm Targeting", new Vector2(100, 200), Color.Blue, 102));
            aisTypes.Add(new MenuItem("Type Targeting", new Vector2(100, 250), Color.Green, 103));
            aisTypes.Add(new MenuItem("Inverse Swarm", new Vector2(100, 300), Color.White, 104));

            genetics = new AIGenetics();
            genetics.init(4, 5, 10);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
            
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            numCycles = 0;
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Texture2D sphere = Content.Load<Texture2D>("sphere");
            Texture2D arrow = Content.Load<Texture2D>("arrow");
            timesNewRoman = Content.Load<SpriteFont>("TimesNewRoman");
            Attack.updateSphere(sphere, .1f);
            Attack.updateArrow(arrow, .1f);
            int windowX = GraphicsDevice.Viewport.Width;
            int windowY = GraphicsDevice.Viewport.Height;
            Character.windowX = windowX;
            Character.windowY = windowY;
            characters.Clear();

            seed2 = new AINode();

            characters.Add(new Character(new Vector2(15, 15), 0f, ((genetic) ? leftAI : seed).getFirstType(), ((genetic) ? leftAI : seed)));
            characters.Add(new Character(new Vector2(windowX - 15, 15), 0f, ((genetic) ? rightAI : seed2).getFirstType(), (genetic) ? rightAI : seed2));
            characters.Add(new Character(new Vector2(15, windowY / 2), 0f, ((genetic) ? leftAI : seed).getSecondType(), (genetic) ? leftAI : seed));
            characters.Add(new Character(new Vector2(windowX - 15, windowY / 2), 0f, ((genetic) ? rightAI : seed2).getSecondType(), (genetic) ? rightAI : seed2));
            characters.Add(new Character(new Vector2(15, windowY - 15), 0f, ((genetic) ? leftAI : seed).getThirdType(), (genetic) ? leftAI : seed));
            characters.Add(new Character(new Vector2(windowX - 15, windowY - 15), 0f, ((genetic) ? rightAI : seed2).getThirdType(), (genetic) ? rightAI : seed2));
            /*characters.Add(new Character(new Vector2(15, 15+30), 0f, Attack.FIRE));
            characters.Add(new Character(new Vector2(windowX - 15, 15+30), 0f, Attack.WATER));
            characters.Add(new Character(new Vector2(15, windowY / 2+30), 0f, Attack.EARTH));
            characters.Add(new Character(new Vector2(windowX - 15, windowY / 2+30), 0f, Attack.FIRE));
            characters.Add(new Character(new Vector2(15, windowY - 15-20), 0f, Attack.WATER));
            characters.Add(new Character(new Vector2(windowX - 15, windowY - 15-20), 0f, Attack.EARTH));
            */

            //for (int ch = 0; ch < 60; ch++)
            //{
            //    characters.Add(new Character(new Vector2(15, windowY/60*ch), 0f, Attack.AIR));
            //    characters.Add(new Character(new Vector2(windowX - 15, windowY/60*ch), 0f, Attack.AIR));
            //}
            redTeam.Clear();
            blueTeam.Clear();
            attacks.Clear();

            for (int a = 0; a < characters.Count; a++)
            {
                if (a % 2 == 0)
                {
                    redTeam.Add(characters.ElementAt<Character>(a));
                }
                else
                {
                    blueTeam.Add(characters.ElementAt<Character>(a));
                }
            }
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //menuState = 50;
            //genetic = true;
            //while (true)
            //{
                numCycles++;

                // Allows the game to exit
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                    this.Exit();


                if (menuState == 0)
                {
                    foreach (int player in redplayers)
                    {
                        if (redTeam.Contains(characters.ElementAt<Character>(player)))
                        {
                            if (attack.ElementAt<ControlButton>(player).update(inputs.ElementAt<Input>(player).getBottomActionButton()))
                            {
                                Attack a = characters.ElementAt<Character>(player).attack(new Vector2(10, 0), inputs.ElementAt<Input>(player).getRightJoystick() * .3f);
                                if (a != null)
                                    attacks.add(a);
                            }
                            if (superAttacks.ElementAt<ControlButton>(player).update(inputs.ElementAt<Input>(player).getRightActionButton()))
                            {
                                foreach (Attack a in characters.ElementAt<Character>(player).block())
                                {
                                    attacks.add(a);
                                }
                            }
                            characters.ElementAt<Character>(player).move(inputs.ElementAt<Input>(player).getLeftJoystick(), characters, true);
                        }
                    }
                    foreach (int player in blueplayers)
                    {
                        if (blueTeam.Contains(characters.ElementAt<Character>(player)))
                        {
                            if (attack.ElementAt<ControlButton>(player).update(inputs.ElementAt<Input>(player).getBottomActionButton()))
                            {
                                Attack a = characters.ElementAt<Character>(player).attack(new Vector2(-10, 0), inputs.ElementAt<Input>(player).getRightJoystick() * .3f);
                                if (a != null)
                                    attacks.add(a);
                            }
                            if (superAttacks.ElementAt<ControlButton>(player).update(inputs.ElementAt<Input>(player).getRightActionButton()))
                            {
                                foreach (Attack a in characters.ElementAt<Character>(player).block())
                                {
                                    attacks.add(a);
                                }
                            }
                            characters.ElementAt<Character>(player).move(inputs.ElementAt<Input>(player).getLeftJoystick(), characters, false);
                        }
                    }
                    foreach (int player in ais)
                    {
                        if (redTeam.Contains(characters.ElementAt<Character>(player)) || blueTeam.Contains(characters.ElementAt<Character>(player)))
                        {
                            List<Attack> aiAttacks = characters.ElementAt<Character>(player).runAI((redTeam.Contains(characters.ElementAt<Character>(player))) ? redTeam : blueTeam, redTeam.Contains(characters.ElementAt<Character>(player)), (redTeam.Contains(characters.ElementAt<Character>(player))) ? blueTeam : redTeam, attacks);
                            foreach (Attack aiAttack in aiAttacks)
                            {
                                attacks.add(aiAttack);
                            }
                        }
                    }


                    attacks.run();

                    for (int a = 0; a < characters.Count; a++)
                    {
                        if (redTeam.Contains(characters.ElementAt<Character>(a)) || blueTeam.Contains(characters.ElementAt<Character>(a)))
                        {
                            int type = attacks.contains(characters.ElementAt<Character>(a).getLocation());
                            if (type != -1)
                            {
                                characters.ElementAt<Character>(a).hit(type);
                                if (characters.ElementAt<Character>(a).isDead())
                                {
                                    if (redTeam.Contains(characters.ElementAt<Character>(a)))
                                    {
                                        redTeam.Remove(characters.ElementAt<Character>(a));
                                        a--;
                                    }
                                    else if (blueTeam.Contains(characters.ElementAt<Character>(a)))
                                    {
                                        blueTeam.Remove(characters.ElementAt<Character>(a));
                                        a--;
                                    }
                                }
                            }
                        }
                    }
                    attacks.collide();

                    redScore = 3 - blueTeam.Count;
                    blueScore = 3 - redTeam.Count;

                    if (redScore == 3 || blueScore == 3)
                    {
                        if (leftAI != null)
                            leftAI.update((redScore == 3));
                        if (rightAI != null)
                            rightAI.update(!(redScore == 3));
                        //LoadContent();
                        if (blueScore == 3)
                        {
                            seed = seed2;
                        }
                        menuState = 1;
                    }
                }
                else if (menuState == 1)
                {
                    if (GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Enter) || genetic || direct)
                    {
                        LoadContent();
                        menuState = 2;
                    }
                }

                else if (menuState == 2)
                {
                    
                    menuInput.run();
                    if (genetic)
                        menuState = 51;
                    if (direct)
                    {
                        menuState = 6;
                        LoadContent();
                        
                    }
                    if (menuInput.getAction() != 3)
                    {
                        menuChoice -= menuInput.getAction();
                        if (menuChoice >= menuItems.Count)
                            menuChoice = 0;
                        if (menuChoice < 0)
                            menuChoice = menuItems.Count - 1;
                    }
                    else
                    {
                        menuState = menuItems.ElementAt<MenuItem>(menuChoice).point();
                    }
                }
                else if (menuState == 4)
                {
                    keyboard = true;
                    keyboard2 = false;
                    inputs.Clear();
                    inputs.Add((controllers >= 1) ? (Input)new ControllerInput(PlayerIndex.One) : (keyboard) ? new KeyboardInput(PlayerIndex.One) : null);
                    inputs.Add((controllers >= 2) ? (Input)new ControllerInput(PlayerIndex.Two) : (keyboard) ? (controllers == 1) ? new KeyboardInput(PlayerIndex.One) : ((keyboard2) ? new KeyboardInput(PlayerIndex.Two) : null) : null);
                    inputs.Add((controllers >= 3) ? (Input)new ControllerInput(PlayerIndex.Three) : (controllers >= 1 && keyboard) ? (controllers == 2) ? new KeyboardInput(PlayerIndex.One) : (keyboard2) ? new KeyboardInput(PlayerIndex.Two) : null : null);
                    inputs.Add((controllers == 4) ? (Input)new ControllerInput(PlayerIndex.Four) : (controllers >= 2 && keyboard) ? (controllers == 3) ? new KeyboardInput(PlayerIndex.One) : (keyboard2) ? new KeyboardInput(PlayerIndex.Two) : null : null);
                    inputs.Add((controllers >= 3) ? (controllers == 4) ? new KeyboardInput(PlayerIndex.One) : (keyboard2) ? new KeyboardInput(PlayerIndex.Two) : null : null);
                    inputs.Add((controllers == 4) ? (keyboard2) ? new KeyboardInput(PlayerIndex.Two) : null : null);

                    redplayers.Clear();
                    blueplayers.Clear();
                    ais.Clear();
                    for (int p = 0; p < controllers + ((keyboard) ? (keyboard2) ? 2 : 1 : 0); p++)
                    {
                        if (p % 2 == 0)
                        {
                            redplayers.Add(p);
                        }
                        else
                        {
                            blueplayers.Add(p);
                        }
                    }
                    for (int p = controllers + ((keyboard) ? (keyboard2) ? 2 : 1 : 0); p < characters.Count; p++)
                    {
                        ais.Add(p);
                    }
                    menuState = 0;
                }
                else if (menuState == 5)
                {
                    keyboard = true;
                    keyboard2 = true;
                    inputs.Clear();
                    inputs.Add((controllers >= 1) ? (Input)new ControllerInput(PlayerIndex.One) : (keyboard) ? new KeyboardInput(PlayerIndex.One) : null);
                    inputs.Add((controllers >= 2) ? (Input)new ControllerInput(PlayerIndex.Two) : (keyboard) ? (controllers == 1) ? new KeyboardInput(PlayerIndex.One) : ((keyboard2) ? new KeyboardInput(PlayerIndex.Two) : null) : null);
                    inputs.Add((controllers >= 3) ? (Input)new ControllerInput(PlayerIndex.Three) : (controllers >= 1 && keyboard) ? (controllers == 2) ? new KeyboardInput(PlayerIndex.One) : (keyboard2) ? new KeyboardInput(PlayerIndex.Two) : null : null);
                    inputs.Add((controllers == 4) ? (Input)new ControllerInput(PlayerIndex.Four) : (controllers >= 2 && keyboard) ? (controllers == 3) ? new KeyboardInput(PlayerIndex.One) : (keyboard2) ? new KeyboardInput(PlayerIndex.Two) : null : null);
                    inputs.Add((controllers >= 3) ? (controllers == 4) ? new KeyboardInput(PlayerIndex.One) : (keyboard2) ? new KeyboardInput(PlayerIndex.Two) : null : null);
                    inputs.Add((controllers == 4) ? (keyboard2) ? new KeyboardInput(PlayerIndex.Two) : null : null);

                    redplayers.Clear();
                    blueplayers.Clear();
                    ais.Clear();
                    for (int p = 0; p < controllers + ((keyboard) ? (keyboard2) ? 2 : 1 : 0); p++)
                    {
                        if (p % 2 == 0)
                        {
                            redplayers.Add(p);
                        }
                        else
                        {
                            blueplayers.Add(p);
                        }
                    }
                    for (int p = controllers + ((keyboard) ? (keyboard2) ? 2 : 1 : 0); p < characters.Count; p++)
                    {
                        ais.Add(p);
                    }
                    menuState = 0;
                }
                else if (menuState == 6)
                {
                    keyboard = false;
                    keyboard2 = false;
                    inputs.Clear();
                    inputs.Add((controllers >= 1) ? (Input)new ControllerInput(PlayerIndex.One) : (keyboard) ? new KeyboardInput(PlayerIndex.One) : null);
                    inputs.Add((controllers >= 2) ? (Input)new ControllerInput(PlayerIndex.Two) : (keyboard) ? (controllers == 1) ? new KeyboardInput(PlayerIndex.One) : ((keyboard2) ? new KeyboardInput(PlayerIndex.Two) : null) : null);
                    inputs.Add((controllers >= 3) ? (Input)new ControllerInput(PlayerIndex.Three) : (controllers >= 1 && keyboard) ? (controllers == 2) ? new KeyboardInput(PlayerIndex.One) : (keyboard2) ? new KeyboardInput(PlayerIndex.Two) : null : null);
                    inputs.Add((controllers == 4) ? (Input)new ControllerInput(PlayerIndex.Four) : (controllers >= 2 && keyboard) ? (controllers == 3) ? new KeyboardInput(PlayerIndex.One) : (keyboard2) ? new KeyboardInput(PlayerIndex.Two) : null : null);
                    inputs.Add((controllers >= 3) ? (controllers == 4) ? new KeyboardInput(PlayerIndex.One) : (keyboard2) ? new KeyboardInput(PlayerIndex.Two) : null : null);
                    inputs.Add((controllers == 4) ? (keyboard2) ? new KeyboardInput(PlayerIndex.Two) : null : null);

                    redplayers.Clear();
                    blueplayers.Clear();
                    ais.Clear();
                    for (int p = 0; p < controllers + ((keyboard) ? (keyboard2) ? 2 : 1 : 0); p++)
                    {
                        if (p % 2 == 0)
                        {
                            redplayers.Add(p);
                        }
                        else
                        {
                            blueplayers.Add(p);
                        }
                    }
                    for (int p = controllers + ((keyboard) ? (keyboard2) ? 2 : 1 : 0); p < characters.Count; p++)
                    {
                        ais.Add(p);
                    }
                    menuState = 0;
                }
                else if (menuState == 7)
                {
                    menuState = 99;
                    menuChoice = 0;
                    leftTeam = true;
                }
                else if (menuState == 8)
                {
                    menuState = 99;
                    menuChoice = 0;
                    leftTeam = false;
                }
                else if (menuState == 50)
                {
                    genetic = true;
                    genetics.init(10,2,1);//seed
                    menuState = 51;
                }
                else if (menuState == 51)
                {
                    List<AINode> nextMatch = genetics.getNextMatch();
                    leftAI = nextMatch.ElementAt<AINode>(0);
                    rightAI = nextMatch.ElementAt<AINode>(1);


                    keyboard = false;
                    keyboard2 = false;
                    inputs.Clear();
                    inputs.Add((controllers >= 1) ? (Input)new ControllerInput(PlayerIndex.One) : (keyboard) ? new KeyboardInput(PlayerIndex.One) : null);
                    inputs.Add((controllers >= 2) ? (Input)new ControllerInput(PlayerIndex.Two) : (keyboard) ? (controllers == 1) ? new KeyboardInput(PlayerIndex.One) : ((keyboard2) ? new KeyboardInput(PlayerIndex.Two) : null) : null);
                    inputs.Add((controllers >= 3) ? (Input)new ControllerInput(PlayerIndex.Three) : (controllers >= 1 && keyboard) ? (controllers == 2) ? new KeyboardInput(PlayerIndex.One) : (keyboard2) ? new KeyboardInput(PlayerIndex.Two) : null : null);
                    inputs.Add((controllers == 4) ? (Input)new ControllerInput(PlayerIndex.Four) : (controllers >= 2 && keyboard) ? (controllers == 3) ? new KeyboardInput(PlayerIndex.One) : (keyboard2) ? new KeyboardInput(PlayerIndex.Two) : null : null);
                    inputs.Add((controllers >= 3) ? (controllers == 4) ? new KeyboardInput(PlayerIndex.One) : (keyboard2) ? new KeyboardInput(PlayerIndex.Two) : null : null);
                    inputs.Add((controllers == 4) ? (keyboard2) ? new KeyboardInput(PlayerIndex.Two) : null : null);

                    redplayers.Clear();
                    blueplayers.Clear();
                    ais.Clear();
                    for (int p = 0; p < controllers + ((keyboard) ? (keyboard2) ? 2 : 1 : 0); p++)
                    {
                        if (p % 2 == 0)
                        {
                            redplayers.Add(p);
                        }
                        else
                        {
                            blueplayers.Add(p);
                        }
                    }
                    for (int p = controllers + ((keyboard) ? (keyboard2) ? 2 : 1 : 0); p < characters.Count; p++)
                    {
                        ais.Add(p);
                    }



                    LoadContent();
                    menuState = 0;
                }
                else if (menuState == 77)
                {
                    direct = true;
                    menuState = 2;
                }

                else if (menuState == 99)
                {
                    menuInput.run();
                    if (menuInput.getAction() != 3)
                    {
                        menuChoice -= menuInput.getAction();
                        if (menuChoice >= aisTypes.Count)
                            menuChoice = 0;
                        if (menuChoice < 0)
                            menuChoice = aisTypes.Count - 1;
                    }
                    else
                    {
                        menuState = aisTypes.ElementAt<MenuItem>(menuChoice).point();
                    }
                }
                else if (menuState == 100)
                {
                    if (leftTeam)
                    {
                        Character.leftAI = Character.RANDOMAI;
                    }
                    else
                    {
                        Character.rightAI = Character.RANDOMAI;
                    }
                    menuState = 2;
                }
                else if (menuState == 101)
                {
                    if (leftTeam)
                    {
                        Character.leftAI = Character.CLOSEAI;
                    }
                    else
                    {
                        Character.rightAI = Character.CLOSEAI;
                    }
                    menuState = 2;
                }
                else if (menuState == 102)
                {
                    if (leftTeam)
                    {
                        Character.leftAI = Character.MOBAI;
                    }
                    else
                    {
                        Character.rightAI = Character.MOBAI;
                    }
                    menuState = 2;
                }
                else if (menuState == 103)
                {
                    if (leftTeam)
                    {
                        Character.leftAI = Character.TYPEAI;
                    }
                    else
                    {
                        Character.rightAI = Character.TYPEAI;
                    }
                    menuState = 2;
                }
                else if (menuState == 104)
                {
                    if (leftTeam)
                    {
                        Character.leftAI = Character.INVSWARM;
                    }
                    else
                    {
                        Character.rightAI = Character.INVSWARM;
                    }
                    menuState = 2;
                }

                else
                {
                    this.Exit();
                }

                base.Update(gameTime);
            //}
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            if (menuState == 0 || menuState == 1)
            {
                foreach (Character c in redTeam)
                {
                    spriteBatch.Draw(Attack.sphere, c.getLocation(), null, Attack.getColor(c.getType()), 0f, Attack.origin, Character.scale, SpriteEffects.None, 0f);
                }
                foreach (Character c in blueTeam)
                {
                    spriteBatch.Draw(Attack.sphere, c.getLocation(), null, Attack.getColor(c.getType()), 0f, Attack.origin, Character.scale, SpriteEffects.None, 0f);
                }

                foreach (Attack attack in attacks.getAttacks())
                {
                    spriteBatch.Draw(Attack.arrow, attack.getLocation(), null, attack.getColor(), attack.getRotation(), Attack.origin, Attack.scale, SpriteEffects.None, 0f);
                }
            }
            if (menuState == 1)
            {
                spriteBatch.DrawString(timesNewRoman, (redTeam.Count == 0) ? "Blue Team Wins!" : "Red Team Wins!", new Vector2(GraphicsDevice.Viewport.Width / 2 - 150, GraphicsDevice.Viewport.Height / 2 - 40), Color.White);
            }
            if (menuState == 2)
            {
                foreach (MenuItem i in menuItems)
                {
                    spriteBatch.DrawString(timesNewRoman, i.text(), i.location(), i.color());
                }
                spriteBatch.Draw(Attack.sphere, menuItems.ElementAt<MenuItem>(menuChoice).location() - new Vector2(15,-25), null, Color.GhostWhite, 0f, Attack.origin, Attack.scale, SpriteEffects.None, 0f);
            }
            if (menuState == 99)
            {
                foreach (MenuItem i in aisTypes)
                {
                    spriteBatch.DrawString(timesNewRoman, i.text(), i.location(), i.color());
                }
                spriteBatch.Draw(Attack.sphere, aisTypes.ElementAt<MenuItem>(menuChoice).location() - new Vector2(15, -25), null, Color.GhostWhite, 0f, Attack.origin, Attack.scale, SpriteEffects.None, 0f);
            }
            

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private Vector2 getVector(GraphicsDevice g)
        {
            return new Vector2(g.Viewport.Width, g.Viewport.Height);
        }
    }
    
}
