using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


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


        static void Main(string[] args)
        {
            //////////////////////
            /// Rotor Geometry ///
            //////////////////////

            ReadWriteINIfile ini = new ReadWriteINIfile(@"C:\DUAS\rotor.ini");

            double rho = double.Parse(ini.ReadINI("fluid", "rho"));
            double mu = double.Parse(ini.ReadINI("fluid", "mu"));

            double rpm = double.Parse(ini.ReadINI("case", "rpm"));
            double omega = 2 * Math.PI * rpm / 60;

            double v_inf = double.Parse(ini.ReadINI("case", "v_inf"));

            string[] sections = ini.ReadINI("rotor", "section").Split(' ');
            string[] str_radius = ini.ReadINI("rotor", "radius").Split(' ');
            string[] str_chord = ini.ReadINI("rotor", "chord").Split(' ');
            string[] str_theta = ini.ReadINI("rotor", "pitch").Split(' ');

            int Nb = int.Parse(ini.ReadINI("rotor", "nblades"));

            double diameter = double.Parse(ini.ReadINI("rotor", "diameter"));



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
            double T = W_gross * g / Nb;    // 6867

            P_req = Math.Pow(T, 1.5) / Math.Sqrt(2 * rho * Math.PI * Math.Pow((diameter / 2), 2)); // 136741.81293172896

            ////////////////////////
            /// Mission Analysis ///
            ////////////////////////
            double Energy_takeoff = 0.0;
            double Energy_cruise = 0.0;
            double Energy_landing = 0.0;

            List<double> Energies = new List<double>();
            Energies.Add(Energy_takeoff);
            Energies.Add(Energy_cruise);
            Energies.Add(Energy_landing);

            double Energy_total = Energies.Sum();


        }
    }
}