using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;


///////////////////////////////////////////////////////////
/// P -> Power, W -> Weight, E -> Energy, V -> Velocity ///
/// 기본 단위 : m, kg, s                                 ///
///////////////////////////////////////////////////////////


namespace DUAS
{
    internal class main
    {
        ////////////////
        /// Geometry ///
        ////////////////
        struct MainWing_Geometry
        {
            public double span;
            public double root;
            public double tip;
            public double sweep;
            public double loading;
            public MainWing_Geometry(double span_, double root_, double tip_, double sweep_, double loading_)
            {
                this.span = span_;
                this.root = root_;
                this.tip = tip_;
                this.sweep = sweep_;
                this.loading = loading_;
            }
        }

        struct Component_Mass
        {
            public double engine;
            public double wing;
            public double body;
            public double vert_tail;
            public double horz_tail;
            public double payload;

            public Component_Mass(double engine_, double wing_, double body_, double vert_tail_, double horz_tail_, double payload_)
            {
                this.engine = engine_;
                this.wing = wing_;
                this.body = body_;
                this.vert_tail = vert_tail_;
                this.horz_tail = horz_tail_;
                this.payload = payload_;
                }
        }

        struct Rotor_Geometry
        {
            public double rho;
            public double mu;
            public Rotor_Geometry(double rho_, double mu_)
            {
                this.rho = rho_;
                this.mu = mu_;
            }
        }

        struct Fuselage_Geometry
        {
            public double FR_nose;
            public double FR_cabin;
            public double FR_tail;

            public Fuselage_Geometry(double FR_nose_, double FR_cabin_, double FR_tail_)
            { 
                this.FR_nose = FR_nose_;
                this.FR_cabin = FR_cabin_;
                this.FR_tail = FR_tail_;
            }
        }

        struct Mission_Hover
        {
            public double P;

            public Mission_Hover(double P_)
            {
                this.P = P_;
            }
        
        }

        struct Mission_Forward
        {
            public double P;

            public Mission_Forward(double P_)
            {
                this.P = P_;
            }
        }

        static void Main(string[] args)
        {
            //////////////////////
            /// Rotor Geometry ///
            //////////////////////
            Rotor_Geometry rotor = new Rotor_Geometry();

            ReadWriteINIfile ini = new ReadWriteINIfile(@"C:\DUAS\rotor.ini");

            rotor.rho = double.Parse(ini.ReadINI("fluid", "rho"));
            rotor.mu = double.Parse(ini.ReadINI("fluid", "mu"));

            double rpm = double.Parse(ini.ReadINI("case", "rpm"));
            double omega = 2 * Math.PI * rpm / 60;

            double v_inf = double.Parse(ini.ReadINI("case", "v_inf"));

            string[] sections = ini.ReadINI("rotor", "section").Split(' ');
            string[] str_radius = ini.ReadINI("rotor", "radius").Split(' ');
            string[] str_chord = ini.ReadINI("rotor", "chord").Split(' ');
            string[] str_theta = ini.ReadINI("rotor", "pitch").Split(' ');

            int N_blades = int.Parse(ini.ReadINI("rotor", "nblades"));
            int N_rotors = int.Parse(ini.ReadINI("rotor", "nrotors"));

            double diameter = double.Parse(ini.ReadINI("rotor", "diameter"));

            List<double> radius = new List<double>();
            List<double> chord = new List<double>();
            for (int i = 0; i < sections.Length; i++)
            {
                radius.Add(double.Parse(str_radius[i]));
                chord.Add(double.Parse(str_chord[i]));
            }



            //////////////
            /// Weight ///
            //////////////
            double W_gross = 2100.0;
            double W_empty = 0.0;
            double Payload = 0.0;



            ////////////////////////////////
            /// Propulsion System Sizing ///
            ////////////////////////////////
            double P_req = 0.0;
            double P_i;
            double P_0;
            double P_p;
            double P_max = 0.0;
            double P_takeoff = 0.0;
            double P_climb1 = 0.0;
            double P_climb2 = 0.0;
            double P_cruise = 0.0;
            double P_decent = 0.0;

            List<double> Powers = new List<double>();
            Powers.Add(P_takeoff);
            Powers.Add(P_climb1);
            Powers.Add(P_climb2);
            Powers.Add(P_cruise);
            Powers.Add(P_decent);

            P_max = Powers.Max();

            List<double> P_analysis = new List<double>();
            double V_max = 0.0;

            List<double> P_perform = new List<double>();
            for (int i = 0; i < V_max; i++)
            {
                P_perform.Add(P_analysis[i]);
            }

            double g = 9.81;

            List<double> dy = new List<double>();
            for (int i = 0; i < sections.Length-1; i++)
            {
                dy.Add(radius[i+1] - radius[i]); 
            }

            double A_blade = 0.0;
            for (int i = 0; i < sections.Length - 1; i++)
            {
                A_blade += chord[i] * dy[i];
            }

            double A_disk = Math.PI * Math.Pow(diameter/2, 2);

            double sigma = N_blades * A_blade / A_disk;

            double Cd0_fuse = 0.0;
            double Cd0_wing = 0.0;
            double Cd0_tail = 0.0;
            double Cd0 = Cd0_fuse + Cd0_wing + Cd0_tail;

            double v_infinity = 55.55555556;

            double N_hubs = 0.0;
            double D_hub = 0.0;
            double D_fuse = 0.340;
            double N_rods = 0.0;
            double D_rod = 0.0;

            double D_total = N_hubs * D_hub + D_fuse + N_rods * D_rod;


            Cd0 = 0.02;                 // 경험식

            double T_total = W_gross * g;
            double T_rotor = T_total / N_rotors;
            double v_tip = omega * radius[sections.Length - 1]; 
            P_i = Math.Pow(T_rotor, 1.5) / Math.Sqrt(2 * rotor.rho * A_disk);
            P_0 = ((sigma * Cd0) / 8) * rotor.rho * A_disk * Math.Pow(v_tip, 3);
            P_p = D_total * v_infinity;

            double P_output = D_total * v_infinity;


            P_req = P_i + P_0;

            double q = 0.5 * rotor.rho * Math.Pow(v_infinity, 2);
            D_hub = 1.2 * Math.Pow(W_gross / 1000, 2 / 3) * q;

            
            Component_Mass m = new Component_Mass();

            Fuselage_Geometry fuse = new Fuselage_Geometry();

            fuse.FR_nose = 0.0;

            ////////////////////////
            /// Mission Analysis ///
            ////////////////////////
            
            Mission_Hover hover = new Mission_Hover();
            Mission_Forward forward = new Mission_Forward();

            hover.P = P_i + P_0;    // 1157079.5390697673
            forward.P = D_total * v_infinity;

            double 
        }
    }
}