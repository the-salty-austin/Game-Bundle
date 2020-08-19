using System;
using System.Linq;

namespace GameBundle
{
    class Sniper
    {
        enum TargetState {Aggressive,Moderate,Calm} // Target Mental State
        public void Launch(ref int score)
        {
            Info();
            Console.WriteLine("Press [Enter] to continue");
            Console.ReadLine();
            Console.Clear();

            score = 0;

            const double G = 10;
            int hc = 0; // hit count
            double dist=0; // bullet travel distance
            bool hitreg; // bullet hit (<=1.0m error)
            bool precision = false; // bullet precise hit (<=0.2m error)

            // Generate sniper initial location (x,y)=(0,0)
            double sx = 0.0; // sx: sniper x location
            double sy = 0.0; // sy: sniper y location

            // Generate target initial conditions (location,mental state,health)
            Random rand = new Random();
            double tx = rand.Next(0,501);
            double ty = 0;
            TargetState state = TargetState.Moderate;
            int health = 100;

            // Generate G-Field strength and direction
            // ( G-Field affects the bullet's trajectory )
            double gSpd = rand.Next(-100,101); 
            gSpd /= 40; // G-Field Speed Range -2.5 ~ +2.5 m/s
            int gDir = rand.Next(0,360); // G-Field Direction 0~359 deg

            do {
                double bx=0; // bullet x
                double by=0; // bullet y
                double gx=0; // G-Field x
                double gy=0; // G-Field y
                double t=0; // time
                string dir="N/A"; // G-Field direction
                
                Console.WriteLine("Target now at ({0},{1})\n",tx,ty);

                // calculate G-Field in the x direction
                gx = GfX(gSpd,gDir); 
                if (gx>=0) {dir="Forward";}
                else if (gx<0) {dir="Backward";}
                Console.WriteLine("Horz. G-Field Bonus: {0} m/s ({1})",Math.Round(Math.Abs(gx),3),dir);
                
                // calculate G-Field in the y direction
                gy = GfY(gSpd,gDir); 
                if (gy>=0) {dir="Downward";}
                else if (gy<0) {dir="Upward";}
                Console.WriteLine("Vert. G-Field Bonus: {0} m/s ({1})",Math.Round(Math.Abs(gy),3),dir);

                // Bullet travel speed is set at 150 m/s.
                // Bullet shoot out angle is input by player.
                double init_spd =150.0; 
                Console.Write("Shoot Angle (deg): ");
                double angle = double.Parse(Console.ReadLine());
                angle = (angle*Math.PI)/180;

                do {
                    // Simulate bullet trajectory until bullet hits target or target lands.
                    t+=0.0001;
                    bx = ShootX(sx,init_spd,angle,G,t,gx); // bx: bullet x location
                    by = ShootY(sy,init_spd,angle,G,t,gy); // by: bullet y location
                    hitreg = HitReg(ref health,ref state,bx,by,tx,ty,sx,sy,ref dist,ref precision);
                    if (by<=0) {break;}
                } while (!hitreg);

                if (hitreg) {
                    // if target is hit, target's mental state is affected
                    hc++;
                    State(ref hc,ref state);

                    // calculate the damage caused by bullet
                    int damage = Damage(ref health,dist,state,precision);

                    // calculate the score that the player has gotten.

                    /* Normally, the score is equal to the damage caused.
                     * If the bullet hits precisely, extra 35% score bonus
                     * If the bullet hits when Target is at Aggressive state, extra 100% score bonus
                     * If the bullet hits when Target is at Moderate state, extra 50% score bonus
                     */
                    int add = 0;
                    if(!precision) add += damage;
                    else if (precision) add += (int)(damage*1.35);

                    if(state==TargetState.Aggressive) score += (int)(add*2.0);
                    else if(state==TargetState.Moderate) score += (int)(add*1.5);
                    else if(state==TargetState.Calm) score += (int)(add*1.0);

                    // If target is hit, generate new G-Field
                    Console.WriteLine("Target Hit!");
                    Console.WriteLine("Current Score: "+score);
                    gSpd = rand.Next(-100,101); 
                    gSpd /= 40;
                    gDir = rand.Next(0,360); 
                }
                else if (!hitreg) {
                    hc= (hc>1) ? hc-=2 : hc--;
                    State(ref hc,ref state);

                    // If target is not hit, remain old G-Field
                    Console.Write("No Hit... ");
                    Console.WriteLine("Your bullet landed at ({0},{1})",(int)bx,(int)by);
                    Console.WriteLine("Current Score: "+score);
                }

                Console.Write("Target State: {0}\nPress Enter to continue.",state);
                Console.ReadLine();
                Console.Clear();

                if ( Move(state) )
                {
                    // Movement probability based on Target's state 
                    /*
                     * If target is at Aggressive state, there is 70% probability 
                     * that it will move to new location. 
                     * If Moderate state, 50%. If Calm state, 10%.
                     */           
                    tx = rand.Next(0,501);
                    ty = 0;
                    Console.WriteLine("[ warning ] Target has relocated.");
                }
            } while (health>0); // Quit game when target health <= 0
            Console.Clear();
            Console.WriteLine("You have eliminated the target.\nWell done, agent.");
            Console.WriteLine("\nPress [Enter] to continue...");
            Console.ReadLine();
        }

