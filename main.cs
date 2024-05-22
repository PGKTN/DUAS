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
            public double battery;
            public double wing;
            public double body;
            public double vert_tail;
            public double horz_tail;
            public double rotor;

            public Component_Mass(double battery_, double wing_, double body_, double vert_tail_, double horz_tail_, double rotor_)
            {
                this.battery = battery_;
                this.wing = wing_;
                this.body = body_;
                this.vert_tail = vert_tail_;
                this.horz_tail = horz_tail_;
                this.rotor = rotor_;
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
        struct Mission_Cruise
        {
            public double P;
            public double E;
            public double t;

            public Mission_Cruise(double P_, double E_, double t_)
            {
                this.P = P_;
                this.E = E_;
                this.t = t_;
            }
        }
        struct Mission_Climb
        {
            public double P;
            public double E;
            public double t;

            public Mission_Climb(double P_, double E_, double t_)
            {
                this.P = P_;
                this.E = E_;
                this.t = t_;

            }
        }
        struct Mission_Descent
        {
            public double P;
            public double E;
            public double t;

            public Mission_Descent(double P_, double E_, double t_)
            {
                this.P = P_;
                this.E = E_;
                this.t = t_;

            }
        }

        static void Main(string[] args)
        {
            Calculate Calc = new Calculate();
            ReadWriteINIfile ini = new ReadWriteINIfile(@"C:\DUAS\rotor.ini");

            Rotor_Geometry rotor = new Rotor_Geometry();
            Fuselage_Geometry fuse = new Fuselage_Geometry();

            Component_Mass m = new Component_Mass();

            Mission_Climb climb = new Mission_Climb();
            Mission_Cruise cruise = new Mission_Cruise();
            Mission_Descent descent = new Mission_Descent();


            //////////////////////
            /// Rotor Geometry ///
            //////////////////////


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

            double g = 9.81;

            // 블레이드 섹션 사이 간격 구하기
            List<double> dy = new List<double>();
            for (int i = 0; i < sections.Length - 1; i++)
            {
                dy.Add(radius[i + 1] - radius[i]);
            }

            // 블레이드 넓이 구하기
            double A_blade = 0.0;
            for (int i = 0; i < sections.Length - 1; i++)
            {
                A_blade += chord[i] * dy[i];
            }

            double A_disk = Math.PI * Math.Pow(diameter / 2, 2);

            double sigma = N_blades * A_blade / A_disk;

            double Cd0_fuse = 0.0;
            double Cd0_wing = 0.0;
            double Cd0_tail = 0.0;
            double Cd0 = Cd0_fuse + Cd0_wing + Cd0_tail;

            double v_infinity = 55.55555556;

            double D_total = 0.0; // 경험식 사용 예정

            Cd0 = 0.02;                 // 경험식

            double T_total = W_gross * g;
            double T_rotor = T_total / N_rotors;
            double v_tip = omega * radius[sections.Length - 1];

            double P_output = D_total * v_infinity;

            double q = 0.5 * rotor.rho * Math.Pow(v_infinity, 2);


            fuse.FR_nose = 0.0;

            ////////////////////////
            /// Mission Analysis ///
            ////////////////////////
            double vi = Math.Sqrt(T_rotor / (2 * rotor.rho * A_disk));
            double vc = 3.333333;

            climb.t = 3 * 60;
            climb.P = Calc.Get_P_i(T_rotor, vi + vc) + Calc.Get_P_0(sigma, Cd0, rotor.rho, A_disk, v_tip);  //1174247 W
            climb.E = climb.P * climb.t;    //211364466 J

            cruise.t = 18 * 60;
            descent.t = 9 * 60;

            double vd = 10 / 9;


            descent.P = Calc.Get_P_i(T_rotor, vi - vd) + Calc.Get_P_0(sigma, Cd0, rotor.rho, A_disk, v_tip);
            descent.E = descent.P * descent.t;

            double Energy_total = climb.E + cruise.E + descent.E;


        }
    }
}