        static void Info()
        {
            Console.WriteLine("\n***** Welcome *****\nSnipe him till death\n");
            Console.WriteLine("***** Introduction *****");
            Console.Write("\tAgent, you're now dispatched to the battlefield. ");
            Console.Write("Your mission is to eliminate the world's most-wanted target. ");
            Console.Write("According to intel, the target is highly dangerous and becomes irritated when being shot. ");
            Console.WriteLine("The more irritated the target is, the more likely he is to escape and relocate.");
            Console.Write("\tYou are equipped with an advanced sniper rifle, which causes a maximum damage of 20. ");
            Console.WriteLine("However, the damage drops as you become farther from the target. Also, the target is highly resistant to bullet damage when he is irritated.");
            Console.Write("\tAdditionally, there is a special \"G-Field\" which attracts your bullet and affects its trajectory. ");
            Console.WriteLine("If your bullet is attracted \"forward\" or \"upward\", it travels farther.");
            Console.WriteLine("\nBefore you go, here are some extra stats we know:");
            Console.WriteLine("\n[ Target Info ]");
            Console.WriteLine($@"{"Mental State", -14} {"Damage Resistance", -18} {"Escape Probability", -20}");
            Console.WriteLine($@"{"Calm", -14} {"Low", -18} {"10%", -20}");
            Console.WriteLine($@"{"Moderate", -14} {"High", -18} {"50%", -20}");
            Console.WriteLine($@"{"Aggressive", -14} {"Very High", -18} {"70%", -20}");
            Console.WriteLine("\n[ Rifle Info ]");
            Console.WriteLine($@"{"Distance", -10} {"Max Damage", -10}");
            Console.WriteLine($@"{"000m", -10} {"20", -10}");
            Console.WriteLine($@"{"100m", -10} {"17", -10}");
            Console.WriteLine($@"{"250m", -10} {"13", -10}");
            Console.WriteLine($@"{"500m", -10} {"09", -10}");
            Console.WriteLine(">>> Bullet travels at 150 m/s <<<");
            Console.WriteLine("\nGood Luck, Agent.\n");
        }
        static double GfX(double gSpd,int gDir)
        {
            // GfX() calculates how much the bullet accelerates per second on the x axis.
            double x_acc = gSpd*Math.Cos((double)(gDir*Math.PI/180));
            return x_acc;
        }
        static double GfY(double gSpd,int gDir)
        {
            // GfY() calculates how much the bullet accelerates per second on the y axis.
            double y_acc = gSpd*Math.Sin((double)(gDir*Math.PI/180));
            return y_acc;
        }
        static double ShootX(double sx, double init_spd,double theta,double G,double time,double gx)
        {
            // ShootX() calculates distance traveled on x axis
            double vx = init_spd*Math.Cos(theta);
            double x_dist = sx+vx*time-(gx*time*time)/2;
            return x_dist;
        }
        static double ShootY(double sy, double init_spd,double theta,double G,double time,double gy)
        {
            // ShootX() calculates distance traveled on y axis
            double vy = init_spd*Math.Sin(theta);
            double y_dist = sy+vy*time-((G+gy)*time*time)/2;
            return y_dist;
        }
        static bool HitReg(ref int health,ref TargetState state,double bx, double by, double tx, double ty,double sx,double sy,ref double dist,ref bool precision)
        {
            // HitReg() verifies whether the bullet has hit the target
            
            bool flag = false; // hitreg
            
            if ( (bx<=tx+1.0 & bx>=tx-1.0) & (by<=ty+1.0 & by>=ty-1.0) )
            {
                dist = Math.Sqrt(Math.Pow((sx-tx),2)+Math.Pow((sy-ty),2)); // sqrt( (tx-bx)^2+(ty-by)^2 )
                if ( (bx<=tx+0.2 & bx>=tx-0.2) & (by<=ty+0.2 & by>=ty-0.2) )
                {
                  precision = true;
                }
                flag = true;
            } 
            else
            {
                flag = false;
            }
            return flag;
        }
        static void State(ref int count, ref TargetState state)
        {
            // State() changes the target's mental state
            
            if (count>=4) {state=TargetState.Aggressive;}
            else if (count>=2) {state=TargetState.Moderate;}
            else if (count>=0) {state=TargetState.Calm;}
        }
        static int Damage(ref int health,double dist,TargetState state,bool precision)
        {
            /* 
            Max bullet damage=20

            Target's immunity to bullet damage:
            Aggressive: >30m damage=0
            Moderate: >300m damage=0
            Calm: @500m damage=9
            */ 
            int damage=0;
            if (state==TargetState.Moderate) {damage = (int)(20*(Math.Exp(-dist/100)));}
            else if (state==TargetState.Aggressive) {damage = (int)(20*(Math.Exp(-dist/10)));}
            else if (state==TargetState.Calm) {damage = (int)(20*(Math.Exp(-dist/700)));}
            
            // precision damage bonus (+50%)
            if (precision){
              damage=(int)(damage*1.5);
              Console.WriteLine("[Precision bonus (+50%)] ");
            }
            health -= damage;
            Console.WriteLine("Damage: {0}, Health Remaining: {1}",damage,health);
            return damage;
        }
        static bool Move(TargetState state)
        {
            // Move(): Target moves.
            // Movement probability is related to State()
            
            bool movement = false;
            switch (state)
            {
                case TargetState.Aggressive :
                // Aggressive: 70% movement
                int[] array = new int[70];
                Random rand = new Random();
                for(int i=0;i<70;i++)
                {
                    array[i] = rand.Next(1,101);
                    
                    for(int j=0;j<i;j++)
                    {
                    while (array[i]==array[j]){
                        array[i]=rand.Next(1,101);
                    }
                    }
                }
                int key = rand.Next(1,101);
                if(array.Contains(key))
                {
                    movement = true;
                }
                break;
                case TargetState.Moderate :
                // Moderate: 50% movement
                rand = new Random();
                array = new int[50];
                for(int i=0;i<50;i++)
                {
                    array[i] = rand.Next(1,101);
                    
                    for(int j=0;j<i;j++)
                    {
                    while (array[i]==array[j]){
                        array[i]=rand.Next(1,101);
                    }
                    }
                }
                key = rand.Next(1,101);
                if(array.Contains(key))
                {
                    movement = true;
                }
                break;
                case TargetState.Calm:
                // Calm: 10% movement
                rand = new Random();
                array = new int[10];
                for(int i=0;i<10;i++)
                {
                    array[i] = rand.Next(1,101);
                    
                    for(int j=0;j<i;j++)
                    {
                    while (array[i]==array[j]){
                        array[i]=rand.Next(1,101);
                    }
                    }
                }
                key = rand.Next(1,101);
                if(array.Contains(key))
                {
                    movement = true;
                }
                break;
        }
        return movement;
      }
    }   
}
